/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/06/28
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ElementEditor
{
	partial class AttributePropertyForm
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
			this.labelAttrbuteName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textAttributeName = new System.Windows.Forms.TextBox();
			this.textAttributeAlias = new System.Windows.Forms.TextBox();
			this.textAttributeGUID = new System.Windows.Forms.TextBox();
			this.textAttributeNotes = new System.Windows.Forms.TextBox();
			this.textAttributePos = new System.Windows.Forms.TextBox();
			this.textAttributeStereotype = new System.Windows.Forms.TextBox();
			this.textAttributeClassifierID = new System.Windows.Forms.TextBox();
			this.textAttributeDefault = new System.Windows.Forms.TextBox();
			this.textAttributeIsStatic = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.44262F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.55738F));
			this.tableLayoutPanel1.Controls.Add(this.labelAttrbuteName, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.label6, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.label7, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this.label8, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeName, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeAlias, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeGUID, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeNotes, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.textAttributePos, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeStereotype, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeClassifierID, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeDefault, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this.textAttributeIsStatic, 1, 8);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 9;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.54099F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.45901F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(732, 248);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// labelAttrbuteName
			// 
			this.labelAttrbuteName.Location = new System.Drawing.Point(3, 0);
			this.labelAttrbuteName.Name = "labelAttrbuteName";
			this.labelAttrbuteName.Size = new System.Drawing.Size(100, 22);
			this.labelAttrbuteName.TabIndex = 0;
			this.labelAttrbuteName.Text = "Name";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Alias";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "GUID";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 3;
			this.label3.Text = "Notes";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 147);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 20);
			this.label4.TabIndex = 4;
			this.label4.Text = "Pos";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 167);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 5;
			this.label5.Text = "Stereotype";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 187);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 20);
			this.label6.TabIndex = 6;
			this.label6.Text = "ClassifierID";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(3, 207);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 20);
			this.label7.TabIndex = 7;
			this.label7.Text = "Default";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 227);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 21);
			this.label8.TabIndex = 8;
			this.label8.Text = "IsStatic";
			// 
			// textAttributeName
			// 
			this.textAttributeName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeName.Location = new System.Drawing.Point(137, 3);
			this.textAttributeName.Name = "textAttributeName";
			this.textAttributeName.ReadOnly = true;
			this.textAttributeName.Size = new System.Drawing.Size(592, 19);
			this.textAttributeName.TabIndex = 9;
			// 
			// textAttributeAlias
			// 
			this.textAttributeAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeAlias.Location = new System.Drawing.Point(137, 25);
			this.textAttributeAlias.Name = "textAttributeAlias";
			this.textAttributeAlias.ReadOnly = true;
			this.textAttributeAlias.Size = new System.Drawing.Size(592, 19);
			this.textAttributeAlias.TabIndex = 10;
			// 
			// textAttributeGUID
			// 
			this.textAttributeGUID.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeGUID.Location = new System.Drawing.Point(137, 50);
			this.textAttributeGUID.Name = "textAttributeGUID";
			this.textAttributeGUID.ReadOnly = true;
			this.textAttributeGUID.Size = new System.Drawing.Size(592, 19);
			this.textAttributeGUID.TabIndex = 11;
			// 
			// textAttributeNotes
			// 
			this.textAttributeNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeNotes.Location = new System.Drawing.Point(137, 80);
			this.textAttributeNotes.Multiline = true;
			this.textAttributeNotes.Name = "textAttributeNotes";
			this.textAttributeNotes.ReadOnly = true;
			this.textAttributeNotes.Size = new System.Drawing.Size(592, 64);
			this.textAttributeNotes.TabIndex = 12;
			// 
			// textAttributePos
			// 
			this.textAttributePos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributePos.Location = new System.Drawing.Point(137, 150);
			this.textAttributePos.Name = "textAttributePos";
			this.textAttributePos.ReadOnly = true;
			this.textAttributePos.Size = new System.Drawing.Size(592, 19);
			this.textAttributePos.TabIndex = 13;
			// 
			// textAttributeStereotype
			// 
			this.textAttributeStereotype.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeStereotype.Location = new System.Drawing.Point(137, 170);
			this.textAttributeStereotype.Name = "textAttributeStereotype";
			this.textAttributeStereotype.ReadOnly = true;
			this.textAttributeStereotype.Size = new System.Drawing.Size(592, 19);
			this.textAttributeStereotype.TabIndex = 14;
			// 
			// textAttributeClassifierID
			// 
			this.textAttributeClassifierID.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeClassifierID.Location = new System.Drawing.Point(137, 190);
			this.textAttributeClassifierID.Name = "textAttributeClassifierID";
			this.textAttributeClassifierID.ReadOnly = true;
			this.textAttributeClassifierID.Size = new System.Drawing.Size(592, 19);
			this.textAttributeClassifierID.TabIndex = 15;
			// 
			// textAttributeDefault
			// 
			this.textAttributeDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeDefault.Location = new System.Drawing.Point(137, 210);
			this.textAttributeDefault.Name = "textAttributeDefault";
			this.textAttributeDefault.ReadOnly = true;
			this.textAttributeDefault.Size = new System.Drawing.Size(592, 19);
			this.textAttributeDefault.TabIndex = 16;
			// 
			// textAttributeIsStatic
			// 
			this.textAttributeIsStatic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textAttributeIsStatic.Location = new System.Drawing.Point(137, 230);
			this.textAttributeIsStatic.Name = "textAttributeIsStatic";
			this.textAttributeIsStatic.ReadOnly = true;
			this.textAttributeIsStatic.Size = new System.Drawing.Size(592, 19);
			this.textAttributeIsStatic.TabIndex = 17;
			// 
			// AttributePropertyForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(756, 265);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "AttributePropertyForm";
			this.Text = "属性のプロパティ";
			this.Load += new System.EventHandler(this.AttributePropertyFormLoad);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.TextBox textAttributeIsStatic;
		private System.Windows.Forms.TextBox textAttributeDefault;
		private System.Windows.Forms.TextBox textAttributeClassifierID;
		private System.Windows.Forms.TextBox textAttributeStereotype;
		private System.Windows.Forms.TextBox textAttributePos;
		private System.Windows.Forms.TextBox textAttributeNotes;
		private System.Windows.Forms.TextBox textAttributeGUID;
		private System.Windows.Forms.TextBox textAttributeAlias;
		private System.Windows.Forms.TextBox textAttributeName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelAttrbuteName;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
