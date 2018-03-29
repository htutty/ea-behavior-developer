/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/10
 * Time: 9:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Xml;
using System.Collections.Generic;
using BehaviorDevelop.vo;
using System.Configuration;
using System.Windows.Forms;

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ArtifactXmlReader.
	/// </summary>
	public class ArtifactXmlReader
	{
		ConnectorSearcher connSearcher = null;
		string projectPath = null;
		
		public ArtifactXmlReader(string project_dir)
		{
			// 内部DBからの接続情報検索オブジェクトを生成
			if ( this.connSearcher == null && ProjectSetting.getVO() != null) {
				this.connSearcher = new ConnectorSearcher();
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
            		
        			try {
	        			readPackages(pkgvo, pkgNode);
        			} catch(Exception ex) {
						MessageBox.Show(ex.StackTrace);
        			}
            	}
            }
        	
        	return pkgvo;
        }
        
        private void readPackages( PackageVO pkgvo, XmlNode parentNode ) {
        	List<PackageVO> retList = new List<PackageVO>();
        	List<ElementVO> retElementList = new List<ElementVO>();        	
    		foreach (XmlNode pkgNode in parentNode.ChildNodes)
            {
    			if ( "package".Equals(pkgNode.Name) ) {
    				PackageVO pkg = new PackageVO();
    				foreach(XmlAttribute attr in pkgNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : 
    							pkg.name = attr.Value ;
    							break;
    						case "guid" : 
    							pkg.guid = attr.Value ;
    							break;
    						case "alias" : 
    							pkg.alias = attr.Value ;
    							break;
    						case "stereotype" :
    							pkg.stereoType = attr.Value ;
    							break;
    						case "TPos" :
    							Int32 p;
	    						if( !Int32.TryParse(attr.Value, out p) ) {
	    							p = 0;
	    						}
    							pkg.treePos = p;
    							break;
    						case "changed" :
    							pkg.changed = attr.Value[0];
								break;
    					}
    				}

    				readPackages(pkg, pkgNode) ;
    				retList.Add(pkg);
            	}
    			
    			if ( "element".Equals(pkgNode.Name) ) {
    				ElementVO elem = new ElementVO();
    				foreach(XmlAttribute attr in pkgNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : 
    							elem.name = attr.Value ;
    							break;
    						case "guid" : 
    							elem.guid = attr.Value ;
    							break;    							
    						case "type" : 
    							elem.eaType = attr.Value ;
    							break;    							
    						case "alias" : 
    							elem.alias = attr.Value ;
    							break;
    						case "stereotype" :
    							elem.stereoType = attr.Value ;
    							break;
    						case "tpos" :
    							Int32 p;
	    						if( !Int32.TryParse(attr.Value, out p) ) {
	    							p = 0;
	    						}
    							elem.treePos = p;
    							break;
    						case "changed" :
    							elem.changed = attr.Value[0];
								break;
    					}
    				}

    				readElementContents(elem, pkgNode);
    				retElementList.Add(elem);
    			}
    		}
    		
    		pkgvo.childPackageList = retList;
    		pkgvo.elements = retElementList;

    		// ConnectionSercherがいない場合は DiffMakerからの呼び出しとして、GUIDでのソートを行う
    		if (this.connSearcher != null) {
	    		pkgvo.sortChildPackages();
    		} else {
    			pkgvo.sortChildPackagesGUID();
    		}
    		
    		if (this.connSearcher != null) {
	    		pkgvo.sortElements();
    		} else {
    			pkgvo.sortElementsGUID();
    		}
    		
        }

        private void readElementContents( ElementVO elemvo, XmlNode parentNode ) {
        	List<AttributeVO> retAttrList = new List<AttributeVO>();
        	List<MethodVO> retMethList = new List<MethodVO>();        	
        	List<TaggedValueVO> retTagValList = new List<TaggedValueVO>();
        	List<ConnectorVO> retConnList = new List<ConnectorVO>();        	
        	
    		foreach (XmlNode eNode in parentNode.ChildNodes)
            {
    			if ( "attribute".Equals(eNode.Name) ) {
    				AttributeVO attvo = new AttributeVO();
					foreach(XmlAttribute attr in eNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : attvo.name = attr.Value; break;
    						case "alias" : attvo.alias = attr.Value; break;
    						case "stereotype" : attvo.stereoType = attr.Value; break;
    						case "guid" : attvo.guid = attr.Value; break;
							case "pos" :
    							Int32 p;
	    						if( !Int32.TryParse(attr.Value, out p) ) {
	    							p = 0;
	    						}
    							attvo.pos = p;
    						    break;
    						case "changed" :
    							attvo.changed = attr.Value[0];
								break;
    					}
    				}
    				
    				retAttrList.Add(attvo);
    			}
    			
    			if ( "method".Equals(eNode.Name) ) {

    				MethodVO mthvo = new MethodVO();
					foreach(XmlAttribute attr in eNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : mthvo.name = attr.Value; break;
    						case "alias" : mthvo.alias = attr.Value; break;
    						case "stereotype" : mthvo.stereoType = attr.Value; break;
    						case "guid" : mthvo.guid = attr.Value; break;
    						case "pos" :
    							Int32 p;
	    						if( !Int32.TryParse(attr.Value, out p) ) {
	    							p = 0;
	    						}
    							mthvo.pos = p;
    						    break;
    						case "changed" :
    							mthvo.changed = attr.Value[0];
								break;
    					}
    				}

    				if ( eNode.SelectSingleNode("behavior") != null ) {
    					mthvo.behavior = eNode.SelectSingleNode("behavior").InnerText;
    				}
    				if ( eNode.SelectSingleNode("notes") != null ) {
						mthvo.notes = eNode.SelectSingleNode("notes").InnerText;
    				}
    				
    				if ( eNode.SelectSingleNode("returnType") != null ) {
						mthvo.returnType = eNode.SelectSingleNode("returnType").InnerText;
    				}
    				
    				if ( eNode.SelectSingleNode("visibility") != null ) {
						mthvo.visibility = eNode.SelectSingleNode("visibility").InnerText;
    				}
    				
    				retMethList.Add(mthvo);
            	}

				// タグ付き値の読み込み
    			if ( "tv".Equals(eNode.Name) ) {
					retTagValList = readTaggedValues(eNode);
    			}    				
    				
    			
