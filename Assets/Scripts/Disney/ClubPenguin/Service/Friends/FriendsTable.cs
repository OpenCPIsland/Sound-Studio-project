using Disney.Database;

namespace Disney.ClubPenguin.Service.Friends
{
	public class FriendsTable : ISqliteTable
	{
		private const string BASE_TABLE_NAME = "friends";

		private string userSwid = string.Empty;

		public string FieldName => "name";

		public string FieldSwid => "swid";

		public string FieldTrusted => "trusted";

		public string Name => "friends" + userSwid;

		public string CreateTableStatement => "CREATE TABLE IF NOT EXISTS " + Name + " (" + FieldName + " text, " + FieldSwid + " text, " + FieldTrusted + " integer)";

		public int VersionNumber => 1;

		public FriendsTable(string aUserSwid)
		{
			userSwid = aUserSwid;
			userSwid = aUserSwid;
			userSwid = userSwid.Replace("{", string.Empty);
			userSwid = userSwid.Replace("}", string.Empty);
			userSwid = userSwid.Replace("-", string.Empty);
		}

		public void UpdateTableVersion(SqliteDatabase sqliteDatabase, int versionNum)
		{
			if (versionNum != VersionNumber)
			{
				sqliteDatabase.ClearTable(Name);
			}
		}
	}
}
