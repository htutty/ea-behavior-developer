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
		private static ProjectSettingVO projectSettingVO = null;

        private static bool isLoaded = false;

		public ProjectSetting()
		{
		}

        /// <summary>
        /// project.bdprjファイルを読み、その内容をProjectSettingVOにセットして返却する
        /// </summary>
        /// <param name="project_file">プロジェクトファイルのパス（カレントが特定できない場合はフルパスで）</param>
        /// <returns>取得されたProjectSettingVOのインスタンス</returns>
        public static ProjectSettingVO readProjectSetting(string project_file)
        {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(project_file);

            // projectノードに移動する
            XmlNode projectNode = xmlDoc.SelectSingleNode("/project");

            ProjectSettingVO settingvo = new ProjectSettingVO();

            if (projectNode != null)
            {
                foreach (XmlNode settingNode in projectNode.ChildNodes)
                {
                    switch (settingNode.Name)
                    {
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

            return settingvo;
        }

        /// <summary>
        /// project.bdprj ファイルを読み、その内容をProjectSettingVOにセットする
        /// </summary>
        /// <returns>bool: 読み込み成功ならtrue</returns>
        public static Boolean load(string project_file) {

            try
            {
                projectSettingVO = readProjectSetting(project_file);
                isLoaded = true;
            }
            catch ( Exception ex )
            {
                Console.WriteLine(ex.Message);

                projectSettingVO = null;
                isLoaded = false;
            }

            return isLoaded;
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
                return projectSettingVO;
            } else {
                return null;
            }
        }

        public static string GetAppProfileDir() {
        	return System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\.bd\\";
        }

        public static EA.Repository getEARepo() {
        	if (projectSettingVO != null) {
        		return projectSettingVO.eaRepo;
        	} else {
        		return null;
        	}
        }

	}
}
