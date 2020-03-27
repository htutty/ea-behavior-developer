using System;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using System.IO;
using System.Configuration;

namespace AsciidocGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string repositoryFile = ConfigurationManager.AppSettings["repositoryFile"];
            bool elementMode = false;

            // 引数チェック（最低リポジトリ名の１つが必要）
            if (args.Length < 1)
            {
                Console.WriteLine("引数が足りません: ");
                Console.WriteLine("Usage: AsciidocGenerator <targetRepository> [-element <elementGuid>] ");
                return;
            }


            string repoName = null;
            string elementGuid = null;

            // 引数が3件以上存在したら、指定elementのみのAsciidoc出力モードで起動
            if (args.Length >= 3 && args[1] == "-element")
            {
                elementMode = true;
                repoName = args[0];
                elementGuid = args[2];
            }
            else
            {
                elementMode = false;
                repoName = args[0];
            }

            // repositories.xmlの読み込みとリポジトリ選択処理
            RepositorySettingVO repositoryVO
                = RepositorySetting.readRepositoryAndSelect(repositoryFile, repoName);

            if (repositoryVO == null)
            {
                Console.WriteLine("指定されたリポジトリ名(targetRepository)が不正です: " + repoName);
                Console.WriteLine("  Usage: AsciidocGenerator targetRepository [-element <elementGuid>] ");
                Console.WriteLine("提供リポジトリ定義ファイル: " + repositoryFile + " の中身を確認ください");
                return;
            }

            // 成果物リストの読み込みとAsciidoc出力
            readArtifactsAndWriteAsciidocs(repositoryVO.projectPath, elementMode, elementGuid);

        }



        /// <summary>
        /// 成果物リストの読み込みとAsciidoc出力
        /// </summary>
        /// <param name="elementMode"></param>
        /// <param name="repositoryVO"></param>
        private static void readArtifactsAndWriteAsciidocs(string projectFilePath, bool elementMode, string elementGuid)
        {
            // .bdprjファイルの読み込み
            ProjectSetting.load(projectFilePath);

            // Asciidocの出力モードによる分岐
            if (elementMode)
            {
                // 要素１つ分のAsciidoc出力

            }
            else
            {
                // 全成果物リストの読み込み
                string artifactDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
                ArtifactsVO allArtifacts = readAllArtifacts(artifactDir);

                // indexDb作成処理
                // (Asciidoc出力処理の中でElementSearcherやConnectorSearcherを利用するため、
                // indexDb未作成だとここでエラーになってしまっていた)
                makeIndexDbIfNotExist(allArtifacts);

                // 全成果物分のAsciidoc出力
                outputAllArtifactAsciidoc(allArtifacts);

                // asciidocFilePath をセットした結果を AllArtifacts.xml ファイルに記録
                AllArtifactsXmlWriter.outputAllArtifactsFile(artifactDir, allArtifacts);
            }
        }


        /// <summary>
        /// IndexDB情報の作成
        /// </summary>
        private static void makeIndexDbIfNotExist(ArtifactsVO allArtifacts)
        {
            string indexDbFilePath =  ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;

            // IndexDBファイルが存在してないなら
            if ( !File.Exists(indexDbFilePath) )
            {
                // Index情報未作成と見なして、IndexDB情報の作成処理を呼び出す
                IndexMaker indexMaker = new IndexMaker(allArtifacts, ProjectSetting.getVO().projectPath);
                indexMaker.doMakeIndex();
                indexMaker.doMakeElementFiles();
            }
        }


        /// <summary>
        /// 成果物XMLを全て読み込み、結果のArtifactsVOを返却する
        /// </summary>
        /// <param name="artifactDir"></param>
        /// <returns></returns>
        private static ArtifactsVO readAllArtifacts(string artifactDir)
        {
            // AllArtifacts.xml を読み込み、成果物の内容が未セット状態のartifactListを設定する
            ArtifactsVO allArtifacts = ArtifactsXmlReader.readAllArtifacts(artifactDir);

            // 成果物パッケージXMLファイル読み込み用のReaderを生成する
            ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactDir, true);

            for (int i = 0; i < allArtifacts.artifactList.Count; i++)
            {
                ArtifactVO atf = allArtifacts.artifactList[i];

                // 成果物XMLファイルの読み込み
                string artifactFileName = "atf_" + atf.guid.Substring(1, 36) + ".xml";
                ArtifactVO artifact = atfReader.readArtifactFile(artifactFileName);

                // 読み込んだ結果の成果物VOでListの中身を置き換える
                allArtifacts.artifactList[i] = artifact;
            }

            return allArtifacts;
        }


        /// <summary>
        /// 全成果物
        /// </summary>
        /// <param name="projectFile"></param>
        private static void outputAllArtifactAsciidoc(ArtifactsVO allArtifacts)
        {
            // Asciidoc出力writerの生成
            string asciidocDir = ProjectSetting.getVO().projectPath + "\\" + "asciidocs";
            ArtifactAsciidocWriter asciidocWriter = new ArtifactAsciidocWriter(asciidocDir);

            for (int i = 0; i < allArtifacts.artifactList.Count; i++)
            {
                ArtifactVO atf = allArtifacts.artifactList[i];

                // asciidocの出力ファイル名の設定
                string adocFileName = filterSpecialChar(atf.name) + "_" + atf.guid.Substring(1, 8) + ".adoc";

                // 成果物のAsciidoc出力 
                asciidocWriter.outputAsciidocFile(atf, adocFileName);

                // 出力されたAsciidocのファイル名を成果物VOに登録（処理後、AllArtifacts.xmlを更新する）
                atf.asciidocFilePath = adocFileName;
                Console.WriteLine("{0}:ドキュメント出力 {1}", i + 1, adocFileName);
            }

        }


        /// <summary>
        /// 引数の文字列から、ファイル名に使用できない文字をフィルタして返却する
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static string filterSpecialChar(string orig)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
            sb.Replace("(", "（");
            sb.Replace(")", "）");
            sb.Replace(" ", "_");
            sb.Replace("^", "＾");
            sb.Replace("？", "");
            sb.Replace("/", "_");
            sb.Replace("\\", "_");
            sb.Replace("\r", "");
            sb.Replace("\n", "");

            return sb.ToString();
        }


    }
}
