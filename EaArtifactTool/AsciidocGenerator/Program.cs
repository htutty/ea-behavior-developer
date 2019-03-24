using System;
using ArtifactFileAccessor.reader;
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

                // 成果物パッケージXMLファイル読み込み
                string artifactDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
                ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactDir, true);

                // 成果物読み込み先の artifacts フォルダが存在する場合
                if (Directory.Exists(artifactDir))
                {
                    // その配下の成果物ファイルを取得
                    string[] atfFiles = Directory.GetFiles(artifactDir, "atf_*.xml");

                    Console.WriteLine("成果物Path=" + artifactDir);



                    for (var i=0; i < atfFiles.Length; i++)
                    {
                        outputArtifactAsciidoc(atfReader, Path.GetFileName(atfFiles[i]), asciidocDir);
                    }

                }


            }
            else
            {
                Console.WriteLine("引数が足りません: ");
                Console.WriteLine("Usage: AsciidocGenerator <.bdprj file> <artifactFile(.xml)> <outputFile(.adoc)>");
            }

        }

        private static void outputArtifactAsciidoc(ArtifactXmlReader atfReader, string artifactFile, string asciidocDir)
        {
            //
            ArtifactVO artifact = atfReader.readArtifactFile(artifactFile);

            Console.WriteLine("ドキュメント出力");
            ArtifactAsciidocWriter aaWriter = new ArtifactAsciidocWriter(artifact);
            string plainFile = Path.GetFileNameWithoutExtension(artifactFile);

            aaWriter.writeFile(asciidocDir + "\\" + plainFile + ".adoc");
        }


        private static void makeAsciidocDirIfNotExist(string asciidocDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(asciidocDir))
            {
                Directory.CreateDirectory(asciidocDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + asciidocDir);
            }
        }

    }
}
