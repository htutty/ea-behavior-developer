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
using EA;
using System.IO;

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

		AttributeVO selectedAttribute = null;
		MethodVO selectedMethod = null;

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
			string leftText, rightText ;
			int longerLine;
			AttributeVO leftAttr, rightAttr;
			MethodVO leftMth, rightMth;

            string artifactsDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
            ArtifactXmlReader reader = new ArtifactXmlReader(artifactsDir);

			tableLayoutPanel1.RowCount = elem.attributes.Count + elem.methods.Count;

			foreach( AttributeVO a in elem.attributes ) {
				leftAttr = a;
				rightAttr = a;
				leftText = "";
				rightText = "";

//				TextBox txtL = new TextBox();
//	            txtL.BackColor = Color.LightCyan;
//
//				TextBox txtR = new TextBox();
//	            txtR.BackColor = Color.LightYellow;

				ListBox listL = new ListBox();
				listL.DrawMode = DrawMode.OwnerDrawFixed;
				// EventHandlerの追加
				listL.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);

				ListBox listR = new ListBox();
				listR.DrawMode = DrawMode.OwnerDrawFixed;
				// EventHandlerの追加
				listR.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);

				switch( a.changed ) {
					case 'U':
						leftAttr = reader.readAttributeDiffDetail(a.guid, "L");
						rightAttr = reader.readAttributeDiffDetail(a.guid, "R");
						getDisagreedAttributeDesc( leftAttr, rightAttr, ref leftText, ref rightText ) ;
                        selectedAttribute = rightAttr;
                        break;

					case 'C':
						rightAttr = reader.readAttributeDiffDetail(a.guid, "R");
						getMonoAttributeDesc( rightAttr, ref rightText ) ;
                        selectedAttribute = rightAttr;
                        break;

					case 'D':
						leftAttr = reader.readAttributeDiffDetail(a.guid, "L");
						getMonoAttributeDesc( leftAttr, ref leftText ) ;
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

//				txtR.Text = rightText;
//				txtL.Text = leftText;
//	            setTextBoxSize(txtL, leftText, longerLine);
//				setTextBoxSize(txtR, rightText, longerLine);

				if ( a.changed == 'D' || a.changed == 'U' ) {
					listL.Tag = leftAttr;
					listL.ContextMenuStrip = contextMenuStrip1;
					listL.Click += new System.EventHandler(this.AttributeListClick);
//					listL.Click += new System.EventHandler(this.AttributeTextClick);
				}

				if ( a.changed == 'C' || a.changed == 'U' ) {
					listR.Tag = rightAttr;
					listR.ContextMenuStrip = contextMenuStrip1;
					listR.Click += new System.EventHandler(this.AttributeListClick);
//					listR.Click += new System.EventHandler(this.AttributeTextClick);
				}

				tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
				tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);

				rowIndex++;
			}


			foreach( MethodVO m in elem.methods ) {
				leftMth = m;
				rightMth = m;

				leftText = "";
				rightText = "";

//				TextBox txtL = new TextBox();
//	            txtL.BackColor = Color.White;
//
//				TextBox txtR = new TextBox();
//	            txtR.BackColor = Color.White;

				ListBox listL = new ListBox();
				listL.DrawMode = DrawMode.OwnerDrawFixed;
				// EventHandlerの追加
				listL.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);

				ListBox listR = new ListBox();
				listR.DrawMode = DrawMode.OwnerDrawFixed;
				// EventHandlerの追加
				listR.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);


				switch( m.changed ) {
					case 'U':
						leftMth = reader.readMethodDiffDetail(m.guid, "L");
						rightMth = reader.readMethodDiffDetail(m.guid, "R");
						getDisagreedMethodDesc( leftMth, rightMth, ref leftText, ref rightText ) ;

						leftDiffBuffer = new StringBuilder();
						rightDiffBuffer = new StringBuilder();
						var simpleDiff = new SimpleDiff<string>(leftText.Split('\n'), rightText.Split('\n'));
						simpleDiff.LineUpdate += new EventHandler<DiffEventArgs<string>> (ElementDiff_LineUpdate);
						simpleDiff.RunDiff();

						leftText = leftDiffBuffer.ToString();
						rightText = rightDiffBuffer.ToString();

                        selectedMethod = rightMth;
                        leftDiffBuffer.Clear();
						rightDiffBuffer.Clear();
						break;

					case 'C':
						rightMth = reader.readMethodDiffDetail(m.guid, "R");
						getMonoMethodDesc( rightMth, ref rightText ) ;

                        selectedMethod = rightMth;
                        break;

					case 'D':
						leftMth = reader.readMethodDiffDetail(m.guid, "L");
						getMonoMethodDesc( leftMth, ref leftText );

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


//				txtL.Text = leftText;
//				txtR.Text = rightText;
//	            setTextBoxSize(txtL, leftText, longerLine);
//				setTextBoxSize(txtR, rightText, longerLine);

				if ( m.changed == 'D' || m.changed == 'U' ) {
					listL.Tag = leftMth;
					listL.ContextMenuStrip = contextMenuStrip1;
					listL.Click += new System.EventHandler(this.MethodListClick);
//					listL.Click += new System.EventHandler(this.MethodTextClick);
				}

				if ( m.changed == 'C' || m.changed == 'U' ) {
					listR.Tag = rightMth;
					listR.ContextMenuStrip = contextMenuStrip1;
					listR.Click += new System.EventHandler(this.MethodListClick);
//					listR.Click += new System.EventHandler(this.MethodTextClick);
				}

				tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
				tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);

				rowIndex++;
			}

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

		/// <summary>
		/// 差異が検出された２つの属性の不一致な項目＝値をつなげた文字列を作成
		/// </summary>
		/// <param name="leftAttr">(in)左の属性VO</param>
		/// <param name="rightAttr">(in)右の属性VO</param>
		/// <param name="leftText">(out)左用の出力テキスト</param>
		/// <param name="rightText">(out)右用の出力テキスト</param>
		/// <returns></returns>
		private void getDisagreedAttributeDesc(AttributeVO leftAttr, AttributeVO rightAttr, ref string leftText, ref string rightText) {

            System.Text.StringBuilder lsb = new System.Text.StringBuilder();
            System.Text.StringBuilder rsb = new System.Text.StringBuilder();

            lsb.Append(leftAttr.name + "[" + leftAttr.alias + "]" + "\r\n");
            rsb.Append(rightAttr.name + "[" + rightAttr.alias + "]" + "\r\n");

            lsb.Append(leftAttr.guid + "\r\n");
			rsb.Append(rightAttr.guid + "\r\n");

//			if( !compareNullable(leftAttr.stereoType, rightAttr.stereoType) ) {
//				lsb.Append("stereoType=" + leftAttr.stereoType + "\r\n");
//				rsb.Append("stereoType=" + rightAttr.stereoType + "\r\n");
//			}

////			if( leftAtr.pos != rightAtr.pos ) {
////				lsb.Append("pos=" + leftAtr.pos + "\n");
////				rsb.Append("pos=" + rightAtr.pos + "\n");
////			}

//			if( !compareNullable(leftAttr.notes, rightAttr.notes) ) {
//				lsb.Append("[notes]\r\n" + leftAttr.notes + "\r\n");
//				rsb.Append("[notes]\r\n" + rightAttr.notes + "\r\n");
//			}

            lsb.Append(leftAttr.getComparedString(rightAttr));
            rsb.Append(rightAttr.getComparedString(leftAttr));

            leftText = lsb.ToString();
            rightText = rsb.ToString();
            return;
		}


		/// <summary>
		/// 片方の属性のダンプ
		/// </summary>
		/// <param name="attr"></param>
		private void getMonoAttributeDesc(AttributeVO attr, ref string text) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(attr.name + "[" + attr.alias + "]" + "\r\n");

			sb.Append("stereoType=" + attr.stereoType + "\r\n");
