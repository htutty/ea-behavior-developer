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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.reader;


namespace ElementEditor
{
	/// <summary>
	/// Description of ElementForm.
	/// </summary>
	public partial class ElementForm : Form
	{
		ElementVO myElement;
		ElementVO savedElement;

		List<MethodVO> methods = new List<MethodVO>();
		List<TextBox> methNameList = new List<TextBox>();
		List<TextBox> behaviorTextList = new List<TextBox>();
				
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
		
		public ElementForm(ref ElementVO element) {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

            // changed のXMLファイルが存在する場合はそちらを読み込む
            if (ElementsXmlReader.existChangedElementFile(element.guid))
            {
                element = ElementsXmlReader.readChangedElementFile(element.guid);
            }

            // このフォームを開いた時点の要素を保存
            savedElement = element;
			
			// このフォーム内で変更されうる要素オブジェクトはクローンで作成
			myElement = element.Clone();
			
			// 
			this.Text = myElement.eaType + " " + myElement.name + " " + myElement.guid;
			
			if( ProjectSetting.getEARepo() == null ) {
				buttonViewDiff.Enabled = false;
			} else {
				buttonViewDiff.Enabled = true;
			}
			
			setElementItems(myElement);
		}

		
		public void repaint(ElementVO refreshElem) {
			this.savedElement = refreshElem;
			this.myElement = refreshElem.Clone();
			this.panel.Controls.Clear();
			
			methods = new List<MethodVO>();
			methNameList = new List<TextBox>();
			behaviorTextList = new List<TextBox>();
			
			setElementItems(myElement);
		}
		
