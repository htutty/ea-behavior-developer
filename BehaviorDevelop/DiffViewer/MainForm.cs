/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/04/09
 * Time: 16:36
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using VoidNish.Diff;


namespace DiffViewer
{
	/// <summary>
	/// Description of DiffElementForm.
	/// </summary>
	public partial class MainForm : Form
	{
		ElementVO myElement;
		List<MethodVO> methods = new List<MethodVO>();
		List<AttributeVO> attributes = new List<AttributeVO>();
        List<TaggedValueVO> taggedValues = new List<TaggedValueVO>();

        AttributeVO selectedAttribute = null;
		MethodVO selectedMethod = null;
        TaggedValueVO selectedTag = null;


        StringBuilder leftDiffBuffer;
		StringBuilder rightDiffBuffer;


		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		public MainForm(ElementVO element) {
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

			// 今開いているEAへのアタッチを試みる
			AttachEA();
		}

		private void AttachEA()
		{
			EA.App eaapp = null;

			try {
				eaapp = (EA.App)Microsoft.VisualBasic.Interaction.GetObject(null, "EA.App");
			} catch(Exception e) {
				toolStripStatusLabel1.Text = "EAが起動していなかったため、EAへの反映機能は使えません : " + e.Message;
				// MessageBox.Show( e.Message );
				return;
			} finally {
			}

			if ( ProjectSetting.getVO() != null ) {
				if( eaapp != null ) {
					EA.Repository repo = eaapp.Repository;
//					eaapp.Visible = true;
					ProjectSetting.getVO().eaRepo = repo;
					toolStripStatusLabel1.Text = "EAへのアタッチ成功 EA接続先=" + repo.ConnectionString;
				} else {
					toolStripStatusLabel1.Text = "EAにアタッチできなかったため、EAへの反映機能は使えません";
				}

			}
		}


		private void addElementLabels( ElementVO elem ) {
			int rowIndex = 0;

            string artifactsDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
            ArtifactXmlReader reader = new ArtifactXmlReader(artifactsDir);

			tableLayoutPanel1.RowCount = elem.attributes.Count + elem.methods.Count + elem.taggedValues.Count;

			foreach( AttributeVO attr in elem.attributes )
            {
                addAttributeBox(rowIndex, reader, attr);
                rowIndex++;
            }

            foreach( MethodVO mth in elem.methods )
            {
                addMethodBox(rowIndex, reader, mth);
                rowIndex++;
			}

            foreach (TaggedValueVO tgv in elem.taggedValues)
            {
                addTaggedValueBox(rowIndex, reader, tgv);
                rowIndex++;
            }

        }



        private void addAttributeBox(int rowIndex, ArtifactXmlReader reader, AttributeVO attr)
        {
            int longerLine;
            AttributeVO leftAttr = attr;
            AttributeVO rightAttr = attr;
            string leftText = "";
            string rightText = "";
            ListBox listL = makeNewListBox();
            ListBox listR = makeNewListBox();

            switch (attr.changed)
            {
                case 'U':
                    leftAttr = reader.readAttributeDiffDetail(attr.guid, "L");
                    rightAttr = reader.readAttributeDiffDetail(attr.guid, "R");
                    DiffPresenter.getDisagreedAttributeDesc(leftAttr, rightAttr, ref leftText, ref rightText);
                    selectedAttribute = rightAttr;
                    break;

                case 'C':
                    rightAttr = reader.readAttributeDiffDetail(attr.guid, "R");
                    DiffPresenter.getMonoAttributeDesc(rightAttr, ref rightText);
                    selectedAttribute = rightAttr;
                    break;

                case 'D':
                    leftAttr = reader.readAttributeDiffDetail(attr.guid, "L");
                    DiffPresenter.getMonoAttributeDesc(leftAttr, ref leftText);
                    selectedAttribute = leftAttr;
                    break;

                default:
                    break;
            }

            longerLine = getLongerLine(leftText, rightText);

            setListItems(listL, leftText);
            setListItems(listR, rightText);
            setListBoxSize(listL, leftText, longerLine);
            setListBoxSize(listR, rightText, longerLine);

            if (attr.changed == 'D' || attr.changed == 'U')
            {
                listL.Tag = leftAttr;
                listL.ContextMenuStrip = contextMenuStrip1;
                listL.Click += new System.EventHandler(this.AttributeListClick);
                //					listL.Click += new System.EventHandler(this.AttributeTextClick);
            }

            if (attr.changed == 'C' || attr.changed == 'U')
            {
                listR.Tag = rightAttr;
                listR.ContextMenuStrip = contextMenuStrip1;
                listR.Click += new System.EventHandler(this.AttributeListClick);
                //					listR.Click += new System.EventHandler(this.AttributeTextClick);
            }

            tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
            tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);

