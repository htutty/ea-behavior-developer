/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/08/11
 * Time: 14:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Forms;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
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
            if (args.Length >= 3 && args[0] == "-local") 
            {
                string projectFile = args[1];
                string elemGuid = args[2];
                runOnLocalFileMode(projectFile, elemGuid);
            }
            else if( args.Length < 1 )
            {
                runOnDependEaMode();
            }
            else
            {
                MessageBox.Show("起動パラメータを付ける場合は以下を指定してください :\r\n" 
                              + "   ElementEditor.exe -local <projectPath> <elementGuid> " );
            }
		}


        private static void runOnLocalFileMode( string projectFile, string elemGuid)
        {
            // プロジェクトファイルの存在チェック
            if( System.IO.File.Exists(projectFile) )
            {
                // プロジェクト設定ファイルのロード
                ProjectSetting.load(projectFile);
            }
            else
            {
                MessageBox.Show("指定されたプロジェクトファイルが見つかりません");
                return;
            }

            try
            {
                // 要素ファイルからGUIDで検索した結果を読み込む
                ElementVO elemvo = ElementsXmlReader.readElementFile(elemGuid);

                ElementForm elementForm = new ElementForm(ref elemvo);
                Application.Run(elementForm);
            }
            catch ( Exception ex )
            {
                MessageBox.Show(ex.Message, "エラー発生");
            }
        }


        private static void runOnDependEaMode()
        {
            EA.App eaapp = null;
            try
            {
                eaapp = (EA.App)Microsoft.VisualBasic.Interaction.GetObject(null, "EA.App");
                EA.Repository repo = eaapp.Repository;

                // EA.Repository repo = new EA.Repository();
                //  repo.OpenFile2(@"C:\DesignHistory\staging\EAPBAK_cuvic_aswea_20190124_20181029.eap", "admin", "p@ssw0rd");
                // repo.OpenFile2(@"C:\DesignHistory\ea-artifact-manage\shortcut.eap", "admin", "p@ssw0rd");
                // repo.App.Visible = true;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (eaapp != null)
                {
                    ProjectSetting.load(@"C:\DesignHistory\ea-artifact-manage\project.bdprj");
                    // projectsettingにEAリポジトリオブジェクトをセット
                    ProjectSetting.getVO().eaRepo = repo;

                    string connStr = repo.ConnectionString;
                    Console.WriteLine("EAへのアタッチ成功 EA接続先=" + connStr);

                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("EAにアタッチできなかったため、異常終了します");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("EAへの接続時に例外が発生しました。 msg=" + ex.Message);
                // Console.WriteLine("EAへの接続時に例外が発生しました。 msg=" + ex.Message);
            }
        }

    }

}
