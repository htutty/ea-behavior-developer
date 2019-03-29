/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/09
 * Time: 18:58
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
	/// Description of ElementSearcher.
	/// </summary>
	public class ElementSearcher
	{
		string db_file = null;
		SQLiteConnection conn;

		public ElementSearcher()
		{
			this.db_file = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
			this.conn = new SQLiteConnection("Data Source="+this.db_file);
		}

		
		public List<ElementSearchVO> findByKeyword( string keyword ) {
			return find( "elemType = 'Class' and elemName like '%" + keyword + "%'" );
		}

		public List<ElementSearchVO> findByGuid( string guid ) {
			return find( "elemGuid like '" + guid + "%'" );
		}

		private List<ElementSearchVO> find(string whereCond) {
			List<ElementSearchVO> retList = new List<ElementSearchVO>() ;
			
			string sql =
				@"select elemGuid, elemName, elemAlias, elemType, ifnull(elemStereotype, ''), artifactGuid, artifactName, artifactPath
				   from t_element where " + whereCond;
			
			conn.Open();
			
			using (var command = conn.CreateCommand())
		    {
	            //クエリの実行
	            command.CommandText = sql;
	            using (var sdr = command.ExecuteReader())
	            {
	            	// 
	            	while(sdr.Read()) {
	            		ElementSearchVO elmsrvo = new ElementSearchVO() ;
	            		
	            		elmsrvo.elemGuid = sdr.GetString(0);
	            		elmsrvo.elemName = sdr.GetString(1);
	            		elmsrvo.elemAlias = sdr.GetString(2);
	            		elmsrvo.elemType = sdr.GetString(3);
		            	elmsrvo.elemStereotype =  sdr.GetString(4);
	            		elmsrvo.artifactGuid = sdr.GetString(5);
	            		elmsrvo.artifactName = sdr.GetString(6);
	            		elmsrvo.artifactPath = sdr.GetString(7);

	            		retList.Add(elmsrvo);
	            	}
	            }
			}

			conn.Close();
			
	        // 一件でも取得できたらtrueを返す
	        return retList;
		}


	}
}
