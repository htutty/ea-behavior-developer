using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ArtifactFileAccessor.vo;
using ElementEditor.vo;
using ElementEditor.usercontrol.vo;
using ElementEditor.util;
using HWND = System.IntPtr;

namespace ElementEditor.usercontrol
{
    /// <summary>
    /// リッチテキストボックス
    /// </summary>
    public class SyntaxRichTextBox : RichTextBox
    {
        #region "定数"
        /// <summary>
        /// ティップスコンポーネントのキー
        /// </summary>
        private const string CONTROL_KEY = "lstTips";

        /// <summary>
        /// ティップスした文字列の後ろの文字列
        /// </summary>
        private const string METHOD_AFTER_WORLD = " ";

        /// <summary>
        /// 半角ドット：（.）
        /// </summary>
        private const string HALF_DOT = ".";

        /// <summary>
        /// 改行コード
        /// </summary>
        private const string NEW_LINE = "\n";
        #endregion

        #region "変数"
        /// <summary>
        /// ヒストリーのインデックス
        /// </summary>
        private int historyIndex = 0;

        /// <summary>
        /// ヒストリーの値
        /// </summary>
        private IList<HistoryBehavior> historyBehaviorValue = new List<HistoryBehavior>();

        /// <summary>
        /// テキストチェンジイベント有効フラグ
        /// </summary>
        private bool _isTextChaneEventInvalid = false;

        /// <summary>
        /// 自動採番フラグ
        /// </summary>
        private bool _numberingFlg = false;
        public bool NumberingFlg
        {
            get { return this._numberingFlg; }
            set { _numberingFlg = value; if (value) { this.BuildUpHeaderNumbers(); } else { this.ClearHeaderNumbers(); } }
        }

        /// <summary>
        /// キーワード
        /// </summary>
        private IList _crossReferenceList = new List<CrossReference>();
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IList CrossReferenceList
        {
            get { return this._crossReferenceList; }
            set { if (value == null) { this._crossReferenceList = new List<CrossReference>(); } else { this._crossReferenceList = value; } }
        }

        /// <summary>
        /// 段落番号
        /// </summary>
        private IList<HeaderNumber> HeaderNumbers = new List<HeaderNumber>();

        /// <summary>
        /// ヒントの横幅
        /// </summary>
        public int LstTipsWidth { get; set; }

        /// <summary>
        /// ヒントの高さ
        /// </summary>
        public int LstTipsHeight { get; set; }
        #endregion

