/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/31
 * Time: 9:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace ProjectDiffMaker
{
	class Program
	{
		public static void Main(string[] args)
		{
			//コマンドライン引数を配列で取得する
			string[] cmds = System.Environment.GetCommandLineArgs();

			if (cmds.Length >= 4) {
				ArtifactsDiffer differ = new ArtifactsDiffer(cmds[2], cmds[3]);
				
				if ( "-skipnotes".Equals(cmds[1]) ) {
					differ.skipElementTPosFlg = true;
					differ.skipAttributeNoteFlg = true;
					differ.skipMethodNoteFlg = true;
					differ.skipAttributePosFlg = true;
					differ.skipMethodPosFlg = true;
				} else if ( "-all".Equals(cmds[1]) ) {
					differ.skipElementTPosFlg = false;
					differ.skipAttributeNoteFlg = false;
					differ.skipMethodNoteFlg = false;
					differ.skipAttributePosFlg = false;
					differ.skipMethodPosFlg = false;
				}
				
				// outputDirで指定される出力フォルダを作成する（存在しない場合）
				createOutputDirIfNotExist(cmds[4]) ;
				differ.outputDir = cmds[4];
				
				differ.readBothArtifacts();
				
				differ.mergeAllArtifacts();
				differ.outputMerged();
			} else {
				Console.WriteLine("引数が足りません");
				Console.WriteLine("usage: ProjectDiffMaker.exe <動作モード(-all/-skipnotes)> <比較元Dir> <比較先Dir> <diffOutputDir> ");
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