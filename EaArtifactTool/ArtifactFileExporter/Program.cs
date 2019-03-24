using System;
using System.IO;
using System.Linq;

namespace ArtifactFileExporter
{
    /// <summary>
    /// EAリポジトリのデータベースをインプットとして
    /// </summary>
    class Program
    {
        // private static string pathName = "";

        static void Main(string[] args)
        {
            if (args.Count() >= 3)
            {
                string eapfile = args[0];
                string projectName = args[1];
                string outputDir = args[2];

                try
                {
                    // 引数のプロジェクトパス配下の成果物フォルダを作成
                    string artifactDir = outputDir + "\\artifacts";
                    makeArtifactDirIfNotExist(artifactDir);

                    ArtifactExporter exporter = new ArtifactExporter(outputDir, artifactDir);
                    exporter.readDataBase(eapfile, projectName);
                    exporter.doExport();
                    // exporter.doMakeIndex();
                    // exporter.doMakeElementFiles();

                }
                catch ( Exception ex )
                {
                    Console.WriteLine(ex.Message);
                }

            }
            else
            {
                Console.WriteLine("引数が違います: ExportArtifactFile.exe <EAPファイル名> <プロジェクト名> <出力先Dir>");
            }

        }



        private static void makeArtifactDirIfNotExist(string artifactDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(artifactDir))
            {
                Directory.CreateDirectory(artifactDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + artifactDir);
            }
        }

    }
}
