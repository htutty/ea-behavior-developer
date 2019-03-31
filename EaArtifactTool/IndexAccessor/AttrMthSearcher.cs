using ArtifactFileAccessor.util;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace IndexAccessor
{
    public class AttrMthSearcher
    {
        SQLiteConnection conn;

        public AttrMthSearcher()
        {
            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            this.conn = new SQLiteConnection("Data Source=" + dbFileName);
        }

        public List<AttrMthSearchItem> findByKeyword(string keyword)
        {
            return find("t_element.elemType IN ('Class', 'Enumeration') and t_element.elemName like '%" + keyword + "%' " +
                "or t_attr_mth.attrMthName like '%" + keyword + "%' ");
        }


        public List<AttrMthSearchItem> findByClassName(string className)
        {
            return find("t_element.elemType IN ('Class', 'Enumeration') and t_element.elemName = '" + className + "' " );
        }



        private List<AttrMthSearchItem> find(string whereCond)
        {
            List<AttrMthSearchItem> retList = new List<AttrMthSearchItem>();

            string fields = @"t_element.elemName, t_element.elemAlias, t_element.elemType, t_element.elemStereotype,
                t_attr_mth.elemGuid, t_attr_mth.attrMthFlg, t_attr_mth.attrMthType,
                t_attr_mth.attrMthGuid, t_attr_mth.attrMthName, t_attr_mth.attrMthAlias, t_attr_mth.attrMthNotes,
                ifnull(t_attr_mth.mthParamDesc, ''), t_element.artifactPath";

            string sql =
                @"select " + fields +
                   " from t_attr_mth inner join t_element on t_attr_mth.elemId = t_element.objectId " +
                   " where " + whereCond;

            conn.Open();

            using (var command = conn.CreateCommand())
            {
                //クエリの実行
                command.CommandText = sql;
                using (var sdr = command.ExecuteReader())
                {
                    //
                    while (sdr.Read())
                    {
                        AttrMthSearchItem attrMth = new AttrMthSearchItem();

                        attrMth.elemName = sdr.GetString(0);
                        attrMth.elemAlias = sdr.GetString(1);
                        attrMth.elemType = sdr.GetString(2);
                        attrMth.elemStereotype = sdr.GetString(3);
                        attrMth.elemGuid = sdr.GetString(4);
                        attrMth.attrMethFlg = sdr.GetString(5);
                        attrMth.attrMethType = sdr.GetString(6);
                        attrMth.attrMethGuid = sdr.GetString(7);
                        attrMth.attrMethName = sdr.GetString(8);
                        attrMth.attrMethAlias = sdr.GetString(9);
                        attrMth.attrMethNotes = sdr.GetString(10);
                        attrMth.methParameterDesc = sdr.GetString(11);
                        attrMth.artifactPath = sdr.GetString(12);

                        retList.Add(attrMth);
                    }
                }
            }

            conn.Close();

            // 一件でも取得できたらtrueを返す
            return retList;
        }

    }
}
