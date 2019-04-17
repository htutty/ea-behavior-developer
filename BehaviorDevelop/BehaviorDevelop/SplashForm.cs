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
using IndexAccessor;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.vo;
using System.Collections.Generic;
using System.Xml;

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
			//string currentDir = System.Windows.Forms.Application.StartupPath ;

            //ファイルを開いて終わるまで待機する
            // Process p = Process.Start( currentDir + "\\ElementIndexer.exe", projectPath + " " + dbfile );
            //					MessageBox.Show("内部データベースを構築します。\n構築処理が完了後、OKボタンを押してください");

            try
            {
                ProjectSetting.load(projectPath + "\\project.bdprj");

                // 全成果物のリストを読み込み
                string artifactsDir = projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
                List<ArtifactVO> artifacts = ArtifactsXmlReader.readArtifactList(artifactsDir, ProjectSetting.getVO().artifactsFile);

                // 全成果物リストの内容を成果物XMLファイルで埋める
                ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactsDir);
                foreach (ArtifactVO atf in artifacts)
                {
                    // 成果物別のXMLファイルの内容読み込み
                    atfReader.readArtifactDesc(atf);
                }

                ArtifactsVO allArtifacts = new ArtifactsVO();
                allArtifacts.artifactList = artifacts;

                ConnectorXmlReader connReader = new ConnectorXmlReader(projectPath);
                List<ConnectorVO> allConnectors = connReader.readConnectorAll();

                IndexDbWriter dbWriter = new IndexDbWriter(projectPath, ProjectSetting.getVO().dbName);

                // 全接続情報をDBに登録
                dbWriter.writeAllConnector(allConnectors);
                // 全要素情報をDBに登録
                dbWriter.writeAllElements(allArtifacts);
                // 全ふるまい情報をDBに登録
                dbWriter.writeAllBehaviors(allArtifacts);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            // サブプロセスが終了後、必要なくなったところでCLOSE
//            p.WaitForExit();
			
			this.Close();
		}
	}
}
