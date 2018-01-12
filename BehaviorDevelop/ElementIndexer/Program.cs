/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/12/27
 * Time: 9:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ElementIndexer
{
	class Program
	{
		public static void Main(string[] args)
		{
			//コマンドライン引数を配列で取得する
			string[] cmds = System.Environment.GetCommandLineArgs();

			if (cmds.Length >= 2) {
				DatabaseWriter writer = new DatabaseWriter(cmds[1], cmds[2]);
				writer.writeAllConnector();
				writer.writeElements();
			} else {
				Console.WriteLine("引数が足りません");
				Console.WriteLine("usage: ElementIndexer.exe <targetDir> <db_file>");
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}