		/// <summary>
		/// 要素（クラス）の名称など、クラスの情報を表示する
		/// </summary>
		/// <param name="elem"></param>
		private void setElementItems( ElementVO elem ) {

			// タイトルのクラス名のテキストをセット
			classNameText.Text = elem.name;
			classNameText.BackColor = Color.White;
			classNameText.Anchor = ((System.Windows.Forms.AnchorStyles)
	                ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                    | System.Windows.Forms.AnchorStyles.Left)| System.Windows.Forms.AnchorStyles.Right)));
			classNameText.ContextMenuStrip = this.classContextMenuStrip;
			
			switch(elem.eaType) {
				case "Screen":
					elementTypePictureBox.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_SCREEN;
					break;
				case "GUIElement":
					elementTypePictureBox.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_UIELEMENT;
					break;
				default:
					elementTypePictureBox.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_CLASS;
					break;
			}
			elementTypePictureBox.Name = "elementTypePictureBox";
			elementTypePictureBox.Location = new System.Drawing.Point(3, 3);
			elementTypePictureBox.Size = new System.Drawing.Size(20, 20);
			elementTypePictureBox.TabStop = false;
			

			if ( myElement.elementReferenceVO != null ) {
				implFileLinkLabel.Text = myElement.elementReferenceVO.fqcn ;
				implFileLinkLabel.Tag = myElement.elementReferenceVO;
			} else {
				implFileLinkLabel.Visible = false;
			}
			
			panel.SuspendLayout();
			makeTaggedValueItems(elem.taggedValues);
			makeAttributeItems(elem.attributes);
			makeMethodItems(elem.methods);
			panel.ResumeLayout();
			
			btnCommit.Enabled = false;
		}

		
		/// <summary>
		/// この要素が保持するタグ付き値の画面表示項目を作成する
		/// </summary>
		/// <param name="attrs">タグ付き値リスト</param>
		private void makeTaggedValueItems( List<TaggedValueVO> tagvals ) {
			
			foreach( TaggedValueVO tv in tagvals ) {
				// タグ付き値を入れるパネルを作成
				FlowLayoutPanel tvPanel = new FlowLayoutPanel();
	            tvPanel.Anchor = ((System.Windows.Forms.AnchorStyles)
	                (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
	                    | System.Windows.Forms.AnchorStyles.Right)));
	            tvPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
	            tvPanel.WrapContents = false;
	            tvPanel.AutoSize = true;
				tvPanel.Margin = new System.Windows.Forms.Padding(1);
				
				Size parentSize = panel.Size;
				
				// tvSymbol
				PictureBox tvSymbol = new PictureBox();
				tvSymbol.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_TAGGEDVALUE;
				tvSymbol.Location = new System.Drawing.Point(3, 3);
				tvSymbol.Name = "tagvalSymbol";
				tvSymbol.Size = new System.Drawing.Size(20, 20);
				tvSymbol.TabStop = false;
				
				// タグ付き値の名前を表示するテキスト
				TextBox tvNameText = new TextBox();
	            tvNameText.ReadOnly = true;
	            tvNameText.Multiline = false;
	            tvNameText.WordWrap = false;
	            tvNameText.AutoSize = true;
				tvNameText.BackColor = Color.Honeydew;
				
				if (tv.changed == ' ') {
					tvNameText.Text = tv.name;
				} else {
					tvNameText.Text = "[" + tv.changed + "] " + tv.name;
				}
	            tvNameText.Size = new Size(parentSize.Width / 2 - 48, 24);
	            
				// タグ付き値の値を表示するテキスト
	            TextBox tvValueText = new TextBox();
	            tvValueText.ReadOnly = true;
				tvValueText.BackColor = Color.Honeydew;
				
				string tmp = "";
				if ("<memo>".Equals(tv.tagValue)) {
					tmp = tv.notes;
				} else {
					tmp = tv.tagValue;
				}
				tvValueText.Text = tmp;

				// 改行文字がある、もしくは文字列の長さが60文字以上の場合
				if( tmp.IndexOf('\n', 0) > 0 || tmp.Length > 50 ) {
		            tvValueText.Size = new Size(parentSize.Width / 2 - 36, 80);
	            	tvValueText.Multiline = true;
				} else { 
		            tvValueText.Size = new Size(parentSize.Width / 2 - 36, 20);
	            	tvValueText.Multiline = false;
				}
	            tvPanel.Controls.Add(tvSymbol);
	            tvPanel.Controls.Add(tvNameText);
	            tvPanel.Controls.Add(tvValueText);
	            
				panel.Controls.Add(tvPanel);
			}
		}

		
		
		/// <summary>
		/// この要素が保持する属性の表示項目を作成する
		/// </summary>
		/// <param name="attrs">属性リスト</param>
		private void makeAttributeItems( List<AttributeVO> attrs ) {
			
			foreach( AttributeVO a in attrs ) {
				// 属性行を入れるパネルを作成
				FlowLayoutPanel attrPanel = new FlowLayoutPanel();
	            attrPanel.Anchor = ((System.Windows.Forms.AnchorStyles)
	                (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
	                    | System.Windows.Forms.AnchorStyles.Right)));
	            attrPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
	            attrPanel.WrapContents = false;
	            attrPanel.AutoSize = true;
				attrPanel.Margin = new System.Windows.Forms.Padding(1);
				
				// attrSymbol
				PictureBox attrSymbol = new PictureBox();
				attrSymbol.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_ATTRIBUTE;
				attrSymbol.Location = new System.Drawing.Point(3, 3);
				attrSymbol.Name = "attrSymbol";
				attrSymbol.Size = new System.Drawing.Size(20, 20);
				attrSymbol.TabStop = false;
				attrSymbol.Tag = a;
				attrSymbol.Click += new System.EventHandler(this.AttributeSymbolClick);
				
				// attrText
				TextBox attrText = new TextBox();
	            attrText.ReadOnly = true;
	            attrText.Multiline = false;
	            attrText.WordWrap = false;
	            attrText.AutoSize = true;
				attrText.BackColor = Color.LightCyan;
				
				if (a.changed == ' ') {
					attrText.Text = a.name;
				} else {
					attrText.Text = "[" + a.changed + "] " + a.name;
				}
				
				Size parentSize = panel.Size;
	            attrText.Size = new Size(parentSize.Width - 48, 24);
	            
	            attrPanel.Controls.Add(attrSymbol);
	            attrPanel.Controls.Add(attrText);
	            
				panel.Controls.Add(attrPanel);
			}
		}

		
		
		/// <summary>
		/// この要素が保持するメソッドの表示項目を作成する
		/// </summary>
		/// <param name="mths">メソッドリスト</param>
		private void makeMethodItems( List<MethodVO> mths ) {
			int i;
			
			i=0;
			foreach( MethodVO m in mths ) {
				// メソッドのタイトル行を入れるパネルを作成
				FlowLayoutPanel methodTitlePanel = new FlowLayoutPanel();
	            methodTitlePanel.Anchor = ((System.Windows.Forms.AnchorStyles)
	                ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                    | System.Windows.Forms.AnchorStyles.Left)| System.Windows.Forms.AnchorStyles.Right)));
	            methodTitlePanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
	            methodTitlePanel.WrapContents = false;
	            methodTitlePanel.AutoSize = true;
				methodTitlePanel.Margin = new System.Windows.Forms.Padding(1);

	            
	            // methSymbol
				PictureBox methSymbol = new PictureBox();
				methSymbol.Image = global::ElementEditor.Properties.Resources.ICON_SYMBOL_METHOD;
				methSymbol.Location = new System.Drawing.Point(3, 3);
				methSymbol.Name = "methSymbol";
				methSymbol.Size = new System.Drawing.Size(20, 20);
				methSymbol.TabStop = false;
				methSymbol.Tag = m;
				methSymbol.Click += new System.EventHandler(this.MethodSymbolClick);
				
	            // methNameText
				TextBox methNameText = new TextBox();
				
	            methNameText.ReadOnly = true;
	            methNameText.Multiline = false;
	            methNameText.WordWrap = false;
	            methNameText.AutoSize = true;
				methNameText.BackColor = Color.LightYellow;
				methNameText.ContextMenuStrip = methodContextMenuStrip;
				methNameText.Tag = m;

                string methodDesc;
                if(m.getParamDesc() != null && m.getParamDesc() != "")
                {
                    methodDesc = m.name + "(" + m.getParamDesc() + ")";
                }
                else
                {
                    methodDesc = m.name + "()";
                }

                if (m.changed == ' ') {
					methNameText.Text = methodDesc;
				} else {
					methNameText.Text = "[" + m.changed + "] " + methodDesc;
				}

				Size parentSize = panel.Size;
	            methNameText.Size = new Size(parentSize.Width - 48, 24);
				methNameList.Add(methNameText);
	            
				methodTitlePanel.Controls.Add(methSymbol);
				methodTitlePanel.Controls.Add(methNameText);
	            panel.Controls.Add(methodTitlePanel);
	            
	            TextBox behaviorText = new TextBox();
				setTextBoxSize(behaviorText, m.behavior);