//    			if ( "connector".Equals(eNode.Name) ) {
//    				ConnectorVO convo = new ConnectorVO();
//					foreach(XmlAttribute attr in eNode.Attributes) {
//    					switch( attr.Name ) {
//    						case "name" : convo.name = attr.Value; break;
////    						case "alias" : convo.alias = attr.Value; break;
////    						case "stereotype" : convo.stereoType = attr.Value; break;
//    						case "guid" : convo.guid = attr.Value; break;
////    						case "pos" : convo.pos = attr.Value; break;
//    					}
//    				}
//    				
//    				this.connReader.readConnectorByGUID(convo, elemvo);
//    				
//    				retConnList.Add(convo);
//            	}

    		}
			
    		elemvo.attributes = retAttrList;
    		elemvo.methods = retMethList;
    		elemvo.taggedValues = retTagValList;
    		
    		// ConnectionSearcher 
    		if (connSearcher == null) {
    			elemvo.connectors = new List<ConnectorVO>();
    			elemvo.sortAttributesGUID();
    			elemvo.sortMethodsGUID();
    			elemvo.sortTaggedValuesGUID();
    		} else {
	    		elemvo.connectors = connSearcher.findByObjectGuid(elemvo.guid);
	    		elemvo.sortAttributes();
    			elemvo.sortMethods();
    			elemvo.sortTaggedValues();
    		}
        }
        
        private List<TaggedValueVO> readTaggedValues(XmlNode tvNode) {
        	List<TaggedValueVO> retTagVal = new List<TaggedValueVO>();
        	
			foreach (XmlNode tagvalNode in tvNode.ChildNodes) {
				TaggedValueVO tvvo = new TaggedValueVO();
				foreach(XmlAttribute attr in tagvalNode.Attributes) {
					switch( attr.Name ) {
						case "name" : tvvo.name = attr.Value; break;
						case "guid" : tvvo.guid = attr.Value; break;
						case "value" :
							tvvo.tagValue = attr.Value;
							if ("<memo>".Equals(attr.Value) ) {
								tvvo.tagValue = tagvalNode.InnerText ;
							}
							break;
						case "changed" :
							tvvo.changed = attr.Value[0];
							break;
					}
				}

				retTagVal.Add(tvvo);
			}
			
        	return retTagVal ;
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
    						case "stereotype" : elemvo.stereoType = attr.Value; break;
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
