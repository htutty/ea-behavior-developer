/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/12/12
 * Time: 21:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;
using System.Collections.Generic;
using BehaviorDevelop.vo;
using System.Configuration;

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ElementsXmlReader.
	/// </summary>
	public class ElementsXmlReader
	{
		private XmlDocument xmlDoc = null;
		ConnectorSearcher connSearcher = null;

		public ElementsXmlReader()
		{
            string target_dir = ConfigurationManager.AppSettings["artifact_dir"];
            string target_file = ConfigurationManager.AppSettings["elements_file"];
			string fileName = target_dir + "/" + target_file;

			if ( connSearcher == null ) {
				this.connSearcher = new ConnectorSearcher();
			}
			
			// XMLテキストをロードする
            this.xmlDoc = new XmlDocument();
            this.xmlDoc.Load( fileName );
		}

		
        // 処理対象XML例：
        // <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        //   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        // </artifacts>

        /// <summary>
        /// ElementList_all.xml を読み、リスト化する
        /// 
        /// </summary>
        /// <returns>ElementVOのリスト</returns>
        public ElementVO readElementByGUID(string guid)
        {
            // 対象のelementノードに移動する
            XmlNode elemNode = this.xmlDoc.SelectSingleNode("//element[@guid='" + guid + "']");

            ElementVO elemvo = new ElementVO();
            if ( elemNode != null ) { 
				elemvo.name = elemNode.SelectSingleNode("@name").Value;
				elemvo.guid = elemNode.SelectSingleNode("@guid").Value;

				readElementContents(elemvo, elemNode);
            } else {
            	elemvo.name = "(Unknown Object)";
            	elemvo.guid = "";
            }
            
            return elemvo;
        }

        private void readElementContents( ElementVO elemvo, XmlNode parentNode ) {
        	List<AttributeVO> retAttrList = new List<AttributeVO>();
        	List<MethodVO> retMethList = new List<MethodVO>();        	
        	IList<ConnectorVO> retConnList = new List<ConnectorVO>();        	
        	
    		foreach (XmlNode elemNode in parentNode.ChildNodes)
            {
    			if ( "attribute".Equals(elemNode.Name) ) {
    				AttributeVO attvo = new AttributeVO();
					foreach(XmlAttribute attr in elemNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : attvo.name = attr.Value; break;
    						case "alias" : attvo.alias = attr.Value; break;
//    						case "stereotype" : attvo.stereoType = attr.Value; break;
    						case "guid" : attvo.guid = attr.Value; break;
//    						case "pos" : attvo.pos = attr.Value; break;
    					}
    				}
    				
    				retAttrList.Add(attvo);
    			}

    			if ( "method".Equals(elemNode.Name) ) {

    				MethodVO mthvo = new MethodVO();
					foreach(XmlAttribute attr in elemNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : mthvo.name = attr.Value; break;
    						case "alias" : mthvo.alias = attr.Value; break;
//    						case "stereotype" : mthvo.stereoType = attr.Value; break;
    						case "guid" : mthvo.guid = attr.Value; break;
//    						case "pos" : mthvo.pos = attr.Value; break;
    					}
    				}

    				if ( elemNode.SelectSingleNode("behavior") != null ) {
    					mthvo.behavior = elemNode.SelectSingleNode("behavior").InnerText;
    				}
    				if ( elemNode.SelectSingleNode("notes") != null ) {
						mthvo.notes = elemNode.SelectSingleNode("notes").InnerText;
    				}
//					mthvo.returnType = elemNode.elemNode.SelectSingleNode("returnType").InnerText;
//					mthvo.visibility = elemNode.elemNode.SelectSingleNode("visibility").InnerText;
    				
    				retMethList.Add(mthvo);
            	}

//    			if ( "connector".Equals(elemNode.Name) ) {
//    				ConnectorVO convo = new ConnectorVO();
//					foreach(XmlAttribute attr in elemNode.Attributes) {
//    					switch( attr.Name ) {
//    						case "name" : convo.name = attr.Value; break;
//    						case "guid" : convo.guid = attr.Value; break;
//    					}
//    				}
//    				
//    				this.connReader.readConnectorByGUID(convo, elemvo);
//    				
//    				retConnList.Add(convo);
//            	}

    		}

			elemvo.attributes = retAttrList;
    		elemvo.sortAttributes();
    		elemvo.methods = retMethList;
    		elemvo.sortMethods();

    		elemvo.connectors = connSearcher.findByObjectGuid(elemvo.guid);
        }
                
	}
	
	
	
}
