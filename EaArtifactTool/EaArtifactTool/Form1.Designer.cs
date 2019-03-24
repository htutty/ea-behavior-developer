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
            this.SuspendLayout();
            // 
            // TxtEapFile
            // 
            this.TxtEapFile.Location = new System.Drawing.Point(98, 43);
            this.TxtEapFile.Name = "TxtEapFile";
            this.TxtEapFile.Size = new System.Drawing.Size(327, 19);
            this.TxtEapFile.TabIndex = 0;
            // 
            // TxtProjectName
            // 
            this.TxtProjectName.Location = new System.Drawing.Point(98, 85);
            this.TxtProjectName.Name = "TxtProjectName";
            this.TxtProjectName.Size = new System.Drawing.Size(148, 19);
            this.TxtProjectName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "EAPファイル";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "プロジェクト名";
            // 
            // TxtOutputFolder
            // 
            this.TxtOutputFolder.Location = new System.Drawing.Point(98, 125);
            this.TxtOutputFolder.Name = "TxtOutputFolder";
            this.TxtOutputFolder.Size = new System.Drawing.Size(327, 19);
            this.TxtOutputFolder.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "出力フォルダ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "■データベースファイルから成果物ファイル生成";
            // 
            // BtnExecOutput
            // 
            this.BtnExecOutput.Location = new System.Drawing.Point(402, 164);
            this.BtnExecOutput.Name = "BtnExecOutput";
            this.BtnExecOutput.Size = new System.Drawing.Size(75, 23);
            this.BtnExecOutput.TabIndex = 7;
            this.BtnExecOutput.Text = "出力開始";
            this.BtnExecOutput.UseVisualStyleBackColor = true;
            this.BtnExecOutput.Click += new System.EventHandler(this.BtnExecOutput_Click);
            // 
            // BtnSelectEapFile
            // 
            this.BtnSelectEapFile.Location = new System.Drawing.Point(432, 43);
            this.BtnSelectEapFile.Name = "BtnSelectEapFile";
            this.BtnSelectEapFile.Size = new System.Drawing.Size(29, 23);
            this.BtnSelectEapFile.TabIndex = 8;
            this.BtnSelectEapFile.Text = "...";
            this.BtnSelectEapFile.UseVisualStyleBackColor = true;
            this.BtnSelectEapFile.Click += new System.EventHandler(this.BtnSelectEapFile_Click);
            // 
            // BtnOutputFolderSelect
            // 
            this.BtnOutputFolderSelect.Location = new System.Drawing.Point(432, 125);
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
            // ArtifactToolStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 444);
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
    }
}

