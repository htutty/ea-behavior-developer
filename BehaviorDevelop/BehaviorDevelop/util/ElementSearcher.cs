﻿/*
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
using BehaviorDevelop.vo;

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ElementSearcher.
	/// </summary>
	public class ElementSearcher
	{
		const string db_file = "element_idx.db";
		SQLiteConnection conn;

		public ElementSearcher()
		{
			this.conn = new SQLiteConnection("Data Source="+db_file);
		}

		public List<ElementSearchVO> findByKeyword(string keyword) {
			List<ElementSearchVO> retList = new List<ElementSearchVO>() ;
			
			string sql =
				@"select elemGuid, elemName, elemAlias, elemType, ifnull(elemStereotype, ''), artifactGuid, artifactName, artifactPath
				   from t_element where  elemType = 'Class' and elemName like '%" + keyword + "%'";
			
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
