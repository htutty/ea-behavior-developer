/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/10/26
 * Time: 10:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BehaviorDevelop.util;
using BehaviorDevelop.vo;

namespace BehaviorDevelop
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		TreeNode rootNode = new TreeNode("ルート");
		
		private IList<ArtifactVO> artifacts;
		
		public string projectPath { get; set; }
		private ElementForm elemForm { get; set; }
		private SearchElementListForm searchElemListForm { get; set; }

		private Dictionary<string, TreeNode> treeNodeMap = new Dictionary<string, TreeNode>();
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			projectPath = null;
		}


		public MainForm(string prjfile)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			if( ProjectSetting.load(prjfile) ) {
				projectPath = Path.GetDirectoryName(prjfile);
				this.Text = prjfile;
			} else {
				MessageBox.Show("プロジェクトファイル読み込みに失敗しました。　再度正しいファイルを選択して下さい。");
				projectPath = null;
			}
			
		}

		
		void MainFormLoad(object sender, EventArgs e)
		{
			init();
		}

		
		private void init() {
			this.treeView1.Nodes.Clear();
			this.treeNodeMap.Clear();
			if( this.tabControl1.TabPages.Count > 1 ) {
				this.tabControl1.TabPages.Clear();
			}
				
			if ( this.projectPath != null) {
				initProject();
				// 使用するDBファイルの存在チェック
				if ( !existDbFile(ProjectSetting.getVO().dbName) ) {
					// 読み込みが終わるまでモーダルでスプラッシュ画面を開く
					SplashForm splashForm = new SplashForm();
					
					splashForm.Show();
					splashForm.CloseOnLoadComplete(this.projectPath, ProjectSetting.getVO().dbName);
				}
			}
			
		}
		
		private Boolean existDbFile(string dbpath) {
			return File.Exists(dbpath);
		}
		
		
		private void initProject() {
			string artifactsFileName = ProjectSetting.getVO().artifactsFile;
			// artifactList.Items.Clear();
			this.artifacts = ArtifactsXmlReader.readArtifactList(projectPath, artifactsFileName);
				
			string atfnodename;
			for ( int i=0; i < artifacts.Count; i++ ) {
				ArtifactVO atf = artifacts[i];
				TreeNode packageNode = addPackageNodes(atf.pathName);
				
				if (atf.changed == ' ' ) {
					atfnodename = atf.name;
				} else {
					atfnodename = atf.name + " [" + atf.changed + "]" ;
				}
				TreeNode atfNode = new TreeNode(atfnodename, 2, 1);
				atfNode.Tag = atf;
				atfNode.ContextMenuStrip = contextMenuStrip1;
				
				packageNode.Nodes.Add(atfNode);
				treeNodeMap.Add(atf.guid, atfNode);
			}

			this.treeView1.Nodes.Add(rootNode);
		}

		
		private TreeNode addPackageNodes( string pathName ) {
			char[] delimiterChars = { '/' };
			String[] paths = pathName.Split(delimiterChars);
			
			TreeNode mynode = rootNode;
			for (int i = 0; i < paths.Length; i++) {
				string s = paths[i];
				if ( !s.Equals("") ) {
					mynode = searchSubNodeByName(mynode,s);
				}
	        }
			return mynode;
		}
		

		private TreeNode searchSubNodeByName( TreeNode myNode, String name ) {
			foreach( TreeNode n in myNode.Nodes ) {
				if ( n.Text.Equals(name) == true ) {
			    	return n ; 
			    }
			}
			
			TreeNode newNode = new TreeNode(name, 0, 1);
			myNode.Nodes.Add(newNode);
			return newNode;
		}
		
		
		public ArtifactVO activateArtifactPanel(ArtifactVO atf) {
			// 既にタブページで対象の成果物を開いていないかをチェックする
			foreach( TabPage page in tabControl1.TabPages ) {
				// 開こうとしている成果物のGUIDとページで記憶しているGUIDが一致したら
				string guid = (string)(page.Tag);
				if (atf.guid.Equals(guid)) {
					// 該当のタブページを選択状態にし、終了
					tabControl1.SelectedTab = page;
					return atf;
				}
			}
			
			ArtifactXmlReader atfReader = new ArtifactXmlReader(projectPath);
			// 成果物パッケージ別のXMLファイル読み込み
			atfReader.readArtifactDesc(atf);

			// 新しく開くアーティファクト内のフォルダツリーを作成
			TreeView folderTree = new TreeView();
            folderTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));

            folderTree.ImageIndex = 0;
            folderTree.ImageList = this.imageList1;
            folderTree.Nodes.Add(makePackageNode(null, atf.package, true));
			folderTree.ExpandAll();

			// アーティファクト内の要素リストを作成
			FlowLayoutPanel elemPanel = new FlowLayoutPanel();
            elemPanel.Anchor = ((System.Windows.Forms.AnchorStyles)
                ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)| System.Windows.Forms.AnchorStyles.Right)));
            elemPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            elemPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            elemPanel.WrapContents = false;
            elemPanel.AutoScroll = true;
            makeElementsPanelContents(elemPanel, atf.package);			
			
			TabPage atfPage = new TabPage();
			atfPage.Controls.Add(elemPanel);

