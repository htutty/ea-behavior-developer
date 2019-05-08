/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/10/26
 * Time: 10:06
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Forms;
using ArtifactFileAccessor.util;

namespace BehaviorDevelop
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//コマンドライン引数を配列で取得する
			string[] cmds = System.Environment.GetCommandLineArgs();
			MainForm form = null;

			if (cmds != null && cmds.Length > 1) {
//				string lin = "";
//				//コマンドライン引数を列挙する
//				for ( int i=0; i < cmds.Length; i++ ) {
//					string cmd = cmds[i];
//					lin = lin + i + ":" + cmd + ", ";
//				}
//				MessageBox.Show("コマンドライン引数 = " + lin);

				form = new MainForm(cmds[1]);
			} else {
				form = new MainForm();
			}

			// プロファイルディレクトリを参照し、無ければ作成
			checkAndMakeProfileDir();

			Application.Run(form);
		}


		private static void checkAndMakeProfileDir() {
			string pdir = ProjectSetting.GetAppProfileDir();
			if (!Directory.Exists(pdir)) {
				Directory.CreateDirectory(pdir);
			}
		}

	}
}
