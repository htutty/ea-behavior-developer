using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ArtifactFileAccessor.util;

namespace BehaviorTextExportImport
{
    class Program
    {


        static void Main(string[] args)
        {

            if (args.Length >= 2)
            {
                string impExp = args[0];
                string projectFile = args[1];

                // .bdprjファイルの読み込み
                ProjectSetting.load(projectFile);

                if( impExp == "-exp" )
                {
                    BehaviorTextExporter textExp = new BehaviorTextExporter();
                    textExp.exportBehaviorText();
                }

            }
            else
            {
                Console.WriteLine("引数が足りません: ");
                Console.WriteLine("Usage: BehaviorTextExportImport (-exp|-imp) <projectFile(.bdprj)>");
                Console.WriteLine("  エクスポートの例） BehaviorTextExportImport -exp <projectFile(.bdprj)>");
                Console.WriteLine("  インポートの例） BehaviorTextExportImport -imp <projectFile(.bdprj)> 対象テキスト");
            }

        }



    }
}