            return;
        }


        private void addMethodBox(int rowIndex, ArtifactXmlReader reader, MethodVO mth)
        {
            int longerLine;
            MethodVO leftMth = mth;
            MethodVO rightMth = mth;
            string leftText = "";
            string rightText = "";
            ListBox listL = makeNewListBox();
            ListBox listR = makeNewListBox();

            switch (mth.changed)
            {
                case 'U':
                    leftMth = reader.readMethodDiffDetail(mth.guid, "L");
                    rightMth = reader.readMethodDiffDetail(mth.guid, "R");
                    DiffPresenter.getDisagreedMethodDesc(leftMth, rightMth, ref leftText, ref rightText);

                    leftDiffBuffer = new StringBuilder();
                    rightDiffBuffer = new StringBuilder();
                    var simpleDiff = new SimpleDiff<string>(leftText.Split('\n'), rightText.Split('\n'));
                    simpleDiff.LineUpdate += new EventHandler<DiffEventArgs<string>>(ElementDiff_LineUpdate);
                    simpleDiff.RunDiff();

                    leftText = leftDiffBuffer.ToString();
                    rightText = rightDiffBuffer.ToString();

                    selectedMethod = rightMth;
                    leftDiffBuffer.Clear();
                    rightDiffBuffer.Clear();
                    break;

                case 'C':
                    rightMth = reader.readMethodDiffDetail(mth.guid, "R");
                    DiffPresenter.getMonoMethodDesc(rightMth, ref rightText);

                    selectedMethod = rightMth;
                    break;

                case 'D':
                    leftMth = reader.readMethodDiffDetail(mth.guid, "L");
                    DiffPresenter.getMonoMethodDesc(leftMth, ref leftText);

                    selectedMethod = leftMth;
                    break;

                default:
                    break;
            }

            longerLine = getLongerLine(leftText, rightText);

            setListItems(listL, leftText);
            setListItems(listR, rightText);
            setListBoxSize(listL, leftText, longerLine);
            setListBoxSize(listR, rightText, longerLine);

            if (mth.changed == 'D' || mth.changed == 'U')
            {
                listL.Tag = leftMth;
                listL.ContextMenuStrip = contextMenuStrip1;
                listL.Click += new System.EventHandler(this.MethodListClick);
                //					listL.Click += new System.EventHandler(this.MethodTextClick);
            }

            if (mth.changed == 'C' || mth.changed == 'U')
            {
                listR.Tag = rightMth;
                listR.ContextMenuStrip = contextMenuStrip1;
                listR.Click += new System.EventHandler(this.MethodListClick);
                //					listR.Click += new System.EventHandler(this.MethodTextClick);
            }

            tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
            tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);

