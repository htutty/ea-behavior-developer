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
        /// AllArtficats.xml を読み、ArtifactsVO を返却する
        /// ただし、この処理ではまだ成果物XMLファイルを読んでいないので、
        /// ArtifactVO には package配下の情報は入っていない
        /// </summary>
        /// <param name="artifactDir"></param>
        /// <returns>読み込まれた全成果物情報</returns>
        public static ArtifactsVO readAllArtifacts(string artifactDir)
        {
            ArtifactsVO retArtifacts = new ArtifactsVO();
            string allArtifactsFile = "AllArtifacts.xml";
            string fileName = artifactDir + "\\" + allArtifactsFile;

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            // artifactsノードに移動する
            XmlNode artifactsNode = xmlDoc.SelectSingleNode("artifacts");

            foreach (XmlAttribute atfAttr in artifactsNode.Attributes)
            {
                switch (atfAttr.Name)
                {
                    case "targetProject":
                        if (atfAttr.Value != null && atfAttr.Value != "")
                        {
                            retArtifacts.targetProject = atfAttr.Value;
                        }
                        break;

                    case "lastUpdated":
                        if (atfAttr.Value != null && atfAttr.Value != "")
                        {
                            retArtifacts.lastUpdated = atfAttr.Value;
                        }
                        break;

                    case "targetModel":
                        if (atfAttr.Value != null && atfAttr.Value != "")
                        {
                            retArtifacts.targetModel = atfAttr.Value;
                        }
                        break;
                }

            }

            List<ArtifactVO> artifacts = new List<ArtifactVO>();

            foreach (XmlNode atfNode in artifactsNode.ChildNodes)
            {
                if (atfNode.Name == "artifact")
                {
                    artifacts.Add(ArtifactXmlReader.readArtifactNode(atfNode));
                }
            }

            retArtifacts.artifactList = artifacts;

            return retArtifacts;
        }


        /// <summary>
        /// All_Artifacts.xml を読み、リスト化する
        ///
        /// XML例：
        /// <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        ///   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        /// </artifacts>
        ///
        /// </summary>
        public static List<ArtifactVO> readArtifactList(string artifactDir)
        {
            string allArtifactsFile = "AllArtifacts.xml";
            return readArtifactList(artifactDir, allArtifactsFile);
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
        public static List<ArtifactVO> readArtifactList(string artifactDir, string allArtifactsFile)
        {
            List<ArtifactVO> artifactList = new List<ArtifactVO>();
            

			string target_dir = null;
			if ( artifactDir != null ) {
				target_dir = artifactDir;
			} else {
				throw new ArgumentException("project_dirを指定してください");
			}

			string target_file = allArtifactsFile;
			if (target_file == null) {
				target_file = "AllArtifacts.xml";
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