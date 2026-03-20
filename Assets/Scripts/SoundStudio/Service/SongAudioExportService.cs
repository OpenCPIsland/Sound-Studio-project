using GroovyCodecs.Mp3;
using GroovyCodecs.Types;
using OggVorbisEncoder;
using SoundStudio.Model;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoundStudio.Service
{
	public class SongAudioExportService
	{
		private const int LOOPING_SOUND_COUNT = 25;

		private const int OUTPUT_CHANNELS = 2;

		private const int MIN_SAMPLE_RATE = 44100;

		private const float OUTPUT_HEADROOM = 0.99f;

		private const float OGG_QUALITY = 1f;

		private const int MP3_BIT_RATE = 320;

		private const int MP3_CHANNEL_MODE = 0;

		private const int MP3_QUALITY = 1;

		private enum AudioExportFormat
		{
			Wav,
			Ogg,
			Mp3
		}

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public void ExportSongToUserSelectedPathAsync(SongVO song, string suggestedName)
		{
			if (song == null || song.recordDataList == null || song.recordDataList.Count == 0 || application == null || application.genreData == null)
			{
				Debug.LogWarning("Sound Studio export skipped because the song or application state was incomplete.");
				return;
			}
			string safeFileName = GetSafeFileName(suggestedName);
			StandaloneFileBrowser.SaveFilePanelAsync("Export Recording", GetInitialDirectory(), safeFileName, GetExportFilters(), delegate(string selectedPath)
			{
				OnSavePathSelected(song, selectedPath);
			});
		}

		private void OnSavePathSelected(SongVO song, string selectedPath)
		{
			if (string.IsNullOrEmpty(selectedPath))
			{
				return;
			}
			AudioExportFormat audioExportFormat = GetAudioExportFormat(selectedPath, AudioExportFormat.Wav);
			string text = EnsureFileExtension(selectedPath, audioExportFormat);
			try
			{
				ExportSongToFile(song, text, audioExportFormat);
				Debug.Log("Exported Sound Studio recording to " + text);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to export Sound Studio recording: " + ex);
			}
		}

		private void ExportSongToFile(SongVO song, string filePath, AudioExportFormat format)
		{
			GenreVO genreByID = application.genreData.getGenreByID(song.GenreID);
			Dictionary<int, ClipRenderData> clipDataBySoundId = LoadClipData(genreByID, song.CalculateUniqueSoundIDs());
			RenderedSong renderedSong = RenderSong(song, clipDataBySoundId);
			string directoryName = Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			switch (format)
			{
			case AudioExportFormat.Wav:
				WaveFileWriter.Write24BitPcm(filePath, renderedSong.samples, renderedSong.sampleRate, renderedSong.channelCount);
				break;
			case AudioExportFormat.Ogg:
				ExportSongToOggFile(renderedSong, filePath);
				break;
			case AudioExportFormat.Mp3:
				ExportSongToMp3File(renderedSong, filePath);
				break;
			default:
				throw new InvalidOperationException("Unsupported export format: " + format);
			}
		}

		private void ExportSongToOggFile(RenderedSong renderedSong, string filePath)
		{
			VorbisInfo vorbisInfo = VorbisInfo.InitVariableBitRate(renderedSong.channelCount, renderedSong.sampleRate, OGG_QUALITY);
			Comments comments = new Comments();
			comments.AddTag("ENCODER", "Sound Studio");
			OggStream oggStream = new OggStream(Environment.TickCount & int.MaxValue);
			oggStream.PacketIn(HeaderPacketBuilder.BuildInfoPacket(vorbisInfo));
			oggStream.PacketIn(HeaderPacketBuilder.BuildCommentsPacket(comments));
			oggStream.PacketIn(HeaderPacketBuilder.BuildBooksPacket(vorbisInfo));
			using (FileStream output = File.Create(filePath))
			{
				WriteAvailableOggPages(output, oggStream, flush: true);
				ProcessingState processingState = ProcessingState.Create(vorbisInfo);
				int num = renderedSong.samples.Length / renderedSong.channelCount;
				int num2 = 0;
				while (num2 < num)
				{
					int num3 = Math.Min(1024, num - num2);
					float[][] array = DeinterleaveSamples(renderedSong.samples, renderedSong.channelCount, num2, num3);
					processingState.WriteData(array, num3, 0);
					WriteAvailableOggPackets(output, oggStream, processingState, flushPages: false);
					num2 += num3;
				}
				processingState.WriteEndOfStream();
				WriteAvailableOggPackets(output, oggStream, processingState, flushPages: true);
				WriteAvailableOggPages(output, oggStream, flush: true);
			}
		}

		private void ExportSongToMp3File(RenderedSong renderedSong, string filePath)
		{
			AudioFormat audioFormat = new AudioFormat();
			audioFormat.SampleRate = renderedSong.sampleRate;
			audioFormat.Channels = (short)renderedSong.channelCount;
			audioFormat.BitsPerSample = 16;
			audioFormat.BigEndian = false;
			audioFormat.IsFloatingPoint = false;
			byte[] pCM16Bytes = GetPCM16Bytes(renderedSong.samples);
			Mp3Encoder mp3Encoder = new Mp3Encoder(audioFormat, MP3_BIT_RATE, MP3_CHANNEL_MODE, MP3_QUALITY, VBR: false);
			byte[] array = new byte[mp3Encoder.OutputBufferSize];
			try
			{
				using (FileStream fileStream = File.Create(filePath))
				{
					int num = 0;
					while (num < pCM16Bytes.Length)
					{
						int num2 = Math.Min(mp3Encoder.InputBufferSize, pCM16Bytes.Length - num);
						int num3 = mp3Encoder.EncodeBuffer(pCM16Bytes, num, num2, array);
						if (num3 > 0)
						{
							fileStream.Write(array, 0, num3);
						}
						num += num2;
					}
					int num4 = mp3Encoder.EncodeFinish(array);
					if (num4 > 0)
					{
						fileStream.Write(array, 0, num4);
					}
				}
			}
			finally
			{
				mp3Encoder.Close();
			}
		}

		private byte[] GetPCM16Bytes(float[] samples)
		{
			byte[] array = new byte[samples.Length * 2];
			for (int i = 0; i < samples.Length; i++)
			{
				short num = (short)Mathf.Clamp(Mathf.RoundToInt(samples[i] * 32767f), -32768, 32767);
				int num2 = i * 2;
				array[num2] = (byte)(num & 0xFF);
				array[num2 + 1] = (byte)((num >> 8) & 0xFF);
			}
			return array;
		}

		private float[][] DeinterleaveSamples(float[] interleavedSamples, int channelCount, int startFrame, int frameCount)
		{
			float[][] array = new float[channelCount][];
			for (int i = 0; i < channelCount; i++)
			{
				array[i] = new float[frameCount];
			}
			for (int j = 0; j < frameCount; j++)
			{
				int num = (startFrame + j) * channelCount;
				for (int k = 0; k < channelCount; k++)
				{
					array[k][j] = interleavedSamples[num + k];
				}
			}
			return array;
		}

		private void WriteAvailableOggPackets(Stream output, OggStream oggStream, ProcessingState processingState, bool flushPages)
		{
			OggPacket packet = null;
			while (processingState.PacketOut(out packet))
			{
				oggStream.PacketIn(packet);
				WriteAvailableOggPages(output, oggStream, flushPages);
			}
		}

		private void WriteAvailableOggPages(Stream output, OggStream oggStream, bool flush)
		{
			OggPage page = null;
			while (oggStream.PageOut(out page, flush))
			{
				output.Write(page.Header, 0, page.Header.Length);
				output.Write(page.Body, 0, page.Body.Length);
				flush = false;
			}
		}

		private Dictionary<int, ClipRenderData> LoadClipData(GenreVO genre, ICollection<int> soundIDs)
		{
			Dictionary<int, ClipRenderData> dictionary = new Dictionary<int, ClipRenderData>();
			foreach (int soundID in soundIDs)
			{
				SoundVO soundByID = genre.GetSoundByID(soundID);
				if (soundByID != null)
				{
					AudioClip audioClip = Resources.Load(soundByID.AudioPath) as AudioClip;
					if (audioClip == null)
					{
						throw new InvalidOperationException("Could not load audio clip for sound id " + soundID + " at path " + soundByID.AudioPath);
					}
					dictionary[soundID] = new ClipRenderData(audioClip);
				}
			}
			return dictionary;
		}

		private RenderedSong RenderSong(SongVO song, Dictionary<int, ClipRenderData> clipDataBySoundId)
		{
			double songDurationMilliseconds = GetSongDurationMilliseconds(song);
			int sampleRate = GetOutputSampleRate(clipDataBySoundId);
			int totalFrames = Mathf.Max(1, MillisecondsToFrameCeiling(songDurationMilliseconds, sampleRate));
			float[] mixBuffer = new float[totalFrames * OUTPUT_CHANNELS];
			bool[] activeLoops = new bool[LOOPING_SOUND_COUNT];
			long previousGridValue = 0L;
			double previousTimeMilliseconds = 0.0;
			foreach (RecordDataVO recordData in song.recordDataList)
			{
				double eventTimeMilliseconds = Math.Max(previousTimeMilliseconds, Math.Min(recordData.timeStamp, songDurationMilliseconds));
				MixActiveLoopSegment(mixBuffer, activeLoops, clipDataBySoundId, previousTimeMilliseconds, eventTimeMilliseconds, sampleRate);
				if (recordData.GridValue == 65535L)
				{
					previousTimeMilliseconds = eventTimeMilliseconds;
					break;
				}
				ApplyLoopStateChanges(activeLoops, previousGridValue, recordData.GridValue);
				MixOneShotTriggers(mixBuffer, clipDataBySoundId, recordData.GridValue, eventTimeMilliseconds, sampleRate);
				previousGridValue = recordData.GridValue;
				previousTimeMilliseconds = eventTimeMilliseconds;
			}
			if (previousTimeMilliseconds < songDurationMilliseconds)
			{
				MixActiveLoopSegment(mixBuffer, activeLoops, clipDataBySoundId, previousTimeMilliseconds, songDurationMilliseconds, sampleRate);
			}
			ApplyHeadroomIfNeeded(mixBuffer);
			return new RenderedSong(mixBuffer, sampleRate, OUTPUT_CHANNELS);
		}

		private void ApplyLoopStateChanges(bool[] activeLoops, long previousGridValue, long currentGridValue)
		{
			for (int i = 0; i < LOOPING_SOUND_COUNT; i++)
			{
				if (RecordDataVO.IsBitSetOn(i, previousGridValue) != RecordDataVO.IsBitSetOn(i, currentGridValue))
				{
					activeLoops[i] = RecordDataVO.IsBitSetOn(i, currentGridValue);
				}
			}
		}

		private void MixOneShotTriggers(float[] mixBuffer, Dictionary<int, ClipRenderData> clipDataBySoundId, long gridValue, double eventTimeMilliseconds, int outputSampleRate)
		{
			for (int i = LOOPING_SOUND_COUNT; i < 40; i++)
			{
				ClipRenderData value;
				if (RecordDataVO.IsBitSetOn(i, gridValue) && clipDataBySoundId.TryGetValue(i, out value))
				{
					MixOneShotClip(mixBuffer, value, eventTimeMilliseconds, outputSampleRate);
				}
			}
		}

		private void MixActiveLoopSegment(float[] mixBuffer, bool[] activeLoops, Dictionary<int, ClipRenderData> clipDataBySoundId, double startTimeMilliseconds, double endTimeMilliseconds, int outputSampleRate)
		{
			// Loop clips stay phase-aligned to the song start; toggles only mute/unmute them.
			int startFrame = MillisecondsToFrameFloor(startTimeMilliseconds, outputSampleRate);
			int endFrame = Mathf.Min(mixBuffer.Length / OUTPUT_CHANNELS, MillisecondsToFrameCeiling(endTimeMilliseconds, outputSampleRate));
			if (endFrame <= startFrame)
			{
				return;
			}
			for (int i = 0; i < activeLoops.Length; i++)
			{
				ClipRenderData value;
				if (activeLoops[i] && clipDataBySoundId.TryGetValue(i, out value))
				{
					MixLoopClip(mixBuffer, value, startFrame, endFrame, outputSampleRate);
				}
			}
		}

		private void MixLoopClip(float[] mixBuffer, ClipRenderData clipData, int startFrame, int endFrame, int outputSampleRate)
		{
			if (clipData.frameCount == 0)
			{
				return;
			}
			double sampleRateScale = (double)clipData.sampleRate / (double)outputSampleRate;
			double clipFramePosition = ((double)startFrame * sampleRateScale) % (double)clipData.frameCount;
			for (int i = startFrame; i < endFrame; i++)
			{
				int num = i * OUTPUT_CHANNELS;
				for (int j = 0; j < OUTPUT_CHANNELS; j++)
				{
					mixBuffer[num + j] += clipData.ReadInterpolatedSample(clipFramePosition, j, looping: true);
				}
				clipFramePosition += sampleRateScale;
				while (clipFramePosition >= (double)clipData.frameCount)
				{
					clipFramePosition -= (double)clipData.frameCount;
				}
			}
		}

		private void MixOneShotClip(float[] mixBuffer, ClipRenderData clipData, double eventTimeMilliseconds, int outputSampleRate)
		{
			if (clipData.frameCount == 0)
			{
				return;
			}
			int num = mixBuffer.Length / OUTPUT_CHANNELS;
			int num2 = MillisecondsToFrameRound(eventTimeMilliseconds, outputSampleRate);
			if (num2 >= num)
			{
				return;
			}
			double sampleRateScale = (double)clipData.sampleRate / (double)outputSampleRate;
			double num3 = 0.0;
			for (int i = num2; i < num; i++)
			{
				if (num3 >= (double)clipData.frameCount)
				{
					return;
				}
				int num4 = i * OUTPUT_CHANNELS;
				for (int j = 0; j < OUTPUT_CHANNELS; j++)
				{
					mixBuffer[num4 + j] += clipData.ReadInterpolatedSample(num3, j, looping: false);
				}
				num3 += sampleRateScale;
			}
		}

		private void ApplyHeadroomIfNeeded(float[] mixBuffer)
		{
			float num = 0f;
			for (int i = 0; i < mixBuffer.Length; i++)
			{
				float num2 = Mathf.Abs(mixBuffer[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			if (num <= OUTPUT_HEADROOM || num < 1E-05f)
			{
				return;
			}
			float num3 = OUTPUT_HEADROOM / num;
			for (int j = 0; j < mixBuffer.Length; j++)
			{
				mixBuffer[j] *= num3;
			}
		}

		private int GetOutputSampleRate(Dictionary<int, ClipRenderData> clipDataBySoundId)
		{
			int num = MIN_SAMPLE_RATE;
			foreach (ClipRenderData value in clipDataBySoundId.Values)
			{
				num = Math.Max(num, value.sampleRate);
			}
			return num;
		}

		private double GetSongDurationMilliseconds(SongVO song)
		{
			if (song.recordDataList == null || song.recordDataList.Count == 0)
			{
				throw new InvalidOperationException("The song does not have any record data to export.");
			}
			return Math.Max(0.0, song.recordDataList[song.recordDataList.Count - 1].timeStamp);
		}

		private int MillisecondsToFrameFloor(double milliseconds, int sampleRate)
		{
			return Mathf.Max(0, (int)Math.Floor(milliseconds * (double)sampleRate / 1000.0));
		}

		private int MillisecondsToFrameCeiling(double milliseconds, int sampleRate)
		{
			return Mathf.Max(0, (int)Math.Ceiling(milliseconds * (double)sampleRate / 1000.0));
		}

		private int MillisecondsToFrameRound(double milliseconds, int sampleRate)
		{
			return Mathf.Max(0, (int)Math.Round(milliseconds * (double)sampleRate / 1000.0));
		}

		private string GetInitialDirectory()
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
			{
				return folderPath;
			}
			string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!string.IsNullOrEmpty(folderPath2) && Directory.Exists(folderPath2))
			{
				return folderPath2;
			}
			return Application.persistentDataPath;
		}

		private static ExtensionFilter[] GetExportFilters()
		{
			return new ExtensionFilter[3]
			{
				new ExtensionFilter("Wave Audio", "wav"),
				new ExtensionFilter("Ogg Vorbis", "ogg"),
				new ExtensionFilter("MP3 Audio", "mp3")
			};
		}

		private string GetSafeFileName(string suggestedName)
		{
			string text = string.IsNullOrEmpty(suggestedName) ? "SoundStudio Recording" : suggestedName.Trim();
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			for (int i = 0; i < invalidFileNameChars.Length; i++)
			{
				text = text.Replace(invalidFileNameChars[i].ToString(), "_");
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "SoundStudio Recording";
			}
			return text;
		}

		private static AudioExportFormat GetAudioExportFormat(string filePath, AudioExportFormat fallbackFormat)
		{
			string extension = Path.GetExtension(filePath);
			if (string.IsNullOrEmpty(extension))
			{
				return fallbackFormat;
			}
			switch (extension.ToLowerInvariant())
			{
			case ".ogg":
				return AudioExportFormat.Ogg;
			case ".mp3":
				return AudioExportFormat.Mp3;
			default:
				return AudioExportFormat.Wav;
			}
		}

		private static string EnsureFileExtension(string filePath, AudioExportFormat format)
		{
			if (!string.IsNullOrEmpty(Path.GetExtension(filePath)))
			{
				return filePath;
			}
			switch (format)
			{
			case AudioExportFormat.Ogg:
				return filePath + ".ogg";
			case AudioExportFormat.Mp3:
				return filePath + ".mp3";
			default:
				return filePath + ".wav";
			}
		}

		private sealed class RenderedSong
		{
			public readonly float[] samples;

			public readonly int sampleRate;

			public readonly int channelCount;

			public RenderedSong(float[] samples, int sampleRate, int channelCount)
			{
				this.samples = samples;
				this.sampleRate = sampleRate;
				this.channelCount = channelCount;
			}
		}

		private sealed class ClipRenderData
		{
			public readonly float[] samples;

			public readonly int sampleRate;

			public readonly int channels;

			public readonly int frameCount;

			public ClipRenderData(AudioClip clip)
			{
				sampleRate = clip.frequency;
				channels = clip.channels;
				frameCount = clip.samples;
				samples = new float[frameCount * channels];
				clip.LoadAudioData();
				if (samples.Length > 0 && !clip.GetData(samples, 0))
				{
					throw new InvalidOperationException("Unable to read clip data for " + clip.name);
				}
			}

			public float ReadInterpolatedSample(double clipFramePosition, int outputChannel, bool looping)
			{
				if (frameCount == 0)
				{
					return 0f;
				}
				int num = Mathf.Clamp((int)Math.Floor(clipFramePosition), 0, frameCount - 1);
				int num2 = num + 1;
				if (looping)
				{
					if (num2 >= frameCount)
					{
						num2 = 0;
					}
				}
				else if (num2 >= frameCount)
				{
					num2 = frameCount - 1;
				}
				float num3 = ReadFrameSample(num, outputChannel);
				float num4 = ReadFrameSample(num2, outputChannel);
				float t = (float)(clipFramePosition - (double)num);
				return Mathf.Lerp(num3, num4, t);
			}

			private float ReadFrameSample(int frameIndex, int outputChannel)
			{
				if (channels <= 1)
				{
					return samples[frameIndex];
				}
				int num = Mathf.Clamp(outputChannel, 0, channels - 1);
				return samples[frameIndex * channels + num];
			}
		}

		private static class WaveFileWriter
		{
			private const short AUDIO_FORMAT_PCM = 1;

			private const short BITS_PER_SAMPLE = 24;

			public static void Write24BitPcm(string filePath, float[] samples, int sampleRate, int channelCount)
			{
				// 24-bit PCM keeps the export lossless enough for this workflow and widely compatible.
				int num = channelCount * (BITS_PER_SAMPLE / 8);
				int num2 = samples.Length * (BITS_PER_SAMPLE / 8);
				using (FileStream output = File.Create(filePath))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(output))
					{
						binaryWriter.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
						binaryWriter.Write(36 + num2);
						binaryWriter.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
						binaryWriter.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
						binaryWriter.Write(16);
						binaryWriter.Write(AUDIO_FORMAT_PCM);
						binaryWriter.Write((short)channelCount);
						binaryWriter.Write(sampleRate);
						binaryWriter.Write(sampleRate * num);
						binaryWriter.Write((short)num);
						binaryWriter.Write(BITS_PER_SAMPLE);
						binaryWriter.Write(System.Text.Encoding.ASCII.GetBytes("data"));
						binaryWriter.Write(num2);
						for (int i = 0; i < samples.Length; i++)
						{
							int num3 = Mathf.Clamp(Mathf.RoundToInt(samples[i] * 8388607f), -8388608, 8388607);
							binaryWriter.Write((byte)(num3 & 0xFF));
							binaryWriter.Write((byte)((num3 >> 8) & 0xFF));
							binaryWriter.Write((byte)((num3 >> 16) & 0xFF));
						}
					}
				}
			}
		}

	}
}