        #region "win32api"
        /// <summary>
        /// フリッカーの防止
        /// </summary>
        [DllImport("user32")]
        private static extern int SendMessage(HWND hwnd, int wMsg, int wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0xB;
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyntaxRichTextBox()
            : base()
        {
            // 自動的に次の行に折り返さない
            base.WordWrap = false;
            // テキストチェンジイベントを無効化にする
            this._isTextChaneEventInvalid = false;
        }
        #endregion

        #region "クリックイベント"
        /// <summary>
        /// クリックイベント
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnClick(EventArgs e)
        {
            // ティップスコンポーネントを非表示する
            if (this.Controls.ContainsKey(CONTROL_KEY))
            {
                TipsListBox lstTips = (TipsListBox)this.Controls[CONTROL_KEY];
                if (lstTips.Visible)
                {
                    lstTips.Hide();
                }
            }
        }
        #endregion

        #region "キーダウンイベント"
        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            #region "ティップス"
            // ティップスのフィルタと位置を調整する
            if (this.Controls.ContainsKey(CONTROL_KEY))
            {
                // フォーカスは最初の位置の場合、処理をキャンセルする
                if (base.SelectionStart == 0)
                {
                    return;
                }

                // ティップス
                TipsListBox lstTips = (TipsListBox)this.Controls[CONTROL_KEY];

                if (e.KeyCode == Keys.Escape)
                {
                    // ESCAPEの場合、ティップスを非表示にする
                    lstTips.Hide();
                    this.Controls.Remove(lstTips);
                    lstTips.Dispose();
                }
                else if (e.KeyCode == Keys.Back)
                {
                    // BACKの場合、ティップスを非表示にする
                    base.SelectionStart -= 1;
                    if (base.SelectionColor == Color.Blue)
                    {
                        lstTips.Hide();
                        this.Controls.Remove(lstTips);
                        lstTips.Dispose();
                    }
                    base.SelectionStart += 1;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (lstTips.Visible)
                    {
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Tab)
                {
                    // DOWN・TABの場合、ティップスをフォーカスする
                    if (lstTips.Visible)
                    {
                        lstTips.Focus();
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    // UP・LEFT・RIGHTの場合、ティップスを非表示にする
                    if (lstTips.Visible)
                    {
                        lstTips.Hide();
                        this.Controls.Remove(lstTips);
                        lstTips.Dispose();
                    }
                }
            }
            #endregion

            #region "段落番号"
            // 段落番号の発番
            if (_numberingFlg && !e.Handled && !string.IsNullOrEmpty(base.Text))
            {
                int selectStart = base.SelectionStart;
                int line = base.GetLineFromCharIndex(selectStart);
                string lineStr = base.Lines[line];

                // 選択行の開始インデックス
                int linestart = 0;
                for (int i = 0; i < line; i++)
                {
                    linestart += base.Lines[i].Length + 1;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    // 当該行が空白以外の場合
                    if (!string.IsNullOrEmpty(lineStr))
                    {
                        // 該当行の段落番号
                        string thisHeaderNumber = lineStr.GetHeaderNumber();

                        // 当該行は段落番号しかない場合、段落番号を更新する
                        if (!string.IsNullOrEmpty(thisHeaderNumber) && (lineStr.Equals(thisHeaderNumber) || lineStr.Equals(thisHeaderNumber + StringUtil.FULLWIDTH_SPACE)))
                        {
                            HeaderNumber nextHeaderNumber = null;
                            for (int i = line + 1; i < base.Lines.Length; i++)
                            {
                                if (this.HeaderNumbers[i].HeaderNumberDeep != -1)
                                {
                                    nextHeaderNumber = this.HeaderNumbers[i];
                                    break;
                                }
                            }

                            if (nextHeaderNumber != null && nextHeaderNumber.HeaderNumberDeep > this.HeaderNumbers[line].HeaderNumberDeep)
                            {
                                e.Handled = true;
                                return;
                            }

                            string parentNextHeaderNumber = lineStr.GetParentNextHeaderNumber();

                            this.HeaderNumbers[line].SetHeaderNumber(parentNextHeaderNumber);

                            // 段落番号がルート番号の場合、前に改行する
                            if (parentNextHeaderNumber.Length == 1)
                            {
                                this.HeaderNumbers.Insert(line, new HeaderNumber());
                                line += 1;
                                parentNextHeaderNumber = NEW_LINE + parentNextHeaderNumber;
                            }

                            // 段落番号が空白以外の場合、最後に全角スペースを追加する
                            if (!string.IsNullOrEmpty(parentNextHeaderNumber))
                            {
                                parentNextHeaderNumber += StringUtil.FULLWIDTH_SPACE;
                            }

                            // 当該行のテキストをクリアする
                            base.SelectionStart = linestart;
                            base.SelectionLength = lineStr.Length;
                            base.SelectedText = string.Empty;

                            // 発番をリセットする
                            base.SelectedText = parentNextHeaderNumber;
                            linestart += parentNextHeaderNumber.Length;

                            // 段落番号を更新する
                            this.UpdateHeaderNumberAdd(line);

                            // フォーカスを当該行の最後に移動する
                            base.SelectionStart = linestart;
                            base.SelectionLength = 0;

                            e.Handled = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Back)
                {
                    if (selectStart == linestart)
                    {
                        base.SelectionStart = selectStart;
                        base.SelectionLength = base.Lines[line].GetHeaderNumber().Length;
                        base.SelectedText = string.Empty;
                        base.SelectionLength = 0;

                        this.HeaderNumbers.RemoveAt(line);
                    }
                    else
                    {
                        // 段落番号の場合、段落番号を削除する
                        if (base.SelectionBackColor == Color.Yellow)
                        {
                            base.SelectionStart = linestart;
                            base.SelectionLength = base.Lines[line].GetHeaderNumber().Length;
                            base.SelectedText = string.Empty;
                            base.SelectionLength = 0;
                            e.Handled = true;

                            int deep = this.HeaderNumbers[line].HeaderNumberDeep;
                            this.HeaderNumbers[line].SetHeaderNumber(string.Empty);

                            for (int i = line - 1; i >= 0; i--)
                            {
                                if (deep == this.HeaderNumbers[i].HeaderNumberDeep)
                                {
                                    this.HeaderNumbers[line].SetHeaderNumber(this.HeaderNumbers[i].GetHeaderNumber());
                                    this.UpdateHeaderNumberAdd(i);
                                    break;
                                }
                                else if (deep > 0 && deep == this.HeaderNumbers[i].HeaderNumberDeep + 1)
                                {
                                    this.HeaderNumbers[line].SetHeaderNumber(this.HeaderNumbers[i].GetHeaderNumber() + "．１");
                                    this.UpdateHeaderNumberAdd(line);
                                    break;
                                }
                            }
                            base.SelectionStart = linestart + base.Lines[line].GetHeaderNumber().Length;
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region "キーアップイベント"
        /// <summary>
        /// キーアップイベント
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            #region "文字列処理"
            try {
	            if (e.Control)
	            {
	                if (e.KeyCode == Keys.A)
	                {
	                    if (!string.IsNullOrEmpty(base.Text))
	                    {
	                        this.SelectAll();
	                    }
	                }
	                else if (e.KeyCode == Keys.X)
	                {
	                    if (!string.IsNullOrEmpty(base.SelectedText))
	                    {
	                        Clipboard.SetText(base.SelectedText);
	                        base.SelectedText = "";
	                        this.InvokeTextChange();
	                    }
	                }
	                else if (e.KeyCode == Keys.C)
	                {
	                    if (!string.IsNullOrEmpty(base.SelectedText))
	                    {
	                        Clipboard.SetText(base.SelectedText);
	                    }
	                }
	                else if (e.KeyCode == Keys.V)
	                {
	                    if (!string.IsNullOrEmpty(Clipboard.GetText()))
	                    {
	                        base.SelectedText = Clipboard.GetText();
	                        this.InvokeTextChange();
	                    }
	                }
	                return;
	            }
            } catch(Exception ex) {
                MessageBox.Show(ex.ToString());
            }
            #endregion

            #region "ティップス"
            if (e.KeyCode == Keys.Enter)
            {
                // ティップスのフィルタと位置を調整する
                if (this.Controls.ContainsKey(CONTROL_KEY))
                {
                    // ティップス
                    TipsListBox lstTips = (TipsListBox)this.Controls[CONTROL_KEY];

                    if (lstTips.Visible)
                    {
                        // ENTERの場合、ティップスに選択された項目を振る舞いに差し込む
                        if (lstTips.SelectedIndex > -1)
                        {
                            string tips = lstTips.SelectedItem.ToString();
                            tips = tips.Substring(lstTips.Prefix.Length, tips.Length - lstTips.Prefix.Length);

                            this.SelectionStart = this.SelectionStart + this.SelectionLength;
                            this.SelectedText = tips + METHOD_AFTER_WORLD;

                            lstTips.Hide();
                            this.Controls.Remove(this);
                            lstTips.Dispose();
                            this.Focus();
                        }
                        e.Handled = true;
                        return;
                    }
                }

                // 前行のテキストを強調表示する
                this.DoPreLineTextHightLight();
            }
            #endregion

            #region "段落番号"
            if (_numberingFlg && !e.Handled)
            {
                int selectStart = base.SelectionStart;
                int line = base.GetLineFromCharIndex(selectStart);

                if (e.KeyCode == Keys.Enter)
                {
                    if (base.SelectionBackColor == Color.Yellow)
                    {
                        e.Handled = true;
                    }

                    string lineStr = base.Lines[line];

                    if (string.IsNullOrEmpty(lineStr.GetHeaderNumber()))
                    {
                        string preLineStr = string.Empty;
                        string preHeaderNumber = string.Empty;

                        for (int i = line - 1; i >= 0; i--)
                        {
                            // テキストが空白の場合、キャンセルする
                            if (string.IsNullOrWhiteSpace(base.Lines[i].ZenToHan())) break;

                            // 前行の情報
                            preLineStr = base.Lines[i];
                            preHeaderNumber = preLineStr.GetHeaderNumber();

                            // 段落番号が存在する場合、キャンセルする
                            if (!string.IsNullOrEmpty(preHeaderNumber)) break;
                        }

                        // 前行の段落番号が存在する場合
                        if (!string.IsNullOrEmpty(preHeaderNumber))
                        {
                            // 当該行の開始番号を取得する
                            int linestart = 0;
                            for (int i = 0; i < line; i++)
                            {
                                linestart += base.Lines[i].Length + 1;
                            }

                            string headerNumber = string.Empty;
                            if (string.IsNullOrEmpty(lineStr))
                            {
                                // 当該行が空白の場合、前行の段落番号のサブ番号をセットする
                                headerNumber = StringUtil.FULLWIDTH_SPACE + preLineStr.GetSubHeaderNumber();
                            }
                            else
                            {
                                // 当該行が空白以外の場合、次の段落番号をセットする
                                headerNumber = preLineStr.GetNextHeaderNumber();
                            }

                            this.HeaderNumbers.Insert(line, new HeaderNumber());
                            this.HeaderNumbers[line].SetHeaderNumber(headerNumber);

                            // 選択状態を初期化する
                            base.SelectionStart = linestart;
                            base.SelectionLength = 0;
                            base.SelectedText = string.Empty;

                            // 段落番号を更新する
                            if (string.IsNullOrEmpty(headerNumber.GetParentHeaderNumber()))
                            {
                                this.HeaderNumbers.Insert(line, new HeaderNumber());

                                // 親段落番号がない場合、改行コードを追加する
                                base.SelectedText = NEW_LINE + headerNumber + StringUtil.FULLWIDTH_SPACE;
                                line += 1;
                            }
                            else
                            {
                                base.SelectedText = headerNumber + StringUtil.FULLWIDTH_SPACE;
                            }
                            selectStart = base.SelectionStart + base.SelectionLength;

                            // 段落番号を更新する
                            this.UpdateHeaderNumberAdd(line);

                            // フォーカスは当該行の最後に移動する
                            base.SelectionStart = selectStart;
                            base.SelectionLength = 0;
                        }
                    }
                }
                else
                {
                    if (base.SelectionBackColor == Color.Yellow)
                    {
                        e.Handled = true;
                    }
                }
            }
            #endregion
        }

        #region "前行のハイライト"
        /// <summary>
        /// 前行のテキストはキーワードをハイライトで表示する
        /// </summary>
        private void DoPreLineTextHightLight()
        {
            if (base.Text != "")
            {
                SendMessage(base.Handle, WM_SETREDRAW, 0, IntPtr.Zero);

                int selectStart = base.SelectionStart;
                int line = base.GetLineFromCharIndex(selectStart);

                // 行テキストをハイライトする
                this.HightLightRichBoxLine(line - 1);

                SendMessage(base.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
                base.Refresh();
            }
        }
        #endregion
        #endregion

        #region "テキストチェンジイベント"
        /// <summary>
        /// テキストチェンジイベント
        /// </summary>
        /// <param name="e">イベント</param>
        protected override void OnTextChanged(EventArgs e)
        {
            if (!_isTextChaneEventInvalid) return;

            this.DoCurrentLineTextHightLight();

            this.ShowTipsListBox();

            this.SaveHistoryBehaviorValue();
        }

        /// <summary>
        /// キーワードをハイライトで表示する
        /// </summary>
        private void DoCurrentLineTextHightLight()
        {
            if (base.Text != "")
            {
                SendMessage(base.Handle, WM_SETREDRAW, 0, IntPtr.Zero);

                int selectStart = base.SelectionStart;
                int line = base.GetLineFromCharIndex(selectStart);

                // 行テキストをハイライトする
                this.HightLightRichBoxLine(line);

                SendMessage(base.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
                base.Refresh();
            }
        }

        #region "補足キーワードリストボックス"
        /// <summary>
        /// 補足キーワードリストボックスを表示する
        /// </summary>
        private void ShowTipsListBox()
        {
            // ティップスコンポーネントを取得する
            TipsListBox lstTips = null;
            if (this.Controls.ContainsKey(CONTROL_KEY))
            {
                lstTips = (TipsListBox)this.Controls[CONTROL_KEY];
            }
            else
            {
                lstTips = new TipsListBox();
                lstTips.Hide();
                lstTips.Name = CONTROL_KEY;
                lstTips.TabIndex = 100;

                lstTips.Width = this.LstTipsWidth;
                lstTips.Height = this.LstTipsHeight;

                this.Controls.Add(lstTips);
            }

            if (base.SelectionStart > 0 && base.SelectionStart <= base.TextLength)
            {
                if (HALF_DOT.Equals(base.Text.Substring(base.SelectionStart - 1, 1)))
                {
                    string beforeText = base.Text.Substring(0, base.SelectionStart - 1);

                    IList<string> hintList = null;
                    foreach (CrossReference crossReference in CrossReferenceList)
                    {
                        if (beforeText.EndsWith(crossReference.Name))
                        {
                            hintList = new List<string>();
                            foreach (var attributesMethod in crossReference.AttributesMethods)
                            {
                                hintList.Add(attributesMethod.Name);
                            }

                            break;
                        }
                    }

                    if (hintList != null && hintList.Count > 0)
                    {
                        lstTips.Items.Clear();
                        foreach (string hintWord in hintList)
                        {
                            lstTips.Items.Add(hintWord);
                        }
                        lstTips.SelectedIndex = 0;
                        lstTips.Show();
                    }
                    else
                    {
                        lstTips.Hide();
                        this.Controls.Remove(lstTips);
                        lstTips.Dispose();
                    }
                }
                else
                {
                    if (lstTips.Visible)
                    {
                        int lineStart = base.SelectionStart;
                        int line = base.GetLineFromCharIndex(lineStart);
                        for (int i = 0; i < line; i++)
                        {
                            lineStart -= base.Lines[i].Length + 1;
                        }

                        string lineStr = base.Lines[line];
                        string lineStartStr = lineStr.Substring(0, lineStart);
                        string prefix = lineStartStr.Substring(lineStartStr.LastIndexOf(HALF_DOT) + 1);

                        lstTips.Prefix = prefix;

                        int itemCnt = lstTips.Items.Count;
                        for (int i = itemCnt - 1; i >= 0; i--)
                        {
                            string itemValue = (string)lstTips.Items[i];
                            if (!itemValue.StartsWith(prefix))
                            {
                                lstTips.Items.RemoveAt(i);
                            }
                        }

                        if (lstTips.Items.Count == 0)
                        {
                            lstTips.Hide();
                            this.Controls.Remove(lstTips);
                            lstTips.Dispose();
                        }
                        else
                        {
                            if (lstTips.SelectedIndex == -1)
                            {
                                lstTips.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }

            // マウスの座標
            Point mousePoint = this.GetPositionFromCharIndex(this.SelectionStart);

            // 座標X
            if (mousePoint.X > this.Width - lstTips.Width)
            {
                lstTips.Left = mousePoint.X - lstTips.Width;
            }
            else
            {
                lstTips.Left = mousePoint.X;
            }

            // 座標Y
            if (mousePoint.Y > this.Height - 30 - lstTips.Height)
            {
                lstTips.Top = mousePoint.Y - lstTips.Height;
            }
            else
            {
                lstTips.Top = mousePoint.Y + 15;
            }
        }
        #endregion
        #endregion

        #region "ハイライトの再表示"
        /// <summary>
        /// キーワードをハイライトで再表示する
        /// </summary>
        public void InvokeTextChange()
        {
            this.DoAllLinesHightLight();
        }

        /// <summary>
        /// キーワードをハイライトで表示する
        /// </summary>
        private void DoAllLinesHightLight()
        {
            if (base.Text != "")
            {
                SendMessage(base.Handle, WM_SETREDRAW, 0, IntPtr.Zero);

                int selectStart = base.SelectionStart;

                for (int line = 0; line < base.Lines.Length; line++)
                {
                    // 行テキストをハイライトする
                    this.HightLightRichBoxLine(line);
                }

                if (_numberingFlg)
                {
                    this.BuildUpHeaderNumbers();
                }

                SendMessage(base.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
                base.Refresh();
            }

            // テキストチェンジイベントを有効化にする
            this._isTextChaneEventInvalid = true;
        }
        #endregion

        #region "入力フォーカスがコントロールを離れるイベント"
        /// <summary>
        ///  入力フォーカスがコントロールを離れると発生します
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnLeave(EventArgs e)
        {
            // ティップスが表示する場合、非表示にする
            if (this.Controls.ContainsKey(CONTROL_KEY))
            {
                TipsListBox lstTips = (TipsListBox)this.Controls[CONTROL_KEY];
                lstTips.Hide();
                this.Controls.Remove(lstTips);
                lstTips.Dispose();
            }

            base.OnLeave(e);
        }
        #endregion

        #region "リッチテキストボックスのハイライト"
        /// <summary>
        /// リッチテキストボックスの指定行をハイライトする
        /// </summary>
        /// <param name="lineNo">行NO</param>
        private void HightLightRichBoxLine(int lineNo)
        {
            if (lineNo < 0 || lineNo > base.Lines.Length - 1)
            {
                // 行NOが範囲以外の場合、処理をキャンセルする
                return;
            }

            // テキストチェンジイベントを無効化にする
            this._isTextChaneEventInvalid = false;

            int selectStart = base.SelectionStart;

            // 選択行のテキスト
            string lineStr = base.Lines[lineNo];

            // 選択行の開始インデックス
            int linestart = 0;
            for (int i = 0; i < lineNo; i++)
            {
                linestart += base.Lines[i].Length + 1;
            }

            // 該当行の表示を初期化する
            base.SelectionStart = linestart;
            base.SelectionLength = lineStr.Length;
            base.SelectionColor = Color.Black;
            base.SelectionBackColor = SystemColors.Window;

            // キーワードをハイライトする
            MatchCollection mc = null;
            foreach (CrossReference crossReference in this.CrossReferenceList)
            {
                // 属性・操作を強調表示する
                foreach (AttributeMethod am in crossReference.AttributesMethods)
                {
                    string patternAM = Regex.Escape(crossReference.Name + HALF_DOT + am.Name);
                    mc = Regex.Matches(lineStr, patternAM);
                    foreach (Match m in mc)
                    {
                        base.SelectionStart = linestart + crossReference.Name.Length + 1 + m.Index;
                        base.SelectionLength = m.Length - crossReference.Name.Length - 1;
                        base.SelectionColor = Color.Peru;
                    }
                }

                // 相互参照対象を強調表示する
                string patternCR = Regex.Escape(crossReference.Name);
                mc = Regex.Matches(lineStr, patternCR);
                foreach (Match m in mc)
                {
                    base.SelectionStart = linestart + m.Index;
                    base.SelectionLength = m.Length;
                    base.SelectionColor = Color.Blue;
                }
            }

            // 段落番号のセット
            if (_numberingFlg)
            {
                string headerNumberStr = lineStr.GetHeaderNumber();
                base.SelectionStart = linestart;
                base.SelectionLength = headerNumberStr.Length;
                base.SelectionBackColor = Color.Yellow;
            }

            // 選択状態を初期化する
            base.SelectionStart = selectStart;
            base.SelectionLength = 0;
            base.SelectionColor = Color.Black;
            base.SelectionBackColor = SystemColors.Window;

            // テキストチェンジイベントを有効化にする
            this._isTextChaneEventInvalid = true;
        }
        #endregion

        #region "段落番号の初期化"
        /// <summary>
        /// 段落番号を初期化する
        /// </summary>
        private void BuildUpHeaderNumbers()
        {
            this.HeaderNumbers.Clear();

            int selectStart = base.SelectionStart;

            int linestart = 0;

            for (int line = 0; line < base.Lines.Length; line++)
            {
                string lineStr = base.Lines[line];
                base.SelectionStart = linestart;
                base.SelectionLength = lineStr.Length;

                lineStr = lineStr.TrimEnd('　');
                base.SelectedText = lineStr;

                string headerNumberStr = base.Lines[line].GetHeaderNumber();

                HeaderNumber headerNumber = new HeaderNumber();
                headerNumber.SetHeaderNumber(headerNumberStr);

                this.HeaderNumbers.Add(headerNumber);

                base.SelectionStart = linestart;
                base.SelectionLength = headerNumberStr.Length;
                base.SelectionBackColor = Color.Yellow;

                linestart += base.Lines[line].Length + 1;
            }

            base.SelectionStart = selectStart;
            base.SelectionLength = 0;
        }
        #endregion

        #region "段落番号の破棄"
        /// <summary>
        /// 段落番号を破棄する
        /// </summary>
        private void ClearHeaderNumbers()
        {
            this.HeaderNumbers.Clear();

            int selectStart = base.SelectionStart;

            int linestart = 0;

            for (int line = 0; line < base.Lines.Length; line++)
            {
                string lineStr = base.Lines[line];
                string headerNumberStr = base.Lines[line].GetHeaderNumber();

                base.SelectionStart = linestart;
                base.SelectionLength = headerNumberStr.Length;
                base.SelectionBackColor = SystemColors.Window;

                linestart += base.Lines[line].Length + 1;
            }

            base.SelectionStart = selectStart;
            base.SelectionLength = 0;
        }
        #endregion

        #region "段落番号の更新"
        /// <summary>
        /// 段落番号を更新する
        /// </summary>
        /// <param name="startLine">開始行No</param>
        private void UpdateHeaderNumberAdd(int startLine)
        {
            HeaderNumber thisHeaderNumber = this.HeaderNumbers[startLine];
            bool isRoot = base.Lines[startLine].IsRootHeaderNumber();

            int currNumber = thisHeaderNumber.LastHeaderNumber();
            int currSubNumber = 1;

            int linestart = 0;
            for (int i = 0; i < startLine; i++)
            {
                linestart += base.Lines[i].Length + 1;
            }

            base.SelectionStart = linestart;
            base.SelectionLength = base.Lines[startLine].GetHeaderNumber().Length;
            base.SelectedText = thisHeaderNumber.GetHeaderNumber();

            linestart += base.Lines[startLine].Length + 1;

            for (int i = startLine + 1; i < base.Lines.Length; i++)
            {
                string lineStr = base.Lines[i];
                HeaderNumber curHeaderNumber = this.HeaderNumbers[i];

                if (curHeaderNumber.HeaderNumberDeep == -1)
                {
                    linestart += lineStr.Length + 1;
                    continue;
                }

                if (curHeaderNumber.HeaderNumberDeep < thisHeaderNumber.HeaderNumberDeep) break;

                for (int j = 0; j < thisHeaderNumber.HeaderNumberDeep; j++)
                {
                    curHeaderNumber.UpdateHeaderNumber(j, thisHeaderNumber.IndexHeaderNumber(j));
                }

                if (curHeaderNumber.HeaderNumberDeep == thisHeaderNumber.HeaderNumberDeep)
                {
                    curHeaderNumber.UpdateHeaderNumber(thisHeaderNumber.HeaderNumberDeep, currNumber + 1);
                    currNumber += 1;
                    currSubNumber = 1;
                }
                else
                {
                    curHeaderNumber.UpdateHeaderNumber(thisHeaderNumber.HeaderNumberDeep, currNumber);
                    if (curHeaderNumber.HeaderNumberDeep == thisHeaderNumber.HeaderNumberDeep + 1)
                    {
                        curHeaderNumber.UpdateHeaderNumber(thisHeaderNumber.HeaderNumberDeep + 1, currSubNumber);
                        currSubNumber += 1;
                    }
                }

                base.SelectionStart = linestart;
                base.SelectionLength = lineStr.GetHeaderNumber().Length;
                base.SelectedText = curHeaderNumber.GetHeaderNumber();

                linestart += base.Lines[i].Length + 1;
            }
        }
        #endregion

        #region "ヒストリー"
        /// <summary>
        /// 前ヒストリーのチェック
        /// </summary>
        public bool HavePreHistory()
        {
            bool rst = false;

            if (this.historyIndex > 0)
            {
                rst = true;
            }

            return rst;
        }

        /// <summary>
        /// 前ヒストリーの表示
        /// </summary>
        public void ShowPreHistory()
        {
            if (HavePreHistory())
            {
                this.historyIndex -= 1;
                base.Text = this.historyBehaviorValue[this.historyIndex].Text;
                base.SelectionStart = this.historyBehaviorValue[this.historyIndex].SelectionStart;
            }
        }

        /// <summary>
        /// 後ヒストリーのチェック
        /// </summary>
        public bool HaveAfterHistory()
        {
            bool rst = false;

            if (this.historyIndex < this.historyBehaviorValue.Count)
            {
                rst = true;
            }

            return rst;
        }

        /// <summary>
        /// 後ヒストリーの表示
        /// </summary>
        public void ShowAfterHistory()
        {
            if (HaveAfterHistory())
            {
                this.historyIndex += 1;
                base.Text = this.historyBehaviorValue[this.historyIndex].Text;
                base.SelectionStart = this.historyBehaviorValue[this.historyIndex].SelectionStart;
            }
        }

        /// <summary>
        /// ヒストリーを追加する
        /// </summary>
        public void SaveHistoryBehaviorValue()
        {
            while (this.historyBehaviorValue.Count > this.historyIndex)
            {
                this.historyBehaviorValue.RemoveAt(this.historyIndex);
            }

            HistoryBehavior historyBehavior = new HistoryBehavior()
            {
                SelectionStart = base.SelectionStart,
                Text = base.Text
            };

            this.historyBehaviorValue.Add(historyBehavior);

            if (this.historyBehaviorValue.Count > 10)
            {
                this.historyBehaviorValue.RemoveAt(0);
            }

            this.historyIndex = this.historyBehaviorValue.Count - 1;
        }
        #endregion

        #region "利用された参照情報の取得"
        /// <summary>
        /// 使用された相互参照の情報をフィルタする
        /// </summary>
        /// <returns>相互参照リスト</returns>
        public List<CrossReference> GetUsedCrossReference()
        {
            // 振る舞いが空白の場合、空リストを戻る
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                return new List<CrossReference>();
            }

            List<CrossReference> rst = new List<CrossReference>();

            foreach (CrossReference crossReference in this.CrossReferenceList)
            {
                // 相互参照対象を強調表示する
                string crossReferenceName = crossReference.Name;
                int nameIdx = this.Text.IndexOf(crossReferenceName);

                // 該当クラスの名前は振る舞いに存在する場合
                if (nameIdx != -1)
                {
                    // 相互参照要素を追加する
                    CrossReference cr = new CrossReference()
                    {
                        // GUID
                        ElementGUID = crossReference.ElementGUID,
                        // 名前
                        Name = crossReference.Name,
                        // クラスフラグ
                        classflg = crossReference.classflg,
                        // 属性・操作一覧
                        AttributesMethods = new List<AttributeMethod>()
                    };

                    // 属性・操作を強調表示する
                    foreach (AttributeMethod attributeMethod in crossReference.AttributesMethods)
                    {
                        // 相互参照要素名．属性・操作名
                        string attributeName = crossReference.Name + HALF_DOT + attributeMethod.Name;
                        int attributeIdx = this.Text.IndexOf(attributeName);

                        if (attributeIdx != -1)
                        {
                            // 属性・操作を追加する
                            AttributeMethod am = new AttributeMethod()
                            {
                                // 名前
                                Name = attributeMethod.Name,
                                // GUID
                                GUID = attributeMethod.GUID
                            };
                            cr.AttributesMethods.Add(am);
                        }
                    }
                    rst.Add(cr);
                }
            }

            return rst;
        }
        #endregion
    }
}
