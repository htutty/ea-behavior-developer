/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/06/28
 * Time: 21:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace BehaviorDevelop
{
	partial class MethodPropertyForm
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.textMethodReturnType = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.labelAlias = new System.Windows.Forms.Label();
			this.labelGUID = new System.Windows.Forms.Label();
			this.labelNotes = new System.Windows.Forms.Label();
			this.textMethodNotes = new System.Windows.Forms.TextBox();
			this.textMethodGUID = new System.Windows.Forms.TextBox();
			this.textMethodAlias = new System.Windows.Forms.TextBox();
			this.textMethodName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textMethodVisibility = new System.Windows.Forms.TextBox();
			this.textMethodStereotype = new System.Windows.Forms.TextBox();
			this.textMethodPos = new System.Windows.Forms.TextBox();
			this.textMethodClassifierID = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.tableLayoutPanel1.Controls.Add(this.textMethodReturnType, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelAlias, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelGUID, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.labelNotes, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.textMethodNotes, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.textMethodGUID, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.textMethodAlias, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.textMethodName, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.textMethodVisibility, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.textMethodStereotype, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.textMethodPos, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this.textMethodClassifierID, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this.label5, 0, 7);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 9;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 146F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(566, 382);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// textMethodReturnType
			// 
			this.textMethodReturnType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodReturnType.Location = new System.Drawing.Point(116, 299);
			this.textMethodReturnType.Name = "textMethodReturnType";
			this.textMethodReturnType.ReadOnly = true;
			this.textMethodReturnType.Size = new System.Drawing.Size(447, 19);
			this.textMethodReturnType.TabIndex = 13;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(3, 0);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(100, 23);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Name";
			// 
			// labelAlias
			// 
			this.labelAlias.Location = new System.Drawing.Point(3, 30);
			this.labelAlias.Name = "labelAlias";
			this.labelAlias.Size = new System.Drawing.Size(100, 23);
			this.labelAlias.TabIndex = 1;
			this.labelAlias.Text = "Alias";
			// 
			// labelGUID
			// 
			this.labelGUID.Location = new System.Drawing.Point(3, 60);
			this.labelGUID.Name = "labelGUID";
			this.labelGUID.Size = new System.Drawing.Size(100, 23);
			this.labelGUID.TabIndex = 2;
			this.labelGUID.Text = "GUID";
			// 
			// labelNotes
			// 
			this.labelNotes.Location = new System.Drawing.Point(3, 90);
			this.labelNotes.Name = "labelNotes";
			this.labelNotes.Size = new System.Drawing.Size(100, 23);
			this.labelNotes.TabIndex = 3;
			this.labelNotes.Text = "Notes";
			// 
			// textMethodNotes
			// 
			this.textMethodNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodNotes.Location = new System.Drawing.Point(116, 93);
			this.textMethodNotes.Multiline = true;
			this.textMethodNotes.Name = "textMethodNotes";
			this.textMethodNotes.Size = new System.Drawing.Size(447, 140);
			this.textMethodNotes.TabIndex = 4;
			this.textMethodNotes.TextChanged += new System.EventHandler(this.TextMethodNotesTextChanged);
			// 
			// textMethodGUID
			// 
			this.textMethodGUID.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodGUID.Location = new System.Drawing.Point(116, 63);
			this.textMethodGUID.Name = "textMethodGUID";
			this.textMethodGUID.ReadOnly = true;
			this.textMethodGUID.Size = new System.Drawing.Size(447, 19);
			this.textMethodGUID.TabIndex = 5;
			// 
			// textMethodAlias
			// 
			this.textMethodAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodAlias.Location = new System.Drawing.Point(116, 33);
			this.textMethodAlias.Name = "textMethodAlias";
			this.textMethodAlias.ReadOnly = true;
			this.textMethodAlias.Size = new System.Drawing.Size(447, 19);
			this.textMethodAlias.TabIndex = 6;
			// 
			// textMethodName
			// 
			this.textMethodName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodName.Location = new System.Drawing.Point(116, 3);
			this.textMethodName.Name = "textMethodName";
			this.textMethodName.ReadOnly = true;
			this.textMethodName.Size = new System.Drawing.Size(447, 19);
			this.textMethodName.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 236);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 22);
			this.label1.TabIndex = 8;
			this.label1.Text = "Stereotype";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 266);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 10;
			this.label2.Text = "Visibility";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 296);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 11;
			this.label3.Text = "ReturnType";
			// 
			// textMethodVisibility
			// 
			this.textMethodVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodVisibility.Location = new System.Drawing.Point(116, 269);
			this.textMethodVisibility.Name = "textMethodVisibility";
			this.textMethodVisibility.ReadOnly = true;
			this.textMethodVisibility.Size = new System.Drawing.Size(447, 19);
			this.textMethodVisibility.TabIndex = 6;
			// 
			// textMethodStereotype
			// 
			this.textMethodStereotype.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodStereotype.Location = new System.Drawing.Point(116, 239);
			this.textMethodStereotype.Name = "textMethodStereotype";
			this.textMethodStereotype.ReadOnly = true;
			this.textMethodStereotype.Size = new System.Drawing.Size(447, 19);
			this.textMethodStereotype.TabIndex = 9;
			// 
			// textMethodPos
			// 
			this.textMethodPos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodPos.Location = new System.Drawing.Point(116, 359);
			this.textMethodPos.Name = "textMethodPos";
			this.textMethodPos.ReadOnly = true;
			this.textMethodPos.Size = new System.Drawing.Size(447, 19);
			this.textMethodPos.TabIndex = 14;
			// 
			// textMethodClassifierID
			// 
			this.textMethodClassifierID.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textMethodClassifierID.Location = new System.Drawing.Point(116, 329);
			this.textMethodClassifierID.Name = "textMethodClassifierID";
			this.textMethodClassifierID.ReadOnly = true;
			this.textMethodClassifierID.Size = new System.Drawing.Size(447, 19);
			this.textMethodClassifierID.TabIndex = 16;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 356);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 20);
			this.label4.TabIndex = 12;
			this.label4.Text = "Pos";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 326);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 22);
			this.label5.TabIndex = 15;
			this.label5.Text = "ClassifierID";
			// 
			// MethodPropertyForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(590, 406);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "MethodPropertyForm";
			this.Text = "操作のプロパティ";
			this.Load += new System.EventHandler(this.MethodPropertyFormLoad);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textMethodClassifierID;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textMethodStereotype;
		private System.Windows.Forms.TextBox textMethodVisibility;
		private System.Windows.Forms.TextBox textMethodReturnType;
		private System.Windows.Forms.TextBox textMethodPos;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMethodName;
		private System.Windows.Forms.TextBox textMethodAlias;
		private System.Windows.Forms.TextBox textMethodGUID;
		private System.Windows.Forms.TextBox textMethodNotes;
		private System.Windows.Forms.Label labelNotes;
		private System.Windows.Forms.Label labelGUID;
		private System.Windows.Forms.Label labelAlias;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