//				sb.Append("pos=" + attr.pos + "\n");
			if ( attr.notes == null || "".Equals(attr.notes) ) {
				sb.Append("[notes]\r\n" + attr.notes + "\r\n");
			}

			text = sb.ToString();

			return;
		}

		/// <summary>
		/// 差異が検出された２つの操作の不一致な項目＝値をつなげた文字列を作成
		/// </summary>
		/// <param name="leftMth">(in)左の操作VO</param>
		/// <param name="rightMth">(in)右の操作VO</param>
		/// <param name="leftText">(out)左用の出力テキスト</param>
		/// <param name="rightText">(out)右用の出力テキスト</param>
		/// <returns></returns>
		private void getDisagreedMethodDesc(MethodVO leftMth, MethodVO rightMth, ref string leftText, ref string rightText) {

			System.Text.StringBuilder lsb = new System.Text.StringBuilder();
			System.Text.StringBuilder rsb = new System.Text.StringBuilder();

			lsb.Append(leftMth.name + "[" + leftMth.alias + "]" + "\r\n");
			rsb.Append(rightMth.name + "[" + rightMth.alias + "]" + "\r\n");

			lsb.Append(leftMth.guid + "\r\n");
			rsb.Append(rightMth.guid + "\r\n");

			if( !compareNullable(leftMth.stereoType, rightMth.stereoType) )
            {
				lsb.Append("stereoType=" + leftMth.stereoType + "\r\n");
				rsb.Append("stereoType=" + rightMth.stereoType + "\r\n");
			}

			if( !compareNullable(leftMth.returnType, rightMth.returnType) )
            {
				lsb.Append("returnType=" + leftMth.returnType + "\r\n");
				rsb.Append("returnType=" + rightMth.returnType + "\r\n");
			}

			if( !compareNullable(leftMth.visibility, rightMth.visibility) )
            {
				lsb.Append("visibility=" + leftMth.visibility + "\r\n");
				rsb.Append("visibility=" + rightMth.visibility + "\r\n");
			}

			if( leftMth.pos != rightMth.pos )
            {
				lsb.Append("pos=" + leftMth.pos + "\r\n");
				rsb.Append("pos=" + rightMth.pos + "\r\n");
			}

			if( !compareNullable(leftMth.notes, rightMth.notes) ) {
				lsb.Append("[notes]\r\n" + leftMth.notes + "\r\n");
				rsb.Append("[notes]\r\n" + rightMth.notes + "\r\n");
			}

			if( !compareNullable(leftMth.behavior, rightMth.behavior) ) {
				lsb.Append("[behavior]\r\n" + leftMth.behavior );
				rsb.Append("[behavior]\r\n" + rightMth.behavior );
			}

			leftText = lsb.ToString();
			rightText = rsb.ToString();

			return;
		}


		/// <summary>
		/// 片方の操作のダンプ
		/// </summary>
		/// <param name="leftAtr"></param>
		/// <param name="rightAtr"></param>
		/// <returns></returns>
		private void getMonoMethodDesc(MethodVO mth, ref string text) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(mth.name + "[" + mth.alias + "]" + "\r\n");
			sb.Append(mth.guid + "\r\n");

			sb.Append("stereoType=" + mth.stereoType + "\r\n");
			sb.Append("returnType=" + mth.returnType + "\r\n");
			sb.Append("visibility=" + mth.visibility + "\r\n");
			sb.Append("pos=" + mth.pos + "\r\n");

			if ( mth.notes != null && !"".Equals(mth.notes) ) {
				sb.Append("[notes]\r\n" + mth.notes + "\r\n");
			}

			if ( mth.behavior != null || !"".Equals(mth.behavior) ) {
				sb.Append("[behavior]\r\n" + mth.behavior + "\r\n");
			}



			text = sb.ToString();

			return;
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


		private Boolean compareNullable(string l, string r) {
			// 左が null の場合
			if( l == null ) {
				// 右も null なら true
				if ( r == null ) {
					return true;
				} else {
					// 右が not null なら false
					return false;
				}
			} else {
				// 左が not null の場合

				// 右が null なら false
				if ( r == null ) {
					return false;
				} else {
					// 両方 not null なので、string#Equalsの結果を返却
					return l.Equals(r);
				}
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

            updateEaAttributeObject();

            this.Close();
        }

		void MethodListClick(object sender, EventArgs e)
		{
			ListBox touchedList = (ListBox)sender;
            // selectedMethod = (MethodVO)touchedList.Tag;
            // selectedAttribute = null;

            updateEaMethodObject();

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
                        updateEaAttributeObject();
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
                        updateEaMethodObject();
					} else {
					    return;
					}

				}

			} else {
				MessageBox.Show("EAにアタッチしていないため、反映できません");
			}

		}


        /// <summary>
        /// EAのAttributeを上書きもしくは追加する
        /// </summary>
        private void updateEaAttributeObject()
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Element elem = null;
            int tmp = -1;

            // EAのAPIを使って属性をGUIDより検索
            EA.Attribute attr = (EA.Attribute)repo.GetAttributeByGuid(selectedAttribute.guid);

            // 取得できなかったら（該当するGUIDの属性が存在しなかったら）
            if (attr == null)
            {
                // この属性を持っているはずの要素をGUIDより検索
                elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
                if (elem == null)
                {
                    return;
                }
                attr = (EA.Attribute)elem.Attributes.AddNew(selectedAttribute.name, selectedAttribute.eaType);
            }

            // 更新前後で更新ログ出力
            writeUpdateLogAttribute(attr, false);

            attr.Name = selectedAttribute.name;
            attr.AttributeGUID = selectedAttribute.guid;
            attr.Alias = selectedAttribute.alias;
            attr.StereotypeEx = selectedAttribute.stereoType;
            attr.Notes = selectedAttribute.notes;

            attr.AllowDuplicates = selectedAttribute.allowDuplicates;
            if ("".Equals(selectedAttribute.classifierID) || !Int32.TryParse(selectedAttribute.classifierID, out tmp))
            {
                selectedAttribute.classifierID = "0";
            }
            else
            {
                attr.ClassifierID = tmp;
            }

            attr.Container = selectedAttribute.container;
            attr.Containment = selectedAttribute.containment;
            attr.Default = selectedAttribute.defaultValue;
            attr.IsCollection = selectedAttribute.isCollection;
            attr.IsConst = selectedAttribute.isConst;
            attr.IsDerived = selectedAttribute.isDerived;
            // attr.IsID = selectedAttribute.;
            attr.IsOrdered = selectedAttribute.isOrdered;
            attr.IsStatic = selectedAttribute.isStatic;
            attr.Length = selectedAttribute.length.ToString();
            attr.LowerBound = selectedAttribute.lowerBound.ToString();
            attr.Precision = selectedAttribute.precision.ToString();
            attr.Pos = selectedAttribute.pos;
            // attr.RedefinedProperty = selectedAttribute.;
            attr.Scale = selectedAttribute.scale.ToString();
            attr.Stereotype = selectedAttribute.stereoType;
            // attr.Style = selectedAttribute.;
            // attr.SubsettedProperty = selectedAttribute.;
            attr.StyleEx = selectedAttribute.styleEx;
            attr.Type = selectedAttribute.eaType;
            attr.UpperBound = selectedAttribute.upperBound.ToString();
            attr.Visibility = selectedAttribute.visibility;

            attr.Update();
            //						elem.Update();

            // 更新前後で更新ログ出力
            writeUpdateLogAttribute(attr, true);
        }

        private void writeUpdateLogAttribute(EA.Attribute attr, bool afterUpdateFlag)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\ea-artifacts\DiffViewerUpdate.log", true);

                if (afterUpdateFlag)
                {
                    sw.WriteLine("■属性の書き込みが完了しました(ID={0}, GUID={1})。", attr.AttributeID, attr.AttributeGUID);
                }
                else
                {
                    sw.WriteLine("〇属性の更新前情報を表示します(ID={0}, GUID={1})。", attr.AttributeID, attr.AttributeGUID);
                }

                sw.WriteLine("  AttributeID = " + attr.AttributeID);
                sw.WriteLine("  AttributeGUID = " + attr.AttributeGUID);
                sw.WriteLine("  Name = " + attr.Name);
                sw.WriteLine("  Alias = " + attr.Alias);
                sw.WriteLine("  StereotypeEx = " + attr.StereotypeEx);
                sw.WriteLine("  Notes = " + attr.Notes);
                sw.WriteLine("  AllowDuplicate = " + attr.AllowDuplicates);
                sw.WriteLine("  ClassifierID = " + attr.ClassifierID);
                sw.WriteLine("  Container = " + attr.Container);
                sw.WriteLine("  Containment = " + attr.Containment);
                sw.WriteLine("  Default = " + attr.Default);
                sw.WriteLine("  IsCollection = " + attr.IsCollection);
                sw.WriteLine("  IsConst = " + attr.IsConst);
                sw.WriteLine("  IsDerived = " + attr.IsDerived);
                sw.WriteLine("  IsOrdered = " + attr.IsOrdered);
                sw.WriteLine("  IsStatic = " + attr.IsStatic);
                sw.WriteLine("  Length = " + attr.Length);
                sw.WriteLine("  LowerBound = " + attr.LowerBound);
                sw.WriteLine("  Precision = " + attr.Precision);
                sw.WriteLine("  Pos = " + attr.Pos);
                sw.WriteLine("  Scale = " + attr.Scale);
                sw.WriteLine("  Stereotype = " + attr.Stereotype);
                sw.WriteLine("  StyleEx = " + attr.StyleEx);
                sw.WriteLine("  Type = " + attr.Type);
                sw.WriteLine("  UpperBound = " + attr.UpperBound);
                sw.WriteLine("  Visibility = " + attr.Visibility);
                sw.WriteLine("");

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }




        /// <summary>
        /// GUIDを指定してEA上の属性オブジェクトを取得する
        /// </summary>
        /// <param name="attributeGuid">検索対象属性のGUID</param>
        /// <returns>合致するGUIDでヒットした属性オブジェクト。ヒットしなかったらnull</returns>
        private EA.Attribute getAttributeByGuid(string attributeGuid)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Attribute attrObj = (EA.Attribute)repo.GetAttributeByGuid(attributeGuid);
            if (attrObj != null)
            {
                return attrObj;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// EAのMethod（operator）を上書きもしくは追加する
        /// </summary>
        private void updateEaMethodObject()
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Element elem = null;

            // EA.Repository の GetMethodByGuid を呼んでEA上の該当メソッドオブジェクトを取得する
            EA.Method mth = getMethodByGuid(selectedMethod.guid);

            if (mth == null)
            {
                elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
                if (elem == null)
                {
                    return;
                }

                mth = (EA.Method)elem.Methods.AddNew(selectedMethod.name, selectedMethod.returnType);
            }

            writeUpdateLogMethod(mth, false);

            mth.Name = selectedMethod.name;
            mth.MethodGUID = selectedMethod.guid;
            mth.Alias = selectedMethod.alias;
            mth.StereotypeEx = selectedMethod.stereoType;
            mth.Notes = selectedMethod.notes;
            mth.Behavior = selectedMethod.behavior;

            mth.Abstract = selectedMethod.isAbstract;
            mth.ClassifierID = selectedMethod.classifierID;
            mth.Code = selectedMethod.code;
            mth.Concurrency = selectedMethod.concurrency;
            mth.IsConst = selectedMethod.isConst;
            mth.IsLeaf = selectedMethod.isLeaf;
            mth.IsPure = selectedMethod.isPure;
            mth.IsQuery = selectedMethod.isQuery;
            mth.IsRoot = selectedMethod.isRoot;
            mth.IsStatic = selectedMethod.isStatic;
            // mth.IsSynchronized = selectedMethod;
            mth.Pos = selectedMethod.pos;
            mth.ReturnIsArray = selectedMethod.returnIsArray;
            mth.ReturnType = selectedMethod.returnType;
            mth.StateFlags = selectedMethod.stateFlags;
            mth.StyleEx = selectedMethod.styleEx;
            mth.Throws = selectedMethod.throws;
            mth.Visibility = selectedMethod.visibility;
            mth.Update();

            // 既にパラメータが設定されている場合は一旦削除
            for (short i = 0; i < mth.Parameters.Count; i++)
            {
                mth.Parameters.Delete(i);
            }

            // XMLから読み込まれたパラメータの値を設定する
            foreach (ParameterVO prm in selectedMethod.parameters)
            {
                EA.Parameter paramObj = (EA.Parameter)mth.Parameters.AddNew(prm.name, prm.eaType);
                paramObj.Alias = prm.alias;
                paramObj.ClassifierID = prm.classifierID;
                paramObj.Default = prm.defaultValue;
                paramObj.IsConst = prm.isConst;
                paramObj.Kind = prm.kind;
                paramObj.Name = prm.name;
                paramObj.Notes = prm.notes;
                paramObj.ParameterGUID = prm.guid;
                paramObj.Position = prm.pos;
                paramObj.StereotypeEx = prm.stereoType;
                // paramObj.Style = prm.Style ;
                // paramObj.StyleEx = prm.StyleEx ;
                paramObj.Type = prm.eaType;
                paramObj.Update();
            }

            mth.Update();

            writeUpdateLogMethod(mth, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mth"></param>
        /// <param name="afterUpdateFlag"></param>
        private void writeUpdateLogMethod(EA.Method mth, bool afterUpdateFlag)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\DesignHistory\DiffViewerUpdate.log", true);


                if (afterUpdateFlag)
                {
                    sw.WriteLine("■操作の書き込みが完了しました(ID={0}, GUID={1})。", mth.MethodID, mth.MethodGUID);
                }
                else
                {
                    sw.WriteLine("〇操作の更新前情報を表示します(ID={0}, GUID={1})。", mth.MethodID, mth.MethodGUID);
                }

                sw.WriteLine("  MethodID =" + mth.MethodID);
                sw.WriteLine("  MethodGUID =" + mth.MethodGUID);
                sw.WriteLine("  Name =" + mth.Name);
                sw.WriteLine("  Alias =" + mth.Alias);
                sw.WriteLine("  StereotypeEx =" + mth.StereotypeEx);
                sw.WriteLine("  Notes =" + mth.Notes);
                sw.WriteLine("  Behavior =" + mth.Behavior);
                sw.WriteLine("  Abstract =" + mth.Abstract);
                sw.WriteLine("  ClassifierID =" + mth.ClassifierID);
                sw.WriteLine("  Code =" + mth.Code);
                sw.WriteLine("  Concurrency =" + mth.Concurrency);
                sw.WriteLine("  IsConst =" + mth.IsConst);
                sw.WriteLine("  IsLeaf =" + mth.IsLeaf);
                sw.WriteLine("  IsPure =" + mth.IsPure);
                sw.WriteLine("  IsQuery =" + mth.IsQuery);
                sw.WriteLine("  IsRoot =" + mth.IsRoot);
                sw.WriteLine("  IsStatic =" + mth.IsStatic);
                sw.WriteLine("  Pos =" + mth.Pos);
                sw.WriteLine("  ReturnIsArray =" + mth.ReturnIsArray);
                sw.WriteLine("  ReturnType =" + mth.ReturnType);
                sw.WriteLine("  StateFlags =" + mth.StateFlags);
                sw.WriteLine("  StyleEx =" + mth.StyleEx);
                sw.WriteLine("  Throws =" + mth.Throws);
                sw.WriteLine("  Visibility =" + mth.Visibility);

                sw.WriteLine("");

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }



        /// <summary>
        /// GUIDを指定してEA上のメソッドオブジェクトを取得する
        /// </summary>
        /// <param name="methodGuid">検索対象メソッドのGUID</param>
        /// <returns>合致するGUIDでヒットしたメソッドオブジェクト。ヒットしなかったらnull</returns>
        private EA.Method getMethodByGuid(string methodGuid)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Method mthObj = (EA.Method)repo.GetMethodByGuid(methodGuid);
            return mthObj;
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
