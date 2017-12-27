/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/12/12
 * Time: 9:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml;
using System.Configuration;
using BehaviorDevelop.vo;

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ConnectorXmlReader.
	/// </summary>
	public class ConnectorXmlReader
	{
		private XmlDocument xmlDoc = null;
		
		public ConnectorXmlReader()
		{
            string target_dir = ConfigurationManager.AppSettings["artifact_dir"];
            string target_file = ConfigurationManager.AppSettings["connectors_file"];
			string fileName = target_dir + "/" + target_file;

			// XMLテキストをロードする
            this.xmlDoc = new XmlDocument();
            this.xmlDoc.Load( fileName );
		}
		
		
		#region "接続リスト読み込み"
        // 処理対象XML例：
        // <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        //   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        // </artifacts>

        /// <summary>
        /// All_Connectors.xml を読み、リスト化する
        /// 
        /// </summary>
        /// <returns>ConnectorVOのリスト</returns>
        public ConnectorVO readConnectorByGUID(ConnectorVO convo, ElementVO myelem)
        {
            // connectorノードに移動する
            XmlNode connectorNode = this.xmlDoc.SelectSingleNode("//connector[@guid='" + convo.guid + "']");

            if ( connectorNode != null ) { 
            	convo.connectionType = connectorNode.SelectSingleNode("@connType").Value;
            	
	            foreach (XmlNode node in connectorNode.ChildNodes) {
            		string objguid ;

	            	if( "startObject".Equals(node.Name) ) {
            			objguid = node.SelectSingleNode("@guid").Value;
            			if ( objguid != null && !myelem.guid.Equals(objguid)) {
	            			convo.targetObjName = node.SelectSingleNode("@name").Value;
		                    convo.targetObjGuid = node.SelectSingleNode("@guid").Value;
		                    return convo;
            			}
	            	}

	            	if( "endObject".Equals(node.Name) ) {
            			objguid = node.SelectSingleNode("@guid").Value;
            			if ( objguid != null && !myelem.guid.Equals(objguid)) {
	            			convo.targetObjName = node.SelectSingleNode("@name").Value;
		                    convo.targetObjGuid = node.SelectSingleNode("@guid").Value;
		                    return convo;
            			}
	            	}
	            } // end of for
            } else {
            	convo.targetObjName = "(Unknown Object)";
            	convo.targetObjGuid = "";
            }
            
            return convo;
        }
        #endregion
        
		#region "接続リスト全読込み"
        // 処理対象XML例：
        // <connector  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        //   <srcObject guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        //   <destObject guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        // </connector>

        /// <summary>
        /// All_Connectors.xml を読み、リスト化する
        /// </summary>
        /// <returns>ConnectorVOのリスト</returns>
        public List<ConnectorVO> readConnectorAll()
        {
        	List<ConnectorVO> outList = new List<ConnectorVO>();
        	
            // connectorノードを全て取得する
            XmlNodeList connectorNodes = this.xmlDoc.SelectNodes("//connector");

            if ( connectorNodes != null ) { 
            	foreach (XmlNode connectorNode in connectorNodes)
            	{
            		ConnectorVO convo = new ConnectorVO();
	            	convo.connectionType = connectorNode.SelectSingleNode("@connType").Value;
	            	convo.guid = connectorNode.SelectSingleNode("@guid").Value;
	            	convo.name = connectorNode.SelectSingleNode("@name").Value;
	            	
		            foreach (XmlNode node in connectorNode.ChildNodes) {
		            	if( "startObject".Equals(node.Name) ) {
	            			convo.srcObjName = node.SelectSingleNode("@name").Value;
		                    convo.srcObjGuid = node.SelectSingleNode("@guid").Value;
		            	}
	
		            	if( "endObject".Equals(node.Name) ) {
	            			convo.destObjName = node.SelectSingleNode("@name").Value;
		                    convo.destObjGuid = node.SelectSingleNode("@guid").Value;
		            	}
		            } // end of foreach

	            	outList.Add(convo);
            	} // end of foreach
            } 
            
            return outList;
        }
        #endregion


        
	}
}
