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
        	System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MethodBehaviorEditForm));
        	this.lblMethodName = new System.Windows.Forms.Label();
        	this.txtMethodName = new System.Windows.Forms.TextBox();
        	this.lblBehavior = new System.Windows.Forms.Label();
        	this.label1 = new System.Windows.Forms.Label();
        	this.relGrid = new System.Windows.Forms.DataGridView();
        	this.GUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.coment = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.package = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.CanDelete = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.AttributesMethods = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.btnSave = new System.Windows.Forms.Button();
        	this.CrossReferenceMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.SwitchCrossReference = new System.Windows.Forms.ToolStripMenuItem();
        	this.DelCrossReference = new System.Windows.Forms.ToolStripMenuItem();
        	this.AddCrossReference = new System.Windows.Forms.ToolStripMenuItem();
        	this.behaviorMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.crossRefPanel = new System.Windows.Forms.Panel();
        	this.buttomButtonPanel = new System.Windows.Forms.Panel();
        	this.btnHelp = new System.Windows.Forms.Button();
        	this.btnClose = new System.Windows.Forms.Button();
        	this.pickMethodTimer = new System.Windows.Forms.Timer(this.components);
        	this.btnMergeBaseline = new System.Windows.Forms.Button();
        	this.tgNumbering = new BehaviorDevelop.usercontrol.ToggleButton();
        	this.txtBehavior = new BehaviorDevelop.usercontrol.SyntaxRichTextBox();
        	((System.ComponentModel.ISupportInitialize)(this.relGrid)).BeginInit();
        	this.CrossReferenceMenuStrip.SuspendLayout();
        	this.behaviorMenuStrip.SuspendLayout();
        	this.crossRefPanel.SuspendLayout();
        	this.buttomButtonPanel.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// lblMethodName
        	// 
        	this.lblMethodName.AutoSize = true;
        	this.lblMethodName.Location = new System.Drawing.Point(30, 31);
        	this.lblMethodName.Name = "lblMethodName";
        	this.lblMethodName.Size = new System.Drawing.Size(35, 12);
        	this.lblMethodName.TabIndex = 0;
        	this.lblMethodName.Text = "名前：";
        	// 
        	// txtMethodName
        	// 
        	this.txtMethodName.Location = new System.Drawing.Point(93, 28);
        	this.txtMethodName.MinimumSize = new System.Drawing.Size(650, 19);
        	this.txtMethodName.Name = "txtMethodName";
        	this.txtMethodName.Size = new System.Drawing.Size(650, 19);
        	this.txtMethodName.TabIndex = 1;
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
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(3, 43);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(59, 12);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "相互参照：";
        	// 
        	// relGrid
        	// 
        	this.relGrid.AllowUserToAddRows = false;
        	this.relGrid.AllowUserToResizeRows = false;
        	this.relGrid.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
        	this.relGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        	this.relGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        	        	        	this.GUID,
        	        	        	this.name,
        	        	        	this.type,
        	        	        	this.coment,
        	        	        	this.package,
        	        	        	this.CanDelete,
        	        	        	this.AttributesMethods});
        	dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        	dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
        	dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
        	dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
        	dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
        	dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
        	dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        	this.relGrid.DefaultCellStyle = dataGridViewCellStyle1;
        	this.relGrid.GridColor = System.Drawing.SystemColors.ActiveBorder;
        	this.relGrid.Location = new System.Drawing.Point(81, 43);
        	this.relGrid.MinimumSize = new System.Drawing.Size(650, 200);
        	this.relGrid.MultiSelect = false;
        	this.relGrid.Name = "relGrid";
        	this.relGrid.RowHeadersVisible = false;
        	this.relGrid.RowTemplate.Height = 21;
        	this.relGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        	this.relGrid.Size = new System.Drawing.Size(650, 200);
        	this.relGrid.TabIndex = 3;
        	this.relGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.relGrid_CellDoubleClick);
        	// 
        	// GUID
        	// 
        	this.GUID.DataPropertyName = "ElementGUID";
        	this.GUID.HeaderText = "GUID";
        	this.GUID.Name = "GUID";
        	this.GUID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
        	this.GUID.Visible = false;
        	// 
        	// name
        	// 
        	this.name.DataPropertyName = "Name";
        	this.name.HeaderText = "名前";
        	this.name.Name = "name";
        	this.name.ReadOnly = true;
        	// 
        	// type
        	// 
        	this.type.DataPropertyName = "Type";
        	this.type.HeaderText = "種類";
        	this.type.Name = "type";
        	this.type.ReadOnly = true;
        	// 
        	// coment
        	// 
        	this.coment.DataPropertyName = "Notes";
        	this.coment.HeaderText = "コメント";
        	this.coment.Name = "coment";
        	this.coment.ReadOnly = true;
        	// 
        	// package
        	// 
        	this.package.DataPropertyName = "PackageName";
        	this.package.HeaderText = "パッケージ";
        	this.package.Name = "package";
        	this.package.ReadOnly = true;
        	// 
        	// CanDelete
        	// 
        	this.CanDelete.DataPropertyName = "CanDelete";
        	this.CanDelete.HeaderText = "CanDelete";
        	this.CanDelete.Name = "CanDelete";
        	this.CanDelete.Visible = false;
        	// 
        	// AttributesMethods
        	// 
        	this.AttributesMethods.DataPropertyName = "AttributesMethods";
        	this.AttributesMethods.HeaderText = "AttributesMethods";
        	this.AttributesMethods.Name = "AttributesMethods";
        	this.AttributesMethods.Visible = false;
        	// 
        	// btnSave
        	// 
        	this.btnSave.Location = new System.Drawing.Point(615, 0);
        	this.btnSave.Name = "btnSave";
        	this.btnSave.Size = new System.Drawing.Size(120, 25);
        	this.btnSave.TabIndex = 6;
        	this.btnSave.Text = "保存";
        	this.btnSave.UseVisualStyleBackColor = true;
        	this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        	// 
        	// CrossReferenceMenuStrip
        	// 
        	this.CrossReferenceMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.SwitchCrossReference,
        	        	        	this.DelCrossReference,
        	        	        	this.AddCrossReference});
        	this.CrossReferenceMenuStrip.Name = "CrossReferenceMenuStrip";
        	this.CrossReferenceMenuStrip.Size = new System.Drawing.Size(161, 70);
        	// 
        	// SwitchCrossReference
        	// 
        	this.SwitchCrossReference.Name = "SwitchCrossReference";
        	this.SwitchCrossReference.Size = new System.Drawing.Size(160, 22);
        	this.SwitchCrossReference.Text = "要素に切り替え";
        	this.SwitchCrossReference.Click += new System.EventHandler(this.SwitchReference_Click);
        	// 
        	// DelCrossReference
        	// 
        	this.DelCrossReference.Name = "DelCrossReference";
        	this.DelCrossReference.Size = new System.Drawing.Size(160, 22);
        	this.DelCrossReference.Text = "削除";
        	this.DelCrossReference.Click += new System.EventHandler(this.DelCrossReference_Click);
        	// 
        	// AddCrossReference
        	// 
        	this.AddCrossReference.Name = "AddCrossReference";
        	this.AddCrossReference.Size = new System.Drawing.Size(160, 22);
        	this.AddCrossReference.Text = "追加";
        	this.AddCrossReference.Click += new System.EventHandler(this.AddCrossReference_Click);
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
        	// crossRefPanel
        	// 
        	this.crossRefPanel.Controls.Add(this.buttomButtonPanel);
        	this.crossRefPanel.Controls.Add(this.label1);
        	this.crossRefPanel.Controls.Add(this.relGrid);
        	this.crossRefPanel.Controls.Add(this.btnSave);
        	this.crossRefPanel.Location = new System.Drawing.Point(10, 300);
        	this.crossRefPanel.MinimumSize = new System.Drawing.Size(750, 300);
        	this.crossRefPanel.Name = "crossRefPanel";
        	this.crossRefPanel.Size = new System.Drawing.Size(750, 300);
        	this.crossRefPanel.TabIndex = 8;
        	// 
        	// buttomButtonPanel
        	// 
        	this.buttomButtonPanel.Controls.Add(this.btnHelp);
        	this.buttomButtonPanel.Controls.Add(this.btnClose);
        	this.buttomButtonPanel.Location = new System.Drawing.Point(480, 250);
        	this.buttomButtonPanel.Name = "buttomButtonPanel";
        	this.buttomButtonPanel.Size = new System.Drawing.Size(260, 40);
        	this.buttomButtonPanel.TabIndex = 7;
        	// 
        	// btnHelp
        	// 
        	this.btnHelp.Location = new System.Drawing.Point(137, 7);
        	this.btnHelp.Name = "btnHelp";
        	this.btnHelp.Size = new System.Drawing.Size(120, 25);
        	this.btnHelp.TabIndex = 6;
        	this.btnHelp.Text = "ヘルプ";
        	this.btnHelp.UseVisualStyleBackColor = true;
        	this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
        	// 
        	// btnClose
        	// 
        	this.btnClose.Location = new System.Drawing.Point(11, 7);
        	this.btnClose.Name = "btnClose";
        	this.btnClose.Size = new System.Drawing.Size(120, 25);
        	this.btnClose.TabIndex = 6;
        	this.btnClose.Text = "閉じる";
        	this.btnClose.UseVisualStyleBackColor = true;
        	this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
        	// 
        	// pickMethodTimer
        	// 
        	this.pickMethodTimer.Interval = 1000;
        	this.pickMethodTimer.Tick += new System.EventHandler(this.pickMethodTimer_Tick);
        	// 
        	// btnMergeBaseline
        	// 
        	this.btnMergeBaseline.Location = new System.Drawing.Point(47, 92);
        	this.btnMergeBaseline.Name = "btnMergeBaseline";
        	this.btnMergeBaseline.Size = new System.Drawing.Size(37, 32);
        	this.btnMergeBaseline.TabIndex = 10;
        	this.btnMergeBaseline.Text = ">M<";
        	this.btnMergeBaseline.UseVisualStyleBackColor = true;
        	this.btnMergeBaseline.Click += new System.EventHandler(this.btnMergeBaseline_Click);
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
        	this.txtBehavior.Location = new System.Drawing.Point(93, 63);
        	this.txtBehavior.LstTipsHeight = 0;
        	this.txtBehavior.LstTipsWidth = 0;
        	this.txtBehavior.MinimumSize = new System.Drawing.Size(650, 220);
        	this.txtBehavior.Name = "txtBehavior";
        	this.txtBehavior.NumberingFlg = false;
        	this.txtBehavior.ShortcutsEnabled = false;
        	this.txtBehavior.Size = new System.Drawing.Size(650, 220);
        	this.txtBehavior.TabIndex = 7;
        	this.txtBehavior.Text = "";
        	this.txtBehavior.WordWrap = false;
        	// 
        	// MethodBehaviorEditForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(784, 612);
        	this.Controls.Add(this.btnMergeBaseline);
        	this.Controls.Add(this.tgNumbering);
        	this.Controls.Add(this.crossRefPanel);
        	this.Controls.Add(this.txtBehavior);
        	this.Controls.Add(this.txtMethodName);
        	this.Controls.Add(this.lblBehavior);
        	this.Controls.Add(this.lblMethodName);
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(800, 650);
        	this.Name = "MethodBehaviorEditForm";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "振る舞い編集画面";
        	this.Activated += new System.EventHandler(this.MethodBehaviorEditForm_Activated);
        	this.Deactivate += new System.EventHandler(this.MethodBehaviorEditForm_Deactivate);
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MethoBehaviorEditForm_FormClosing);
        	this.Load += new System.EventHandler(this.TestWindowsForm_Load);
        	this.Resize += new System.EventHandler(this.MethoBehaviorEditForm_Resize);
        	((System.ComponentModel.ISupportInitialize)(this.relGrid)).EndInit();
        	this.CrossReferenceMenuStrip.ResumeLayout(false);
        	this.behaviorMenuStrip.ResumeLayout(false);
        	this.crossRefPanel.ResumeLayout(false);
        	this.crossRefPanel.PerformLayout();
        	this.buttomButtonPanel.ResumeLayout(false);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblMethodName;
        private System.Windows.Forms.TextBox txtMethodName;
        private System.Windows.Forms.Label lblBehavior;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView relGrid;
        private System.Windows.Forms.Button btnSave;
        private BehaviorDevelop.usercontrol.SyntaxRichTextBox txtBehavior;
        private System.Windows.Forms.ContextMenuStrip CrossReferenceMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem DelCrossReference;
        private System.Windows.Forms.ContextMenuStrip behaviorMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddCrossReference;
        private System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
        private System.Windows.Forms.Panel crossRefPanel;
        private BehaviorDevelop.usercontrol.ToggleButton tgNumbering;
        private System.Windows.Forms.DataGridViewTextBoxColumn GUID;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn coment;
        private System.Windows.Forms.DataGridViewTextBoxColumn package;
        private System.Windows.Forms.DataGridViewTextBoxColumn CanDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn AttributesMethods;
        private System.Windows.Forms.Timer pickMethodTimer;
        private System.Windows.Forms.Panel buttomButtonPanel;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolStripMenuItem SwitchCrossReference;
        private System.Windows.Forms.Button btnMergeBaseline;

    }
}