//			atfPage.Text = ((atf.package.stereoType != null) ? "<<" + atf.package.stereoType + ">> " : "" ) + atf.name ;
			atfPage.Text = atf.name ;
			atfPage.Tag = atf.guid ;
			// タブクローズ用のコンテキストメニューをTabPageに登録
			atfPage.ContextMenuStrip = tabContextMenuStrip;
			
			// 作成したタブページをタブコントロールに追加し、そのタブを選択状態にする
			tabControl1.TabPages.Add(atfPage);
			tabControl1.SelectedTab = atfPage;
			
			return atf ;
		}

		
		private void makeElementsPanelContents(FlowLayoutPanel elemPanel, PackageVO pac) {
			addPackageLabels(elemPanel, pac, 0);
		}
		
		private void addPackageLabels(FlowLayoutPanel elemPanel, PackageVO pac, int depth ) {
			Label pacLabel = new Label();
			pacLabel.Text = "          ".Substring(1, depth) +"□" + pac.name ;
			pacLabel.AutoSize = true ;
			elemPanel.Controls.Add(pacLabel);

			addElementLabels(elemPanel, pac.elements, depth+1);
			
			foreach( PackageVO c in pac.childPackageList ) {
				addPackageLabels(elemPanel, c, depth+1);
			}
			
		}

		/// <summary>
		/// 成果物パッケージ内の要素
		/// </summary>
		/// <param name="elemPanel"></param>
		/// <param name="elements"></param>
		/// <param name="depth"></param>
		private void addElementLabels(FlowLayoutPanel elemPanel, IList<ElementVO> elements, int depth ) {
			foreach( ElementVO elem in elements ) {

				if ( !"Note".Equals(elem.eaType ) ) {
					Button btnElemOpen = new Button();
					btnElemOpen.Click += new System.EventHandler(this.BtnElemOpenClick);
					if( elem.changed == ' ' ) {
						btnElemOpen.Text = "■" + elem.name;
					} else {
						btnElemOpen.Text = "■" + elem.name + " [" + elem.changed + "]";
					}

					btnElemOpen.Tag = elem;
					btnElemOpen.AutoSize = true ;
					elemPanel.Controls.Add(btnElemOpen);
				}				
			}

		}
		
		
		void BtnElemOpenClick(object sender,  EventArgs e)
		{
			Button btn = (Button)sender;
			ElementVO elem = (ElementVO)btn.Tag;
//			MessageBox.Show( "guid =" + elem.guid );
			openNewElementForm(elem);
		}
		
		
		public void openNewElementForm(ElementVO elemvo) {
			ElementForm eForm = new ElementForm( elemvo );
			eForm.Show(this);			
		}
		
		
		private TreeNode makePackageNode( TreeNode parentNode, PackageVO pac, Boolean isRoot ) {
			TreeNode targetNode = new TreeNode(pac.name, 0, 1);
			if( isRoot == false ) {
				parentNode.Nodes.Add(targetNode);
			}
			
			foreach( PackageVO c in pac.childPackageList ) {
				makePackageNode( targetNode, c, false );
			}
			
			return targetNode;
		}
				
		
		void TreeView1NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{

			if ( e.Node.Tag != null ) {
				
				// 待機状態
				Cursor.Current = Cursors.WaitCursor;

//				MessageBox.Show("ノードクリック : " + e.Node + "/" + e.Node.Tag);
//				int idx = (int)e.Node.Tag;
				activateArtifactPanel( (ArtifactVO)e.Node.Tag );
				
				// 標準に戻す
				Cursor.Current = Cursors.Default;
			}		
		}
		
		void TabControl1SelectedIndexChanged(object sender, EventArgs e)
		{
            string guid = (string)tabControl1.TabPages[tabControl1.SelectedIndex].Tag ;
            TreeNode tn = null;
            treeNodeMap.TryGetValue(guid, out tn);
            treeView1.SelectedNode = tn ;
            treeView1.Focus();
		}
		
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			//OpenFileDialogクラスのインスタンスを作成
			OpenFileDialog dialog = new OpenFileDialog();
			
			//はじめに「ファイル名」で表示される文字列を指定する
			dialog.FileName = "project.bdprj";
			//はじめに表示されるフォルダを指定する
			//指定しない（空の文字列）の時は、現在のディレクトリが表示される
