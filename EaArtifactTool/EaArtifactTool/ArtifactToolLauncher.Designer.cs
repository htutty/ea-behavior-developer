namespace EaArtifactTool
{
    partial class ArtifactToolStartForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.TxtEapFile = new System.Windows.Forms.TextBox();
            this.TxtProjectName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtOutputFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.BtnExecOutput = new System.Windows.Forms.Button();
            this.BtnSelectEapFile = new System.Windows.Forms.Button();
            this.BtnOutputFolderSelect = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.TxtCmpSrcProject = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TxtCmpDestProject = new System.Windows.Forms.TextBox();
            this.BtnCompareStart = new System.Windows.Forms.Button();
            this.TxtResultDir = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.CheckSkipNotes = new System.Windows.Forms.CheckBox();
            this.BtnSelectSrcProject = new System.Windows.Forms.Button();
            this.BtnSelectDestProject = new System.Windows.Forms.Button();
            this.BtnSelectResultDir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TxtEapFile
            // 
            this.TxtEapFile.Location = new System.Drawing.Point(98, 52);
            this.TxtEapFile.Name = "TxtEapFile";
            this.TxtEapFile.Size = new System.Drawing.Size(327, 19);
            this.TxtEapFile.TabIndex = 0;
            // 
            // TxtProjectName
            // 
            this.TxtProjectName.Location = new System.Drawing.Point(98, 88);
            this.TxtProjectName.Name = "TxtProjectName";
            this.TxtProjectName.Size = new System.Drawing.Size(148, 19);
            this.TxtProjectName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "EAPファイル";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "プロジェクト名";
            // 
            // TxtOutputFolder
            // 
            this.TxtOutputFolder.Location = new System.Drawing.Point(98, 123);
            this.TxtOutputFolder.Name = "TxtOutputFolder";
            this.TxtOutputFolder.Size = new System.Drawing.Size(327, 19);
            this.TxtOutputFolder.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "出力フォルダ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "■データベースファイルから成果物ファイル生成";
            // 
            // BtnExecOutput
            // 
            this.BtnExecOutput.Location = new System.Drawing.Point(421, 161);
            this.BtnExecOutput.Name = "BtnExecOutput";
            this.BtnExecOutput.Size = new System.Drawing.Size(75, 23);
            this.BtnExecOutput.TabIndex = 7;
            this.BtnExecOutput.Text = "出力開始";
            this.BtnExecOutput.UseVisualStyleBackColor = true;
            this.BtnExecOutput.Click += new System.EventHandler(this.BtnExecOutput_Click);
            // 
            // BtnSelectEapFile
            // 
            this.BtnSelectEapFile.Location = new System.Drawing.Point(432, 52);
            this.BtnSelectEapFile.Name = "BtnSelectEapFile";
            this.BtnSelectEapFile.Size = new System.Drawing.Size(29, 23);
            this.BtnSelectEapFile.TabIndex = 8;
            this.BtnSelectEapFile.Text = "...";
            this.BtnSelectEapFile.UseVisualStyleBackColor = true;
            this.BtnSelectEapFile.Click += new System.EventHandler(this.BtnSelectEapFile_Click);
            // 
            // BtnOutputFolderSelect
            // 
            this.BtnOutputFolderSelect.Location = new System.Drawing.Point(432, 123);
            this.BtnOutputFolderSelect.Name = "BtnOutputFolderSelect";
            this.BtnOutputFolderSelect.Size = new System.Drawing.Size(29, 23);
            this.BtnOutputFolderSelect.TabIndex = 9;
            this.BtnOutputFolderSelect.Text = "...";
            this.BtnOutputFolderSelect.UseVisualStyleBackColor = true;
            this.BtnOutputFolderSelect.Click += new System.EventHandler(this.BtnOutputFolderSelect_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "■成果物ファイル同士の比較";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 248);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "プロジェクト1（比較元）";
            // 
            // TxtCmpSrcProject
            // 
            this.TxtCmpSrcProject.Location = new System.Drawing.Point(134, 245);
            this.TxtCmpSrcProject.Name = "TxtCmpSrcProject";
            this.TxtCmpSrcProject.Size = new System.Drawing.Size(327, 19);
            this.TxtCmpSrcProject.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 285);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "プロジェクト2（比較先）";
            // 
            // TxtCmpDestProject
            // 
            this.TxtCmpDestProject.Location = new System.Drawing.Point(134, 282);
            this.TxtCmpDestProject.Name = "TxtCmpDestProject";
            this.TxtCmpDestProject.Size = new System.Drawing.Size(327, 19);
            this.TxtCmpDestProject.TabIndex = 13;
            // 
            // BtnCompareStart
            // 
            this.BtnCompareStart.Location = new System.Drawing.Point(421, 375);
            this.BtnCompareStart.Name = "BtnCompareStart";
            this.BtnCompareStart.Size = new System.Drawing.Size(75, 23);
            this.BtnCompareStart.TabIndex = 15;
            this.BtnCompareStart.Text = "比較開始";
            this.BtnCompareStart.UseVisualStyleBackColor = true;
            this.BtnCompareStart.Click += new System.EventHandler(this.BtnCompareStart_Click);
            // 
            // TxtResultDir
            // 
            this.TxtResultDir.Location = new System.Drawing.Point(134, 317);
            this.TxtResultDir.Name = "TxtResultDir";
            this.TxtResultDir.Size = new System.Drawing.Size(327, 19);
            this.TxtResultDir.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 324);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 17;
            this.label8.Text = "比較結果出力先";
            // 
            // CheckSkipNotes
            // 
            this.CheckSkipNotes.AutoSize = true;
            this.CheckSkipNotes.Location = new System.Drawing.Point(152, 351);
            this.CheckSkipNotes.Name = "CheckSkipNotes";
            this.CheckSkipNotes.Size = new System.Drawing.Size(168, 16);
            this.CheckSkipNotes.TabIndex = 18;
            this.CheckSkipNotes.Text = "ノート項目を無視して比較する";
            this.CheckSkipNotes.UseVisualStyleBackColor = true;
            // 
            // BtnSelectSrcProject
            // 
            this.BtnSelectSrcProject.Location = new System.Drawing.Point(467, 243);
            this.BtnSelectSrcProject.Name = "BtnSelectSrcProject";
            this.BtnSelectSrcProject.Size = new System.Drawing.Size(29, 23);
            this.BtnSelectSrcProject.TabIndex = 19;
            this.BtnSelectSrcProject.Text = "...";
            this.BtnSelectSrcProject.UseVisualStyleBackColor = true;
            this.BtnSelectSrcProject.Click += new System.EventHandler(this.BtnSelectSrcProject_Click);
            // 
            // BtnSelectDestProject
            // 
            this.BtnSelectDestProject.Location = new System.Drawing.Point(467, 282);
            this.BtnSelectDestProject.Name = "BtnSelectDestProject";
            this.BtnSelectDestProject.Size = new System.Drawing.Size(29, 23);
            this.BtnSelectDestProject.TabIndex = 20;
            this.BtnSelectDestProject.Text = "...";
            this.BtnSelectDestProject.UseVisualStyleBackColor = true;
            this.BtnSelectDestProject.Click += new System.EventHandler(this.BtnSelectDestProject_Click);
            // 
            // BtnSelectResultDir
            // 
            this.BtnSelectResultDir.Location = new System.Drawing.Point(467, 319);
            this.BtnSelectResultDir.Name = "BtnSelectResultDir";
            this.BtnSelectResultDir.Size = new System.Drawing.Size(29, 23);
            this.BtnSelectResultDir.TabIndex = 21;
            this.BtnSelectResultDir.Text = "...";
            this.BtnSelectResultDir.UseVisualStyleBackColor = true;
            this.BtnSelectResultDir.Click += new System.EventHandler(this.BtnSelectResultDir_Click);
            // 
            // ArtifactToolStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 444);
            this.Controls.Add(this.BtnSelectResultDir);
            this.Controls.Add(this.BtnSelectDestProject);
            this.Controls.Add(this.BtnSelectSrcProject);
            this.Controls.Add(this.CheckSkipNotes);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TxtResultDir);
            this.Controls.Add(this.BtnCompareStart);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TxtCmpDestProject);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TxtCmpSrcProject);
            this.Controls.Add(this.BtnOutputFolderSelect);
            this.Controls.Add(this.BtnSelectEapFile);
            this.Controls.Add(this.BtnExecOutput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtOutputFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtProjectName);
            this.Controls.Add(this.TxtEapFile);
            this.Name = "ArtifactToolStartForm";
            this.Text = "EA成果物管理ツール実行";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtEapFile;
        private System.Windows.Forms.TextBox TxtProjectName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtOutputFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnExecOutput;
        private System.Windows.Forms.Button BtnSelectEapFile;
        private System.Windows.Forms.Button BtnOutputFolderSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TxtCmpSrcProject;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TxtCmpDestProject;
        private System.Windows.Forms.Button BtnCompareStart;
        private System.Windows.Forms.TextBox TxtResultDir;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox CheckSkipNotes;
        private System.Windows.Forms.Button BtnSelectSrcProject;
        private System.Windows.Forms.Button BtnSelectDestProject;
        private System.Windows.Forms.Button BtnSelectResultDir;
    }
}

