using System.Collections.Generic;
using System.Data.SQLite;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using System;

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
            return find("t_change_log.ElementGuid = '" + guid + "'"); 
        }


        public List<ChangeLogSearchItem> findRecentlyChanged()
        {
            return findAll(100);
        }


        private List<ChangeLogSearchItem> find(string whereCond)
        {
            return find(whereCond, 0);
        }


        private List<ChangeLogSearchItem> findAll(int rowsCountLimit)
        {
            string sql =
                @"select t_change_log.ChangeLogId, t_change_log.ElementGuid, t_change_log.Notes, t_change_log.ChangeUser, 
                       t_change_log.ChangeDateTime, t_change_log.ChangeType, t_change_log.ChangeItemName, 
	                   t_change_log.Metadata, t_change_log.LogItem,
	                   t_element.elemName, t_element.elementPath
                 from t_change_log left outer join t_element on t_change_log.ElementGuid = t_element.elemGuid 
                 order by ChangeDateTime desc ";

            return findBySql(sql, rowsCountLimit);
        }


        private List<ChangeLogSearchItem> find(string whereCond, int rowsCountLimit)
        {
            string sql =
                @"select t_change_log.ChangeLogId, t_change_log.ElementGuid, t_change_log.Notes, t_change_log.ChangeUser, 
                        t_change_log.ChangeDateTime, t_change_log.ChangeType, t_change_log.ChangeItemName, 
	                    t_change_log.Metadata, t_change_log.LogItem,
	                    t_element.elemName, t_element.elementPath
                  from t_change_log left outer join t_element on t_change_log.ElementGuid = t_element.elemGuid 
                  where " + whereCond + 
                @"order by ChangeDateTime desc";

            return findBySql(sql, rowsCountLimit);
        }


        /// <summary>
        /// 与えらえたSQL文(select)を実行し取得結果をChangeLogSearchItemリストに詰めて返す
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <param name="rowsCountLimit">行数の制限値(0なら無制限)</param>
        /// <returns></returns>
        private List<ChangeLogSearchItem> findBySql(string sql, int rowsCountLimit) {
			List<ChangeLogSearchItem> retList = new List<ChangeLogSearchItem>() ;
            int rowsCount=0;

			conn.Open();

			using (var command = conn.CreateCommand())
		    {
	            //クエリの実行
	            command.CommandText = sql;
	            using (var sdr = command.ExecuteReader())
	            {
	            	//
	            	while(sdr.Read() && (rowsCountLimit==0||rowsCountLimit>rowsCount)) {
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

                        if (sdr.GetValue(9) == DBNull.Value)
                        {
                            chLogItem.elemName = "";
                        }
                        else
                        {
                            chLogItem.elemName = sdr.GetString(9);
                        }

                        if (sdr.GetValue(10) == DBNull.Value)
                        {
                            chLogItem.elemPath = "";
                        }
                        else
                        {
                            chLogItem.elemPath = sdr.GetString(10);
                        }

                        retList.Add(chLogItem);
                        rowsCount++;
                    }
	            }
			}

			conn.Close();

	        // 一件でも取得できたらtrueを返す
	        return retList;
		}



    }
}
