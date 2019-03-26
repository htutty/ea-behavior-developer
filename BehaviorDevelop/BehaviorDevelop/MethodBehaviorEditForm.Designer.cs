namespace BehaviorDevelop
{
    partial class MethodBehaviorEditForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MethodBehaviorEditForm));
        	this.lblBehavior = new System.Windows.Forms.Label();
        	this.behaviorMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.pickMethodTimer = new System.Windows.Forms.Timer(this.components);
        	this.tgNumbering = new BehaviorDevelop.usercontrol.ToggleButton();
        	this.txtBehavior = new BehaviorDevelop.usercontrol.SyntaxRichTextBox();
        	this.btnSave = new System.Windows.Forms.Button();
        	this.btnClose = new System.Windows.Forms.Button();
        	this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        	this.lblMethodName = new System.Windows.Forms.Label();
        	this.txtMethodName = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.labelMethodAlias = new System.Windows.Forms.Label();
        	this.behaviorMenuStrip.SuspendLayout();
        	this.flowLayoutPanel1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// lblBehavior
        	// 
        	this.lblBehavior.AutoSize = true;
        	this.lblBehavior.Location = new System.Drawing.Point(30, 63);
        	this.lblBehavior.Name = "lblBehavior";
        	this.lblBehavior.Size = new System.Drawing.Size(54, 12);
        	this.lblBehavior.TabIndex = 0;
        	this.lblBehavior.Text = "振る舞い：";
        	// 
        	// behaviorMenuStrip
        	// 
        	this.behaviorMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.SelectAllToolStripMenuItem,
        	        	        	this.cutToolStripMenuItem,
        	        	        	this.copyToolStripMenuItem,
        	        	        	this.pasteToolStripMenuItem,
        	        	        	this.deleteToolStripMenuItem});
        	this.behaviorMenuStrip.Name = "behaviorMenuStrip";
        	this.behaviorMenuStrip.Size = new System.Drawing.Size(137, 114);
        	// 
        	// SelectAllToolStripMenuItem
        	// 
        	this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
        	this.SelectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
        	this.SelectAllToolStripMenuItem.Text = "すべて選択";
        	this.SelectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
        	// 
        	// cutToolStripMenuItem
        	// 
        	this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
        	this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
        	this.cutToolStripMenuItem.Text = "切り取り";
        	this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
        	// 
        	// copyToolStripMenuItem
        	// 
        	this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        	this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
        	this.copyToolStripMenuItem.Text = "コピー";
        	this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
        	// 
        	// pasteToolStripMenuItem
        	// 
        	this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
        	this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
        	this.pasteToolStripMenuItem.Text = "貼り付け";
        	this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
        	// 
        	// deleteToolStripMenuItem
        	// 
        	this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
        	this.deleteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
        	this.deleteToolStripMenuItem.Text = "削除";
        	this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
        	// 
        	// pickMethodTimer
        	// 
        	this.pickMethodTimer.Interval = 1000;
        	this.pickMethodTimer.Tick += new System.EventHandler(this.pickMethodTimer_Tick);
        	// 
        	// tgNumbering
        	// 
        	this.tgNumbering.Appearance = System.Windows.Forms.Appearance.Button;
        	this.tgNumbering.AutoSize = true;
        	this.tgNumbering.Image = ((System.Drawing.Image)(resources.GetObject("tgNumbering.Image")));
        	this.tgNumbering.Location = new System.Drawing.Point(47, 157);
        	this.tgNumbering.Name = "tgNumbering";
        	this.tgNumbering.Size = new System.Drawing.Size(34, 29);
        	this.tgNumbering.TabIndex = 9;
        	this.tgNumbering.CheckedChanged += new System.EventHandler(this.tgNumbering_CheckedChanged);
        	// 
        	// txtBehavior
        	// 
        	this.txtBehavior.Location = new System.Drawing.Point(93, 51);
        	this.txtBehavior.LstTipsHeight = 0;
        	this.txtBehavior.LstTipsWidth = 0;
        	this.txtBehavior.MinimumSize = new System.Drawing.Size(650, 300);
        	this.txtBehavior.Name = "txtBehavior";
        	this.txtBehavior.NumberingFlg = false;
        	this.txtBehavior.ShortcutsEnabled = false;
        	this.txtBehavior.Size = new System.Drawing.Size(679, 318);
        	this.txtBehavior.TabIndex = 7;
        	this.txtBehavior.Text = "";
        	this.txtBehavior.WordWrap = false;
        	// 
        	// btnSave
        	// 
        	this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.btnSave.Location = new System.Drawing.Point(585, 375);
        	this.btnSave.Name = "btnSave";
        	this.btnSave.Size = new System.Drawing.Size(91, 25);
        	this.btnSave.TabIndex = 6;
        	this.btnSave.Text = "保存して閉じる";
        	this.btnSave.UseVisualStyleBackColor = true;
        	this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        	// 
        	// btnClose
        	// 
        	this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.btnClose.Location = new System.Drawing.Point(697, 375);
        	this.btnClose.Name = "btnClose";
        	this.btnClose.Size = new System.Drawing.Size(75, 23);
        	this.btnClose.TabIndex = 11;
        	this.btnClose.Text = "キャンセル";
        	this.btnClose.UseVisualStyleBackColor = true;
        	this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
        	// 
        	// flowLayoutPanel1
        	// 
        	this.flowLayoutPanel1.Controls.Add(this.lblMethodName);
        	this.flowLayoutPanel1.Controls.Add(this.txtMethodName);
        	this.flowLayoutPanel1.Controls.Add(this.label1);
        	this.flowLayoutPanel1.Controls.Add(this.labelMethodAlias);
        	this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 2);
        	this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        	this.flowLayoutPanel1.Size = new System.Drawing.Size(769, 27);
        	this.flowLayoutPanel1.TabIndex = 12;
        	// 
        	// lblMethodName
        	// 
        	this.lblMethodName.AutoSize = true;
        	this.lblMethodName.Location = new System.Drawing.Point(3, 0);
        	this.lblMethodName.Name = "lblMethodName";
        	this.lblMethodName.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
        	this.lblMethodName.Size = new System.Drawing.Size(35, 16);
        	this.lblMethodName.TabIndex = 1;
        	this.lblMethodName.Text = "名前：";
        	// 
        	// txtMethodName
        	// 
        	this.txtMethodName.Location = new System.Drawing.Point(44, 3);
        	this.txtMethodName.Name = "txtMethodName";
        	this.txtMethodName.Size = new System.Drawing.Size(307, 19);
        	this.txtMethodName.TabIndex = 2;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(357, 0);
        	this.label1.Name = "label1";
        	this.label1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
        	this.label1.Size = new System.Drawing.Size(35, 16);
        	this.label1.TabIndex = 3;
        	this.label1.Text = "別名：";
        	// 
        	// labelMethodAlias
        	// 
        	this.labelMethodAlias.AutoSize = true;
        	this.labelMethodAlias.Location = new System.Drawing.Point(398, 0);
        	this.labelMethodAlias.Name = "labelMethodAlias";
        	this.labelMethodAlias.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
        	this.labelMethodAlias.Size = new System.Drawing.Size(29, 16);
        	this.labelMethodAlias.TabIndex = 4;
        	this.labelMethodAlias.Text = "別名";
        	// 
        	// MethodBehaviorEditForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        	this.ClientSize = new System.Drawing.Size(784, 412);
        	this.Controls.Add(this.flowLayoutPanel1);
        	this.Controls.Add(this.btnClose);
        	this.Controls.Add(this.btnSave);
        	this.Controls.Add(this.tgNumbering);
        	this.Controls.Add(this.txtBehavior);
        	this.Controls.Add(this.lblBehavior);
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(800, 450);
        	this.Name = "MethodBehaviorEditForm";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "振る舞い編集画面";
        	this.Activated += new System.EventHandler(this.MethodBehaviorEditForm_Activated);
        	this.Deactivate += new System.EventHandler(this.MethodBehaviorEditForm_Deactivate);
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MethoBehaviorEditForm_FormClosing);
        	this.Load += new System.EventHandler(this.Form_Load);
        	this.Resize += new System.EventHandler(this.MethoBehaviorEditForm_Resize);
        	this.behaviorMenuStrip.ResumeLayout(false);
        	this.flowLayoutPanel1.ResumeLayout(false);
        	this.flowLayoutPanel1.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Label labelMethodAlias;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnClose;

        #endregion

        private System.Windows.Forms.Label lblMethodName;
        private System.Windows.Forms.TextBox txtMethodName;
        private System.Windows.Forms.Label lblBehavior;
        private System.Windows.Forms.Button btnSave;
        private BehaviorDevelop.usercontrol.SyntaxRichTextBox txtBehavior;
        private System.Windows.Forms.ContextMenuStrip behaviorMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
        private BehaviorDevelop.usercontrol.ToggleButton tgNumbering;
        private System.Windows.Forms.Timer pickMethodTimer;

    }
}