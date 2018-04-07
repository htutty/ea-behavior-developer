/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/20
 * Time: 11:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using BehaviorDevelop.vo;
using BehaviorDevelop.util;

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
			this.Text = "クラス: " + myElement.name + " " + myElement.guid;
			
			addElementLabels(myElement);
		}

		
		private void addElementLabels( ElementVO elem ) {
			
			addAttributeLabels(elem.attributes);
			addMethodLabels(elem.methods);
		}

		
		private void addAttributeLabels( IList<AttributeVO> attrs ) {
			
			foreach( AttributeVO a in attrs ) {
				Label attrLabel = new Label();
				
				if (a.changed == ' ') {
					attrLabel.Text = a.name;
				} else {
					attrLabel.Text = "[" + a.changed + "] " + a.name;
				}
				
//				attrLabel.Text =  "+ " + a.name ;
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
				// メソッドのタイトル行を入れるパネルを作成
				FlowLayoutPanel methodTitlePanel = new FlowLayoutPanel();
	            methodTitlePanel.Anchor = ((System.Windows.Forms.AnchorStyles)
	                ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                    | System.Windows.Forms.AnchorStyles.Left)| System.Windows.Forms.AnchorStyles.Right)));
	            methodTitlePanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
	            methodTitlePanel.WrapContents = false;
	            methodTitlePanel.AutoSize = true;

	            Button btnJump = new Button();
				btnJump.Name = "jumpbutton";
				btnJump.Text = ">> ";
				btnJump.AutoSize = true;
				btnJump.Click += new System.EventHandler(this.BtnCancelClick);
				methodTitlePanel.Controls.Add(btnJump);
				
	            Label mthLabel = new Label();
				if (m.changed == ' ') {
					mthLabel.Text = m.name;
				} else {
					mthLabel.Text = "[" + m.changed + "] " + m.name;
				}
				mthLabel.AutoSize = true;
//	            mthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)
//	                                (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
	            mthLabel.BackColor = Color.Magenta;
				methodTitlePanel.Controls.Add(mthLabel);

				
				
				
	            panel.Controls.Add(methodTitlePanel);
	            
	            TextBox mthText = new TextBox();
				setTextBoxSize(mthText, m.behavior);
				
	            mthText.ReadOnly = true;
	            mthText.Multiline = true;
	            mthText.WordWrap = false;

	            // 複数行のテキストボックスは自動サイズが利かない（常に1行分の高さになる）
//	            mthText.AutoSize = true;
	            mthText.DoubleClick += new System.EventHandler(this.MethodTextBoxDoubleClick);
	            mthText.Tag = m ;
				methodTitlePanel.Controls.Add(mthText);

	            panel.Controls.Add(mthText);
				
				this.methods.Add(m);
			}
		}
		
		public void openOrigMethodFileOnWinmerge( string guid ) {
			string commandStr = "winmergeU.exe ";
			
			string targetPath = ProjectSetting.getVO().projectPath ;
			
			commandStr += targetPath + "\\#method_" + guid.Substring(1,36) + "_L.xml  ";
			commandStr += targetPath + "\\#method_" + guid.Substring(1,36) + "_R.xml";
			
			//ブラウザで開く
		    System.Diagnostics.Process.Start(commandStr);
		}
		
		
		void setTextBoxSize(TextBox mthText, string behavior) {
            mthText.Text = behavior;
            
            // テキストボックスのサイズ設定
			string[] ary = behavior.Split('\n');
			Int32 height = 16 + ary.Length * 12;
			Int32 width = panel.Size.Width - 24;
            if ( height > 800 ) {
	            mthText.Size = new Size(width, 800);
	            mthText.ScrollBars = ScrollBars.Vertical;
            } else {
	            mthText.Size = new Size(width, height);
	            mthText.ScrollBars = ScrollBars.None;
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
		
		
		void ElementFormResize(object sender, EventArgs e)
		{
			// 変更後のフォームサイズ
            Size newFormSize = this.Size;
			
			this.panel.Width = newFormSize.Width - 32;
			this.panel.Height = newFormSize.Height - 80;
			
			foreach( Control c in panel.Controls ) {
				c.Width = newFormSize.Width - 60 ;
			}

		}

		
		void ButtonCopyContentClick(object sender, EventArgs e) {
			try {
				Clipboard.SetText(myElement.toDescriptorString());
				MessageBox.Show( "クラス情報がクリップボードにコピーされました" );
			} catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
		
		
		void ButtonOutputJavaClick(object sender, EventArgs e) {
			try {
				Encoding utf8Enc = Encoding.GetEncoding("utf-8");
				StreamWriter writer =
				 new StreamWriter( ProjectSetting.GetAppProfileDir() + "\\Test.java", true, utf8Enc);
				writer.WriteLine( myElement.toDescriptorString() );
				writer.Close();

				MessageBox.Show( "Javaソースをファイル出力しました" );
			} catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
		

	}
}