            return;
        }


        private void addTaggedValueBox(int rowIndex, ArtifactXmlReader reader, TaggedValueVO tgv)
        {
            int longerLine;
            TaggedValueVO leftTgv = tgv;
            TaggedValueVO rightTgv = tgv;
            string leftText = "";
            string rightText = "";
            ListBox listL = makeNewListBox();
            ListBox listR = makeNewListBox();

            switch (tgv.changed)
            {
                case 'U':
                    leftTgv = reader.readTaggedValueDiffDetail(tgv.guid, "L");
                    rightTgv = reader.readTaggedValueDiffDetail(tgv.guid, "R");
                    DiffPresenter.getDisagreedTaggedValueDesc(leftTgv, rightTgv, ref leftText, ref rightText);

                    leftDiffBuffer = new StringBuilder();
                    rightDiffBuffer = new StringBuilder();
                    var simpleDiff = new SimpleDiff<string>(leftText.Split('\n'), rightText.Split('\n'));
                    simpleDiff.LineUpdate += new EventHandler<DiffEventArgs<string>>(ElementDiff_LineUpdate);
                    simpleDiff.RunDiff();

                    leftText = leftDiffBuffer.ToString();
                    rightText = rightDiffBuffer.ToString();

                    selectedTag = rightTgv;
                    leftDiffBuffer.Clear();
                    rightDiffBuffer.Clear();
                    break;

                case 'C':
                    rightTgv = reader.readTaggedValueDiffDetail(tgv.guid, "R");
                    DiffPresenter.getMonoTaggedValueDesc(rightTgv, ref rightText);

                    selectedTag = rightTgv;
                    break;

                case 'D':
                    leftTgv = reader.readTaggedValueDiffDetail(tgv.guid, "L");
                    DiffPresenter.getMonoTaggedValueDesc(leftTgv, ref leftText);

                    selectedTag = leftTgv;
                    break;

                default:
                    break;
            }

            longerLine = getLongerLine(leftText, rightText);

            setListItems(listL, leftText);
            setListItems(listR, rightText);
            setListBoxSize(listL, leftText, longerLine);
            setListBoxSize(listR, rightText, longerLine);

            if (tgv.changed == 'D' || tgv.changed == 'U')
            {
                listL.Tag = leftTgv;
                listL.ContextMenuStrip = contextMenuStrip1;
                listL.Click += new System.EventHandler(this.TaggedValueListClick);
                //					listL.Click += new System.EventHandler(this.MethodTextClick);
            }

            if (tgv.changed == 'C' || tgv.changed == 'U')
            {
                listR.Tag = rightTgv;
                listR.ContextMenuStrip = contextMenuStrip1;
                listR.Click += new System.EventHandler(this.TaggedValueListClick);
                //					listR.Click += new System.EventHandler(this.MethodTextClick);
            }

            tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
            tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);

            return;
        }



        private ListBox makeNewListBox()
        {
            ListBox listL = new ListBox();
            listL.DrawMode = DrawMode.OwnerDrawFixed;
            listL.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);
            return listL;
        }

        private void setListItems(ListBox lst, string setStr ) {
			string[] ary = setStr.Split('\n');

			for( int i = 0; i < ary.Length; i++ ) {
				lst.Items.Add(ary[i]);
			}
		}


		private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
	        Color backcolor = Color.White;
	        SolidBrush brush;

	        ListBox lb = (ListBox)sender;


			if (e.Index < 0) return;
			string t = (string)lb.Items[e.Index];

			if ( t.Length > 0 ) {
				switch( t.Substring(0,1) ) {
					case " " :
						backcolor = Color.White;
						break;
					case "+" :
						backcolor = Color.LightSalmon;
						break;
					case "-" :
						backcolor = Color.LightCyan;
						break;
				}

			} else {
				backcolor = Color.White;
			}

			brush = new SolidBrush(Color.FromArgb(32, 32, 32));

			e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State , e.ForeColor, backcolor );
			e.DrawBackground();

			e.Graphics.DrawString(lb.Items[e.Index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
			e.DrawFocusRectangle();
		}


		private Int32 getLongerLine(string leftText, string rightText) {

            // テキストの行数のうち大きいほうを返却
			string[] lary = leftText.Split('\n');
			string[] rary = rightText.Split('\n');

			if ( lary.Length >= rary.Length ) {
				return lary.Length;
			} else {
				return rary.Length;
			}

		}

        private void setTextBoxSize(TextBox txtBox, string content, int lines) {
            txtBox.Text = content;
            txtBox.ReadOnly = true;
            txtBox.Multiline = true;
            txtBox.WordWrap = false;

            // テキストボックスのサイズ設定
			Int32 height = 20 + lines * 12;
			Int32 width = 800 ; // panel.Size.Width - 24;
            if ( height > 400 ) {
	            txtBox.Size = new Size(width, 400);
	            txtBox.ScrollBars = ScrollBars.Vertical;
            } else {
	            txtBox.Size = new Size(width, height);
	            txtBox.ScrollBars = ScrollBars.None;
            }

		}


		private void setListBoxSize(ListBox lstBox, string content, int lines) {
            lstBox.Text = content;

            // テキストボックスのサイズ設定
			Int32 height = 20 + lines * 12;
			Int32 width = 800 ; // panel.Size.Width - 24;
            if ( height > 400 ) {
	            lstBox.Size = new Size(width, 400);
//	            lstBox.ScrollBars = ScrollBars.Vertical;
            } else {
	            lstBox.Size = new Size(width, height);
//	            lstBox.ScrollBars = ScrollBars.None;
            }

		}



//		void AttributeTextClick(object sender, EventArgs e)
//		{

//			TextBox touchedText =  (TextBox)sender;
//			if ( selectedTextBox != null && selectedTextBox != touchedText ) {
//				selectedTextBox.BackColor = Color.LightYellow;
//			}
//			touchedText.BackColor = Color.LightPink;
//			selectedTextBox = touchedText;

//			selectedAttribute = (AttributeVO)touchedText.Tag;

//			selectedMethod = null;
//		}

//		void MethodTextClick(object sender, EventArgs e)
//		{
//			TextBox touchedText =  (TextBox)sender;
//			if ( selectedTextBox != null && selectedTextBox != touchedText ) {
//				selectedTextBox.BackColor = Color.LightYellow;
//			}
//			touchedText.BackColor = Color.LightPink;
//			selectedTextBox = touchedText;

//			selectedMethod = (MethodVO)touchedText.Tag;
////			if( selectedMethod != null ) {
////				MessageBox.Show("操作が選択されました: " + selectedMethod.guid);
////			}
//			selectedAttribute = null;
//		}


		void AttributeListClick(object sender, EventArgs e)
		{
			ListBox touchedList = (ListBox)sender;
            // selectedAttribute = (AttributeVO)touchedList.Tag;
            // selectedMethod = null;

            DiffPresenter.updateEaAttributeObject(myElement, selectedAttribute);

            this.Close();
        }

		void MethodListClick(object sender, EventArgs e)
		{
			ListBox touchedList = (ListBox)sender;
            // selectedMethod = (MethodVO)touchedList.Tag;
            // selectedAttribute = null;

            DiffPresenter.updateEaMethodObject(myElement, selectedMethod);

            this.Close();
        }


        void TaggedValueListClick(object sender, EventArgs e)
        {
            ListBox touchedList = (ListBox)sender;
            // selectedMethod = (MethodVO)touchedList.Tag;
            // selectedAttribute = null;

            DiffPresenter.updateEaTaggedValueObject(myElement, selectedTag);

            this.Close();
        }



        void ReflectToEAToolStripMenuItemClick(object sender, EventArgs e)
		{
            EA.Repository repo = ProjectSetting.getVO().eaRepo;

            if ( repo != null ) {

				// 選択された属性に対する更新処理
				if ( selectedAttribute != null ) {
					//メッセージボックスを表示する
					DialogResult result = MessageBox.Show("EAのリポジトリの属性を上書き、もしくは追加します。よろしいですか？",
					    "質問",
					    MessageBoxButtons.YesNoCancel,
					    MessageBoxIcon.Exclamation,
					    MessageBoxDefaultButton.Button1);

					//何が選択されたか調べる
					if (result == DialogResult.Yes)
					{
                        DiffPresenter.updateEaAttributeObject(myElement, selectedAttribute);
					} else {
					    return;
					}

				}

				// 選択された操作に対する更新処理
				if ( selectedMethod != null ) {
					//メッセージボックスを表示する
					DialogResult result = MessageBox.Show("EAのリポジトリの操作を上書き、もしくは追加します。よろしいですか？",
					    "質問",
					    MessageBoxButtons.YesNoCancel,
					    MessageBoxIcon.Exclamation,
					    MessageBoxDefaultButton.Button1);

					//何が選択されたか調べる
					if (result == DialogResult.Yes)
					{
                        DiffPresenter.updateEaMethodObject(myElement, selectedMethod);
					} else {
					    return;
					}
				}

                // 選択されたタグ付き値に対する更新処理
                if (selectedTag != null)
                {
                    //メッセージボックスを表示する
                    DialogResult result = MessageBox.Show("EAのリポジトリのタグ付き値を上書き、もしくは追加します。よろしいですか？",
                        "質問",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);

                    //何が選択されたか調べる
                    if (result == DialogResult.Yes)
                    {
                        DiffPresenter.updateEaTaggedValueObject(myElement, selectedTag);
                    }
                    else
                    {
                        return;
                    }

                }


            }
            else {
				MessageBox.Show("EAにアタッチしていないため、反映できません");
			}

		}






        #region ボタンクリック、ストリップメニュー選択時のイベントハンドラ

        void EASelectObjectToolStripMenuItemClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			if (repo != null ) {
				// 選択された属性に対する更新処理
				if ( selectedAttribute != null ) {
					EA.Attribute attr = (EA.Attribute)repo.GetAttributeByGuid(selectedAttribute.guid);
					if ( attr != null ) {
						repo.ShowInProjectView(attr);
					} else {
						// 属性がGUIDで空振りしたら要素GUIDで再検索
						EA.Element elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
						if ( elem != null ) {
							repo.ShowInProjectView(elem);
						}
					}
				}

				// 選択された操作に対する更新処理
				if ( selectedMethod != null ) {
					EA.Method mth =  (EA.Method)repo.GetMethodByGuid(selectedMethod.guid);
					if( mth != null ) {
						repo.ShowInProjectView(mth);
					} else {
						// 操作がGUIDで空振りしたら要素GUIDで再検索
						EA.Element elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
						if ( elem != null ) {
							repo.ShowInProjectView(elem);
						}
					}
				}
			} else {
				MessageBox.Show("EAにアタッチしていないため、選択できません");
			}

		}


		public void ElementDiff_LineUpdate(object sender, DiffEventArgs<string> e)
        {
            String indicator = " ";
            switch (e.DiffType)
            {
                case DiffType.Add:
                    indicator = "+";
		            this.rightDiffBuffer.Append(indicator + e.LineValue + "\r\n");
                    break;

                case DiffType.Subtract:
                    indicator = "-";
		            this.leftDiffBuffer.Append(indicator + e.LineValue + "\r\n");
                    break;

                default:
                    indicator = " ";
		            this.leftDiffBuffer.Append(indicator + e.LineValue + "\r\n");
		            this.rightDiffBuffer.Append(indicator + e.LineValue + "\r\n");
		            break;
            }

//  StringBuilder diffSb = (StringBuilder)sender ;
//  Console.WriteLine("{0}{1}", indicator, e.LineValue);
        }

		void ContextMenuStrip1Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			if ( repo != null ) {
				contextMenuStrip1.Enabled = true ;
			} else {
				contextMenuStrip1.Enabled = false ;
			}
		}
        #endregion

        /// <summary>
        /// 右クリックメニュー - GUIDをコピー選択時のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyGuidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 選択された属性に対する更新処理
            if (selectedAttribute != null)
            {
                MessageBox.Show("属性のGUID: " + selectedAttribute.guid);
                return;
            }

            // 選択された属性に対する更新処理
            if (selectedMethod != null)
            {
                MessageBox.Show("操作のGUID: " + selectedMethod.guid);
                return;
            }

            MessageBox.Show("GUIDが表示できません");
        }

    }
}
