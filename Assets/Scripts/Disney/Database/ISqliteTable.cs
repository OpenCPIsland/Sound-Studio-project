namespace Disney.Database
{
	public interface ISqliteTable
	{
		string Name
		{
			get;
		}

		string CreateTableStatement
		{
			get;
		}

		int VersionNumber
		{
			get;
		}

		void UpdateTableVersion(SqliteDatabase sqliteDatabase, int versionNum);
	}
}
