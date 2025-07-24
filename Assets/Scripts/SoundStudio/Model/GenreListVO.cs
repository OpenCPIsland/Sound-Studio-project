using System.Collections.Generic;

namespace SoundStudio.Model
{
	public class GenreListVO
	{
		private Dictionary<int, GenreVO> genreSet = new Dictionary<int, GenreVO>();

		public string version
		{
			get;
			set;
		}

		public List<GenreVO> genreList
		{
			set
			{
				foreach (GenreVO item in value)
				{
					AddGenreVO(item);
				}
			}
		}

		public IEnumerable<GenreVO> Collection => genreSet.Values;

		public void AddGenreVO(GenreVO genreVo)
		{
			genreSet.Add(genreVo.id, genreVo);
		}

		public GenreVO getGenreByID(int genreID)
		{
			if (genreSet.ContainsKey(genreID))
			{
				return genreSet[genreID];
			}
			throw new KeyNotFoundException("The genre id was not found. ID: " + genreID);
		}
	}
}
