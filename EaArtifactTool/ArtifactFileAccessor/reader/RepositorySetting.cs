using System.Collections.Generic;
using System.Xml;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

namespace ArtifactFileAccessor.reader
{
	/// <summary>
	/// Description of ProjectSetting.
	/// </summary>
	public class RepositorySetting
	{

		public RepositorySetting()
		{
		}


        /// <summary>
        /// リポジトリ設定ファイルから読み込んだリポジトリリストから引数の名前と一致したリポジトリ情報を返却
        /// </summary>
        /// <param name="repositoryFile"></param>
        /// <param name="repoName"></param>
        /// <returns></returns>
        public static RepositorySettingVO readRepositoryAndSelect(string repositoryFile, string repoName)
        {
            List<RepositorySettingVO> repoList = load(repositoryFile);

            foreach (RepositorySettingVO repo in repoList)
            {
                if (repo.name == repoName)
                {
                    return repo;
                }
            }

            return null;
        }


        /// <summary>
        /// repositories.xml ファイルを読み、その内容をRepositorySettingVOのリストにセットして返却する
        /// </summary>
        /// <returns>bool: 読み込み成功ならtrue</returns>
        public static List<RepositorySettingVO> load(string repositoryFile) {
            //string repositoriesFilename = "repositories.xml";
            //string repositoryFile = basePath + "\\" + repositoriesFilename;

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(repositoryFile);

            // 返却用のRepositorySettingVOリストを生成
            List<RepositorySettingVO> repositorySettings = new List<RepositorySettingVO>();

            // projectノードに移動する
            XmlNode repositoriesNode = xmlDoc.SelectSingleNode("/repositories");
            if ( repositoriesNode != null ) {
	            foreach (XmlNode repoNode in repositoriesNode.ChildNodes) {
                    if( repoNode.Name == "repository" )
                    {
                        RepositorySettingVO settingvo = readRepositoryNode(repoNode);
                        settingvo.projectSettingVO = ProjectSetting.readProjectSetting(settingvo.projectPath);
                        settingvo.indexDbPath = settingvo.projectPath + "\\" + settingvo.projectSettingVO.dbName;
                        repositorySettings.Add(settingvo);
                    }
	            }
            }

            return repositorySettings;
        }


        /// <summary>
        /// repositoryノードを読み込み、voを返却する
        /// </summary>
        /// <param name="repoNode"></param>
        /// <returns></returns>
        private static RepositorySettingVO readRepositoryNode(XmlNode repoNode)
        {

            RepositorySettingVO settingvo = new RepositorySettingVO();

            foreach (XmlNode riNode in repoNode.ChildNodes)
            {
                switch (riNode.Name)
                {
                    case "repositoryName":
                        settingvo.name = riNode.InnerText;
                        break;

                    case "baseRepository":
                        settingvo.baseRepository = riNode.InnerText;
                        break;

                    case "localPath":
                        settingvo.localPath = riNode.InnerText;
                        break;

                    case "projectPath":
                        settingvo.projectPath = riNode.InnerText;
                        break;

                    case "connectionString":
                        settingvo.connectionString = riNode.InnerText;
                        break;

                    default:
                        // do nothing
                        break;
                }
            }

            return settingvo;
        }

    }

}
