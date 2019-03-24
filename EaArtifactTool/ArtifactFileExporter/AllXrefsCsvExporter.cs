
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;

namespace ArtifactFileExporter
{
    class AllXrefsCsvExporter
    {
        // DBのコネクション
        private OleDbConnection objConn;
        public List<CrossRefVO> allXrefList;

        public AllXrefsCsvExporter(OleDbConnection conn)
        {
            this.objConn = conn;
            this.allXrefList = new List<CrossRefVO>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="outputDir"></param>
        public void outputAllCrossRefs(string outputDir)
        {
            Console.WriteLine("outputAllCrossRefs() ");

            StreamWriter sw = new StreamWriter(outputDir + @"\AllXrefs.csv", false,
                System.Text.Encoding.GetEncoding("shift_jis"));

            // ConnectorList の内容をテキスト出力

            string strSQL, strFields;

            // 読み込む項目リスト
            strFields =
              " xrefId     , name      , Type       , visibility , NameSpace , " +
              " requirement, constraint, behavior   , partition  , description , " +
              " client     , supplier  , link        ";

            // SQL文 を作成
            strSQL =
              " SELECT " + strFields +
              " FROM  t_xref " +
              " WHERE Type = 'Transformation' " +
              " ORDER BY xrefId ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = this.objConn;

            using (OleDbDataReader reader = dbCom.ExecuteReader())
            {
                // CSVのヘッダーを出力
                sw.Write("xrefId,");
                sw.Write(" name,");
                sw.Write(" eaType,");
                sw.Write(" visibility,");
                sw.Write(" eaNameSpace,");
                sw.Write(" requirement,");
                sw.Write(" constrain,");
                sw.Write(" behavior,");
                sw.Write(" partition,");
                sw.Write(" description,");
                sw.Write(" client,");
                sw.Write(" supplier,");
                sw.Write(" eaVisibility,");
                sw.Write(" link");
                sw.Write("\r\n");

                // 結果を表示します。
                while (reader.Read())
                {
                    CrossRefVO xrefvo = new CrossRefVO();
                    xrefvo.xrefId = DbUtil.readStringField(reader, 0);
                    xrefvo.name = DbUtil.readStringField(reader, 1);
                    xrefvo.eaType = DbUtil.readStringField(reader, 2);
                    xrefvo.visibility = DbUtil.readStringField(reader, 3);
                    xrefvo.eaNameSpace = DbUtil.readStringField(reader, 4);
                    xrefvo.requirement = DbUtil.readStringField(reader, 5);
                    xrefvo.constraint = DbUtil.readStringField(reader, 6);
                    xrefvo.behavior = DbUtil.readStringField(reader, 7);
                    xrefvo.partition = DbUtil.readStringField(reader, 8);
                    xrefvo.description = DbUtil.readStringField(reader, 9);
                    xrefvo.client = DbUtil.readStringField(reader, 10);
                    xrefvo.supplier = DbUtil.readStringField(reader, 11);
                    xrefvo.link = DbUtil.readStringField(reader, 12);
                    allXrefList.Add(xrefvo);

                    sw.Write(" \"" + xrefvo.xrefId + "\",");
                    sw.Write(" \"" + xrefvo.name + "\",");
                    sw.Write(" \"" + xrefvo.eaType + "\",");
                    sw.Write(" \"" + xrefvo.visibility + "\",");
                    sw.Write(" \"" + xrefvo.eaNameSpace + "\",");
                    sw.Write(" \"" + xrefvo.requirement + "\",");
                    sw.Write(" \"" + xrefvo.constraint + "\",");
                    sw.Write(" \"" + xrefvo.behavior + "\",");
                    sw.Write(" \"" + xrefvo.partition + "\",");
                    sw.Write(" \"" + xrefvo.description + "\",");
                    sw.Write(" \"" + xrefvo.client + "\",");
                    sw.Write(" \"" + xrefvo.supplier + "\",");
                    sw.Write(" \"" + xrefvo.visibility + "\",");
                    sw.Write(" \"" + xrefvo.link + "\" ");
                    sw.Write("\r\n");

                }

            }

            sw.Close();
        }


    }
}
