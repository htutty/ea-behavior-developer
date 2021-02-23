using System;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using System.IO;
using System.Configuration;
using System.Text;

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
        /// 全成果物のAsciidocファイルを出力する
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

                // 成果物のAsciidoc出力
                string adocFilePath = asciidocWriter.outputAsciidocFile(atf);

                // 出力されたAsciidocのファイル名を成果物VOに登録（処理後、AllArtifacts.xmlを更新する）
                atf.asciidocFilePath = adocFilePath;
                Console.WriteLine("{0}:ドキュメント出力 {1}", i + 1, adocFilePath);
            }

            // 最後に、このドキュメント一覧を見るためのindex.htmlを出力する
            outputIndexHtml(allArtifacts, asciidocDir);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="allArtifacts"></param>
        private static void outputIndexHtml(ArtifactsVO allArtifacts, string asciidocDir)
        {

            // Asciidoc出力フォルダ上に、指定されたファイル名で出力する
            string filepath = asciidocDir + "\\" + "index.html";
            //BOM無しのUTF8でテキストファイルを作成する
            StreamWriter sw = new StreamWriter(filepath);

            sw.WriteLine(@"<!DOCTYPE html>
                <html>
                <head>
                    <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
                    <meta charset='utf-8' />
                    < title >ASW-DOM-BE Class documents page</ title >
                </head>
                <body>
            ");


            sw.WriteLine("<ul>");
            foreach(var atf in allArtifacts.artifactList)
            {
                sw.Write("<li>" + atf.pathName + " - <a href='" + atf.asciidocFilePath +"'>" + atf.name + "</a>" );
                sw.WriteLine("</li>");
            }
            sw.WriteLine("</ul>");


            sw.WriteLine("</body>");
            sw.WriteLine("</html>");

            sw.Close();
        }

    }
}
