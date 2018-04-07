/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/20
 * Time: 11:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace BehaviorDevelop
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
			this.panel = new System.Windows.Forms.FlowLayoutPanel();
			this.btnCommit = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.buttonCopyContent = new System.Windows.Forms.Button();
			this.buttonOutputJava = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.AutoScroll = true;
			this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.panel.Location = new System.Drawing.Point(10, 10);
			this.panel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
			this.panel.MinimumSize = new System.Drawing.Size(400, 300);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(822, 413);
			this.panel.TabIndex = 0;
			this.panel.WrapContents = false;
			// 
			// btnCommit
			// 
			this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCommit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCommit.Location = new System.Drawing.Point(676, 432);
			this.btnCommit.Name = "btnCommit";
			this.btnCommit.Size = new System.Drawing.Size(75, 23);
			this.btnCommit.TabIndex = 1;
			this.btnCommit.Text = "確定";
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
			this.btnCancel.Text = "キャンセル";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
			// 
			// buttonCopyContent
			// 
			this.buttonCopyContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCopyContent.Cursor = System.Windows.Forms.Cursors.Default;
			this.buttonCopyContent.Location = new System.Drawing.Point(12, 432);
			this.buttonCopyContent.Name = "buttonCopyContent";
			this.buttonCopyContent.Size = new System.Drawing.Size(95, 23);
			this.buttonCopyContent.TabIndex = 3;
			this.buttonCopyContent.Text = "クラス内容コピー";
			this.buttonCopyContent.UseVisualStyleBackColor = true;
			this.buttonCopyContent.Click += new System.EventHandler(this.ButtonCopyContentClick);
			// 
			// buttonOutputJava
			// 
			this.buttonOutputJava.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOutputJava.Location = new System.Drawing.Point(132, 432);
			this.buttonOutputJava.Name = "buttonOutputJava";
			this.buttonOutputJava.Size = new System.Drawing.Size(95, 23);
			this.buttonOutputJava.TabIndex = 4;
			this.buttonOutputJava.Text = "簡易Java出力";
			this.buttonOutputJava.UseVisualStyleBackColor = true;
			this.buttonOutputJava.Click += new System.EventHandler(this.ButtonOutputJavaClick);
			// 
			// ElementForm
			// 
			this.AcceptButton = this.btnCommit;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(844, 462);
			this.Controls.Add(this.buttonOutputJava);
			this.Controls.Add(this.buttonCopyContent);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnCommit);
			this.Controls.Add(this.panel);
			this.Name = "ElementForm";
			this.Text = "クラスプロパティ";
			this.Resize += new System.EventHandler(this.ElementFormResize);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button buttonOutputJava;
		private System.Windows.Forms.Button buttonCopyContent;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnCommit;
		private System.Windows.Forms.FlowLayoutPanel panel;

	}
}
