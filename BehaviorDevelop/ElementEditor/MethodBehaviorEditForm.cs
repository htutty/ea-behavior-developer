using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BDFileReader.vo;
using BDFileReader.reader;
using BDFileReader.util;
using ElementEditor.vo;
using System.IO;
using System.Reflection;
using EA;

namespace ElementEditor
{
    /// <summary>
    /// 操作の振る舞い編集画面
    /// </summary>
    public partial class MethodBehaviorEditForm : Form
    {
        #region "定数"
        /// <summary>
        /// 相互参照のタグ付き値の名前
        /// </summary>
        private const string CROSS_REFERENCE = "CrossReference";

        /// <summary>
        /// 画面表示配置ファイル名
        /// </summary>
        public const string DISPLAY_CONFIG_FILENAME = @"DisplayConfig.xml";

        /// <summary>
        /// プロジェクト配置ファイル名
        /// </summary>
        public const string PROJECT_CONFIG_FILENAME = @"ProjectConfig.xml";

        /// <summary>
        /// 振る舞いの属性名
        /// </summary>
        public const string PROPERTY_NAME = @"Behavior";
        #endregion

        #region "変数"
        /// <summary>
        /// フォームクローズ状態
        /// </summary>
        private bool isFormClosing = false;

        /// <summary>
        /// Element, Methodオブジェクト（ツールの内部型）
        /// </summary>
        private ElementVO element = null;
        private MethodVO method = null;

        /// <summary>
        /// 前回保存の操作の名前
        /// </summary>
        private string oldMethodName = string.Empty;

        /// <summary>
        /// 前回保存の振る舞いの値
        /// </summary>
        private string oldBehaviorValue = string.Empty;

        /// <summary>
        /// 相互参照一覧のデータソース
        /// </summary>
        private BindingList<CrossReference> relGridDataSource = null;
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="element">対象の要素（Connector取得用）</param>
        /// <param name="method">対象のメソッド</param>
        public MethodBehaviorEditForm(ElementVO element, MethodVO method)
        {
            this.element = element;
            this.method = method;

            InitializeComponent();
        }
		#endregion

        #region "画面ロード"
        /// <summary>
        /// 画面ロード
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">引数</param>
        private void Form_Load(object sender, EventArgs e)
        {
//            this.InitCommonCrossReference();

            this.Init();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Init()
        {
            
            // 前回保存の操作の名前
            this.oldMethodName = this.method.name;
            // 前回の振る舞いを保存する
            this.oldBehaviorValue = this.method.behavior;

            // 操作の名前
            this.txtMethodName.Text = this.method.name;
			this.labelMethodAlias.Text = this.method.alias;
            
            // 振る舞いの最新値を更新する
            this.txtBehavior.Text = this.method.behavior;

            this.txtBehavior.InvokeTextChange();

            // 右クリックメニューをバインディングする
            this.txtBehavior.ContextMenuStrip = this.behaviorMenuStrip;
//            this.relGrid.ContextMenuStrip = this.CrossReferenceMenuStrip;
        }
        #endregion

        #region "操作の所属クラスの接続要素の取得"
        /// <summary>
        /// 操作の所属クラスの接続要素を取得する
        /// </summary>
        /// <param name="method">EA.Method</param>
        public IList<CrossReference> GetConnectionClassList()
        {
            IList<CrossReference> crList = new List<CrossReference>();
            ElementsXmlReader elemReader = new ElementsXmlReader(ProjectSetting.getVO().projectPath);

            foreach(ConnectorVO convo in this.element.connectors ) {
            	CrossReference cref = new CrossReference();
            	cref.Name = convo.targetObjName;
                cref.ElementGUID = convo.targetObjGuid;
                
                ElementVO destObject = elemReader.readElementByGUID(cref.ElementGUID);
                
                // タイプ
                cref.Type = convo.connectorType;

				// コメント
//                cref.Notes = connector.Notes;
				cref.Notes = destObject.notes;
					
                // パッケージ
//                cref.PackageName = repository.GetPackageByID(targetElement.PackageID).Name;
				cref.PackageName = "packageName";

					// 属性・操作一覧
                cref.AttributesMethods = this.GetAttributesMethodsFromElement(destObject);
					
                // クラスフラグ
                cref.classflg = false;
                // 削除権限フラグ
                cref.CanDelete = false;
            		
            	crList.Add(cref);
            }

            return crList;
        }
        #endregion
        
        
        #region "要素の属性と操作の名前リストの取得"
        /// <summary>
        /// 要素の属性と操作の名前リストを取得する
        /// </summary>
        /// <param name="element">要素</param>
        /// <returns>属性と操作の名前リスト</returns>
        private IList<AttributeMethod> GetAttributesMethodsFromElement(ElementVO element)
        {
            IList<AttributeMethod> rtnList = new List<AttributeMethod>();

            foreach (AttributeVO att in element.attributes)
            {
                AttributeMethod attributeMethod = new AttributeMethod()
                {
                    Name = att.name,
                    GUID = att.guid
                };

                rtnList.Add(attributeMethod);
            }

            foreach (MethodVO method in element.methods)
            {
                AttributeMethod attributeMethod = new AttributeMethod()
                {
                    Name = method.name,
                    GUID = method.guid
                };
                rtnList.Add(attributeMethod);
            }

            return rtnList;
        }
        #endregion

        
        #region "振る舞いコントロールのクリックイベント"
        /// <summary>
        /// 振る舞いコントロールのクリックイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void txtBehavior_Click(object sender, EventArgs e)
        {
            if (txtBehavior.SelectionColor == Color.Blue)
            {
                int clickSelectionStart = txtBehavior.SelectionStart;

                string selectedText = "";

                foreach (var row in this.relGridDataSource)
                {
                    string testString = row.Name;
                    int testStrLen = testString.Length;
                    int startIndex = txtBehavior.Text.IndexOf(testString, 0);
                    while (startIndex >= 0)
                    {
                        if (clickSelectionStart > startIndex && clickSelectionStart < startIndex + testStrLen)
                        {
                            selectedText = testString;
                            break;
                        }
                        startIndex = txtBehavior.Text.IndexOf(testString, startIndex + testStrLen);
                    }
                }

                if (!String.IsNullOrWhiteSpace(selectedText))
                {
                    MessageBox.Show(selectedText);
                }
            }
        }
        #endregion

        #region "編集結果の保存処理"
        /// <summary>
        /// 編集結果を保存する
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SavaMethodChange();
            this.Close();
        }

