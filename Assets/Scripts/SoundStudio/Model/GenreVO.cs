using System;
using System.Collections.Generic;
using System.Text;

namespace SoundStudio.Model
{
	public class GenreVO
	{
		private List<SoundVO> soundList = new List<SoundVO>();

		private Dictionary<int, SoundVO> SoundIdToVO = new Dictionary<int, SoundVO>();

		public string name
		{
			get;
			set;
		}

		public string buttonSound
		{
			get;
			set;
		}

		public int id
		{
			get;
			set;
		}

		public string productIdentifier
		{
			get
			{
				foreach (ProductIdentifier productIdentifier in productIdentifiers)
				{
					if (productIdentifier.store == "Google")
					{
						return productIdentifier.id;
					}
				}
				throw new InvalidOperationException("The product identifier has not been set for this genre");
			}
		}

		public List<ProductIdentifier> productIdentifiers
		{
			get;
			set;
		}

		public List<SoundVO> SoundList
		{
			get
			{
				return soundList;
			}
			set
			{
				soundList = value;
				soundList.Sort(SoundVO.sortByGroupID);
			}
		}

		public SoundVO GetSoundByID(int id)
		{
			foreach (SoundVO sound in soundList)
			{
				if (sound.id == id)
				{
					return sound;
				}
			}
			return null;
		}

		public string GetGridStatus()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < soundList.Count; i++)
			{
				if (soundList[i].IsPlaying())
				{
					stringBuilder.Append("1");
				}
				else
				{
					stringBuilder.Append("0");
				}
			}
			return stringBuilder.ToString();
		}

		public List<SoundVO> GetSoundsByGroupID(int id)
		{
			List<SoundVO> list = new List<SoundVO>();
			foreach (SoundVO sound in soundList)
			{
				if (sound.GroupID == id && sound.IsLoop())
				{
					list.Add(sound);
				}
			}
			return list;
		}

		public void ResetOneShots()
		{
			foreach (SoundVO sound in soundList)
			{
				if (!sound.IsLoop())
				{
					sound.StopOneShot();
				}
			}
		}

		public override string ToString()
		{
			return $"name: {name}, id: {id}, productIdentifier: {productIdentifier}";
		}
	}
}
