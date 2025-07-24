using Disney.Utility;
using Mono.Data.Sqlite;
using System;
using System.Collections;

namespace Disney.Database
{
	public class SqliteDatabase
	{
		private const string TABLE_NAME_VERSIONS = "versions";

		private const string FIELD_TABLE_NAME = "tableName";

		private const string FIELD_TABLE_VERSION = "versionNumber";

		private const string CREATE_TABLE_VERSIONS = "CREATE TABLE IF NOT EXISTS versions (tableName text, versionNumber integer)";

		private string dbConnection;

		private static readonly object locker = new object();

		public SqliteDatabase(string aConnectionString)
		{
			dbConnection = aConnectionString;
			Util.Log(dbConnection);
			createVersionsTable();
		}

		public Hashtable GetRecord(string sql)
		{
			Hashtable hashtable = new Hashtable();
			try
			{
				SqliteConnection sqliteConnection = new SqliteConnection(dbConnection);
				sqliteConnection.Open();
				SqliteCommand sqliteCommand = new SqliteCommand(sql, sqliteConnection);
				SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
				sqliteDataReader.Read();
				for (int i = 0; i < sqliteDataReader.FieldCount; i++)
				{
					hashtable[sqliteDataReader.GetName(i).ToString()] = sqliteDataReader[sqliteDataReader.GetName(i).ToString()];
				}
				sqliteDataReader.Close();
				sqliteConnection.Close();
				return hashtable;
			}
			catch (Exception aMessage)
			{
				Util.Log(aMessage);
				return hashtable;
			}
		}

		public ArrayList GetRecords(string sql)
		{
			ArrayList arrayList = new ArrayList();
			try
			{
				SqliteConnection sqliteConnection = new SqliteConnection(dbConnection);
				sqliteConnection.Open();
				SqliteCommand sqliteCommand = new SqliteCommand(sql, sqliteConnection);
				SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
				while (sqliteDataReader.Read())
				{
					Hashtable hashtable = new Hashtable();
					for (int i = 0; i < sqliteDataReader.FieldCount; i++)
					{
						hashtable[sqliteDataReader.GetName(i).ToString()] = sqliteDataReader[sqliteDataReader.GetName(i).ToString()];
					}
					if (hashtable != null)
					{
						arrayList.Add(hashtable);
					}
				}
				sqliteDataReader.Close();
				sqliteConnection.Close();
				return arrayList;
			}
			catch (Exception aMessage)
			{
				Util.Log(aMessage);
				return arrayList;
			}
		}

		public bool IsRecordInTable(string sql)
		{
			bool result = false;
			try
			{
				SqliteConnection sqliteConnection = new SqliteConnection(dbConnection);
				sqliteConnection.Open();
				SqliteCommand sqliteCommand = new SqliteCommand(sql, sqliteConnection);
				SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
				sqliteDataReader.Read();
				if (sqliteDataReader.FieldCount > 0)
				{
					Util.Log(GetType().Name + " key:value = " + sqliteDataReader.GetName(0).ToString() + " " + sqliteDataReader[sqliteDataReader.GetName(0).ToString()]);
					Util.Log(sqliteDataReader[sqliteDataReader.GetName(0).ToString()].GetType());
					if (!DBNull.Value.Equals(sqliteDataReader[sqliteDataReader.GetName(0).ToString()]))
					{
						result = true;
					}
				}
				sqliteDataReader.Close();
				sqliteConnection.Close();
				return result;
			}
			catch (Exception aMessage)
			{
				Util.Log(aMessage);
				return false;
				IL_00e9:
				return result;
			}
		}

		public int ExecuteNonQuery(string sql)
		{
			try
			{
				SqliteConnection sqliteConnection = new SqliteConnection(dbConnection);
				sqliteConnection.Open();
				SqliteCommand sqliteCommand = new SqliteCommand(sqliteConnection);
				sqliteCommand.CommandText = sql;
				int result = sqliteCommand.ExecuteNonQuery();
				sqliteConnection.Close();
				return result;
				IL_0035:;
			}
			catch (Exception aMessage)
			{
				Util.Log(aMessage);
			}
			return 0;
		}