//			dialog.InitialDirectory = @"C:\";
			dialog.InitialDirectory = "";

			//[ファイルの種類]に表示される選択肢を指定する
			//指定しないとすべてのファイルが表示される
			dialog.Filter = "振る舞いプロジェクトファイル(*.bdprj)|*.bdprj|すべてのファイル(*.*)|*.*";
			//[ファイルの種類]ではじめに選択されるものを指定する
			//1番目の「すべてのファイル」が選択されているようにする
			dialog.FilterIndex = 1;
			//タイトルを設定する
			dialog.Title = "開くプロジェクトファイルを選択してください";
			//ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
			dialog.RestoreDirectory = true;
			//存在しないファイルの名前が指定されたとき警告を表示する
			//デフォルトでTrueなので指定する必要はない
			dialog.CheckFileExists = true;
			//存在しないパスが指定されたとき警告を表示する
			//デフォルトでTrueなので指定する必要はない
			dialog.CheckPathExists = true;
			
			//ダイアログを表示する
			if (dialog.ShowDialog() == DialogResult.OK)
			{
			    //OKボタンがクリックされたとき、選択されたファイル名を表示する
			    Console.WriteLine(dialog.FileName);


			    string prjfile = dialog.FileName;
			    if (ProjectSetting.load(prjfile)) {
				    this.projectPath = Path.GetDirectoryName(prjfile);
					this.Text = prjfile;
					
//					this.MainFormLoad(this, null);
					init();
			    } else {
					MessageBox.Show("プロジェクトファイル読み込みに失敗しました。　再度正しいファイルを選択して下さい。");
					projectPath = null;
			    }
			}
			
		}
		
		void ClassToolStripMenuItemClick(object sender, EventArgs e)
		{
			searchElemListForm = new SearchElementListForm( );
			searchElemListForm.Show(this);
		}
		
		public ArtifactVO getArtifactByGuid(string guid) {
			TreeNode tn = null;
            treeNodeMap.TryGetValue(guid, out tn);

            ArtifactVO atfvo = (ArtifactVO)tn.Tag;
			return activateArtifactPanel(atfvo);
		}
		
		
		void EditCopyTextToolStripMenuItemClick(object sender, EventArgs e)
		{
			TreeNode node = treeView1.SelectedNode;
			
			if (node != null && node.Tag != null ) {
				ArtifactVO atfvo = (ArtifactVO)node.Tag;

				try {
					Clipboard.SetText(atfvo.package.toDescriptorString());
					MessageBox.Show( "成果物情報がクリップボードにコピーされました" );
				} catch(System.Runtime.InteropServices.ExternalException ex) {
					MessageBox.Show( "クリップボードの書き込みに失敗しました。\r\n" + ex.Message );
				}
			} else {
				MessageBox.Show( "フォルダツリーから成果物パッケージを選択して下さい" );
			}
		}
		
		void ExitAppToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		
		void ViewGuidToolStripMenuItemClick(object sender, EventArgs e)
		{
			TreeNode tn = treeView1.SelectedNode ;
			ArtifactVO atf = (ArtifactVO)tn.Tag;
			MessageBox.Show("GUID=" + atf.guid);
		}
		
		
		
		void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
		{
			TabPage tabp = tabControl1.SelectedTab ;
			tabControl1.TabPages.Remove(tabControl1.SelectedTab);
			
			MessageBox.Show("選択されているタブページがクローズされたよ");
		}
	}
}
