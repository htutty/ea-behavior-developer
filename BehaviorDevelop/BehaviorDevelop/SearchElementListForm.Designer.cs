/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/09
 * Time: 20:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace BehaviorDevelop
{
	partial class SearchElementListForm
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
			this.SearchTermTextBox = new System.Windows.Forms.TextBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.elemName = new System.Windows.Forms.ColumnHeader();
			this.elemAlias = new System.Windows.Forms.ColumnHeader();
			this.elemType = new System.Windows.Forms.ColumnHeader();
			this.elemStereotype = new System.Windows.Forms.ColumnHeader();
			this.artifactPath = new System.Windows.Forms.ColumnHeader();
			this.artifactName = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// SearchTermTextBox
			// 
			this.SearchTermTextBox.Location = new System.Drawing.Point(14, 10);
			this.SearchTermTextBox.Name = "SearchTermTextBox";
			this.SearchTermTextBox.Size = new System.Drawing.Size(852, 19);
			this.SearchTermTextBox.TabIndex = 0;
			this.SearchTermTextBox.TextChanged += new System.EventHandler(this.SearchTermTextBoxTextChanged);
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.elemName,
									this.elemAlias,
									this.elemType,
									this.elemStereotype,
									this.artifactPath,
									this.artifactName});
			this.listView1.Location = new System.Drawing.Point(14, 35);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(852, 517);
			this.listView1.TabIndex = 1;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView1MouseDoubleClick);
			// 
			// elemName
			// 
			this.elemName.Text = "要素名称";
			this.elemName.Width = 160;
			// 
			// elemAlias
			// 
			this.elemAlias.Text = "別名";
			this.elemAlias.Width = 127;
			// 
			// elemType
			// 
			this.elemType.Text = "要素型";
			// 
			// elemStereotype
			// 
			this.elemStereotype.Text = "ステレオタイプ";
			this.elemStereotype.Width = 90;
			// 
			// artifactPath
			// 
			this.artifactPath.Text = "成果物パス";
			this.artifactPath.Width = 200;
			// 
			// artifactName
			// 
			this.artifactName.Text = "成果物名称";
			this.artifactName.Width = 100;
			// 
			// SearchElementListForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(878, 558);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.SearchTermTextBox);
			this.Name = "SearchElementListForm";
			this.Text = "SearchElementListForm";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ColumnHeader artifactName;
		private System.Windows.Forms.ColumnHeader artifactPath;
		private System.Windows.Forms.ColumnHeader elemAlias;
		private System.Windows.Forms.ColumnHeader elemName;
		private System.Windows.Forms.ColumnHeader elemStereotype;
		private System.Windows.Forms.ColumnHeader elemType;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.TextBox SearchTermTextBox;
	}
}
