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

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ConnectorSearcher.
	/// </summary>
	public class ConnectorSearcher
	{
		string db_file = null;
		SQLiteConnection conn;

		public ConnectorSearcher()
		{
			this.db_file = ProjectSetting.getVO().dbName;
			this.conn = new SQLiteConnection("Data Source="+this.db_file);
		}

		
		public List<ConnectorVO> findByObjectGuid(string objGuid) {
			List<ConnectorVO> retList = new List<ConnectorVO>() ;
			
			string sql =
				@"select connGuid, connName, connType, srcObjGuid, srcObjName, destObjGuid, destObjName 
                  from t_connector c where srcObjGuid = '" +  objGuid + "' or destObjGuid = '" + objGuid + "'";
			
			conn.Open();
			
			using (var command = conn.CreateCommand())
		    {
	            //クエリの実行
	            command.CommandText = sql;
	            using (var sdr = command.ExecuteReader())
	            {
	            	// 
	            	while(sdr.Read()) {
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
