﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

namespace IndexAccessor
{
    /// <summary>
    /// Description of LoggerElementAccess.
    /// </summary>
    public class LoggerElementAccess
    {
		SQLiteConnection conn;
        string projectPath;
        string dbName;

        public LoggerElementAccess(string projectPath, string dbName)
		{
            this.projectPath = projectPath;
            this.dbName = dbName;

            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            this.conn = new SQLiteConnection("Data Source=" + dbFileName);
        }

        public void createLog()
        {
            string sql = 
                @"insert into t_element_log ( changeType, elementId, eventDate ) 
                  values (@changeType, @elementId, @eventDate )" ;

            conn.Open();
            conn.BeginTransaction();

            using (SQLiteCommand command = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                      new SQLiteParameter("@changeType", "Create"),
                      new SQLiteParameter("@elementId", 1001),
                      new SQLiteParameter("@eventDate", DateTime.Now )
                    };

                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }

            conn.Close();
        }

        public void updateLog()
        {

        }

        public void deleteLog()
        {

        }



        public List<ElementSearchItem> findByKeyword( string keyword ) {
			return find( "elemType = 'Class' and elemName like '%" + keyword + "%'" );
		}


        public List<ElementSearchItem> findByKeywords(string[] keywords)
        {
            string nameCondStr = "";
            for(var i=0; i < keywords.Length; i++)
            {
                nameCondStr = nameCondStr + " and elemName like '%" + keywords[i] + "%' ";
            }

            return find("elemType = 'Class' " + nameCondStr);
        }


        public ElementSearchItem findByGuid(string guid)
        {
            List <ElementSearchItem> ret = find("elemGuid = '" + guid + "'");

            // 戻り値のリストがちょうど1件だった場合のみ、0件目を返す
            if (ret != null && ret.Count == 1) return ret[0];
            else return null;
        }

        public List<ElementSearchItem> findByGuidPart(string guid)
        {
            return find("elemGuid like '%" + guid + "%'");
        }

        public ElementSearchItem findByElementId(int elementId)
        {
            List<ElementSearchItem> ret = find("objectId = " + elementId + "");

            // 戻り値のリストがちょうど1件だった場合のみ、0件目を返す
            if (ret != null && ret.Count == 1) return ret[0];
            else return null;
        }


        private List<ElementSearchItem> find(string whereCond) {
			List<ElementSearchItem> retList = new List<ElementSearchItem>() ;

			string sql =
                @"select objectId, elemGuid, elemName, elemAlias, elemType, ifnull(elemStereotype, ''),
                         elementPath, artifactGuid,  artifactName
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
	            		ElementSearchItem elemSrcItem = new ElementSearchItem() ;

                        elemSrcItem.elementId = sdr.GetInt32(0);
                        elemSrcItem.elemGuid = sdr.GetString(1);
	            		elemSrcItem.elemName = sdr.GetString(2);
	            		elemSrcItem.elemAlias = sdr.GetString(3);
	            		elemSrcItem.elemType = sdr.GetString(4);
		            	elemSrcItem.elemStereotype =  sdr.GetString(5);
                        elemSrcItem.elemPath = sdr.GetString(6);
                        elemSrcItem.artifactGuid = sdr.GetString(7);
                        elemSrcItem.artifactName = sdr.GetString(8);

                        retList.Add(elemSrcItem);
	            	}
	            }
			}

			conn.Close();

	        // 一件でも取得できたらtrueを返す
	        return retList;
		}


	}
}
