using System;
using System.IO;

namespace ProjectDiffMaker
{
	class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length >= 4) {
                string modeStr, srcProj, destProj, diffOutputDir;

                modeStr = args[0];
                srcProj = args[1];
                destProj = args[2];
                diffOutputDir = args[3];

                ArtifactsDiffer differ = new ArtifactsDiffer(srcProj, destProj, true);

                if ( "-skipnotes".Equals(modeStr) ) {
					differ.skipElementTPosFlg = true;
					differ.skipAttributeNoteFlg = true;
					differ.skipMethodNoteFlg = true;
					differ.skipAttributePosFlg = true;
					differ.skipMethodPosFlg = true;
				} else if ( "-all".Equals(modeStr) ) {
					differ.skipElementTPosFlg = false;
					differ.skipAttributeNoteFlg = false;
					differ.skipMethodNoteFlg = false;
					differ.skipAttributePosFlg = false;
					differ.skipMethodPosFlg = false;
				}

				// outputDirで指定される出力フォルダを作成する（存在しない場合）
				createOutputDirIfNotExist(diffOutputDir) ;

                // 比較クラスに出力フォルダパスを渡す
				differ.outputDir = diffOutputDir;

                // Left,Right両方の成果物をファイルから読み込み
				differ.readBothArtifacts();
                // 成果物単位でのマージ実行
				differ.mergeAllArtifacts();
                // マージ後に残った差異分の成果物をファイル出力
                differ.outputMerged();
			} else {
				Console.WriteLine("引数が足りません");
				Console.WriteLine("usage: ProjectDiffMaker.exe <動作モード(-all/-skipnotes)> <比較元Project(.bdprj)> <比較先Project(.bdprj)> <diffOutputDir> ");
			}
			
//			Console.Write("Press any key to continue . . . ");
//			Console.ReadKey(true);
		}

		
		private static void createOutputDirIfNotExist(string outputDir) {
			// outputDir およびその配下の detail が存在しない場合
			if ( !Directory.Exists(outputDir) ) {
				Directory.CreateDirectory( outputDir + "\\detail") ;
				Console.WriteLine("出力ディレクトリを作成しました。 : " + outputDir + "\\detail");
			}
		}
		
	}
}