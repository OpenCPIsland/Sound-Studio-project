using Disney.Database;
using Disney.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.ClubPenguin.Service.Friends
{
	[Implements]
	public class FriendsDatabaseApi
	{
		public const string DB_FILENAME_STRING = "friends.sqlite";

		private SqliteDatabase sqliteDatabase;

		private Hashtable friendsTables = new Hashtable();

		[Inject]
		private IFriendsManager FriendsManager
		{
			get;
			set;
		}

		public static string ConnectionString => "URI=file:" + Application.persistentDataPath + "/friends.sqlite,version=3";

		public FriendsDatabaseApi(SqliteDatabase aSqliteDatabase)
		{
			sqliteDatabase = aSqliteDatabase;
			if (sqliteDatabase == null)
			{
				Util.Log("WARNING: Attempted to start FriendsDatabaseApi without a valid SqliteDatabase");
			}
		}

		private void createTables(string aUserSwid)
		{
			if (friendsTables[aUserSwid] == null)
			{
				friendsTables[aUserSwid] = new FriendsTable(aUserSwid);
				sqliteDatabase.RegisterTable((ISqliteTable)friendsTables[aUserSwid]);
			}
		}

		public void saveFriends(string aUserSwid)
		{
			createTables(aUserSwid);
			FriendsTable friendsTable = (FriendsTable)friendsTables[aUserSwid];
			sqliteDatabase.ClearTable(friendsTable.Name);
			foreach (Friend friend in FriendsManager.Friends)
			{
				Hashtable hashtable = new Hashtable();
				hashtable[friendsTable.FieldName] = friend.Name;
				hashtable[friendsTable.FieldSwid] = friend.Swid;
				hashtable[friendsTable.FieldTrusted] = (friend.Trusted ? 1 : 0);
				int updateReturn = 0;
				sqliteDatabase.UpdateRecord(friendsTable.Name, hashtable, friendsTable.FieldTrusted + "='" + friend.Swid + "'", out updateReturn);
				if (updateReturn < 1)
				{
					sqliteDatabase.Insert(friendsTable.Name, hashtable);
				}
			}
		}

		public List<Friend> loadFriends(string aUserSwid)
		{
			createTables(aUserSwid);
			FriendsTable friendsTable = (FriendsTable)friendsTables[aUserSwid];
			List<Friend> list = new List<Friend>();
			ArrayList records = sqliteDatabase.GetRecords("SELECT * FROM " + friendsTable.Name);
			foreach (Hashtable item in records)
			{
				string aName = (string)item[friendsTable.FieldName];
				string aSwid = (string)item[friendsTable.FieldSwid];
				Friend friend = FriendsManager.GenerateFriend(aName, aSwid);
				if ((int)(long)item[friendsTable.FieldTrusted] == 0)
				{
					friend.Trusted = false;
				}
				else
				{
					friend.Trusted = true;
				}
				list.Add(friend);
			}
			return list;
		}

		public void ClearData()
		{
		}
	}
}
