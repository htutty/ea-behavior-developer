using System;
using System.IO;
using System.Linq;

namespace ArtifactFileExporter
{
    /// <summary>
    /// EAリポジトリの.eapファイルをデータベースとして読み込み、
    /// 成果物パッケージ毎のXMLファイルを基本とした各種テキストのエクスポートファイルを出力する。
    /// 合わせて、テキスト形式だと各種の検索が遅くなるため、
    /// 成果物ファイル利用の利便性を高めるためのインデックス情報をSQLiteのDBとして出力する。
    /// </summary>
    class Program
    {
        // private static string pathName = "";

        static void Main(string[] args)
        {
            string eapfile;
            string projectName;
            string outputDir;
            bool makeIndexFlg = false;

            if (args.Count() >= 4)
            {
                if(args[0] == "-index")
                {
                    makeIndexFlg = true;
                }

                eapfile = args[1];
                projectName = args[2];
                outputDir = args[3];
            }
            else if (args.Count() == 3)
            {
                makeIndexFlg = false;
                eapfile = args[0];
                projectName = args[1];
                outputDir = args[2];
            }
            else
            {
                Console.WriteLine("Usage: ArtifactFileExporter.exe (-index) <EAPファイル名> <プロジェクト名> <出力先Dir>");
                return;
            }


            try
            {
                // 引数のプロジェクトパス配下の成果物フォルダを作成
                string artifactDir = outputDir + "\\artifacts";
                makeArtifactDirIfNotExist(artifactDir);

                ArtifactExporter exporter = new ArtifactExporter(outputDir, artifactDir);
                exporter.readDataBase(eapfile, projectName);
                exporter.doExport();

                // インデックス情報を作成するかを判断
                if (makeIndexFlg)
                {
                    exporter.doMakeIndex();
                    exporter.doMakeElementFiles();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
