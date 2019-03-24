using System;
using System.IO;
using System.Xml;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.util
{
	/// <summary>
	/// Description of ProjectSetting.
	/// </summary>
	public class ProjectSetting
	{
		private static ProjectSettingVO vo = null;

        private static bool isLoaded = false;

		public ProjectSetting()
		{
		}

		/// <summary>
        /// project.bdprj ファイルを読み、その内容をProjectSettingVOにセットする
        /// </summary>
        /// <returns>bool: 読み込み成功ならtrue</returns>
		public static Boolean load(string project_file) {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( project_file );

            // projectノードに移動する
            XmlNode projectNode = xmlDoc.SelectSingleNode("/project");

            ProjectSettingVO settingvo = new ProjectSettingVO();

            if ( projectNode != null ) {
	            foreach (XmlNode settingNode in projectNode.ChildNodes) {
	            	switch( settingNode.Name ) {
	            		case "projectName":
            				settingvo.projectName = settingNode.InnerText;
	            			break;

	            		case "dbName":
            				settingvo.dbName = settingNode.InnerText;
            				break;

	            		case "artifactsFile":
                            settingvo.artifactsFile = settingNode.InnerText;
            				break;

                        case "artifactFile":
                            readArtifactFileNode(settingvo, settingNode);
                            break;

                        case "allConnector":
                            settingvo.allConnectorFile = settingNode.InnerText;
                            break;

                        case "artifactsPath":
                            settingvo.artifactsPath = settingNode.InnerText;
                            break;

                        default:
	            			// do nothing
            				break;
	            	}
	            }
            }

            settingvo.projectPath = Path.GetDirectoryName(Path.GetFullPath(project_file));

            // dbファイル名が取得できたら、プロファイルディレクトリを追加
            //if (settingvo.dbName != null) {
            //	settingvo.dbName = GetAppProfileDir() + settingvo.dbName ;
            //}

            vo = settingvo;
            isLoaded = true;
            return true;
		}


        private static void readArtifactFileNode( ProjectSettingVO settingvo,  XmlNode atfFileNode )
        {

            foreach(XmlAttribute xattr in atfFileNode.Attributes)
            {
                switch (xattr.Name)
                {
                    case "prefix":
                        settingvo.artifactFilePrefix = xattr.Value;
                        break;

                    case "suffix":
                        settingvo.artifactFileSuffix = xattr.Value;
                        break;

                    default:
                        // do nothing
                        break;
                }
            }
        }


        public static ProjectSettingVO getVO( ) {
            if(isLoaded)
            {
                return vo;
            } else {
                return null;
            }
        }

        public static string GetAppProfileDir() {
        	return System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\.bd\\";
        }

        public static EA.Repository getEARepo() {
        	if (vo != null) {
        		return vo.eaRepo;
        	} else {
        		return null;
        	}
        }

	}
}
