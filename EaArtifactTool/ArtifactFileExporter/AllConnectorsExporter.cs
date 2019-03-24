using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;

namespace ArtifactFileExporter
{
    class AllConnectorsExporter
    {
        // DBのコネクション
        private OleDbConnection objConn;
        private Dictionary<int, PackageVO> AllPackageMap;
        public List<ConnectorVO> allConnectorList;

        public AllConnectorsExporter(OleDbConnection conn, Dictionary<int, PackageVO> allPackageMap)
        {
            this.objConn = conn;
            this.AllPackageMap = allPackageMap;
            this.allConnectorList = new List<ConnectorVO>();
        }

        public void outputAllConnectors(string outputDir)
        {
            Console.WriteLine("outputAllConnectors() " );

            StreamWriter sw = new StreamWriter(outputDir + @"\AllConnectors.xml");

            // ConnectorList の内容をテキスト出力
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sw.WriteLine("<ConnectorList>");

            string strSQL, strFields;

            // 読み込む項目リスト
            strFields =
              " conn.Connector_ID, conn.ea_guid, conn.Name, conn.Direction, conn.Connector_Type " +
              " ,conn.Start_Object_ID, conn.End_Object_ID  " +
              " ,sta_obj.ea_guid as Start_OBJ_GUID, sta_obj.Name as Start_OBJ_Name, sta_obj.Package_ID as Start_OBJ_PackageID " +
              " ,end_obj.ea_guid as End_OBJ_GUID, end_obj.Name as End_OBJ_Name, end_obj.Package_ID as End_OBJ_PackageID  ";

            // objRS(0).Value = conn.Connector_ID,
            // objRS(1).Value = conn.ea_guid,
            // objRS(2).Value = conn.Name,
            // objRS(3).Value = conn.Direction,
            // objRS(4).Value = conn.Connector_Type
            // objRS(5).Value = conn.Start_Object_ID
            // objRS(6).Value = conn.End_Object_ID
            // objRS(7).Value = sta_obj.ea_guid as Start_OBJ_GUID
            // objRS(8).Value = sta_obj.Name as Start_OBJ_Name
            // objRS(9).Value = sta_obj.Package_ID as Start_OBJ_PackageID
            // objRS(10).Value = end_obj.ea_guid as End_OBJ_GUID
            // objRS(11).Value = end_obj.Name as End_OBJ_Name
            // objRS(12).Value = end_obj.Package_ID as End_OBJ_PackageID

            // SQL文 を作成
            strSQL =
              " SELECT " + strFields +
              " FROM ( " +
              "        ( t_connector as conn " +
              "         INNER JOIN t_object as sta_obj ON conn.Start_Object_ID = sta_obj.Object_ID ) " +
              "       INNER JOIN t_object as end_obj ON conn.End_Object_ID = end_obj.Object_ID ) " +
              " ORDER BY conn.ea_guid ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            using (OleDbDataReader reader = dbCom.ExecuteReader())
            {
                // 結果を表示します。
                while (reader.Read())
                {
                    //  <connector guid='{0BC66689-179A-48bc-9D84-87E09ABE3C3F}' name='null' direction='Source -> Destination' connType='Aggregation' rel='←' targetObjectId='169587'  />
                    //  </connector>

                    sw.Write("  <connector");
                    sw.Write(" connectorID='" + readIntField(reader, 0) + "'");
                    sw.Write(" guid='" + escapeXML(readStringField(reader, 1)) + "'");
                    sw.Write(" name='" + escapeXML(readStringField(reader, 2)) + "'");
                    sw.Write(" direction='" + escapeXML(readStringField(reader, 3)) + "'");
                    sw.Write(" connType='" + readStringField(reader, 4) + "'");
                    sw.Write(" startObjId='" + readIntField(reader, 5) + "'");
                    sw.Write(" endObjId='" + readIntField(reader, 6) + "'");

                    sw.WriteLine(">");

                    PackageVO startPack, endPack;
                    startPack = searchFromAllPackageMap(readIntField(reader, 9));
                    endPack = searchFromAllPackageMap(readIntField(reader, 12));

                    sw.Write("    <startObject");
                    sw.Write(" objectID='" + readIntField(reader, 5) + "'");
                    sw.Write(" guid='" + escapeXML(readStringField(reader, 7)) + "'");
                    sw.Write(" name='" + escapeXML(readStringField(reader, 8)) + "'");
                    sw.Write(" path='" + escapeXML(startPack.pathName) + "'");
                    sw.WriteLine(" />");

                    sw.Write("    <endObject");
                    sw.Write(" objectID='" + readIntField(reader, 6) + "'");
                    sw.Write(" guid='" + escapeXML(readStringField(reader, 10)) + "'");
                    sw.Write(" name='" + escapeXML(readStringField(reader, 11)) + "'");
                    sw.Write(" path='" + escapeXML(endPack.pathName) + "'");
                    sw.WriteLine(" />");

                    sw.WriteLine("  </connector>");

                    ConnectorVO connvo = new ConnectorVO();
                    connvo.connectorId = readIntField(reader, 0);
                    connvo.guid = readStringField(reader, 1);
                    connvo.name = readStringField(reader, 2);
                    // connvo.direction = readStringField(reader, 3);
                    connvo.connectorType = readStringField(reader, 4);
                    connvo.srcObjId = readIntField(reader, 5);
                    connvo.srcObjGuid = readStringField(reader, 7);
                    connvo.srcObjName = readStringField(reader, 8);

                    connvo.destObjId = readIntField(reader, 6);
                    connvo.destObjGuid = readStringField(reader, 10);
                    connvo.destObjName = readStringField(reader, 11);

                    allConnectorList.Add(connvo);
                }
            }

            sw.WriteLine("</ConnectorList>");
            sw.Close();
        }

        private PackageVO searchFromAllPackageMap(int packageId) {
            PackageVO pack = new PackageVO();
            if (AllPackageMap.ContainsKey(packageId))
            {
                pack = AllPackageMap[packageId];
            }

            return pack;
        }



        #region ユーティリティメソッド
        /// <summary>
        /// DbDataReaderからbool項目の取得
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        private static bool readBoolField(OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetBoolean(colIndex);
            return false;
        }

        /// <summary>
        /// DbDataReaderから文字列項目の取得
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        private static string readStringField(OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetString(colIndex);
            return string.Empty;
        }

        /// <summary>
        /// DbDataReaderからint項目の取得
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        private static int readIntField(OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetInt32(colIndex);
            return 0;
        }

        /// <summary>
        /// DbDataReaderから項目を取得する際にDB側の型定義が数値なものをboolに変換して取得する
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        private static bool readBooleanIntField(OleDbDataReader reader, int colIndex)
        {
            if (reader.IsDBNull(colIndex)) return false;
            int b = reader.GetInt32(colIndex);
            if (b == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// DbDataReaderから項目を取得する際にDB側の型定義が文字列なものをboolに変換して取得する
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        private static bool readBooleanStringField(OleDbDataReader reader, int colIndex)
        {
            if (reader.IsDBNull(colIndex)) return false;
            string b = reader.GetString(colIndex);
            if (b == "1")
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        private static string escapeXML(string orig)
        {
            if (orig == null)
            {
                return "";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&apos;");
            return sb.ToString();
        }

        #endregion
    }
}
