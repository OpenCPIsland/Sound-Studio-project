using System;

namespace SoundStudio.Model
{
	public class RecordDataVO
	{
		public readonly float timeStamp;

		private long gridvalue;

		public string gridStatus
		{
			get
			{
				if (gridvalue == 65535)
				{
					return Convert.ToString(gridvalue, 2);
				}
				return Convert.ToString(gridvalue, 2).PadLeft(40, '0');
			}
		}

		public long GridValue => gridvalue;

		public RecordDataVO(string gridStatus, float timeStamp)
		{
			if (!"1111111111111111".Equals(gridStatus) && gridStatus.Length != 40)
			{
				throw new InvalidOperationException("The grid status is of the wrong length. Grid was: " + gridStatus);
			}
			gridvalue = Convert.ToInt64(gridStatus, 2);
			this.timeStamp = timeStamp;
		}

		public bool IsBitSetOn(int gridIndex)
		{
			return IsBitSetOn(gridIndex, gridvalue);
		}

		public static bool IsBitSetOn(int gridIndex, long value)
		{
			long num = 1L << 39 - gridIndex;
			return (value & num) != 0;
		}

		public override string ToString()
		{
			return gridStatus + "|" + timeStamp.ToString();
		}
	}
}
