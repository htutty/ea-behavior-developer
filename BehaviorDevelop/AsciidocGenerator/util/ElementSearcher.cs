/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/09
 * Time: 18:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections.Generic;
using System.Data.SQLite;
using BDFileReader.vo;
using BDFileReader.util;

namespace AsciidocGenerator.util
{
	/// <summary>
	/// Description of ElementSearcher.
	/// </summary>
	public class ElementSearcher
	{
		SQLiteConnection conn;

		public ElementSearcher()
		{
            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            this.conn = new SQLiteConnection("Data Source=" + dbFileName);
        }
		
		public List<ElementVO> findByKeyword( string keyword ) {
			return find( "elemType = 'Class' and elemName like '%" + keyword + "%'" );
		}


        public List<ElementVO> findByKeywords(string[] keywords)
        {
            string nameCondStr = "";
            for(var i=0; i < keywords.Length; i++)
            {
                nameCondStr = nameCondStr + " and elemName like '%" + keywords[i] + "%' ";
            }

            return find("elemType = 'Class' " + nameCondStr);
        }


        public ElementVO findByGuid(string guid)
        {
            List <ElementVO> ret = find("elemGuid = '" + guid + "'");

            // 戻り値のリストがちょうど1件だった場合のみ、0件目を返す
            if (ret != null && ret.Count == 1) return ret[0];
            else return null;
        }

        public List<ElementVO> findByGuidPart(string guid)
        {
            return find("elemGuid like '%" + guid + "%'");
        }

        public ElementVO findByElementId(int elementId)
        {
            List<ElementVO> ret = find("objectId = " + elementId + "");

            // 戻り値のリストがちょうど1件だった場合のみ、0件目を返す
            if (ret != null && ret.Count == 1) return ret[0];
            else return null;
        }


        private List<ElementVO> find(string whereCond) {
			List<ElementVO> retList = new List<ElementVO>() ;
			
			string sql =
                @"select objectId, elemGuid, elemName, elemAlias, elemType, ifnull(elemStereotype, '')
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
	            		ElementVO elemvo = new ElementVO() ;

                        elemvo.elementId = sdr.GetInt32(0);
                        elemvo.guid = sdr.GetString(1);
	            		elemvo.name = sdr.GetString(2);
	            		elemvo.alias = sdr.GetString(3);
	            		elemvo.eaType = sdr.GetString(4);
		            	elemvo.stereoType =  sdr.GetString(5);

	            		retList.Add(elemvo);
	            	}
	            }
			}

			conn.Close();
			
	        // 一件でも取得できたらtrueを返す
	        return retList;
		}


	}
}
