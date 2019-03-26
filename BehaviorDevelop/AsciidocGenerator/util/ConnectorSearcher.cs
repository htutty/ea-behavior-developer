/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/24
 * Time: 15:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using BDFileReader.vo;
using BDFileReader.util;

namespace AsciidocGenerator.util
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
                    @"select connGuid, connName, connType, srcObjGuid, srcObjName, destObjGuid, destObjName 
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
	            		convo.srcObjGuid = sdr.GetString(3);
		            	convo.srcObjName =  sdr.GetString(4);
	            		convo.destObjGuid = sdr.GetString(5);
	            		convo.destObjName = sdr.GetString(6);
	            		
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
