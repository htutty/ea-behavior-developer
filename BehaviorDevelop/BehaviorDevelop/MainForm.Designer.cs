/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/10/26
 * Time: 10:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace BehaviorDevelop
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ViewGuidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.focusEAPackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateArtifactByEAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AttachEAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditRefreshArtifactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.artifactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.tabContextMenuStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(12, 34);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(350, 485);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1NodeMouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ICON_FOLDER_CLOSING.png");
            this.imageList1.Images.SetKeyName(1, "ICON_COLDER_OPEN.png");
            this.imageList1.Images.SetKeyName(2, "ICON_FOLDER_ARTIFACT.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewGuidToolStripMenuItem,
            this.focusEAPackageToolStripMenuItem,
            this.updateArtifactByEAToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(208, 70);
            // 
            // ViewGuidToolStripMenuItem
            // 
            this.ViewGuidToolStripMenuItem.Name = "ViewGuidToolStripMenuItem";
            this.ViewGuidToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.ViewGuidToolStripMenuItem.Text = "パッケージGUIDを表示";
            this.ViewGuidToolStripMenuItem.Click += new System.EventHandler(this.ViewGuidToolStripMenuItemClick);
            // 
            // focusEAPackageToolStripMenuItem
            // 
            this.focusEAPackageToolStripMenuItem.Name = "focusEAPackageToolStripMenuItem";
            this.focusEAPackageToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.focusEAPackageToolStripMenuItem.Text = "この成果物PKGをEAで選択";
            this.focusEAPackageToolStripMenuItem.Click += new System.EventHandler(this.FocusEAPackageToolStripMenuItemClick);
            // 
            // updateArtifactByEAToolStripMenuItem
            // 
            this.updateArtifactByEAToolStripMenuItem.Name = "updateArtifactByEAToolStripMenuItem";
            this.updateArtifactByEAToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.updateArtifactByEAToolStripMenuItem.Text = "この成果物をEAから取得";
            this.updateArtifactByEAToolStripMenuItem.Click += new System.EventHandler(this.UpdateArtifactByEAToolStripMenuItemClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Location = new System.Drawing.Point(386, 29);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(634, 485);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1SelectedIndexChanged);
            // 
            // tabContextMenuStrip
            // 
            this.tabContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeTabToolStripMenuItem});
            this.tabContextMenuStrip.Name = "tabContextMenuStrip";
            this.tabContextMenuStrip.Size = new System.Drawing.Size(124, 26);
            this.tabContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.TabContextMenuStripOpening);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.closeTabToolStripMenuItem.Text = "Close Tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.CloseTabToolStripMenuItemClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1041, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.AttachEAToolStripMenuItem,
            this.ExitAppToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.fileToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.openToolStripMenuItem.Text = "開く(&O)";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // AttachEAToolStripMenuItem
            // 
            this.AttachEAToolStripMenuItem.Name = "AttachEAToolStripMenuItem";
            this.AttachEAToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.AttachEAToolStripMenuItem.Text = "EAにアタッチ";
            this.AttachEAToolStripMenuItem.Click += new System.EventHandler(this.AttachEAToolStripMenuItemClick);
            // 
            // ExitAppToolStripMenuItem
            // 
            this.ExitAppToolStripMenuItem.Name = "ExitAppToolStripMenuItem";
            this.ExitAppToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.ExitAppToolStripMenuItem.Text = "終了(&X)";
            this.ExitAppToolStripMenuItem.Click += new System.EventHandler(this.ExitAppToolStripMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCopyTextToolStripMenuItem,
            this.EditRefreshArtifactToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.editToolStripMenuItem.Text = "編集(&E)";
            // 
            // editCopyTextToolStripMenuItem
            // 
            this.editCopyTextToolStripMenuItem.Name = "editCopyTextToolStripMenuItem";
            this.editCopyTextToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.editCopyTextToolStripMenuItem.Text = "テキストとしてコピー";
            this.editCopyTextToolStripMenuItem.Click += new System.EventHandler(this.EditCopyTextToolStripMenuItemClick);
            // 
            // EditRefreshArtifactToolStripMenuItem
            // 
            this.EditRefreshArtifactToolStripMenuItem.Name = "EditRefreshArtifactToolStripMenuItem";
            this.EditRefreshArtifactToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.EditRefreshArtifactToolStripMenuItem.Text = "成果物情報更新";
            this.EditRefreshArtifactToolStripMenuItem.Click += new System.EventHandler(this.EditRefreshArtifactToolStripMenuItemClick);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.artifactToolStripMenuItem,
            this.SearchClassToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.searchToolStripMenuItem.Text = "検索(&S)";
            // 
            // artifactToolStripMenuItem
            // 
            this.artifactToolStripMenuItem.Name = "artifactToolStripMenuItem";
            this.artifactToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.artifactToolStripMenuItem.Text = "成果物を検索";
            this.artifactToolStripMenuItem.Click += new System.EventHandler(this.artifactToolStripMenuItem_Click);
            // 
            // SearchClassToolStripMenuItem
            // 
            this.SearchClassToolStripMenuItem.Name = "SearchClassToolStripMenuItem";
            this.SearchClassToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.SearchClassToolStripMenuItem.Text = "クラスを検索";
            this.SearchClassToolStripMenuItem.Click += new System.EventHandler(this.SearchClassToolStripMenuItemClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 543);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1041, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 565);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "BehaviorDevelop";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabContextMenuStrip.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem updateArtifactByEAToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem focusEAPackageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem EditRefreshArtifactToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripMenuItem AttachEAToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip tabContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem ViewGuidToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem ExitAppToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editCopyTextToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SearchClassToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem artifactToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TreeView treeView1;
	}
}