		public string ExecuteScalar(string sql)
		{
			try
			{
				SqliteConnection sqliteConnection = new SqliteConnection(dbConnection);
				sqliteConnection.Open();
				SqliteCommand sqliteCommand = new SqliteCommand(sqliteConnection);
				sqliteCommand.CommandText = sql;
				object obj = sqliteCommand.ExecuteScalar();
				sqliteConnection.Close();
				if (obj != null)
				{
					return obj.ToString();
				}
			}
			catch (Exception aMessage)
			{
				Util.Log(aMessage);
			}
			return string.Empty;
		}

		public bool UpdateRecord(string tableName, Hashtable data, string where, out int updateReturn)
		{
			updateReturn = -1;
			string text = string.Empty;
			bool result = true;
			if (data.Count >= 1)
			{
				foreach (DictionaryEntry datum in data)
				{
					text += $" {datum.Key} = '{datum.Value}',";
				}
				text = text.Substring(0, text.Length - 1);
			}
			try
			{
				string sql = $"UPDATE {tableName} SET {text} WHERE {where};";
				updateReturn = ExecuteNonQuery(sql);
				return result;
			}
			catch
			{
				return false;
			}
		}

		public bool UpdateRecord(string tableName, Hashtable data, string where)
		{
			int updateReturn = 0;
			return UpdateRecord(tableName, data, where, out updateReturn);
		}

		public bool Delete(string tableName, string where)
		{
			bool result = true;
			try
			{
				if (where != null)
				{
					ExecuteNonQuery($"delete from {tableName} where {where};");
					return result;
				}
				ExecuteNonQuery($"delete from {tableName}");
				return result;
			}
			catch (Exception ex)
			{
				Util.Log(ex.Message);
				return false;
			}
		}

		public bool Insert(string tableName, Hashtable data)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			bool result = true;
			foreach (DictionaryEntry datum in data)
			{
				text += $" {datum.Key},";
				text2 += $" '{datum.Value}',";
			}
			text = text.Substring(0, text.Length - 1);
			text2 = text2.Substring(0, text2.Length - 1);
			try
			{
				string text3 = $"insert into {tableName}({text}) values({text2});";
				Util.Log(GetType().Name + " " + text3);
				ExecuteNonQuery(text3);
				return result;
			}
			catch (Exception ex)
			{
				Util.Log(ex.Message);
				return false;
			}
		}

		public bool ClearDB()
		{
			return false;
		}

		public bool ClearTable(string table)
		{
			try
			{
				ExecuteNonQuery($"delete from {table};");
				return true;
				IL_0019:
				bool result;
				return result;
			}
			catch
			{
				return false;
				IL_0026:
				bool result;
				return result;
			}
		}

		private void createVersionsTable()
		{
			lock (locker)
			{
				ExecuteNonQuery("CREATE TABLE IF NOT EXISTS versions (tableName text, versionNumber integer)");
			}
		}

		public void RegisterTable(ISqliteTable sqliteTable)
		{
			ExecuteNonQuery(sqliteTable.CreateTableStatement);
			int tableVersion = getTableVersion(sqliteTable.Name);
			if (tableVersion != -1)
			{
				sqliteTable.UpdateTableVersion(this, tableVersion);
			}
			if (tableVersion == -1 || tableVersion != sqliteTable.VersionNumber)
			{
				setTableVersion(sqliteTable.Name, sqliteTable.VersionNumber);
			}
		}

		private void setTableVersion(string tableName, int versionNumber)
		{
			lock (locker)
			{
				Hashtable hashtable = new Hashtable();
				hashtable["tableName"] = tableName;
				hashtable["versionNumber"] = versionNumber;
				int updateReturn = 0;
				UpdateRecord("versions", hashtable, "tableName='" + tableName + "'", out updateReturn);
				if (updateReturn < 1)
				{
					Insert("versions", hashtable);
				}
			}
		}

		private int getTableVersion(string tableName)
		{
			int result = -1;
			lock (locker)
			{
				ArrayList records = GetRecords("SELECT * FROM versions WHERE tableName='" + tableName + "'");
				foreach (Hashtable item in records)
				{
					if (item.ContainsKey("versionNumber"))
					{
						result = (int)(long)item["versionNumber"];
					}
				}
				return result;
			}
		}
	}
}
