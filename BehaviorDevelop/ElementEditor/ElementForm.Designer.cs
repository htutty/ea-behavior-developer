/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/20
 * Time: 11:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ElementEditor
{
	partial class ElementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElementForm));
            this.panel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.elementTypePictureBox = new System.Windows.Forms.PictureBox();
            this.classNameText = new System.Windows.Forms.TextBox();
            this.implFileLinkLabel = new System.Windows.Forms.LinkLabel();
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.classContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.focusEAClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGuidClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.methodContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.focusEAMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGuidMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonViewDiff = new System.Windows.Forms.Button();
            this.ButtonRepaint = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elementTypePictureBox)).BeginInit();
            this.classContextMenuStrip.SuspendLayout();
            this.methodContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panel.Location = new System.Drawing.Point(10, 42);
            this.panel.MinimumSize = new System.Drawing.Size(300, 200);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(822, 381);
            this.panel.TabIndex = 0;
            this.panel.WrapContents = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.elementTypePictureBox);
            this.flowLayoutPanel1.Controls.Add(this.classNameText);
            this.flowLayoutPanel1.Controls.Add(this.implFileLinkLabel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(821, 30);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // elementTypePictureBox
            // 
            this.elementTypePictureBox.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_TAGGEDVALUE;
            this.elementTypePictureBox.Location = new System.Drawing.Point(3, 3);
            this.elementTypePictureBox.Name = "elementTypePictureBox";
            this.elementTypePictureBox.Size = new System.Drawing.Size(20, 20);
            this.elementTypePictureBox.TabIndex = 1;
            this.elementTypePictureBox.TabStop = false;
            // 
            // classNameText
            // 
            this.classNameText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.classNameText.Location = new System.Drawing.Point(29, 3);
            this.classNameText.Name = "classNameText";
            this.classNameText.ReadOnly = true;
            this.classNameText.Size = new System.Drawing.Size(382, 19);
            this.classNameText.TabIndex = 0;
            // 
            // implFileLinkLabel
            // 
            this.implFileLinkLabel.Location = new System.Drawing.Point(417, 3);
            this.implFileLinkLabel.Margin = new System.Windows.Forms.Padding(3);
            this.implFileLinkLabel.Name = "implFileLinkLabel";
            this.implFileLinkLabel.Size = new System.Drawing.Size(386, 23);
            this.implFileLinkLabel.TabIndex = 2;
            this.implFileLinkLabel.TabStop = true;
            this.implFileLinkLabel.Text = "link";
            this.toolTip1.SetToolTip(this.implFileLinkLabel, "実装ファイルへのリンク");
            this.implFileLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ImplFileLinkLabelLinkClicked);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCommit.Location = new System.Drawing.Point(676, 432);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 1;
            this.btnCommit.Text = "一時保存";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.BtnCommitClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(757, 432);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "閉じる";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ICON_SYMBOL_ATTRIBUTE.png");
            this.imageList1.Images.SetKeyName(1, "ICON_SYMBOL_METHOD.png");
            // 
            // classContextMenuStrip
            // 
            this.classContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.focusEAClassToolStripMenuItem,
            this.copyGuidClassToolStripMenuItem});
            this.classContextMenuStrip.Name = "classContextMenuStrip";
            this.classContextMenuStrip.Size = new System.Drawing.Size(176, 48);
            this.classContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ClassContextMenuStripOpening);
            // 
            // focusEAClassToolStripMenuItem
            // 
            this.focusEAClassToolStripMenuItem.Name = "focusEAClassToolStripMenuItem";
            this.focusEAClassToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.focusEAClassToolStripMenuItem.Text = "EAでこのクラスを選択";
            this.focusEAClassToolStripMenuItem.Click += new System.EventHandler(this.FocusEAClassToolStripMenuItemClick);
            // 
            // copyGuidClassToolStripMenuItem
            // 
            this.copyGuidClassToolStripMenuItem.Name = "copyGuidClassToolStripMenuItem";
            this.copyGuidClassToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.copyGuidClassToolStripMenuItem.Text = "GUIDをコピー";
            this.copyGuidClassToolStripMenuItem.Click += new System.EventHandler(this.CopyGuidClassToolStripMenuItemClick);
            // 
            // methodContextMenuStrip
            // 
            this.methodContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.focusEAMethodToolStripMenuItem,
            this.copyGuidMethodToolStripMenuItem});
            this.methodContextMenuStrip.Name = "methodContextMenuStrip";
            this.methodContextMenuStrip.Size = new System.Drawing.Size(183, 48);
            this.methodContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.MethodContextMenuStripOpening);
            // 
            // focusEAMethodToolStripMenuItem
            // 
            this.focusEAMethodToolStripMenuItem.Name = "focusEAMethodToolStripMenuItem";
            this.focusEAMethodToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.focusEAMethodToolStripMenuItem.Text = "EAでこのメソッドを選択";
            this.focusEAMethodToolStripMenuItem.Click += new System.EventHandler(this.FocusEAMethodToolStripMenuItemClick);
            // 
            // copyGuidMethodToolStripMenuItem
            // 
            this.copyGuidMethodToolStripMenuItem.Name = "copyGuidMethodToolStripMenuItem";
            this.copyGuidMethodToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.copyGuidMethodToolStripMenuItem.Text = "GUIDをコピー";
            this.copyGuidMethodToolStripMenuItem.Click += new System.EventHandler(this.CopyGuidMethodToolStripMenuItemClick);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "実装へのリンク";
            // 
            // buttonViewDiff
            // 
            this.buttonViewDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonViewDiff.Location = new System.Drawing.Point(572, 432);
            this.buttonViewDiff.Name = "buttonViewDiff";
            this.buttonViewDiff.Size = new System.Drawing.Size(92, 23);
            this.buttonViewDiff.TabIndex = 5;
            this.buttonViewDiff.Text = "変更差分確認";
            this.buttonViewDiff.UseVisualStyleBackColor = true;
            this.buttonViewDiff.Click += new System.EventHandler(this.ButtonViewDiffClick);
            // 
            // ButtonRepaint
            // 
            this.ButtonRepaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonRepaint.Location = new System.Drawing.Point(10, 432);
            this.ButtonRepaint.Name = "ButtonRepaint";
            this.ButtonRepaint.Size = new System.Drawing.Size(55, 23);
            this.ButtonRepaint.TabIndex = 6;
            this.ButtonRepaint.Text = "再表示";
            this.ButtonRepaint.UseVisualStyleBackColor = true;
            this.ButtonRepaint.Click += new System.EventHandler(this.ButtonRepaint_Click);
            // 
            // ElementForm
            // 
            this.AcceptButton = this.btnCommit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(844, 462);
            this.Controls.Add(this.ButtonRepaint);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.buttonViewDiff);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.panel);
            this.Name = "ElementForm";
            this.Text = "クラスプロパティ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ElementForm_FormClosed);
            this.Resize += new System.EventHandler(this.ElementFormResize);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elementTypePictureBox)).EndInit();
            this.classContextMenuStrip.ResumeLayout(false);
            this.methodContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button buttonViewDiff;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.LinkLabel implFileLinkLabel;
		private System.Windows.Forms.ToolStripMenuItem copyGuidMethodToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem focusEAMethodToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip methodContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem copyGuidClassToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem focusEAClassToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip classContextMenuStrip;
		private System.Windows.Forms.TextBox classNameText;
		private System.Windows.Forms.PictureBox elementTypePictureBox;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnCommit;
		private System.Windows.Forms.FlowLayoutPanel panel;
        private System.Windows.Forms.Button ButtonRepaint;
    }
}
