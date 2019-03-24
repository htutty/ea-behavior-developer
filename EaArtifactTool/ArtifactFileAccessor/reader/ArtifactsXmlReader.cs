using System.Collections.Generic;
using System.Xml;
using System;
using System.Configuration;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.reader
{
    /// <summary>
    /// XMLの共通処理
    /// </summary>
    public class ArtifactsXmlReader
    {
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
        public static List<ArtifactVO> readArtifactList(string artifactDir, string artifactsfile)
        {
            List<ArtifactVO> artifactList = new List<ArtifactVO>();

			string target_dir = null;
			if ( artifactDir != null ) {
				target_dir = artifactDir;
			} else {
				throw new ArgumentException("project_dirを指定してください");
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
                if (atfNode.Name == "artifact")
                {
                    artifactList.Add(ArtifactXmlReader.readArtifactNode(atfNode));
                }
            }

            return artifactList;
        }

    }
}