        /// <summary>
        /// 保存処理
        /// </summary>
        private void SavaMethodChange()
        {
            // 前回保存の操作の名前
            this.oldMethodName = this.method.name;
            // 前回の振る舞いを保存する
            this.oldBehaviorValue = changeLfToCrlf(this.txtBehavior.Text);

            // 名前
            this.method.name = this.txtMethodName.Text;
            // 振る舞い
            this.method.behavior = changeLfToCrlf(this.txtBehavior.Text);
            
            this.method.changed = 'U';
            this.element.changed = 'U';
            
            ElementForm parentForm = (ElementForm)(this.Owner);
			parentForm.repaintFormMethod(this.method);
            
        }
        #endregion

        /// <summary>
        /// 文字列の改行コードをLFからCRLFに変換（TextBox→String）
        /// </summary>
        /// <param name="srcStr">変換元の文字列</param>
        /// <returns>変換後の文字列</returns>
        private string changeLfToCrlf(string srcStr) {
        	if( srcStr != null ) {
        		return srcStr.Replace("\n","\r\n");
        	} else {
        		return srcStr;
        	}
        }
        
        /// <summary>
        /// 文字列の改行コードをCRLFからLFに変換（String→TextBox）
        /// </summary>
        /// <param name="srcStr">変換元の文字列</param>
        /// <returns>変換後の文字列</returns>
        private string changeCrlfToLf(string srcStr) {
        	if( srcStr != null ) {
        		return srcStr.Replace("\r\n", "\n");
        	} else {
        		return srcStr;
        	}        	
        }
        
        
        
