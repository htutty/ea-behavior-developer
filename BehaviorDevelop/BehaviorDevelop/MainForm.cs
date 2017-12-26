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
using BehaviorDevelop.util;
using BehaviorDevelop.vo;
using System.IO;

namespace BehaviorDevelop
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		TreeNode rootNode = new TreeNode("ルート");
		
		IList<ArtifactVO> artifacts;
		
		string projectPath { get; set; }
		
		ElementForm elemForm { get; set; }		

		Dictionary<string, TreeNode> treeNodeMap = new Dictionary<string, TreeNode>();
		
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			string projectfile = "All_Artifacts.xml";
			projectPath = ConfigurationManager.AppSettings["artifact_dir"];
			
			this.Text = projectPath + "\\" + projectfile;
		}


		public MainForm(string prjfile)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			projectPath = Path.GetDirectoryName(prjfile);
			this.Text = prjfile;
		}

		void MainFormLoad(object sender, EventArgs e)
		{
			
			// artifactList.Items.Clear();
			artifacts = ArtifactsXmlReader.readArtifactList(projectPath);
					
			for ( int i=0; i < artifacts.Count; i++ ) {
				ArtifactVO atf = artifacts[i];
				TreeNode packageNode = addPackageNodes(atf.pathName);
				
				TreeNode atfNode = new TreeNode(atf.name, 2, 1);
//				atfNode.Tag = i;
				atfNode.Tag = atf;
				packageNode.Nodes.Add(atfNode);
				treeNodeMap.Add(atf.guid, atfNode);
			}

			this.treeView1.Nodes.Add(rootNode);
		}
		
		void MainFormLoad__(object sender, EventArgs e)
		{
//			tabControl1.TabPages.Clear();
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
		
//		void TreeView1NodeMouseClick__(object sender, TreeNodeMouseClickEventArgs e) {
//			if ( e.Node.Tag != null ) {
//				int idx = (int)e.Node.Tag;
//				atfForm = new ArtifactForm( artifacts[idx] );
//				atfForm.Show(this);
//				activateArtifactPanel( artifacts[idx] );
//			}
//		}
		

		private void activateArtifactPanel(ArtifactVO atf) {
			ArtifactXmlReader atfReader = new ArtifactXmlReader();
			// 成果物パッケージ別のXMLファイル読み込み
			atfReader.readArtifactDesc(atf, projectPath);

			// 新しく開くアーティファクト内のフォルダツリーを作成
			TreeView folderTree = new TreeView();
            folderTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
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
            makeElementsPanelContents(elemPanel, atf.package);			
			
			SplitContainer spCnt = new SplitContainer();
            spCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                    | System.Windows.Forms.AnchorStyles.Left)
                                    | System.Windows.Forms.AnchorStyles.Right)));

            spCnt.Orientation = System.Windows.Forms.Orientation.Horizontal;
			spCnt.Panel1.Controls.Add(folderTree);
			spCnt.Panel2.Controls.Add(elemPanel);
			
			TabPage atfPage = new TabPage();
			atfPage.Controls.Add(spCnt);
			atfPage.Text = ((atf.package.stereoType != null) ? "<<" + atf.package.stereoType + ">> " : "" ) + atf.name ;
//			atfPage.Text = atf.name ;
			
			atfPage.Tag = atf.guid ;
			tabControl1.TabPages.Add(atfPage) ;
		}

		
		private void makeElementsPanelContents(FlowLayoutPanel elemPanel, PackageVO pac) {
			addPackageLabels(elemPanel, pac, 0);
		}
		
		private void addPackageLabels(FlowLayoutPanel elemPanel, PackageVO pac, int depth ) {
			Label pacLabel = new Label();
			pacLabel.Text = "          ".Substring(1, depth) +"□" + pac.name ;
			pacLabel.AutoSize = true ;
			elemPanel.Controls.Add(pacLabel);
			
			foreach( PackageVO c in pac.childPackageList ) {
				addPackageLabels(elemPanel, c, depth+1);
				addElementLabels(elemPanel, c.elements, depth+1);
			}
			
		}

		
		private void addElementLabels( FlowLayoutPanel elemPanel, IList<ElementVO> elements, int depth ) {
			foreach( ElementVO elem in elements ) {
//				Label elemLabel = new Label();
//				elemLabel.Text = "          ".Substring(1, depth+1) +"■" + elem.name ;
//	            elemLabel.Size = new Size(340,20);
//	            
//				panel.Controls.Add(elemLabel);
				
				Button btnElemOpen = new Button();
				btnElemOpen.Click += new System.EventHandler(this.BtnElemOpenClick);
				btnElemOpen.Text = "■" + elem.name;
				btnElemOpen.Tag = elem;
				btnElemOpen.AutoSize = true ;
				elemPanel.Controls.Add(btnElemOpen);
				
//				addAttributeLabels(elem.attributes, depth+2);
//				addMethodLabels(elem.methods, depth+2);
			}

		}
		
		
		void BtnElemOpenClick(object sender,  EventArgs e)
		{
			Button btn = (Button)sender;
			ElementVO elem = (ElementVO)btn.Tag;
//			MessageBox.Show( "guid =" + elem.guid );
			elemForm = new ElementForm( elem );
			elemForm.Show(this);
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
//				MessageBox.Show("ノードクリック : " + e.Node + "/" + e.Node.Tag);
//				int idx = (int)e.Node.Tag;
				activateArtifactPanel( (ArtifactVO)e.Node.Tag );
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
			
		}
		
		void ClassToolStripMenuItemClick(object sender, EventArgs e)
		{
			
		}
		
		
	}
}
