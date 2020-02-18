using System;
using System.Collections.Generic;
using System.Xml;

namespace AuditLogTransfer
{
    public class AuditLogXmlReader
    {
        private string xmlFilePath;

        public AuditLogXmlReader(string filepath)
        {
            this.xmlFilePath = filepath;
        }

        /// <summary>
        /// 監査ログのXMLファイルを読み込み、Rowタグ配下の内容を全て読みこんだ
        /// </summary>
        /// <returns></returns>
        public List<AuditLogItem> readXmlAllDatas( )
        {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.Load(xmlFilePath);

                // 一番上のEADATAノードに移動する
                XmlNode eaDataNode = xmlDoc.SelectSingleNode("EADATA");

                //
                if (eaDataNode != null)
                {
                    // 配下の Dataset_0 ノードを検索する
                    XmlNode dataSetNode = eaDataNode.SelectSingleNode("Dataset_0");
                    if (dataSetNode != null)
                    {
                        // Dataset配下の Data タグを探しに行く
                        return readDataNode(dataSetNode);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("XMLファイル読み込みにて例外が発生しました:");
                Console.WriteLine(ex.Message);
            }


            return null;
        }


        private List<AuditLogItem> readDataNode(XmlNode dataSetNode)
        {
            XmlNode dataNode = dataSetNode.SelectSingleNode("Data");

            //
            if (dataNode != null)
            {
                return readDataRowsNode(dataNode);
            }
            else
            {
                return null;
            }
        }


        private List<AuditLogItem> readDataRowsNode(XmlNode dataNode)
        {
            List<AuditLogItem> retLogItems = new List<AuditLogItem>();

            foreach (XmlNode rowNode in dataNode.ChildNodes)
            {
                if (rowNode.Name == "Row")
                {
                    AuditLogItem item = readRowNode(rowNode);
                    retLogItems.Add(item);
                }
            }

            return retLogItems;
        }

        private AuditLogItem readRowNode(XmlNode rowNode)
        {
            AuditLogItem logItem = new AuditLogItem();

            foreach( XmlNode itemNode in rowNode.ChildNodes )
            {
                switch (itemNode.Name)
                {
                    case "SnapshotID":
                        logItem.snapshotId = itemNode.InnerText;
                        break;
                    case "SeriesID":
                        logItem.seriesId = itemNode.InnerText;
                        break;
                    case "Position":
                        logItem.position = readIntValue(itemNode.InnerText);
                        break;
                    case "SnapshotName":
                        logItem.snapshotName = itemNode.InnerText;
                        break;
                    case "Notes":
                        logItem.notes = itemNode.InnerText;
                        break;
                    case "Style":
                        logItem.style = itemNode.InnerText;
                        break;
                    case "ElementID":
                        logItem.elementId = readIntValue(itemNode.InnerText);
                        break;
                    case "ElementType":
                        logItem.elementType = itemNode.InnerText;
                        break;
                    case "StrContent":
                        logItem.strContent = itemNode.InnerText;
                        break;
                    case "BinContent1":
//                        logItem.binContent1 = itemNode.InnerText;
                        break;
                    case "BinContent2":
//                        logItem.binContent2 = itemNode.InnerText;
                        break;
                }
            }

            return logItem;
        }

        /// <summary>
        /// 文字列を数値と解釈しLong型で取得する
        /// </summary>
        /// <param name="value">文字列</param>
        /// <returns>解釈で得られたLong値（解釈に失敗した場合は強制的に0）</returns>
        private static long readLongValue(string value)
        {
            long p;
            if (!long.TryParse(value, out p))
            {
                p = 0;
            }
            return p;
        }


        /// <summary>
        /// 文字列を数値と解釈しint型(32bit)で取得する
        /// </summary>
        /// <param name="value">文字列</param>
        /// <returns>解釈で得られたInt32値（解釈に失敗した場合は強制的に0）</returns>
        private static int readIntValue(string value)
        {
            Int32 p;
            if (!Int32.TryParse(value, out p))
            {
                p = 0;
            }
            return p;
        }

    }
}
