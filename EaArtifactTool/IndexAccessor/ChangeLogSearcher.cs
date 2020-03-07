using System.Collections.Generic;
using System.Data.SQLite;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

namespace IndexAccessor
{
	/// <summary>
	/// Description of ElementSearcher.
	/// </summary>
	public class ChangeLogSearcher
    {
		SQLiteConnection conn;

		public ChangeLogSearcher()
		{
            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            this.conn = new SQLiteConnection("Data Source=" + dbFileName);
        }

        public ChangeLogSearcher(string dbFileName)
        {
            this.conn = new SQLiteConnection("Data Source=" + dbFileName);
        }


        public List<ChangeLogSearchItem> findByElemGuid(string guid)
        {
            return find("ElementGuid = '" + guid + "'"); 
        }


        private List<ChangeLogSearchItem> find(string whereCond) {
			List<ChangeLogSearchItem> retList = new List<ChangeLogSearchItem>() ;

			string sql =
                @"select ChangeLogId, ElementGuid, Notes, ChangeUser, ChangeDateTime, 
                         ChangeType, ChangeItemName, Metadata, LogItem
				   from t_change_log where " + whereCond;

			conn.Open();

			using (var command = conn.CreateCommand())
		    {
	            //クエリの実行
	            command.CommandText = sql;
	            using (var sdr = command.ExecuteReader())
	            {
	            	//
	            	while(sdr.Read()) {
	            		ChangeLogSearchItem chLogItem = new ChangeLogSearchItem() ;

                        chLogItem.changeLogId = sdr.GetInt32(0);
                        chLogItem.elemGuid = sdr.GetString(1);
                        chLogItem.notes = sdr.GetString(2);
                        chLogItem.changeUser = sdr.GetString(3);
                        chLogItem.changeDateTime = sdr.GetDateTime(4);
                        chLogItem.changeType = sdr.GetString(5);
                        chLogItem.changeItem = sdr.GetString(6);
                        chLogItem.metadata = sdr.GetString(7);
                        chLogItem.logItem = sdr.GetString(8);

                        retList.Add(chLogItem);
	            	}
	            }
			}

			conn.Close();

	        // 一件でも取得できたらtrueを返す
	        return retList;
		}


	}
}