//	            behaviorText.ReadOnly = false;
	            behaviorText.ReadOnly = true;
	            behaviorText.Multiline = true;
	            behaviorText.WordWrap = false;
	            behaviorText.Visible = false;
	            
	            
	            // 複数行のテキストボックスは自動サイズが利かない（常に1行分の高さになる）
//	            mthText.AutoSize = true;
	            behaviorText.DoubleClick += new System.EventHandler(this.MethodTextBoxDoubleClick);
	            behaviorText.Tag = m ;
	            panel.Controls.Add(behaviorText);
				behaviorTextList.Add(behaviorText);
				
				Label behaviorEllipsisLabel = new Label();
				behaviorEllipsisLabel.Text = "    ...";
				behaviorEllipsisLabel.Tag = i;
	            behaviorEllipsisLabel.Click += new System.EventHandler(this.BehaviorEllipsisLabelClick);

	            panel.Controls.Add(behaviorEllipsisLabel);
	            i++;
				
				this.methods.Add(m);
			}
		}
		
		
		void setTextBoxSize(TextBox mthText, string behavior) {
            mthText.Text = behavior;
            
            if ( behavior != null ) {
	            // テキストボックスのサイズ設定
				string[] ary = behavior.Split('\n');
				Int32 height = 16 + ary.Length * 12;
				Int32 width = panel.Size.Width - 54;
	            if ( height > 800 ) {
		            mthText.Size = new Size(width, 800);
		            mthText.ScrollBars = ScrollBars.Vertical;
	            } else {
		            mthText.Size = new Size(width, height);
		            mthText.ScrollBars = ScrollBars.None;
	            }
            }

			mthText.Margin = new System.Windows.Forms.Padding(30,3,3,3);            
		}
		
		
        #region "ボタンなどのコントロールのイベントハンドラ"
		
        /// <summary>
        /// 属性アイコンクリック時のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void AttributeSymbolClick(object sender, EventArgs e)
		{
			PictureBox targetIcon = (PictureBox)sender;
			AttributeVO attrvo = (AttributeVO)targetIcon.Tag;
			
			AttributePropertyForm attrPropForm = new AttributePropertyForm(attrvo);
			attrPropForm.Show(this);			
		}
        
		/// <summary>
        /// 操作アイコンクリック時のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void MethodSymbolClick(object sender, EventArgs e)
		{
			PictureBox targetIcon = (PictureBox)sender;
			MethodVO methvo = (MethodVO)targetIcon.Tag;
			
			MethodPropertyForm methPropForm = new MethodPropertyForm(methvo);
			methPropForm.Show(this);			
		}
		
        /// <summary>
        /// 振る舞いを省略するラベルのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void BehaviorEllipsisLabelClick(object sender, EventArgs e)
		{
			Label targetLabel = (Label)sender;
			int idx = (int)targetLabel.Tag;
			
			TextBox behaviorText = behaviorTextList[idx];
			behaviorText.Visible = !(behaviorText.Visible);
			
			targetLabel.Visible = false;
		}
		
		/// <summary>
		/// メソッド振る舞いのテキストボックスのダブルクリックイベントハンドラ(振る舞い編集画面を開く)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MethodTextBoxDoubleClick(object sender, EventArgs e)
		{
			MethodVO method = (MethodVO)((TextBox)sender).Tag ;
            //			MessageBox.Show("method text box double clicked");

            // Form f = new MethodBehaviorEditForm(myElement, method);
            // f.ShowDialog(this);

            var window = new BehaviorEditor(myElement, method);
            // var window = new BehaviorWindow();
            ElementHost.EnableModelessKeyboardInterop(window);

            window.Show();

        }


        /// <summary>
        /// 一時保存ボタンのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnCommitClick(object sender, EventArgs e)
		{
			string outputDir = ProjectSetting.getVO().projectPath;
			
			StreamWriter swe = null;
			try { 
				//BOM無しのUTF8でテキストファイルを作成する
				swe = new StreamWriter(outputDir + @"\elements\" + myElement.guid.Substring(1,36) + "_changed.xml");
				swe.WriteLine( @"<?xml version=""1.0"" encoding=""UTF-8""?> " );

				ElementXmlWriter.writeElementXml(myElement, 0, swe);
				btnCommit.Enabled = false;
			} catch(Exception exp) {
				MessageBox.Show(exp.Message);
			} finally {
				if (swe != null) swe.Close();
			}
			
			savedElement = myElement;
			
			//メッセージボックスを表示する
			MessageBox.Show("クラスの変更内容をローカルファイルに記録しました",
			    "確認",
			    MessageBoxButtons.OK,
			    MessageBoxIcon.Information);
			
//			this.Close();
		}

		/// <summary>
		/// キャンセルボタンのイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void BtnCancelClick(object sender, EventArgs e)
		{
			// 保存ボタンが押下できるか？（＝未保存のふるまい変更があるか）
			if ( btnCommit.Enabled == true ) {
				DialogResult res = MessageBox.Show("まだ保存されていない変更がありますが、クローズしますか？", "確認", 
				                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
				
				if ( res == DialogResult.Yes ) {
					// elementをこのフォームを開いた時の状態に戻す（属性リスト、メソッドリスト、changed）
					myElement.attributes = savedElement.attributes;
					myElement.methods = savedElement.methods;
					myElement.changed = savedElement.changed;
					this.Close();
				}
				
			} else {
				this.Close();
			}
		}
		
		/// <summary>
		/// フォームのサイズ変更時のイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ElementFormResize(object sender, EventArgs e)
		{
			// 変更後のフォームサイズ
            Size newFormSize = this.Size;
			
			this.panel.Width = newFormSize.Width - 32;
			this.panel.Height = newFormSize.Height - 110;
			
			foreach(Control c in panel.Controls) {
				c.Width = newFormSize.Width - 90;
			}

		}

		/// <summary>
		/// クラス内容をコピーボタンのクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCopyContentClick(object sender, EventArgs e) {
			try {
				Clipboard.SetText(myElement.toDescriptorString());
				MessageBox.Show( "クラス情報がクリップボードにコピーされました" );
			} catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		
		/// <summary>
		/// 簡易Java出力ボタンのクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonOutputJavaClick(object sender, EventArgs e) {
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
		

		/// <summary>
		/// クラスのコンテキストメニュー-「EAでこのクラスを選択」のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void FocusEAClassToolStripMenuItemClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;			
			
			if (repo != null) {
				EA.Element elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
				if ( elem != null ) {
					repo.ShowInProjectView(elem);
				}
			}
		}

		/// <summary>
		/// クラスのコンテキストメニュー-「GUIDをコピー」のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void CopyGuidClassToolStripMenuItemClick(object sender, EventArgs e)
		{
			try {
				Clipboard.SetText(myElement.guid);
				MessageBox.Show("クラスGUIDがクリップボードにコピーされました");
			} catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		
		/// <summary>
		/// メソッドのコンテキストメニュー-「EAでこのメソッドを選択」のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void FocusEAMethodToolStripMenuItemClick(object sender, EventArgs e)
		{
			//ContextMenuStripを表示しているコントロールを取得する
			Control source = methodContextMenuStrip.SourceControl;
			if (source != null) {
				MethodVO mth = (MethodVO)((Control)source).Tag;
				EA.Repository repo = ProjectSetting.getVO().eaRepo;			
				
				if (mth != null && repo != null) {
					EA.Method meth = (EA.Method)repo.GetMethodByGuid(mth.guid);
					if ( meth != null ) {
						repo.ShowInProjectView(meth);
					}
				}

			}
		}


		/// <summary>
		/// メソッドのコンテキストメニュー-「GUIDをコピー」のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void CopyGuidMethodToolStripMenuItemClick(object sender, EventArgs e)
		{
			Control source = methodContextMenuStrip.SourceControl;
			if (source != null) {
				MethodVO mth = (MethodVO)((Control)source).Tag;
				EA.Repository repo = ProjectSetting.getVO().eaRepo;			
				
				try {
					if (mth != null) {
						Clipboard.SetText(mth.guid);
						MessageBox.Show("メソッドのGUIDがクリップボードにコピーされました");
					}
				} catch(Exception ex) {
					Console.WriteLine(ex.Message);
				}

			}
			
		}

		
		/// <summary>
		/// クラス名のコンテキストメニューの表示可否を設定
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ClassContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;			
			if ( repo != null ) {
				focusEAClassToolStripMenuItem.Enabled = true;
//				classContextMenuStrip.Enabled = true ;
			} else {
				focusEAClassToolStripMenuItem.Enabled = false;
//				classContextMenuStrip.Enabled = false ;
			}			
		}
				
		/// <summary>
		/// メソッドのコンテキストメニューのオープン時イベントハンドラ（利用可否制御）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MethodContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;			
			if ( repo != null ) {
				focusEAMethodToolStripMenuItem.Enabled = true;
			} else {
				focusEAMethodToolStripMenuItem.Enabled = false;
			}
		}
		
		
		/// <summary>
		/// 変更差分確認ボタンのクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ButtonViewDiffClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			EA.Element eaElemObj = (EA.Element)repo.GetElementByGuid(myElement.guid) ;
			ElementVO eaElement = ObjectEAConverter.getElementFromEAObject(eaElemObj);

			// 差分フォームを開く
			DiffElementForm diffForm = new DiffElementForm(eaElement, ref myElement);
			diffForm.ShowDialog(this);
			diffForm.Dispose();
		}
		
		
		/// <summary>
		/// 実装Javaファイルリンクのクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ImplFileLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ElementReferenceVO elemref = (ElementReferenceVO)((LinkLabel)sender).Tag ;
			//Javaファイルをエディタで開く
			System.Diagnostics.Process p =
			    System.Diagnostics.Process.Start(elemref.genfile);
		}
		#endregion
		
		/// <summary>
		/// 編集後の振る舞いを再表示する処理（子画面の振る舞い編集画面から呼び出される）
		/// </summary>
		/// <param name="method">再表示対象のメソッド</param>
		public void repaintFormMethod(MethodVO method) {
			foreach(TextBox mtxt in methNameList) {
				if( method == mtxt.Tag ) {
					MethodVO m = (MethodVO)mtxt.Tag;
					if (m.changed == ' ') {
						mtxt.Text = m.name;
					} else {
						mtxt.Text = "[" + m.changed + "] " + m.name;
					}
				}
			}

			foreach(TextBox btxt in behaviorTextList) {
				if( method == btxt.Tag ) {
					MethodVO m = (MethodVO)btxt.Tag;
					setTextBoxSize(btxt, m.behavior);
				}
			}
			
			btnCommit.Enabled = true;
		}

        private void ElementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( this.Owner != null )
            {
                MainForm main = (MainForm)this.Owner;
                main.deleteOpenedElement(myElement);
            }

        }
    }
}
