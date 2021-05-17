using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AuditLogTransfer
{
    public class MetadataXmlReader
    {
        public MetadataXmlReader()
        {
        }

        /// <summary>
        /// metadataタグの入ったXMLデータを読み込む
        /// </summary>
        /// <returns></returns>
        public static Tracker readXmlMetaData(string xmlData)
        {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();

            Tracker track = new Tracker();

            try
            {
                //
                xmlDoc.Load(new StringReader(xmlData));

                // 一番上のmetadataノードに移動する
                XmlNode metaDataNode = xmlDoc.SelectSingleNode("metadata");

                //
                if (metaDataNode != null)
                {
                    foreach (XmlNode rowNode in metaDataNode.ChildNodes)
                    {
                        switch(rowNode.Name)
                        {
                            //case "Row":
                            //    AuditLogItem item = readRowNode(rowNode);
                            //    break;
                            case "Details":
                                track = readDetailsNode(rowNode);
                                //retLogItems.Add(item);
                                break;
                        }

                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("XMLファイル読み込みにて例外が発生しました:");
                Console.WriteLine(ex.Message);
            }

            return track;
        }



        //private AuditLogItem readRowNode(XmlNode rowNode)
        //{
        //    AuditLogItem logItem = new AuditLogItem();

        //    foreach( XmlNode itemNode in rowNode.ChildNodes )
        //    {
        //        switch (itemNode.Name)
        //        {
        //            case "SnapshotID":
        //                logItem.snapshotId = itemNode.InnerText;
        //                break;

        //        }
        //    }

        //    return logItem;
        //}


        /// <summary>
        /// Detailsノードを読み、User と DateTime からなる更新トラッカー情報を返却する
        /// </summary>
        /// <param name="rowNode"></param>
        /// <returns></returns>
        private static Tracker readDetailsNode(XmlNode rowNode)
        {
            Tracker tracker = new Tracker();

            foreach (XmlAttribute detailsAttr in rowNode.Attributes)
            {
                switch (detailsAttr.Name)
                {
                    case "User":
                        tracker.user = detailsAttr.Value;
                        break;

                    case "DateTime":
                        tracker.dateTime = detailsAttr.Value;
                        break;
                }
            }

            return tracker;
        }

    }


    public class Tracker
    {
        public string user { get; set; }
        public string dateTime { get; set; }
    }


}
