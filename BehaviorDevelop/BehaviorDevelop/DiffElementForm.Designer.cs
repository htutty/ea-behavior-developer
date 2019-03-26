/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/04/09
 * Time: 16:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace BehaviorDevelop
{
	partial class DiffElementForm
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.EASelectObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ReflectToEAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonCommitClose = new System.Windows.Forms.Button();
			this.buttonRevert = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.buttonOutputDocument = new System.Windows.Forms.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoScroll = true;
			this.tableLayoutPanel1.AutoScrollMinSize = new System.Drawing.Size(100, 200);
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1210, 456);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.EASelectObjectToolStripMenuItem,
									this.ReflectToEAToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(200, 48);
			// 
			// EASelectObjectToolStripMenuItem
			// 
			this.EASelectObjectToolStripMenuItem.Name = "EASelectObjectToolStripMenuItem";
			this.EASelectObjectToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
			this.EASelectObjectToolStripMenuItem.Text = "EA上で属性操作を選択";
			this.EASelectObjectToolStripMenuItem.Click += new System.EventHandler(this.EASelectObjectToolStripMenuItemClick);
			// 
			// ReflectToEAToolStripMenuItem
			// 
			this.ReflectToEAToolStripMenuItem.Name = "ReflectToEAToolStripMenuItem";
			this.ReflectToEAToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
			this.ReflectToEAToolStripMenuItem.Text = "EAに反映";
			this.ReflectToEAToolStripMenuItem.Click += new System.EventHandler(this.ReflectToEAToolStripMenuItemClick);
			// 
			// buttonCommitClose
			// 
			this.buttonCommitClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCommitClose.Location = new System.Drawing.Point(950, 475);
			this.buttonCommitClose.Name = "buttonCommitClose";
			this.buttonCommitClose.Size = new System.Drawing.Size(75, 23);
			this.buttonCommitClose.TabIndex = 1;
			this.buttonCommitClose.Text = "EAに反映";
			this.buttonCommitClose.UseVisualStyleBackColor = true;
			this.buttonCommitClose.Click += new System.EventHandler(this.ButtonCommitCloseClick);
			// 
			// buttonRevert
			// 
			this.buttonRevert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRevert.Location = new System.Drawing.Point(1043, 475);
			this.buttonRevert.Name = "buttonRevert";
			this.buttonRevert.Size = new System.Drawing.Size(75, 23);
			this.buttonRevert.TabIndex = 2;
			this.buttonRevert.Text = "Revert";
			this.buttonRevert.UseVisualStyleBackColor = true;
			this.buttonRevert.Click += new System.EventHandler(this.ButtonRevertClick);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Location = new System.Drawing.Point(1138, 475);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 3;
			this.buttonClose.Text = "閉じる";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.ButtonCloseClick);
			// 
			// buttonOutputDocument
			// 
			this.buttonOutputDocument.Location = new System.Drawing.Point(798, 475);
			this.buttonOutputDocument.Name = "buttonOutputDocument";
			this.buttonOutputDocument.Size = new System.Drawing.Size(98, 23);
			this.buttonOutputDocument.TabIndex = 4;
			this.buttonOutputDocument.Text = "ドキュメント出力";
			this.buttonOutputDocument.UseVisualStyleBackColor = true;
			this.buttonOutputDocument.Click += new System.EventHandler(this.ButtonOutputDocumentClick);
			// 
			// DiffElementForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1234, 509);
			this.Controls.Add(this.buttonOutputDocument);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonRevert);
			this.Controls.Add(this.buttonCommitClose);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "DiffElementForm";
			this.Text = "DiffElementForm";
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button buttonOutputDocument;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonRevert;
		private System.Windows.Forms.Button buttonCommitClose;
		private System.Windows.Forms.ToolStripMenuItem EASelectObjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ReflectToEAToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
