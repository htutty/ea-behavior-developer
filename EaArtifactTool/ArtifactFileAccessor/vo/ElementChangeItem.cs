using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ArtifactFileAccessor.vo
{
    public class ElementChangeItem
    {
        public int elementId { get; set; }
        public string elementGuid { get; set; }
        public string elementName { get; set; }
        public string changeTableName { get; set; }

        public ChangeItemType targetItemType { get; set; }

        /// <summary>変更ユーザー</summary>
        public string changeUser { get; set; }

        /// <summary>変更日時</summary>
        public DateTime changeDateTime { get; set; }

        /// <summary>変更の種類(INSERT/UPDATE/DELETE)</summary>
        public string changeType { get; set; }

        public string targetGuid { get; set; }
        public string targetName { get; set; }

        public List<ChangeItemColumn> changeItemColumns;

        //public Dictionary<string, string[]> logItemsNewOldMap;

        // LogItem XMLデータのサンプル
        // EAの監査ログの機能を有効にするとこのようなログデータがXML形式で出力される。
        // それをいい具合に画面表示させるためにkey-valueのMapに
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
        /// 
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public bool parseLogItemXml(string xmlData)
        {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                // 
                xmlDoc.Load(new StringReader(xmlData));

                // 一番上のmetadataノードに移動する
                XmlNode logItemNode = xmlDoc.SelectSingleNode("LogItem");

                //
                if (logItemNode != null)
                {
                    // LogItem内のRowノードを読み込み
                    // （基本は LogItem:1 vs Row:1 vs Column:n のはずだが..）
                    foreach (XmlNode rowNode in logItemNode.ChildNodes)
                    {
                        if( rowNode.Name == "Row" )
                        {
                            this.changeItemColumns = readRowNode(rowNode);
                            break;
                        }

                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("XMLファイル読み込みにて例外が発生しました:");
                Console.WriteLine(ex.Message);
                return false;
            }

        }


        /// <summary>
        /// Rowノード内の
        /// </summary>
        /// <param name="rowNode"></param>
        /// <returns></returns>
        private List<ChangeItemColumn> readRowNode(XmlNode rowNode) {
            //Dictionary<string, string[]> retValMap = new Dictionary<string, string[]>();
            List<ChangeItemColumn> retList = new List<ChangeItemColumn>();

            foreach ( XmlNode colNode in rowNode.ChildNodes )
            {
                if( colNode.Name == "Column" )
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

        private string[] readColumnChildNode(XmlNode colNode)
        {
            string oldValue=null, newValue = null;
            string[] retStrAry = new string[2];

            foreach( XmlNode valNode in colNode.ChildNodes)
            {
                switch(valNode.Name)
                {
                    case "Old":
                        oldValue = readValueAttribute(valNode);
                        break;

                    case "New":
                        newValue = readValueAttribute(valNode);
                        break;

                }

            }

            if( oldValue != null && newValue != null )
            {
                retStrAry[0] = oldValue;
                retStrAry[1] = newValue;
            }

            return retStrAry;
        }

        private string readColumnNameAttribute(XmlNode colNode)
        {
            foreach( XmlAttribute att in colNode.Attributes )
            {
                if( att.Name == "Name" )
                {
                    return att.Value;
                }
            }
            return "";
        }


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


    public enum ChangeItemType
    {
        TYPE_ELEMENT,
        TYPE_ATTRIBUTE,
        TYPE_METHOD,
        TYPE_METHOD_PARAM
    }

}
