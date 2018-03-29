/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/26
 * Time: 13:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace BehaviorDevelop
{
	/// <summary>
	/// Description of SplashForm.
	/// </summary>
	public partial class SplashForm : Form
	{
		Boolean loaded;
		
		public SplashForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			loaded = false;
		}

		void SplashFormLoad(object sender, EventArgs e)
		{
			loaded = true; 
		}
		
		public void CloseOnLoadComplete(string projectPath, string dbfile) {
			
			if (!this.loaded) {
				Int32 count;
				for(count=0; count < 1000; count++ ) {
					if( this.loaded ) {
						break;
					} else {
						System.Threading.Thread.Sleep(100);
					}
				}
			}
			
			// このプログラム(.exe)が置かれたフォルダを取得
			string currentDir = System.Windows.Forms.Application.StartupPath ;

			//ファイルを開いて終わるまで待機する
			Process p = Process.Start( currentDir + "\\ElementIndexer.exe", projectPath + " " + dbfile );
//					MessageBox.Show("内部データベースを構築します。\n構築処理が完了後、OKボタンを押してください");
			
			// サブプロセスが終了後、必要なくなったところでCLOSE
			p.WaitForExit();
			
			this.Close();
		}
	}
}
