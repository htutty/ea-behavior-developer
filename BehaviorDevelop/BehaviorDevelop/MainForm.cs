using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using ElementEditor;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.writer;
using Microsoft.VisualBasic;
using AsciidocGenerator;
using System.Diagnostics;

namespace BehaviorDevelop
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		TreeNode rootNode;
		
		private IList<ArtifactVO> artifacts;
		
		private ElementForm elemForm { get; set; }

		private SearchElementListForm searchElemListForm { get; set; }

		private Dictionary<string, TreeNode> treeNodeMap = new Dictionary<string, TreeNode>();
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.artifacts = new List<ArtifactVO>();
			
			// projectPath = null;
		}


		public MainForm(string prjfile)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.artifacts = new List<ArtifactVO>();

			if( ProjectSetting.load(prjfile) ) {
				this.Text = prjfile;
			} else {
				MessageBox.Show("プロジェクトファイル読み込みに失敗しました。　再度正しいファイルを選択して下さい。");
				// projectPath = null;
			}
			
		}

		#region "フォームの初期化およびパッケージツリーの作成処理"
		
		void MainFormLoad(object sender, EventArgs e)
		{
			init();
		}

		
		private void init() {

			this.artifacts.Clear();
            this.rootNode = new TreeNode("ルート");
            this.treeView1.Nodes.Clear();
			this.treeNodeMap.Clear();
			if( this.tabControl1.TabPages.Count > 1 ) {
				this.tabControl1.TabPages.Clear();
			}
			
            // プロジェクトファイル(.bdprj)が読み込み済みの場合
			if (ProjectSetting.getVO() != null) {
				initProject();

                // 使用するIndexDBファイルの存在チェック
                //if ( !System.IO.File.Exists(ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName) ) {
                //	// 読み込みが終わるまでモーダルでスプラッシュ画面を開く
                //	SplashForm splashForm = new SplashForm();

                //	splashForm.Show();
                //	splashForm.CloseOnLoadComplete(ProjectSetting.getVO().projectPath, ProjectSetting.getVO().dbName);
                //}

                // 展開IndexDBが未作成の場合にここで作成する処理を、一旦、削除する。
                // TODO: IndexDBと要素毎に展開される成果物ファイルの存在が無いと、データ更新処理などで色々とできないことが出てくるため、
                // 　　　ProjectSettingなどでこのフラグを管理して、画面の動作モードを一元的に管理できるようにする。
                // 　　　既に "EAと接続済みか否か" の情報を持っていたりするので、その延長で。

            }

            AttachEA();			
		}
		
		
		private void initProject() {
			string artifactsFileName = ProjectSetting.getVO().artifactsFile;
            string artifactDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;

			// artifactList.Items.Clear();
			this.artifacts = ArtifactsXmlReader.readArtifactList(artifactDir, artifactsFileName);
				
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

		#endregion

		
		#region "外部から呼び出される公開メソッド"
		/// <summary>
		/// GUIDにより成果物リストを検索し、成果物パネルをアクティベートする。
		/// 主にクラス検索画面から要素編集画面を開くついでに呼び出される。
		/// </summary>
		/// <param name="guid">成果物パッケージのGUID</param>
		/// <returns>成果物VO</returns>
		public ArtifactVO getArtifactByGuid(string guid) {
			TreeNode tn = null;
            treeNodeMap.TryGetValue(guid, out tn);

            ArtifactVO atfvo = (ArtifactVO)tn.Tag;
			return activateArtifactPanel(atfvo);
		}
		
		/// <summary>
		/// 引数の要素VOで要素編集画面をモードレスで開く。
		/// 主にクラス検索画面から呼び出される。
		/// </summary>
		/// <param name="elemvo"></param>
		public void openNewElementForm(ElementVO elemvo) {
			ElementForm eForm = new ElementForm( ref elemvo );
			eForm.Show(this);			
		}
		
		/// <summary>
		/// 成果物パネルのアクティベート。
		/// 主にパッケージツリーで成果物PKGを選択した時に呼び出され、新たに成果物情報をファイルから読んで成果物パネルを表示する。
		/// あるいは既に成果物パネルの開いているタブ内で表示されている場合はそのタブをアクティベートして終了する。
		/// </summary>
		/// <param name="atf"></param>
		/// <returns></returns>
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

            string artifactDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;

            ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactDir, true);
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
		
		#endregion
		
		#region "成果物パネル内の作成処理"
		/// <summary>
		/// 成果物パネルの内容を作成する。
		/// </summary>
		/// <param name="elemPanel">処理対象のパネル（成果物タブ内のパネルを想定）</param>
		/// <param name="pac">処理対象の成果物パッケージ</param>
		private void makeElementsPanelContents(FlowLayoutPanel elemPanel, PackageVO pac) {
			addPackageLabels(elemPanel, pac, 0);
		}
		
		
		/// <summary>
		/// パッケージ階層に基づいてパッケージ・要素のラベルをパネルに追加する。（再帰処理）
		/// </summary>
		/// <param name="elemPanel"></param>
		/// <param name="pac"></param>
		/// <param name="depth"></param>
		private void addPackageLabels(FlowLayoutPanel elemPanel, PackageVO pac, int depth ) {
			Label pacLabel = new Label();
			pacLabel.Margin = new System.Windows.Forms.Padding(12*depth,3,3,3);
			pacLabel.Text = "□" + pac.name ;
			
			pacLabel.AutoSize = true ;
			elemPanel.Controls.Add(pacLabel);
			
			// パッケージ内の要素で変更済み(_changed)のファイルがいたら、その内容で置き換える
			pac.elements = replaceElementIfExistChangedData(pac.elements);
			addElementLabels(elemPanel, pac.elements, depth+1);
			
			foreach( PackageVO c in pac.childPackageList ) {
				addPackageLabels(elemPanel, c, depth+1);
			}
			
		}

		/// <summary>
		/// 引数の要素リストをなめ、その中に変更済(_changed)があればその内容で置き換えて返却する。
		/// このツール内で振る舞いを修正し、保存してあるとパスのelements内に変更済み(#GUID#_changed.xml)ファイルが作成されている。
		/// 成果物オブジェクトを作成した時は一旦変更済みファイルを無視して読み込まれるため、この処理で変更済みファイルを読み込み、
		/// リスト内の要素を変更済みのに置き換えて返却する。
		/// </summary>
		/// <param name="srcList"></param>
		/// <returns></returns>
		private List<ElementVO> replaceElementIfExistChangedData(List <ElementVO> srcList) {
			List<ElementVO> outList = new List<ElementVO>();
			foreach(ElementVO e in srcList) {
				if(ElementsXmlReader.existChangedElementFile(e.guid)) {
					outList.Add(ElementsXmlReader.readChangedElementFile(e.guid));
				} else {				
					outList.Add(e);
				}
			}
			return outList;
		}
		
		
		
		/// <summary>
		/// 成果物内の要素を、要素編集画面を開くリンク付きでパネルに表示する。
		/// </summary>
		/// <param name="elemPanel">表示すべき成果物パネル</param>
		/// <param name="elements">要素リスト</param>
		/// <param name="depth">ネストの深さ</param>
		private void addElementLabels(FlowLayoutPanel elemPanel, IList<ElementVO> elements, int depth ) {
            string nameLabel;
            string stereotypeStr;

			foreach( ElementVO elem in elements ) {

				if ( checkDisplayableType(elem.eaType) ) {
					LinkLabel labelElement = new LinkLabel();

                    if (elem.stereoType != null && elem.stereoType != "") {
                        stereotypeStr = "<<" + elem.stereoType + ">> ";
                    } else {
                        stereotypeStr = "";
                    }

                    // ノート要素の場合、もしくはname属性の中身が空の場合
                    if ("Notes".Equals(elem.eaType) || elem.name == null || elem.name == "" )
                    {
                        // ノートの始めの30文字分を出だしとして出力
                        if(elem.notes.Length > 30)
                        {
                            nameLabel = "(" + elem.eaType + ") " + elem.notes.Substring(0, 30) + "..";
                        } else {
                            nameLabel = "(" + elem.eaType + ") " + elem.notes;
                        }

                    } else {
                        nameLabel = "(" + elem.eaType + ") " + stereotypeStr + elem.name;
                    }

					switch(elem.changed) {
						case ' ':
							labelElement.Text = nameLabel;
							labelElement.Click += new System.EventHandler(this.LblElemOpenClick);
							break;
						case 'C':
						case 'D':
							labelElement.Text = nameLabel + " [" + elem.changed + "]";
							labelElement.Click += new System.EventHandler(this.LblElemOpenClick);
							break;

						case 'U':
							labelElement.Text = nameLabel + " [" + elem.changed + "]";
							labelElement.Click += new System.EventHandler(this.LblElemOpenClick);
							break;
					}

					labelElement.Margin = new System.Windows.Forms.Padding(12*depth, 3, 3, 3);
					labelElement.Tag = elem;
					labelElement.AutoSize = true;
					elemPanel.Controls.Add(labelElement);
				}				
			}

		}

        /// <summary>
        /// EA内の要素型から、成果物内の要素一覧に出力できるか
        /// </summary>
        /// <param name="eaType"></param>
        /// <returns></returns>
        private bool checkDisplayableType(string eaType)
        {
            string[] typeNames = { "Class", "Interface", "Enumeration", "GUIElement", "Screen",
                "Note", "Artifact", "UseCase", "Actor", "Object", "Requirement", "Action", "Activity"};

            foreach ( string tname in typeNames)
            {
                if (tname.Equals(eaType)) return true;
            }

            return false;
        }

        #endregion

        #region "メニューバーのメニュー項目のイベントハンドラ"

        /// <summary>
        /// メニュー - ファイル - 開く のメニュー項目のクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    // ProjectSetting.getVO().projectPath = Path.GetDirectoryName(prjfile);
					this.Text = prjfile;

					init();
			    } else {
					MessageBox.Show("プロジェクトファイル読み込みに失敗しました。　再度正しいファイルを選択して下さい。");
					// projectPath = null;
			    }
			}
			
		}
		
		/// <summary>
		/// メニュー - 検索 - クラスを検索のメニュー項目のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void SearchClassToolStripMenuItemClick(object sender, EventArgs e)
		{
			// 要素検索リスト画面を開く
			searchElemListForm = new SearchElementListForm( );
			searchElemListForm.Show(this);
		}
		
		/// <summary>
		/// メニュー - 編集 - テキストとしてコピー メニュー項目のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EditCopyTextToolStripMenuItemClick(object sender, EventArgs e)
		{
			TreeNode node = treeView1.SelectedNode;
			
			if (node != null && node.Tag != null ) {
				ArtifactVO atfvo = (ArtifactVO)node.Tag;

				try {
					Clipboard.SetText(atfvo.package.toDescriptorString());
					MessageBox.Show( "成果物情報テキストがクリップボードにコピーされました" );
					
				} catch(System.Runtime.InteropServices.ExternalException ex) {
					MessageBox.Show( "クリップボードの書き込みに失敗しました。\r\n" + ex.Message );
				}
			} else {
				MessageBox.Show( "フォルダツリーから成果物パッケージを選択して下さい" );
			}
		}

		
		/// <summary>
		///  メニュー - 編集 - （全）成果物を更新 メニュークリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EditRefreshArtifactToolStripMenuItemClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getEARepo();
		
			try {
				// 確認メッセージを表示
				MessageBox.Show( "全ての成果物パッケージの内容をEAから取得しローカルを更新します");
				
				// マウスカーソルを待機状態にする
				Cursor.Current = Cursors.WaitCursor;
			
				// 全成果物をなめ、それぞれの成果物パッケージをEAから取得してローカルを更新
				foreach( ArtifactVO atfvo in artifacts ) {
					
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
						MessageBox.Show(ex.Message);
					}
	
				}				

				// マウスカーソルを標準に戻す
				Cursor.Current = Cursors.Default;
				
				MessageBox.Show( "EAの最新情報をローカルに取り込みました" );
			} catch ( Exception ex ) {
				MessageBox.Show(ex.Message);
			}
			
		}
		
		
		/// <summary>
		/// メニュー - ファイル - 終了 メニュー項目のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ExitAppToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		
		void ViewGuidToolStripMenuItemClick(object sender, EventArgs e)
		{
			TreeNode tn = treeView1.SelectedNode ;
			ArtifactVO atf = (ArtifactVO)tn.Tag;
			MessageBox.Show("成果物パッケージGUID=" + atf.guid );
		}
		
		
		/// <summary>
		/// メニュー - EAにアタッチ のメニュー項目クリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void AttachEAToolStripMenuItemClick(object sender, EventArgs e)
		{
			AttachEA();
		}
		
		
		private void AttachEA() 
		{
			EA.App eaapp = null;
			
			if ( ProjectSetting.getVO() == null ) {
				toolStripStatusLabel1.Text = "[ファイル]メニューの[開く]を選択しプロジェクトをオープンしてください" ;
				return;
			}
			
			try {
				eaapp = (EA.App)Microsoft.VisualBasic.Interaction.GetObject(null, "EA.App");
				
				if( eaapp != null ) {
					EA.Repository repo = eaapp.Repository;
//					eaapp.Visible = true;
					ProjectSetting.getVO().eaRepo = repo;
					string connStr = repo.ConnectionString;
					if (connStr.Length > 50) {
						connStr = connStr.Substring(0,50);
					}
					toolStripStatusLabel1.Text = "EAへのアタッチ成功 EA接続先=" + connStr;
				} else {
					toolStripStatusLabel1.Text = "EAにアタッチできなかったため、EAへの反映機能は使えません";
				}
			} catch(Exception ex) {
				toolStripStatusLabel1.Text = "EAが起動していなかったため、EAへの反映機能は使えません : " + ex.Message;
				return;
			} 
			
		}

        #endregion

        /// <summary>
        /// ドキュメント出力 - 全AsciiDoc出力メニューのClickイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllAsciiDocOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ArtifactVO atf in artifacts)
            {

            }

        }

        /// <summary>
        /// 検索 - 成果物 メニューの Click イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void artifactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool foundFlg = false;
            string inputText = Interaction.InputBox("パッケージ名（一部）", "成果物検索", "");

            foreach (TreeNode tn in treeNodeMap.Values)
            {
                // 入力値がツリーノードの名前に一致したものが見つかったら
                if (tn.Name.IndexOf(inputText) > 0)
                {
                    // treeView上で、最初に見つかったノードにフォーカスする
                    treeView1.SelectedNode = tn;
                    treeView1.Focus();
                    foundFlg = true;
                    break;
                }

            }

            if (!foundFlg)
            {
                MessageBox.Show("入力されたキーワードにヒットした成果物はありませんでした");
            }

        }


        #region "メニュー以外のイベントハンドラ"

        /// <summary>
        /// タブの切り替え（SelectedIndexChanged）時のイベントハンドラ
        /// タブは開いている成果物パッケージの中身を示しているため、タブを切り替えた時に
        /// パッケージツリーの方もその成果物にフォーカスさせるために使用。
        /// （タブをたくさん開いていると、パッケージツリー上のどこにある成果物なのかが分からなくなるため）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabControl1SelectedIndexChanged(object sender, EventArgs e)
		{

            if(tabControl1.SelectedIndex < 0)
            {
                return;
            }

            string guid = (string)tabControl1.TabPages[tabControl1.SelectedIndex].Tag ;
            TreeNode tn = null;
            treeNodeMap.TryGetValue(guid, out tn);
            treeView1.SelectedNode = tn ;
            treeView1.Focus();
		}
		
		/// <summary>
		/// ツリービューのノード（特に成果物のノード）クリック時イベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TreeView1NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{

			if ( e.Node.Tag != null ) {
				// 待機状態
				Cursor.Current = Cursors.WaitCursor;
				
				// 成果物パネルを追加する（既にこの成果物がタブで開いているなら単にアクティベートする）
				activateArtifactPanel( (ArtifactVO)e.Node.Tag );
				
				// 標準に戻す
				Cursor.Current = Cursors.Default;
			}
		}


		/// <summary>
		/// ツリービューのコンテキストメニュー-「この成果物をEAで選択」のクリック時イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void FocusEAPackageToolStripMenuItemClick(object sender, EventArgs e)
		{
			TreeNode tn = treeView1.SelectedNode ;
			ArtifactVO atf = (ArtifactVO)tn.Tag;
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			
			if( repo != null ) {
				EA.Package pak =  repo.GetPackageByGuid( atf.guid );
				repo.ShowInProjectView(pak);
				MessageBox.Show("EA上で成果物パッケージGUID=" + atf.guid + "を選択しました");
			}

		}

        


		/// <summary>
		/// ツリービューのコンテキストメニュー-「この成果物をEAから取得」のクリック時イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void UpdateArtifactByEAToolStripMenuItemClick(object sender, EventArgs e)
		{
			TreeNode node = treeView1.SelectedNode;
			EA.Repository repo = ProjectSetting.getEARepo();
			
			if (node != null && node.Tag != null ) {
				ArtifactVO atfvo = (ArtifactVO)node.Tag;

				try {
					// 確認メッセージを表示
					MessageBox.Show( "選択されたパッケージの内容をEAから取得しローカルを更新します");
					
					// マウスカーソルを待機状態にする
					Cursor.Current = Cursors.WaitCursor;
				
					EA.Package atfPacObj = repo.GetPackageByGuid(atfvo.guid);
					
					EAArtifactXmlMaker maker = new EAArtifactXmlMaker(atfPacObj);
					ArtifactVO newArtifact = maker.makeArtifactXml();

					// 現在読み込まれている成果物の内容を今読んだもので置き換え
					atfvo.package = newArtifact.package;
					
					// elements配下の要素XMLファイルを今読んだもので置き換え
					ArtifactXmlWriter writer = new ArtifactXmlWriter();
					writer.rewriteElementXmlFiles(atfvo);
					
					// 標準に戻す
					Cursor.Current = Cursors.Default;
					
					MessageBox.Show( "EAの最新情報をローカルに取り込みました" );
				} catch ( Exception ex ) {
					MessageBox.Show(ex.Message);
				}

			} else {
				MessageBox.Show( "フォルダツリーから成果物パッケージを選択して下さい" );
			}
		}
		
		/// <summary>
		/// コンテキストメニュー:"Close Tab"選択時イベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
		{
			TabPage tabp = tabControl1.SelectedTab ;
			tabControl1.TabPages.Remove(tabControl1.SelectedTab);
		}
		
		/// <summary>
		/// コンテキストメニュー(タブ)を開く時のイベントハンドラ（メニュー項目の活性制御）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TabContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(tabControl1.TabPages.Count > 1) {
				closeTabToolStripMenuItem.Enabled = true;
			} else {
				closeTabToolStripMenuItem.Enabled = false;
			}
		}
		
		#endregion
		
		#region "成果物ペイン内の動的ボタンのイベントハンドラ"
		/// <summary>
		/// 要素リンクラベルのクリック時イベントハンドラ（要素編集画面を開く）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void LblElemOpenClick(object sender,  EventArgs e)
		{
			LinkLabel lbl = (LinkLabel)sender;
			ElementVO elem = (ElementVO)lbl.Tag;

            if( elem.changed == 'U' )
            {
                DiffElementForm eForm = new DiffElementForm(elem);
                eForm.Show(this);
            } else {
                openNewElementForm(elem);
            }
        }

        /// <summary>
        /// 要素リンクラベルのクリック時イベントハンドラ（要素差分確認画面を開く）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //		void BtnDiffElemOpenClick(object sender,  EventArgs e)
        //		{
        //			Button btn = (Button)sender;
        //			ElementVO elemvo = (ElementVO)btn.Tag;
        ////			MessageBox.Show( "guid =" + elem.guid );

        //			DiffElementForm eForm = new DiffElementForm( elemvo );
        //			eForm.Show(this);			
        //		}

        #endregion


        private void exportAsciidocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            ArtifactVO atf = (ArtifactVO)tn.Tag;

            // 出力フォルダの存在チェック＆無ければ作成
            string asciidocDir = ProjectSetting.getVO().projectPath + "\\" + "asciidocs";
            makeAsciidocDirIfNotExist(asciidocDir);

            string partGuid = atf.guid.Substring(1, 8);
            string adoctFileName = asciidocDir + "\\atf_" + filterSpecialChar(atf.name) + "_" + partGuid + ".adoc";

            ArtifactAsciidocWriter.outputAsciidocFile(atf, adoctFileName);

            // 出力が成功し、目的のAsciidocファイルが出力されていたら
            if( File.Exists(adoctFileName) )
            {
                Process p = Process.Start(adoctFileName);
                MessageBox.Show("Asciidocの出力が完了しました。\r\n" + adoctFileName);


            }

        }

        /// <summary>
        /// Asciidoc出力用フォルダを作る。
        /// 先に存在チェックし、なかった場合だけフォルダ作成を行う。
        /// </summary>
        /// <param name="asciidocDir">出力先asciidocパス</param>
        private static void makeAsciidocDirIfNotExist(string asciidocDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(asciidocDir))
            {
                Directory.CreateDirectory(asciidocDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + asciidocDir);
            }
        }


        /// <summary>
        /// 引数の文字列から、ファイル名に使用できない文字をフィルタして返却する
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static string filterSpecialChar(string orig)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
            sb.Replace("(", "（");
            sb.Replace(")", "）");
            sb.Replace(" ", "_");
            sb.Replace("^", "＾");
            sb.Replace("？", "");
            sb.Replace("/", "_");
            sb.Replace("\\", "_");
            sb.Replace("\r", "");
            sb.Replace("\n", "");

            return sb.ToString();
        }

    }
}
