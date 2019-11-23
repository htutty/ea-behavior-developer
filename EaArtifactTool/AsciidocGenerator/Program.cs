using System;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using System.IO;

namespace AsciidocGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            if ( args.Length >= 1 )
            {
                string projectFile = args[0];
                //string artifactFile = args[1];
                //string adocFile = args[2];

                // .bdprjファイルの読み込み
                ProjectSetting.load(projectFile);

                // 出力フォルダの存在チェック＆無ければ作成
                string asciidocDir = ProjectSetting.getVO().projectPath + "\\" + "asciidocs";
                makeAsciidocDirIfNotExist(asciidocDir);

                // 全成果物リストの読み込み
                string artifactDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
                ArtifactsVO allArtifacts = ArtifactsXmlReader.readAllArtifacts(artifactDir);

                // 成果物パッケージXMLファイル読み込み
                ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactDir, true);

                for(int i=0; i < allArtifacts.artifactList.Count; i++)
                {
                    ArtifactVO atf = allArtifacts.artifactList[i];
                    string artifactFileName = "atf_" + atf.guid.Substring(1, 36) + ".xml";
                    string adocfile = outputArtifactAsciidoc(atfReader, artifactFileName, asciidocDir);

                    Console.WriteLine("{0}:ドキュメント出力 {1}", i + 1, adocfile);
                    atf.asciidocFilePath = "asciidocs\\" + artifactFileName;
                }

                // asciidocFilePath をセットした結果を AllArtifacts.xml ファイルに記録
                AllArtifactsXmlWriter.outputAllArtifactsFile(artifactDir, allArtifacts);
            }
            else
            {
                Console.WriteLine("引数が足りません: ");
                Console.WriteLine("Usage: AsciidocGenerator <.bdprj file> <artifactFile(.xml)> <outputFile(.adoc)>");
            }

        }

        /// <summary>
        /// 成果物ごとのAsciiDocファイルを出力する
        /// </summary>
        /// <param name="atfReader"></param>
        /// <param name="artifactFile"></param>
        /// <param name="asciidocDir"></param>
        /// <returns></returns>
        private static string outputArtifactAsciidoc(ArtifactXmlReader atfReader, string artifactFile, string asciidocDir)
        {
            // 成果物XMLファイルの読み込み
            ArtifactVO artifact = atfReader.readArtifactFile(artifactFile);

            string partGuid = artifact.guid.Substring(1, 8);
            string artifactFileName = "atf_" + filterSpecialChar(artifact.name) + "_" + partGuid + ".adoc";

            // ArtifactAsciidocWriter aaWriter = new ArtifactAsciidocWriter(artifact);
            // aaWriter.writeFile(asciidocDir + "\\" + artifactFileName);
            ArtifactAsciidocWriter.outputAsciidocFile(artifact, asciidocDir + "\\" + artifactFileName);
            return artifactFileName;
        }

        /// <summary>
        /// Asciidoc出力用フォルダを作る。
        /// 先に存在チェックし、なかった場合だけフォルダ作成を行う。
        /// </summary>
        /// <param name="asciidocDir">出力先asciidocパス</param>
        private static void makeAsciidocDirIfNotExist(string asciidocDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(asciidocDir))
            {
                Directory.CreateDirectory(asciidocDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + asciidocDir);
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
