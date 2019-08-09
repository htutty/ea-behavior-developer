using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

namespace IndexAccessor
{
	/// <summary>
	/// Description of ConnectorSearcher.
	/// </summary>
	public class ConnectorSearcher
	{
		SQLiteConnection conn;

		public ConnectorSearcher()
		{
			string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
			this.conn = new SQLiteConnection("Data Source=" + dbFileName);
		}

		public List<ConnectorVO> findByObjectGuid(string objGuid) {
			List<ConnectorVO> retList = new List<ConnectorVO>() ;

			conn.Open();

			using (var command = conn.CreateCommand())
		    {
                //クエリの実行
                string sql =
                    @"select connGuid, connName, connType, srcObjId, srcObjGuid, srcObjName, destObjId, destObjGuid, destObjName
                      from t_connector where srcObjGuid = '" + objGuid + "' or destObjGuid = '" + objGuid + "'";
                command.CommandText = sql;

                // selectの結果がSQLiteDataReaderに入ってくるので、終端まで読み込み
                using (var sdr = command.ExecuteReader())
	            {
                    //
                    while (sdr.Read()) {
	            		ConnectorVO convo = new ConnectorVO() ;

	            		convo.guid = sdr.GetString(0);
	            		convo.name = sdr.GetString(1);
	            		convo.connectorType = sdr.GetString(2);

                        convo.srcObjId = sdr.GetInt32(3);
                        convo.srcObjGuid = sdr.GetString(4);
		            	convo.srcObjName =  sdr.GetString(5);

                        convo.destObjId = sdr.GetInt32(6);
                        convo.destObjGuid = sdr.GetString(7);
	            		convo.destObjName = sdr.GetString(8);

	            		if ( convo.srcObjGuid.Equals(objGuid) ) {
	            			convo.targetObjGuid = convo.destObjGuid;
	            			convo.targetObjName = convo.destObjName;
	            		} else {
	            			convo.targetObjGuid = convo.srcObjGuid;
	            			convo.targetObjName = convo.srcObjName;
	            		}

	            		retList.Add(convo);
	            	}
	            }
			}

			conn.Close();

	        // 取得したリストを返す
	        // この条件でヒットしない場合は0件のリストを返す
	        return retList;
		}


	}
}
