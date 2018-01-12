/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/10
 * Time: 9:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Xml;
using System.Collections.Generic;
using BehaviorDevelop.vo;
using System.Configuration;

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ArtifactXmlReader.
	/// </summary>
	public class ArtifactXmlReader
	{
		ConnectorXmlReader connReader = null;
		string projectPath = null;
		
		public ArtifactXmlReader(string project_dir)
		{
        	if ( this.connReader　== null ) {
        		this.connReader = new ConnectorXmlReader(project_dir);
        	}
			
			this.projectPath = project_dir;
		}
		
		#region "アーティファクトファイル読み込み"
        /// <summary>
        /// Artifact XML (atf_${guid}.xml) を読み、リスト化する
        /// 
        /// XML例：
        /// <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        ///   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        /// </artifacts>
        /// 
        /// </summary>
        /// <returns>ArtifactVOのリスト</returns>
        public void readArtifactDesc(ArtifactVO artifact)
        {
        	if ( this.connReader　== null ) {
        		this.connReader = new ConnectorXmlReader(this.projectPath);
        	}
        	
			string target_dir = null;
			if ( this.projectPath != null ) {
				target_dir = this.projectPath;
			} else {
	            target_dir = ConfigurationManager.AppSettings["artifact_dir"];
			}

            string fileName = target_dir + "/" + "atf_" + artifact.guid.Substring(1, 36) + ".xml"  ;

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( fileName );

            // artifactノードに移動する
            XmlNode artifactNode = xmlDoc.SelectSingleNode("artifact");

            if ( artifactNode != null ) {
            	artifact.package = readRootPackage(artifactNode) ;
            }

        }
        #endregion
        
        
        private PackageVO readRootPackage( XmlNode parentNode ) {
        	PackageVO pkgvo = null;
        	
        	foreach (XmlNode pkgNode in parentNode.ChildNodes)
            {
    			if ( "package".Equals(pkgNode.Name) ) {
    				pkgvo = new PackageVO {
    					name = pkgNode.SelectSingleNode("@name").Value
    				};
            		
        			readPackages(pkgvo, pkgNode);	
            	}
            }
        	
        	return pkgvo;
        }
        
        private void readPackages( PackageVO pkgvo, XmlNode parentNode ) {
        	IList<PackageVO> retList = new List<PackageVO>();
        	IList<ElementVO> retElementList = new List<ElementVO>();        	
    		foreach (XmlNode pkgNode in parentNode.ChildNodes)
            {
    			if ( "package".Equals(pkgNode.Name) ) {
    				PackageVO pkg = new PackageVO();
    				foreach(XmlAttribute attr in pkgNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : 
    							pkg.name = attr.Value ;
    							break;
    						case "alias" : 
    							pkg.alias = attr.Value ;
    							break;
    						case "stereotype" :
    							pkg.stereoType = attr.Value ;
    							break;
//    						case "TPos" : pkg = attr.Value ;
    					}
    				}

    				readPackages(pkg, pkgNode) ;
    				retList.Add(pkg);
            	}
    			
    			if ( "element".Equals(pkgNode.Name) ) {
    				ElementVO elem = new ElementVO {
    					name = pkgNode.SelectSingleNode("@name").Value,
    					eaType = pkgNode.SelectSingleNode("@type").Value,
    					guid = pkgNode.SelectSingleNode("@guid").Value
    				};

    				readElementContents(elem, pkgNode);
    				retElementList.Add(elem);
    			}
    		}
    		
    		pkgvo.childPackageList = retList;
    		pkgvo.elements = retElementList;
        }

        private void readElementContents( ElementVO elemvo, XmlNode parentNode ) {
        	IList<AttributeVO> retAttrList = new List<AttributeVO>();
        	IList<MethodVO> retMethList = new List<MethodVO>();        	
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

    			if ( "connector".Equals(elemNode.Name) ) {
    				ConnectorVO convo = new ConnectorVO();
					foreach(XmlAttribute attr in elemNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : convo.name = attr.Value; break;
//    						case "alias" : convo.alias = attr.Value; break;
//    						case "stereotype" : mthvo.stereoType = attr.Value; break;
    						case "guid" : convo.guid = attr.Value; break;
//    						case "pos" : mthvo.pos = attr.Value; break;
    					}
    				}
    				
    				this.connReader.readConnectorByGUID(convo, elemvo);
    				
    				retConnList.Add(convo);
            	}

    		}
    		
    		elemvo.attributes = retAttrList;
    		elemvo.methods = retMethList;
    		elemvo.connectors = retConnList;
        }

        
        public List<ElementVO> readAllElements(ArtifactVO artifact, string project_dir)
        {
			string target_dir = null;
			if ( this.projectPath != null ) {
				target_dir = this.projectPath;
			} else {
	            target_dir = ConfigurationManager.AppSettings["artifact_dir"];
			}

            string fileName = target_dir + "/" + "atf_" + artifact.guid.Substring(1, 36) + ".xml"  ;

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( fileName );
            
        	List<ElementVO> retList = new List<ElementVO>();

            // elementノードを全て取得する
            XmlNodeList elemNodes = xmlDoc.SelectNodes("//element");

            if ( elemNodes != null ) { 
            	foreach( XmlNode elemNode in elemNodes ) {
	            	ElementVO elemvo = new ElementVO();
	            	foreach(XmlAttribute attr in elemNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : elemvo.name = attr.Value; break;
    						case "alias" : elemvo.alias = attr.Value; break;
    						case "type" : elemvo.eaType = attr.Value; break;
    						case "stereotype" : elemvo.stereoTypeEx = attr.Value; break;
    						case "guid" : elemvo.guid = attr.Value; break;
    					}
    				}
					retList.Add(elemvo);
            	}
	        }
            
            return retList;
        }

        
	}
}
