using System;
using System.IO;
using File=System.IO.File;

using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.util;

using VoidNish.Diff;

namespace ElementEditor
{
	/// <summary>
	/// Description of DiffElementForm.
	/// </summary>
	public partial class DiffElementForm : Form
	{
		ElementVO myElement;
		
		ElementVO leftElement;
		ElementVO rightElement;
		ElementVO mergedElement;
		
		List<MethodVO> methods = new List<MethodVO>();

		StringBuilder leftDiffBuffer;
		StringBuilder rightDiffBuffer;
		
		
//		AttributeVO selectedAttribute = null;
//		MethodVO selectedMethod = null;
//		TextBox selectedTextBox = null;

		public DiffElementForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

        /// <summary>
        /// コンストラクタ(外部で比較した結果の要素ファイルがある時用)
        /// </summary>
        /// <param name="element"></param>
        public DiffElementForm(ElementVO element) {
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
			
			setElementItems(myElement);
		}

        /// <summary>
        /// コンストラクタ(ローカルファイルのorigとchangedの比較用)
        /// </summary>
        /// <param name="lElem">ローカルのorig要素ファイルの内容</param>
        /// <param name="rElem">ローカルのchanged要素ファイルの内容</param>
        public DiffElementForm(ElementVO lElem, ref ElementVO rElem) {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			myElement = lElem;

			leftElement = lElem;
			rightElement = rElem;
			
			// 
			this.Text = "クラス: " + leftElement.name + " " + leftElement.guid;
			
			setElementItems(leftElement, rightElement);
		}

		
		/// <summary>
        /// ProjectDiffMakerなどで作成された、マージ済みの要素を受け取ってSide-by-Side形式での差分表示を行う
        /// </summary>
        /// <param name="elem"></param>
		private void setElementItems( ElementVO elem ) {
            tableLayoutPanel1.RowCount = 1 + elem.attributes.Count + elem.methods.Count;

            int rowIndex = 0;

            // 要素のプロパティの表示項目を設定
            addItemElementProperty(elem, ref rowIndex);

            // 属性の表示項目を設定
            addItemAttributes(elem, ref rowIndex);

            // メソッドの表示項目を設定
            addItemMethods(elem, ref rowIndex);
        }

		
		/// <summary>
		/// ２つのテキストのうち、テキストの行数のうち大きいほうを返却
		/// </summary>
		/// <param name="leftText"></param>
		/// <param name="rightText"></param>
		/// <returns></returns>
		private int getLongerLine(string leftText, string rightText) {
			
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
        /// 要素内の画面項目をセット（差分のあった項目のみ）
        /// </summary>
        /// <param name="lElem">左の要素</param>
        /// <param name="rElem">右の要素</param>
        private void setElementItems(ElementVO lElem, ElementVO rElem) {

            // 引数の２つの要素を比較して差分の内容を表示
            mergedElement = ElementDiffer.getMergedElement(lElem, rElem);
            if (mergedElement == null) {
                return;
            }

            tableLayoutPanel1.RowCount = 1 + mergedElement.attributes.Count + mergedElement.methods.Count;

            int rowIndex = 0;

            // 要素プロパティの表示項目を設定
            addItemElementProperty(mergedElement, ref rowIndex);

            if (mergedElement.attributes != null)
            {
                // 属性の表示項目を設定
                addItemAttributes(mergedElement, ref rowIndex);
            }

            if (mergedElement.methods != null)
            {
                // メソッドの表示項目を設定
                addItemMethods(mergedElement, ref rowIndex);
            }

        }


        /// <summary>
        /// 要素プロパティの表示項目を追加する
        /// </summary>
        /// <param name="mergedElem"></param>
        /// <param name="leftElem"></param>
        /// <param name="rightElem"></param>
        /// <param name="rowIndex"></param>
        void addItemElementProperty(ElementVO mergedElem, ref int rowIndex)
        {
            string leftText = "", rightText = "";

            if( mergedElem.propertyChanged == 'U' )
            {
                // 要素プロパティから左右のテキストボックスに設定される内容を取得
                getDisagreedElemPropertyDesc(mergedElem.srcElementProperty, mergedElem.destElementProperty, ref leftText, ref rightText);
                int longerLine = getLongerLine(leftText, rightText);

                // 左テキストボックスのプロパティセット
                TextBox txtL = new TextBox();
                txtL.BackColor = Color.LightCyan;
                txtL.Text = leftText;
                setTextBoxSize(txtL, leftText, longerLine);
                txtL.Tag = mergedElem.srcElementProperty;

                // 右テキストボックスのプロパティセット
                TextBox txtR = new TextBox();
                txtR.BackColor = Color.LightYellow;
                txtR.Text = rightText;
                txtR.Tag = mergedElem.destElementProperty;
                setTextBoxSize(txtR, rightText, longerLine);

                // 左右テキストボックスをレイアウトパネルに追加
                tableLayoutPanel1.Controls.Add(txtL, 0, rowIndex);
                tableLayoutPanel1.Controls.Add(txtR, 1, rowIndex);
                rowIndex++;
            }

        }



        /// <summary>
        /// 属性の表示項目を追加する
        /// </summary>
        /// <param name="mergedElem"></param>
        /// <param name="rowIndex"></param>
        void addItemAttributes(ElementVO mergedElem, ref int rowIndex)
		{
            AttributeVO leftAttr, rightAttr;
            string leftText, rightText;

            int longerLine;

            // マージ済要素のメソッド（何か内容に差異のあったメソッド）分ループ
            foreach (AttributeVO att in mergedElem.attributes)
            {
                leftAttr = att;
                rightAttr = att;

                ListBox listL = new ListBox();
                listL.DrawMode = DrawMode.OwnerDrawFixed;
                // EventHandlerの追加
                listL.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);

                ListBox listR = new ListBox();
                listR.DrawMode = DrawMode.OwnerDrawFixed;
                // EventHandlerの追加
                listR.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);

                // メソッドVOのchangedの値にしたがって左右のテキストの内容を設定する
                leftText = "";
                rightText = "";

                switch (att.changed)
                {
                    case 'U':
                        leftAttr = att.srcAttribute;
                        rightAttr = att.destAttribute;
                        getDisagreedAttributeDesc(leftAttr, rightAttr, ref leftText, ref rightText);

                        leftDiffBuffer = new StringBuilder();
                        rightDiffBuffer = new StringBuilder();
                        var simpleDiff = new SimpleDiff<string>(leftText.Split('\n'), rightText.Split('\n'));
                        simpleDiff.LineUpdate += new EventHandler<DiffEventArgs<string>>(ElementDiff_LineUpdate);
                        simpleDiff.RunDiff();

                        leftText = leftDiffBuffer.ToString();
                        rightText = rightDiffBuffer.ToString();
                        break;

                    case 'C':
                        getMonoAttributeDesc(rightAttr, ref rightText);
                        break;
                    case 'D':
                        getMonoAttributeDesc(leftAttr, ref leftText);
                        break;
                    default:
                        break;
                }

                longerLine = getLongerLine(leftText, rightText);

                setListItems(listL, leftText);
                setListItems(listR, rightText);
                setListBoxSize(listL, leftText, longerLine);
                setListBoxSize(listR, rightText, longerLine);

                if (att.changed == 'D' || att.changed == 'U')
                {
                    listL.ContextMenuStrip = contextMenuStrip1;
                }
                if (att.changed == 'C' || att.changed == 'U')
                {
                    listR.ContextMenuStrip = contextMenuStrip1;
                }

                tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
                tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);
                rowIndex++;
            }

        }

        /// <summary>
        /// 操作の表示項目を追加する
        /// </summary>
        /// <param name="mergedElem"></param>
        /// <param name="leftElem"></param>
        /// <param name="rightElem"></param>
        /// <param name="rowIndex"></param>
        void addItemMethods(ElementVO mergedElem, ref int rowIndex)
		{
			MethodVO leftMth, rightMth;
			string leftText, rightText ;

			int longerLine;
			
			// マージ済要素のメソッド（何か内容に差異のあったメソッド）分ループ
			foreach (MethodVO m in mergedElem.methods) {
				leftMth = m;
				rightMth = m;
				
				ListBox listL = new ListBox();
				listL.DrawMode = DrawMode.OwnerDrawFixed;
				// EventHandlerの追加
				listL.DrawItem += new DrawItemEventHandler(ListBox_DrawItem); 

				ListBox listR = new ListBox();
				listR.DrawMode = DrawMode.OwnerDrawFixed;
				// EventHandlerの追加
				listR.DrawItem += new DrawItemEventHandler(ListBox_DrawItem); 
				
				// メソッドVOのchangedの値にしたがって左右のテキストの内容を設定する
				leftText = "";
				rightText = "";

				switch (m.changed) {
					case 'U':
                        leftMth = m.srcMethod;
                        rightMth = m.destMethod;
						getDisagreedMethodDesc(leftMth, rightMth, ref leftText, ref rightText);
						
						leftDiffBuffer = new StringBuilder();
						rightDiffBuffer = new StringBuilder();
						var simpleDiff = new SimpleDiff<string>(leftText.Split('\n'), rightText.Split('\n'));
						simpleDiff.LineUpdate += new EventHandler<DiffEventArgs<string>> (ElementDiff_LineUpdate);
						simpleDiff.RunDiff();
						
						leftText = leftDiffBuffer.ToString();
						rightText = rightDiffBuffer.ToString();
						break;

					case 'C':
                        // rightMth = searchMethodByGuid(rightElement.methods, m.guid);
                        getMonoMethodDesc(rightMth, ref rightText);
						break;
					case 'D':
                        // leftMth = searchMethodByGuid(leftElement.methods, m.guid);
                        getMonoMethodDesc(leftMth, ref leftText);
						break;
					default:
						break;
				}
				
	            longerLine = getLongerLine(leftText, rightText); 
	            
	            setListItems(listL, leftText);
	            setListItems(listR, rightText);
	            setListBoxSize(listL, leftText, longerLine);
				setListBoxSize(listR, rightText, longerLine);

				if (m.changed == 'D' || m.changed == 'U') {
					listL.ContextMenuStrip = contextMenuStrip1;
				}
				if (m.changed == 'C' || m.changed == 'U') {
					listR.ContextMenuStrip = contextMenuStrip1;
				}

				tableLayoutPanel1.Controls.Add(listL, 0, rowIndex);
				tableLayoutPanel1.Controls.Add(listR, 1, rowIndex);
				rowIndex++;
			}
		}


		/// <summary>
		/// 属性リストからGUIDをキーに対象の属性を検索する
		/// </summary>
		/// <param name="attrList"></param>
		/// <param name="guid"></param>
		/// <returns>見つかった属性（仮にヒットしなかった場合は空の属性オブジェクトを返却）</returns>
		private AttributeVO searchAttributeByGuid(List<AttributeVO> attrList, string guid) {
			foreach(AttributeVO a in attrList) {
				if (a.guid.Equals(guid)) {
					return a;
				}
			}
			return new AttributeVO();
		}

		
		/// <summary>
		/// 操作リストからGUIDをキーに対象の操作を検索する
		/// </summary>
		/// <param name="methList"></param>
		/// <param name="guid"></param>
		/// <returns>見つかったメソッド（仮にヒットしなかった場合は空のメソッドオブジェクトを返却）</returns>
		private MethodVO searchMethodByGuid(List<MethodVO> methList, string guid) {
			foreach(MethodVO m in methList) {
				if (m.guid.Equals(guid)) {
					return m;
				}
			}
			return new MethodVO();
		}

        /// <summary>
        /// 差異が検出された２つの属性の不一致な項目＝値をつなげた文字列を作成
        /// </summary>
        /// <param name="leftElemProp">(in)左の属性VO</param>
        /// <param name="rightElemProp">(in)右の属性VO</param>
        /// <param name="leftText">(out)左用の出力テキスト</param>
        /// <param name="rightText">(out)右用の出力テキスト</param>
        /// <returns></returns>
        private void getDisagreedElemPropertyDesc(ElementPropertyVO leftElemProp, ElementPropertyVO rightElemProp, ref string leftText, ref string rightText)
        {

            System.Text.StringBuilder lsb = new System.Text.StringBuilder();
            lsb.Append(leftElemProp.name + "[" + leftElemProp.alias + "]" + "\r\n");
            lsb.Append(leftElemProp.getComparedString(rightElemProp));
            leftText = lsb.ToString();

            System.Text.StringBuilder rsb = new System.Text.StringBuilder();
            rsb.Append(rightElemProp.name + "[" + rightElemProp.alias + "]" + "\r\n");
            rsb.Append(rightElemProp.getComparedString(leftElemProp));
            rightText = rsb.ToString();


            //            if ( !compareNullable(leftAttr.stereoType, rightAttr.stereoType) ) {
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

            return;
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
			lsb.Append(leftAttr.name + "[" + leftAttr.alias + "]" + "\r\n");
			lsb.Append(leftAttr.guid + "\r\n");
            lsb.Append(leftAttr.getComparedString(rightAttr));
            leftText = lsb.ToString();

            System.Text.StringBuilder rsb = new System.Text.StringBuilder();
            rsb.Append(rightAttr.name + "[" + rightAttr.alias + "]" + "\r\n");
            rsb.Append(rightAttr.guid + "\r\n");
            rsb.Append(rightAttr.getComparedString(leftAttr));
            rightText = rsb.ToString();


            //            if ( !compareNullable(leftAttr.stereoType, rightAttr.stereoType) ) {
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
			
			if( !compareNullable(leftMth.stereoType, rightMth.stereoType) ) {
				lsb.Append("stereoType=" + leftMth.stereoType + "\r\n");
				rsb.Append("stereoType=" + rightMth.stereoType + "\r\n");
			}
			
			if( !compareNullable(leftMth.returnType, rightMth.returnType) ) {
				lsb.Append("returnType=" + leftMth.returnType + "\r\n");
				rsb.Append("returnType=" + rightMth.returnType + "\r\n");
			}

			if( !compareNullable(leftMth.visibility, rightMth.visibility) ) {
				lsb.Append("visibility=" + leftMth.visibility + "\r\n");
				rsb.Append("visibility=" + rightMth.visibility + "\r\n");
			}

//			if( !compareNullable(leftMth.pos, rightMth.pos) ) {
//				lsb.Append("pos=" + leftMth.pos + "\r\n");
//				rsb.Append("pos=" + rightMth.pos + "\r\n");
//			}
			
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
		
		
		private void setListItems(ListBox lst, string setStr ) {
			string[] ary = setStr.Split('\n');
			
			for( int i = 0; i < ary.Length; i++ ) {
				lst.Items.Add(ary[i]);
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
		
		
		/// <summary>
		/// 引数のcontentの内容からテキストボックスに横幅(800) と 高さ（行数*12px）を設定する
		/// </summary>
		/// <param name="txtBox"></param>
		/// <param name="content"></param>
		/// <param name="lines"></param>
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
		
		
		public void ElementDiff_LineUpdate(object sender, DiffEventArgs<string> e)
        {
            String indicator = " ";
            switch (e.DiffType)
            {
                case DiffType.Add:
                    indicator = "+";
		            this.rightDiffBuffer.Append(indicator + e.LineValue + "\n");
                    break;

                case DiffType.Subtract:
                    indicator = "-";
		            this.leftDiffBuffer.Append(indicator + e.LineValue + "\n");
                    break;
               
                default:
                    indicator = " ";
		            this.leftDiffBuffer.Append(indicator + e.LineValue + "\n");
		            this.rightDiffBuffer.Append(indicator + e.LineValue + "\n");
		            break;
            }

//            StringBuilder diffSb = (StringBuilder)sender ;
//            Console.WriteLine("{0}{1}", indicator, e.LineValue);
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
		
		
		
		#region "ボタンなどのイベントハンドラ"
		
		/// <summary>
		/// コンテキストメニュー：EAに反映 のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ReflectToEAToolStripMenuItemClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			
			if( repo != null ) {

				Control source = contextMenuStrip1.SourceControl;
				if (source != null) {
					if( source.Tag is AttributeVO ) {
						AttributeVO att = (AttributeVO)source.Tag;
						confirmUpdateAttribute(repo, att);
						
					} else if ( source.Tag is MethodVO ) {
						MethodVO mth = (MethodVO)source.Tag;
						confirmUpdateMethod(repo, mth);
					}
				}
				
			} else {
				MessageBox.Show("EAにアタッチしていないため、反映できません");
			}
			
		}

		/// <summary>
		/// 選択された属性に対する更新処理
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="selectedAttribute"></param>
		private void confirmUpdateAttribute(EA.Repository repo, AttributeVO selectedAttribute)
		{
			if (selectedAttribute != null) {
				// メッセージボックスを表示する
				DialogResult result = MessageBox.Show("EAのリポジトリの属性を上書き、もしくは追加します。よろしいですか？",
				                                      "質問", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation,
				                                      MessageBoxDefaultButton.Button1);

				// 確認ダイアログでYesが選択された場合
				if (result == DialogResult.Yes) {
					execUpdateAttribute(repo, selectedAttribute);
				} else {
					return;
				}
			}
		}

		
		/// <summary>
		/// 属性をEAに向けて更新実行する
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="selectedAttribute"></param>
		void execUpdateAttribute(EA.Repository repo, AttributeVO selectedAttribute)
		{
			EA.Attribute attr = (EA.Attribute)repo.GetAttributeByGuid(selectedAttribute.guid);
			if (attr == null) {
				EA.Element elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
				if (elem == null) {
					return;
				}
				attr = (EA.Attribute)elem.Attributes.AddNew(selectedAttribute.name, "String");
			}
			attr.AttributeGUID = selectedAttribute.guid;
			attr.Alias = selectedAttribute.alias;
			attr.StereotypeEx = selectedAttribute.stereoType;
			attr.Notes = selectedAttribute.notes;
			attr.Update();
		}
		
		
		/// <summary>
		/// 選択された操作に対する更新処理
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="selectedMethod"></param>
		private void confirmUpdateMethod(EA.Repository repo, MethodVO selectedMethod)
		{
			if (selectedMethod != null) {
				//メッセージボックスを表示する
				DialogResult result = MessageBox.Show("EAのリポジトリの操作を上書き、もしくは追加します。よろしいですか？",
				                                      "質問", MessageBoxButtons.YesNoCancel,
				                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

				// 確認ダイアログでYesが選択された場合
				if (result == DialogResult.Yes) {
					execUpdateMethod(repo, selectedMethod);
				} else {
					return;
				}
			}
		}

		/// <summary>
		/// メソッドをEAに向けて更新実行する
		/// </summary>
		/// <param name="repo"></param>
		/// <param name="selectedMethod"></param>
		private void execUpdateMethod(EA.Repository repo, MethodVO selectedMethod)
		{
			EA.Method mth = getMethodByGuid(selectedMethod.guid);

			if (mth == null) {
				EA.Element elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
				if (elem == null) {
					return;
				}
				mth = (EA.Method)elem.Methods.AddNew(selectedMethod.name, selectedMethod.returnType);
				mth.MethodGUID = selectedMethod.guid;
			}
//					mth.StereotypeEx = selectedMethod.stereoType;
//					mth.Notes = selectedMethod.notes;
			mth.Alias = selectedMethod.alias;
			mth.Behavior = selectedMethod.behavior;
			mth.Update();
		}

		
		/// <summary>
		/// コンテキストメニュー：EAで選択のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EASelectObjectToolStripMenuItemClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			if (repo != null ) {
				// メニューを開くために右クリックされたコントロールを取得
				Control source = contextMenuStrip1.SourceControl;
				if (source != null) {
					if( source.Tag is AttributeVO ) {
						AttributeVO att = (AttributeVO)source.Tag;
						
						// EAでGUIDをキーとして対象属性の取得を試み、取れたらプロジェクトブラウザーで選択する
						EA.Attribute attrObj = (EA.Attribute)repo.GetAttributeByGuid(att.guid);
						if ( attrObj != null ) {
							repo.ShowInProjectView(attrObj);
						}
						
					} else if ( source.Tag is MethodVO ) {

						MethodVO mth = (MethodVO)source.Tag;
						
						// EAでGUIDをキーとして対象操作の取得を試み、取れたらプロジェクトブラウザーで選択する
						EA.Method mthObj =  (EA.Method)repo.GetMethodByGuid(mth.guid);
						if( mthObj != null ) {
							repo.ShowInProjectView(mthObj);
						}
					}
				}

			} else {
				MessageBox.Show("EAにアタッチしていないため、選択できません");
			}
			
		}

		
		/// <summary>
		/// GUIDによりEAから属性オブジェクトを取得
		/// </summary>
		/// <param name="attributeGuid"></param>
		/// <returns></returns>
		private EA.Attribute getAttributeByGuid( string attributeGuid ) {
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			EA.Attribute attrObj = (EA.Attribute)repo.GetAttributeByGuid(attributeGuid) ;
			if( attrObj != null ) {
				return attrObj ;
			} else {
				return null;
			}
		}

		
		/// <summary>
		/// GUIDによりEAからメソッドオブジェクトを取得
		/// </summary>
		/// <param name="methodGuid"></param>
		/// <returns></returns>
		private EA.Method getMethodByGuid( string methodGuid ) {
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			EA.Method mthObj = (EA.Method)repo.GetMethodByGuid(methodGuid) ;
			if( mthObj != null ) {
				return mthObj ;
			} else {
				return null;
			}
		}
		
		#endregion
		
		void ButtonCloseClick(object sender, EventArgs e)
		{
			this.Close();
		}

		
		void ButtonCommitCloseClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;
			
			if (mergedElement == null) {
				MessageBox.Show("反映するべき差分が見つからなかったため、終了します");
				return ;
			}
			
			// 変更のあったメソッドを順にEAに反映する
			foreach (MethodVO m in mergedElement.methods) {
				MethodVO updMth = searchMethodByGuid(rightElement.methods, m.guid);
				if ( m.changed == 'U' ) {
					execUpdateMethod(repo, updMth);
				}
			}
			
			// 反映後のEAから取得した要素オブジェクトの情報を elements 配下のXMLに反映する
			ElementVO refreshedElement = null;
			EA.Element eaElemObj = (EA.Element)repo.GetElementByGuid(mergedElement.guid) ;
			if( eaElemObj == null ) {
				MessageBox.Show("EAから要素の取得に失敗しました。 異常終了しました。 GUID=" + mergedElement.guid);
				return;
			} else {
				refreshedElement = ObjectEAConverter.getElementFromEAObject(eaElemObj);
			}
						
			// 反映後のEAから取得した要素オブジェクトの情報を elements 配下のXMLに反映する
			StreamWriter elsw = null;
			string outputDir = ProjectSetting.getVO().projectPath;
			string elemFilePath = outputDir + @"\elements\" + refreshedElement.guid.Substring(1,1) + @"\" + refreshedElement.guid.Substring(2,1) + @"\" + refreshedElement.guid.Substring(1,36) + ".xml";
			try {
				//BOM無しのUTF8でテキストファイルを作成する
				elsw = new StreamWriter(elemFilePath);
				elsw.WriteLine( @"<?xml version=""1.0"" encoding=""UTF-8""?> " );
				ElementXmlWriter.writeElementXml(refreshedElement, 0, elsw);
			} catch( Exception ex ) {
				Console.WriteLine(ex.Message);
			} finally {
				if( elsw != null ) elsw.Close();
			}
			
			// changedFile が存在したら削除する
			try {
				string changedElemFilePath = outputDir + @"\elements\" + refreshedElement.guid.Substring(1,1) + @"\" + refreshedElement.guid.Substring(2,1) + @"\" + refreshedElement.guid.Substring(1,36) + "_changed.xml";
				if( System.IO.File.Exists(changedElemFilePath) ) {
					System.IO.File.Delete(changedElemFilePath);
				}
			} catch( Exception ex ) {
				Console.WriteLine(ex.Message);
			}
						
			// 親フォームの再描画を依頼
			ElementForm oya = (ElementForm)this.Owner;
			oya.repaint(refreshedElement);

			// XMLファイルをコミットするためにTortoiseGitを起動する
			//string exename = @"TortoiseGitProc.exe";
			//string arguments = @"/command:commit /path:" + elemFilePath;
			//Process p = Process.Start( exename, arguments );
			
			// 自分をクローズ
			this.Close();
		}
		

//		void ButtonOutputDocumentClick(object sender, EventArgs e)
//		{
//			string docFileName = selectDocumentFile(mergedElement);
//			
//			if (docFileName != null) {
//				ClassDocumentMaker docmaker = new ClassDocumentMaker();
//				docmaker.make(mergedElement, docFileName);
//				
//				// docxファイルが出来たらそのファイルを開く
//				Process p = Process.Start( docFileName );
//			}
//		}
		
		/// <summary>
		/// ファイル選択ダイアログを開き、出力するドキュメントのパスを入力させる
		/// </summary>
		/// <param name="mergedElement">デフォルトのファイル名取得用のElement</param>
		/// <returns>出力ファイル名(.docx)フルパス。　ファイル選択時にキャンセルした場合は null</returns>
		private string selectDocumentFile(ElementVO mergedElement) {
			//OpenFileDialogクラスのインスタンスを作成
			OpenFileDialog dialog = new OpenFileDialog();
			
			string defaultName = mergedElement.name + "_" + mergedElement.guid.Substring(1, 8) + ".docx";
			
			//はじめに「ファイル名」で表示される文字列を指定する
			dialog.FileName = defaultName;
			
			//はじめに表示されるフォルダを指定する
			//指定しない（空の文字列）の時は、現在のディレクトリが表示される
//			dialog.InitialDirectory = @"C:\";
			dialog.InitialDirectory = "";

			//[ファイルの種類]に表示される選択肢を指定する
			//指定しないとすべてのファイルが表示される
			dialog.Filter = "ドキュメントファイル(*.docx)|*.docx|すべてのファイル(*.*)|*.*";
			//[ファイルの種類]ではじめに選択されるものを指定する
			//1番目の「すべてのファイル」が選択されているようにする
			dialog.FilterIndex = 1;
			//タイトルを設定する
			dialog.Title = "出力するWordファイル名(docx)を指定してください";
			//ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
			dialog.RestoreDirectory = true;
			//存在しないファイルの名前が指定されたとき警告を表示する
			//デフォルトでTrueなので指定する必要はない
			dialog.CheckFileExists = false;
			//存在しないパスが指定されたとき警告を表示する
			//デフォルトでTrueなので指定する必要はない
			dialog.CheckPathExists = true;
			
			//ダイアログを表示する
			if (dialog.ShowDialog() == DialogResult.OK) {
			    return dialog.FileName;
			} else {
				return null;
			}

			
		}
		
		
		void ButtonRevertClick(object sender, EventArgs e)
		{
			EA.Repository repo = ProjectSetting.getVO().eaRepo;

			// 反映前のEAから取得した要素オブジェクトの情報を elements 配下のXMLに反映する
			ElementVO revertedElement = null;
			EA.Element eaElemObj = (EA.Element)repo.GetElementByGuid(mergedElement.guid) ;
			if( eaElemObj == null ) {
				MessageBox.Show("EAから要素の取得に失敗しました。 異常終了しました。 GUID=" + mergedElement.guid);
				return;
			} else {
				revertedElement = ObjectEAConverter.getElementFromEAObject(eaElemObj);
			}


			// 反映後のEAから取得した要素オブジェクトの情報を elements 配下のXMLに反映する
			ArtifactXmlWriter writer = new ArtifactXmlWriter();
			StreamWriter elsw = null;
			string outputDir = ProjectSetting.getVO().projectPath;
			string elemFilePath = outputDir + @"\elements\" + revertedElement.guid.Substring(1,1) + @"\" + revertedElement.guid.Substring(2,1) + @"\" + revertedElement.guid.Substring(1,36) + ".xml";
			try {
				//BOM無しのUTF8でテキストファイルを作成する
				elsw = new StreamWriter(elemFilePath);
				elsw.WriteLine( @"<?xml version=""1.0"" encoding=""UTF-8""?> " );
				ElementXmlWriter.writeElementXml(revertedElement, 0, elsw);
			} catch( Exception ex ) {
				Console.WriteLine(ex.Message);
			} finally {
				if( elsw != null ) elsw.Close();
			}
			
			// changedFile が存在したら削除する
			try {
				string changedElemFilePath = outputDir + @"\elements\" + revertedElement.guid.Substring(1,1) + @"\" + revertedElement.guid.Substring(2,1) + @"\" + revertedElement.guid.Substring(1,36) + "_changed.xml";
				if( System.IO.File.Exists(changedElemFilePath) ) {
					System.IO.File.Delete(changedElemFilePath);
				}
			} catch( Exception ex ) {
				Console.WriteLine(ex.Message);
			}

			// 呼び出し元から受領した更新後ElementをRevert後の要素で置き換え
//			rightElement = revertedElement;
			
			// 親フォームの再描画を依頼
			ElementForm oya = (ElementForm)this.Owner;
			oya.repaint(revertedElement);

			// XMLファイルをコミットするためにTortoiseGitを起動する
			//string exename = @"TortoiseGitProc.exe";
			//string arguments = @"/command:commit /path:" + elemFilePath;
			
			//Process p = Process.Start( exename, arguments );
			
			// 自分をクローズ
			this.Close();
		}
	}
}
