using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArtifactFileAccessor.vo
{
    public class ChangeLogItem
    {
        public int changeLogId { get; set; }

        //  <SnapshotID>L20</SnapshotID>
        public string snapshotId { get; set; }

        //  <SeriesID>LOG</SeriesID>
        public string seriesId { get; set; }

        //  <Notes>{871F1CE7-6B10-4a77-97E6-C27AE7801532},属性の追加テスト２,属性,
        //    {50410763-C9B4-4695-BBAF-9D1EE2834E4E},会員情報サービス,Class,</Notes>
        public string notes { get; set; }

        // 変更タイプ(INSERT/UPDATE/DELETE)
        public string changeType { get; set; }

        // 変更テーブル名
        public string changeItemName { get; set; }

        public string elementGuid { get; set; }

        public string metadata { get; set; }

        public string logItem { get; set; }

        public string changeUser { get; set; }

        public string changeDateTime { get; set; }



        // LogItem XMLデータのサンプル
        // EAの監査ログの機能を有効にするとこのようなログデータがXML形式で出力される。
        // それをいい具合に画面表示させるためにList<ChangeItemColumn>に詰め直す処理
        //<LogItem>
        //  <Row Number="0">
        //    <Column Name="object_id"><Old Value=""/><New Value="277503"/></Column>
        //    <Column Name="object_type"><Old Value=""/><New Value="Note"/></Column>
        //    <Column Name="parentid"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="author"><Old Value=""/><New Value="鈴木　啓示 CTC"/></Column>
        //    <Column Name="complexity"><Old Value=""/><New Value="1"/></Column>
        //    <Column Name="backcolor"><Old Value=""/><New Value="-1"/></Column>
        //    <Column Name="borderwidth"><Old Value=""/><New Value="-1"/></Column>
        //    <Column Name="fontcolor"><Old Value=""/><New Value="-1"/></Column>
        //    <Column Name="bordercolor"><Old Value=""/><New Value="-1"/></Column>
        //    <Column Name="modifieddate"><Old Value=""/><New Value="2019-12-04 09:19:43"/></Column>
        //    <Column Name="status"><Old Value=""/><New Value="設計中"/></Column>
        //    <Column Name="abstract"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="gentype"><Old Value=""/><New Value="&lt;none&gt;"/></Column>
        //    <Column Name="phase"><Old Value=""/><New Value="1.0"/></Column>
        //    <Column Name="scope"><Old Value=""/><New Value="Public"/></Column>
        //    <Column Name="classifier"><Old Value=""/><New Value="0"/></Column>
        //    <appliesTo><Element Type="Note"/><Element Type=""/></appliesTo>
        //    <Column Name="ea_guid"><Old Value=""/><New Value="{341A5E45-A923-4f5c-9573-18ADE6E684EB}"/></Column>
        //    <Column Name="diagram_id"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="version"><Old Value=""/><New Value="1.0"/></Column>
        //    <Column Name="package_id"><Old Value=""/><New Value="38353"/></Column>
        //    <Column Name="ntype"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="effort"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="borderstyle"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="createddate"><Old Value=""/><New Value="2019-12-04 09:19:43"/></Column>
        //    <Column Name="tagged"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="isroot"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="isleaf"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="isspec"><Old Value=""/><New Value="0"/></Column>
        //    <Column Name="isactive"><Old Value=""/><New Value="0"/></Column>
        //  </Row>
        //  <Details User="CTC0436(ASYNET\CTC0436)" DateTime="2019-12-04 09:19:43"/>
        //</LogItem>

        /// <summary>
        /// LogItemtタグの中身を読み込む
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns>ChangeItemColumn型の入ったリスト</returns>
        public List<ChangeItemColumn> parseLogItemXml(string xmlData)
        {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();

            // 
            xmlDoc.Load(new StringReader(xmlData));

            // 一番上のmetadataノードに移動する
            XmlNode logItemNode = xmlDoc.SelectSingleNode("LogItem");

            // 返却するリストを用意
            List<ChangeItemColumn> changeItemColumns = new List<ChangeItemColumn>();

            // LogItemタグが読み取れなかったらfalseを返す
            if (logItemNode != null)
            {
                
                // LogItem内のRowノードを読み込み
                // （基本は LogItem:1 vs Row:1 vs Column:n のはずだが..）
                foreach (XmlNode rowNode in logItemNode.ChildNodes)
                {
                    if (rowNode.Name == "Row")
                    {
                        changeItemColumns = readRowNode(rowNode);
                        break;
                    }
                }
                
            }

            return changeItemColumns;
        }


        /// <summary>
        /// Rowノード内の読み込み
        /// </summary>
        /// <param name="rowNode"></param>
        /// <returns></returns>
        private List<ChangeItemColumn> readRowNode(XmlNode rowNode)
        {
            List<ChangeItemColumn> retList = new List<ChangeItemColumn>();

            foreach (XmlNode colNode in rowNode.ChildNodes)
            {
                if (colNode.Name == "Column")
                {
                    ChangeItemColumn colItem = new ChangeItemColumn();
                    string[] col = readColumnChildNode(colNode);

                    colItem.columnName = readColumnNameAttribute(colNode);
                    colItem.oldValue = col[0];
                    colItem.newValue = col[1];

                    retList.Add(colItem);
                }

            }

            return retList;
        }

        /// <summary>
        /// Columnタグの中身を読み込む
        /// </summary>
        /// <param name="colNode">Columnノード</param>
        /// <returns>old,newのタグの中身(string配列で返却:[0]=Old/[1]=New)</returns>
        private string[] readColumnChildNode(XmlNode colNode)
        {
            string oldValue = null, newValue = null;
            string[] retStrAry = new string[2];

            foreach (XmlNode valNode in colNode.ChildNodes)
            {
                switch (valNode.Name)
                {
                    case "Old":
                        oldValue = readValueAttribute(valNode);
                        break;

                    case "New":
                        newValue = readValueAttribute(valNode);
                        break;

                }

            }

            if (oldValue != null && newValue != null)
            {
                retStrAry[0] = oldValue;
                retStrAry[1] = newValue;
            }

            return retStrAry;
        }

        /// <summary>
        /// Columnタグの属性を読み込む（Name属性のみ）
        /// </summary>
        /// <param name="colNode">Columnノード</param>
        /// <returns>ColumnタグのName属性の内容</returns>
        private string readColumnNameAttribute(XmlNode colNode)
        {
            foreach (XmlAttribute att in colNode.Attributes)
            {
                if (att.Name == "Name")
                {
                    return att.Value;
                }
            }
            return "";
        }

        /// <summary>
        /// Old/NewタグのValue属性を読み込む
        /// </summary>
        /// <param name="valNode">Old/Newノード（便宜上、Valノードと呼称）</param>
        /// <returns>Old/NewタグのValue属性の内容</returns>
        private string readValueAttribute(XmlNode valNode)
        {
            foreach (XmlAttribute att in valNode.Attributes)
            {
                if (att.Name == "Value")
                {
                    return att.Value;
                }
            }
            return "";
        }

    }
}
