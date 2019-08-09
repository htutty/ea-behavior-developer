using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using ArtifactFileAccessor.util;
using IndexAccessor;

namespace ExportGraphData
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length >= 1)
            {
                string projectFile = args[0];

                // .bdprjファイルの読み込み
                ProjectSetting.load(projectFile);

                // エクスポーターを生成し、エクスポート処理実行
                GraphDataExporter export = new GraphDataExporter(ProjectSetting.getVO().projectPath);
                export.doExport();

            }
            else
            {
                Console.WriteLine("引数が足りません: ");
                Console.WriteLine("Usage:  ExportGraphData <projectFile(.bdprj)> <outputFile.txt>");
            }
        }



    }
}