        #region "振る舞いテキストのコンテキストメニュー選択時の処理"
        /// <summary>
        /// 振る舞いのすべて選択
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtBehavior.Text))
            {
                this.txtBehavior.SelectAll();
            }
        }

        /// <summary>
        /// 振る舞いの切り取り処理
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtBehavior.SelectedText))
            {
            	try {
	                Clipboard.SetText(this.txtBehavior.SelectedText);
	                this.txtBehavior.SelectedText = "";
	                this.txtBehavior.InvokeTextChange();
            	} catch(Exception ex) {
            		Console.WriteLine(ex.Message);
            	}
            }
        }

        /// <summary>
        /// 振る舞いのコピー処理
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try {
	            if (!string.IsNullOrEmpty(this.txtBehavior.SelectedText)) {
	                Clipboard.SetText(this.txtBehavior.SelectedText, TextDataFormat.UnicodeText);
	            }
        	} catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        	
        }

        /// <summary>
        /// 振る舞いの貼り付け処理
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try {
	        	if (!string.IsNullOrEmpty(Clipboard.GetText())) {
	                this.txtBehavior.SelectedText = Clipboard.GetText();
	                this.txtBehavior.InvokeTextChange();
	            }
        	} catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 振る舞いの削除処理
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtBehavior.SelectedText))
            {
                this.txtBehavior.SelectedText = "";
                this.txtBehavior.InvokeTextChange();
            }
        }
        #endregion

        #region "画面クローズイベント"
        /// <summary>
        /// 画面クローズイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MethoBehaviorEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isFormClosing = true;
            
			string nowMethodNameText = txtMethodName.Text;
            string nowBehaviorText = changeLfToCrlf(this.txtBehavior.Text);
            
            if (!this.oldBehaviorValue.Equals(nowBehaviorText) || !this.oldMethodName.Equals(nowMethodNameText))
            {
                DialogResult rst = MessageBox.Show("変更データを保存しますか？\n・はい：保存します。\n・いいえ：保存しません。\n・キャンセイル：画面に戻ります。", "保存確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (rst == DialogResult.Yes)
                {
                    // 保存する
                    this.SavaMethodChange();
                }
                else if (rst == DialogResult.No)
                {
                    method.name = oldMethodName;
                    method.behavior = oldBehaviorValue;
                    
                }
                else if (rst == DialogResult.Cancel)
                {
                    this.isFormClosing = false;

                    // クローズイベントをキャンセルする
                    e.Cancel = true;
                }
            }

        }
        #endregion


        #region "画面のサイズ変更イベント"
        /// <summary>
        /// 画面のサイズ変更イベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MethoBehaviorEditForm_Resize(object sender, EventArgs e)
        {
            // 画面の最小サイズ(既定のサイズ)
            Size minFormSize = this.MinimumSize;
            // 変更してフォームのサイズ
            Size newFormSize = this.Size;

            // 操作名前の幅の変更
            this.txtMethodName.Width = this.txtMethodName.MinimumSize.Width + (newFormSize.Width - minFormSize.Width);

            // 振る舞いのサイズの変更
            this.txtBehavior.Width = this.txtBehavior.MinimumSize.Width + (newFormSize.Width - minFormSize.Width);
            this.txtBehavior.Height = this.txtBehavior.MinimumSize.Height + (newFormSize.Height - minFormSize.Height);

            int btnSavePointX = 615 + (newFormSize.Width - minFormSize.Width);
            int btnSavePointY = 0;
            // 更新ボタンの最新の座標を更新する
            this.btnSave.Location = new Point(btnSavePointX, btnSavePointY);

            int btmBtnPanalPointX = 480 + (newFormSize.Width - minFormSize.Width);
//            int btmBtnPanalPointY = 250;
            
        }
        #endregion

        #region "採番トグルボタンのチェックチェンジ"
        /// <summary>
        /// 採番トグルボタンのチェックチェンジ
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void tgNumbering_CheckedChanged(object sender, EventArgs e)
        {
            this.txtBehavior.NumberingFlg = tgNumbering.Checked;
        }
        #endregion

        #region "フォームのアクティブイベント"
        /// <summary>
        /// フォームのアクティブイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MethodBehaviorEditForm_Activated(object sender, EventArgs e)
        {
            if (this.pickMethodTimer.Enabled)
            {
                this.pickMethodTimer.Stop();
                this.DoSeletedMethodChange();
            }
        }
        #endregion

        #region "フォームのディアクティブイベント"
        /// <summary>
        /// フォームのディアクティブイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MethodBehaviorEditForm_Deactivate(object sender, EventArgs e)
        {
            if (this.isFormClosing) return;

            this.pickMethodTimer.Start();
        }
        #endregion

        #region "タイマーのチックイベント"
        /// <summary>
        /// タイマーのチックイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void pickMethodTimer_Tick(object sender, EventArgs e)
        {
            this.pickMethodTimer.Stop();
            bool rtn = this.DoSeletedMethodChange();
            if (rtn) this.pickMethodTimer.Start();
        }
        #endregion

        #region "操作の再選択"
        /// <summary>
        /// 操作を再選択する
        /// </summary>
        /// <returns>タイマーフラグ</returns>
        private bool DoSeletedMethodChange()
        {
            bool rtn = true;

//            // 待機カーソル
//            Cursor.Current = Cursors.WaitCursor;
//
//            // 現在選択されている項目の情報を返します。
//            EA.ObjectType objectType = repository.GetContextItemType();
//
//            if (objectType == EA.ObjectType.otMethod)
//            {
//                // 選択されている要素を取得します。
//                Object selectedObject = null;
//                repository.GetContextItem(out selectedObject);
//
//                EA.Method selectedMethod = (EA.Method)selectedObject;
//                if (this.method.MethodID != selectedMethod.MethodID)
//                {
//                    if (!this.oldBehaviorValue.Equals(this.txtBehavior.Text) || !this.oldMethodName.Equals(this.txtMethodName.Text))
//                    {
//                        // 確認ダイアグラム
//                        DialogResult rst = MessageBox.Show("'振る舞い'タグの変更が保存されていません。\n\n変更内容を保存しますか？",
//                            "振る舞い編集", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
//
//                        switch (rst)
//                        {
//                            case DialogResult.Yes:
//                                // 保存する
//                                this.SavaMethodChange();
//                                // 選択される操作を表示する
//                                this.method = selectedMethod;
//                                this.Init();
//                                break;
//
//                            case DialogResult.No:
//                                // 選択される操作を表示する
//                                this.method = selectedMethod;
//                                this.Init();
//                                break;
//
//                            case DialogResult.Cancel:
//                                // 操作要素を選択する
//                                this.repository.ShowInProjectView(this.method);
//                                // 振る舞い画面をアクティブにする
//                                this.Activate();
//                                rtn = false;
//                                break;
//
//                            default:
//                                break;
//                        }
//                    }
//                    else
//                    {
//                        // 選択される操作を表示する
//                        this.method = selectedMethod;
//                        this.Init();
//                    }
//                }
//                
//            }
//
//            // 既定のカーソル
//            Cursor.Current = Cursors.Default;

            return rtn;
        }
        #endregion

        /// <summary>
        /// 画面を閉じる
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

