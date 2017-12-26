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
        /// <returns>ArtifactVOのリスト</returns>
        public static IList<ArtifactVO> readArtifactList(string project_dir)
        {
            IList<ArtifactVO> artifactList = new List<ArtifactVO>();
			// string fileName = "C:/DesignHistory/ea-artifacts/unify_asweadb/20171101/All_Artifacts.xml";
			
			string target_dir = null;
			if ( project_dir != null ) {
				target_dir = project_dir;
			} else {
	            target_dir = ConfigurationManager.AppSettings["artifact_dir"];
			}

			string target_file = ConfigurationManager.AppSettings["artifacts_file"];
			string fileName = target_dir + "/" + target_file;
            
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( fileName );

            // artifactsノードに移動する
            XmlNode artifactsNode = xmlDoc.SelectSingleNode("//artifacts");

            foreach (XmlNode atfNode in artifactsNode.ChildNodes)
            {
                // 相互参照対象
                ArtifactVO atf = new ArtifactVO()
                {
                	name = atfNode.SelectSingleNode("@name").Value,
                    guid = atfNode.SelectSingleNode("@guid").Value,
                    projectName = atfNode.SelectSingleNode("@project").Value,
                    pathName = atfNode.SelectSingleNode("@path").Value,
                    stereoType = atfNode.SelectSingleNode("@stereotype").Value
                };

                artifactList.Add(atf);
            }

            return artifactList;
        }
        #endregion

    }
}