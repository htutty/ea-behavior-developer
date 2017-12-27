/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/20
 * Time: 11:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using BehaviorDevelop.vo;

namespace BehaviorDevelop
{
	/// <summary>
	/// Description of ElementForm.
	/// </summary>
	public partial class ElementForm : Form
	{

		ElementVO myElement;

		IList<MethodVO> methods = new List<MethodVO>();

		
		
		public ElementForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public ElementForm(ElementVO element) {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			myElement = element;
			
			// 
			this.Text = "■クラス: " + myElement.name;
			
			addElementLabels(myElement);
		}

		
		private void addElementLabels( ElementVO elem ) {
			
			addAttributeLabels(elem.attributes);
			addMethodLabels(elem.methods);
		}

		
		private void addAttributeLabels( IList<AttributeVO> attrs ) {
			
			foreach( AttributeVO a in attrs ) {
				Label attrLabel = new Label();
				attrLabel.Text =  "+ " + a.name ;
				attrLabel.TextAlign = ContentAlignment.MiddleLeft ;
//	            attrLabel.AutoSize=true;
	            attrLabel.Anchor = ((System.Windows.Forms.AnchorStyles)
	                                (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
	            attrLabel.BackColor = Color.LightCyan;
	            
				panel.Controls.Add(attrLabel);
			}
		}
		
		private void addMethodLabels( IList<MethodVO> mths ) {
			
			foreach( MethodVO m in mths ) {
				Label mthLabel = new Label();
				mthLabel.Text = m.name ;
	            mthLabel.AutoSize=true;
//	            mthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)
//	                                (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
	            mthLabel.BackColor = Color.Magenta;
	            panel.Controls.Add(mthLabel);
	            
	            TextBox mthText = new TextBox();
	            mthText.Text = m.behavior;
				string[] ary = m.behavior.Split('\n');
	            if ( ary.Length > 10 ) {
		            mthText.Size = new Size(800,200);
	            } else if( ary.Length > 3 ) {
		            mthText.Size = new Size(800,100);
	            } else {
		            mthText.Size = new Size(800,40);
	            }
//				mthText.Anchor = ((System.Windows.Forms.AnchorStyles)
//	                                (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));

	            mthText.ReadOnly = true;
	            mthText.Multiline = true;
	            mthText.WordWrap = false;
	            mthText.ScrollBars = ScrollBars.Both;
	            mthText.DoubleClick += new System.EventHandler(this.MethodTextBoxDoubleClick);
	            mthText.Tag = m ;
	            // mthText.ScrollBars = true;
				panel.Controls.Add(mthText);
				
				this.methods.Add(m);
			}
		}
		
		
		void MethodTextBoxDoubleClick(object sender, EventArgs e)
		{
			MethodVO method = (MethodVO)((TextBox)sender).Tag ;
//			MessageBox.Show("method text box double clicked");
			Form f = new MethodBehaviorEditForm(myElement, method);
	   		f.ShowDialog(this);
		}
		
				
		void BtnCommitClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void BtnCancelClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		
		void TextBox1DoubleClick(object sender, EventArgs e)
		{
		
		}
	}
}
