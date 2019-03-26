/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/06/19
 * Time: 9:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;

using EA;
using BDFileReader.vo;
using BDFileReader.util;
using BDFileReader.reader;
using BDFileReader.writer;

namespace EAArtifactLoader
{
	class Program
	{
		public static void Main(string[] args)
		{
						
			//コマンドライン引数を配列で取得する
			string[] cmds = System.Environment.GetCommandLineArgs();

			string prjfile, projectPath;
			if (cmds != null && cmds.Length > 1) {
				prjfile = cmds[1];
			} else {
				Console.WriteLine("引数が足りません");
				Console.WriteLine("Usage: EAArtifactLoader.exe <projectFilePath>");
				return;
			}		

			if( ProjectSetting.load(prjfile) ) {
				projectPath = Path.GetDirectoryName(prjfile);
				Console.WriteLine("プロジェクトファイルを読み込み : " + prjfile );
			} else {
				Console.WriteLine("プロジェクトファイル読み込みに失敗しました。　再度正しいファイルを選択して下さい。");
				return;
			}
			
			EA.App eaapp = (EA.App)Microsoft.VisualBasic.Interaction.GetObject(null, "EA.App");
			EA.Repository repo;

			if( eaapp != null ) {
				repo = eaapp.Repository;
				ProjectSetting.getVO().eaRepo = repo;
				Console.WriteLine("EAにアタッチ成功 : 接続文字列 = " + repo.ConnectionString );
			} else {
				Console.WriteLine("EAにアタッチできないため処理できません。終了します。");
				return;
			}

			// All_Artifact.xml から全成果物リストを取得
			Console.WriteLine("全成果物XMLファイルを読み込み :");
			string artifactsFileName = ProjectSetting.getVO().artifactsFile;
			List<ArtifactVO> artifacts = ArtifactsXmlReader.readArtifactList(projectPath, artifactsFileName);

			Console.WriteLine("成果物パッケージの件数： " + artifacts.Count);
			
			// 全成果物をなめ、それぞれの成果物パッケージをEAから取得してローカルを更新
			for(int i=0; i < artifacts.Count; i++) {
				ArtifactVO atfvo = artifacts[i];
				Console.WriteLine( i + ": guid=" + atfvo.guid + ", name=" + atfvo.name);
				
				try {
					EA.Package atfPacObj = repo.GetPackageByGuid(atfvo.guid);
					
					EAArtifactXmlMaker maker = new EAArtifactXmlMaker(atfPacObj);
					ArtifactVO newArtifact = maker.makeArtifactXml();

					// 現在読み込まれている成果物の内容を今読んだもので置き換え
					atfvo.package = newArtifact.package;
					
					// elements配下の要素XMLファイルを今読んだもので置き換え
					ArtifactXmlWriter writer = new ArtifactXmlWriter();
					writer.rewriteElementXmlFiles(atfvo);
					
				} catch ( Exception ex ) {
					Console.WriteLine(ex.Message);
				}

			}				
		
		}
		
	}
}