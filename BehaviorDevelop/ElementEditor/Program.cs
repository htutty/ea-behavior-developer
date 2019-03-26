/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/08/11
 * Time: 14:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using BDFileReader.util;
using BDFileReader.vo;
using EA;


namespace ElementEditor
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
            EA.App eaapp = null;

			try {
				eaapp = (EA.App)Microsoft.VisualBasic.Interaction.GetObject(null, "EA.App");
                EA.Repository repo = eaapp.Repository;

                // EA.Repository repo = new EA.Repository();
                //  repo.OpenFile2(@"C:\DesignHistory\staging\EAPBAK_cuvic_aswea_20190124_20181029.eap", "admin", "p@ssw0rd");
                // repo.OpenFile2(@"C:\DesignHistory\ea-artifact-manage\shortcut.eap", "admin", "p@ssw0rd");
                // repo.App.Visible = true;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if ( eaapp != null ) {					
					ProjectSetting.load(@"C:\DesignHistory\ea-artifact-manage\project.bdprj");
					// projectsettingにEAリポジトリオブジェクトをセット
					ProjectSetting.getVO().eaRepo = repo;

					string connStr = repo.ConnectionString;
					Console.WriteLine("EAへのアタッチ成功 EA接続先=" + connStr);

                    Application.Run(new MainForm());
                }
                else {
                    MessageBox.Show("EAにアタッチできなかったため、異常終了します");
				}
			} catch(Exception ex) {
                MessageBox.Show("EAへの接続時に例外が発生しました。 msg=" + ex.Message);
                // Console.WriteLine("EAへの接続時に例外が発生しました。 msg=" + ex.Message);
            } 

		}
		
	}
}
