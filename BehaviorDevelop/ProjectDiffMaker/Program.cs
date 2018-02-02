/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/31
 * Time: 9:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ProjectDiffMaker
{
	class Program
	{
		public static void Main(string[] args)
		{
			//コマンドライン引数を配列で取得する
			string[] cmds = System.Environment.GetCommandLineArgs();

			if (cmds.Length >= 2) {
				ArtifactsDiffer differ = new ArtifactsDiffer(cmds[1], cmds[2]);
				differ.readBothArtifacts();
				differ.mergeAllArtifacts();
				differ.outputMerged(cmds[3]);
			} else {
				Console.WriteLine("引数が足りません");
				Console.WriteLine("usage: ProjectDiffMaker.exe <比較元.bdprj> <比較先.bdprj> <diffOutputDir> ");
			}
			
//			Console.Write("Press any key to continue . . . ");
//			Console.ReadKey(true);
		}
	}
}