using System.Collections.Generic;
using System.Xml;
using System.Configuration;
using BehaviorDevelop.vo;

namespace BehaviorDevelop.util
{
    /// <summary>
    /// XMLの共通処理
    /// </summary>
    public class ArtifactsXmlReader
    {
        #region "定数"
        /// <summary>
        /// クラスフラグのプロパティー名
        /// </summary>
        private const string CLASSFLG_PROPERTY = @"@classflg";

        /// <summary>
        /// クラス：１
        /// </summary>
        private const string CLASSFLG_CLASS = @"1";
        #endregion

        #region "アーティファクトリスト読み込み"

        /// <summary>
        /// All_Artifacts.xml を読み、リスト化する
        /// 
        /// XML例：
        /// <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        ///   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        /// </artifacts>
        /// 
        /// </summary>
        public static List<ArtifactVO> readArtifactList(string project_dir)
        {
        	return readArtifactList(project_dir, null);
        }
        
        /// <summary>
        /// 指定された成果物一覧のxmlを読み、リスト化する
        /// 
        /// XML例：
        /// <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        ///   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        /// </artifacts>
        /// 
        /// </summary>
        /// <returns>ArtifactVOのリスト</returns>
        public static List<ArtifactVO> readArtifactList(string project_dir, string artifactsfile)
        {
            List<ArtifactVO> artifactList = new List<ArtifactVO>();
			// string fileName = "C:/DesignHistory/ea-artifacts/unify_asweadb/20171101/All_Artifacts.xml";
			
			string target_dir = null;
			if ( project_dir != null ) {
				target_dir = project_dir;
			} else {
	            target_dir = ConfigurationManager.AppSettings["artifact_dir"];
			}

			string target_file = artifactsfile;
			if (target_file == null) {
				target_file = "All_Artifacts.xml";
			}
			
			string fileName = target_dir + "/" + target_file;
            
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( fileName );

            // artifactsノードに移動する
            XmlNode artifactsNode = xmlDoc.SelectSingleNode("//artifacts");

            foreach (XmlNode atfNode in artifactsNode.ChildNodes)
            {
                // 成果物ノードを読み込んで vo を１件作成
                ArtifactVO atf = new ArtifactVO();
				foreach(XmlAttribute attr in atfNode.Attributes) {
					switch( attr.Name ) {
						case "name" : 
							atf.name = attr.Value ;
							break;
						case "guid" : 
							atf.guid = attr.Value ;
							break;
						case "project" : 
							atf.projectName = attr.Value ;
							break;
						case "stereotype" :
							atf.stereoType = attr.Value ;
							break;
						case "path" :
							atf.pathName = attr.Value;
							break;
						case "changed" :
							atf.changed = attr.Value[0];
							break;
					}
				}
                
                artifactList.Add(atf);
            }

            return artifactList;
        }
        #endregion

    }
}