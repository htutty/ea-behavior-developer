using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BehaviorDevelop.vo;
using BehaviorDevelop.util;
using System.IO;
using System.Reflection;
using EA;

namespace BehaviorDevelop
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
        /// 全てのモデルやパッケージ・要素などを含む主要なコンテナ
        /// </summary>
        private EA.Repository repository = null;

        /// <summary>
        /// Element, Methodオブジェクト
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
        /// 共通相互参照クラス
        /// </summary>
        private List<CrossReference> commonCrossReferencs = null;

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
        private void TestWindowsForm_Load(object sender, EventArgs e)
        {
            this.InitDisplayConfig();

            this.InitCommonCrossReference();

            this.Init();
        }
        #endregion

        #region "画面初期表示配置の適用"
        /// <summary>
        /// 画面初期表示配置を適用する
        /// </summary>
        private void InitDisplayConfig()
        {
            // 画面のサイズ
//            DisplayConfig displayConfig = ConfigUtil.GetInitConfigObject<DisplayConfig>(DISPLAY_CONFIG_FILENAME);
			
//            if (displayConfig != null)
//            {
//                this.Width = displayConfig.ViewWidth;
//                this.Height = displayConfig.ViewHeight;
//
//                this.Location = new Point(displayConfig.ViewLocationX, displayConfig.ViewLocationY);
//
//                this.relGrid.Columns["name"].Width = displayConfig.NameColumnWidth;
//                this.relGrid.Columns["type"].Width = displayConfig.TypeColumnWidth;
//                this.relGrid.Columns["coment"].Width = displayConfig.ComentColumnWidth;
//                this.relGrid.Columns["package"].Width = displayConfig.PackageColumnWidth;
//            }
//
//            // ヒントのサイズ
//            ProjectConfig projectConfig = ConfigUtil.GetInitConfigObject<ProjectConfig>(PROJECT_CONFIG_FILENAME);
//            if (projectConfig != null)
//            {
//                this.txtBehavior.LstTipsWidth = projectConfig.LstTipsWidth;
//                this.txtBehavior.LstTipsHeight = projectConfig.LstTipsHeight;
//            }
        }
        #endregion

        #region "共通相互参照のイニシャライズ"
        /// <summary>
        /// 共通相互参照をイニシャライズする
        /// </summary>
        private void InitCommonCrossReference()
        {
            this.commonCrossReferencs = new List<CrossReference>();

//            // ヒントのサイズ
//            ProjectConfig projectConfig = ConfigUtil.GetInitConfigObject<ProjectConfig>(PROJECT_CONFIG_FILENAME);
//
//            EA.Element packageElement = this.repository.GetElementByGuid(projectConfig.PackageGUID);
//            if (packageElement != null)
//            {
//                EA.TaggedValue crossRferenceTaggedValue = null;
//                foreach (EA.TaggedValue tv in packageElement.TaggedValues)
//                {
//                    if (CROSS_REFERENCE.Equals(tv.Name))
//                    {
//                        crossRferenceTaggedValue = tv;
//                        break;
//                    }
//                }
//                if (crossRferenceTaggedValue != null)
//                {
//                    IList<string> guidList = XMLUtil.ConverMethodTagValueToGuidList(crossRferenceTaggedValue.Notes);
//                    foreach (string guid in guidList)
//                    {
//                        EA.Element element = repository.GetElementByGuid(guid);
//
//                        if (element != null)
//                        {
//                            // 選択された要素の情報を一覧に追加する
//                            CrossReference crossReference = new CrossReference()
//                            {
//                                // GUID
//                                ElementGUID = element.ElementGUID,
//                                // 名前
//                                Name = element.Name,
//                                // タイプ
//                                Type = packageElement.Type,
//                                // コメント
//                                Notes = element.Notes,
//                                // パッケージ
//                                PackageName = repository.GetPackageByID(element.PackageID).Name,
//                                // 属性・操作一覧
//                                AttributesMethods = this.GetAttributesMethodsFromElement(element),
//                                // クラスフラグ
//                                classflg = true,
//                                // 削除権限フラグ
//                                CanDelete = false
//                            };
//
//                            this.commonCrossReferencs.Add(crossReference);
//                        }
//                    }
//                }
//            }
        }
        #endregion

        #region "イニシャル"
        /// <summary>
        /// イニシャル
        /// </summary>
        private void Init()
        {
            // 操作要素を選択する
//            this.repository.ShowInProjectView(this.method);

            // 相互参照一覧
            IList<CrossReference> taggedlCrossReferences = this.GetConnectionClassList();
            
//            // 相互参照のタグ付き値
//            EA.MethodTag crMethodTag = GetCrossReferenceMethodTag(this.method);

            // 古い相互参照要素
//            IList<Item> oldCrItem = XMLUtil.ConverMethodTagValueToItemList(crMethodTag.Notes);
			IList<Item> oldCrItem = new List<Item>();

            // 前回保存の操作の名前
            this.oldMethodName = this.method.name;
            // 前回の振る舞いを保存する
            this.oldBehaviorValue = this.method.behavior.Replace("\r\n", "\n");

            // 相互参照一覧のリソースを更新する
            this.relGrid.AutoGenerateColumns = false;
            this.relGridDataSource = new BindingList<CrossReference>();
            HashSet<string> guidSet = new HashSet<string>();

            // 共通要素
            foreach (CrossReference cr in this.commonCrossReferencs)
            {
                if (!guidSet.Contains(cr.ElementGUID))
                {
                    guidSet.Add(cr.ElementGUID);
                    this.relGridDataSource.Add(cr);
                }

            }

            // タグ付き値の要素
            foreach (CrossReference cr in taggedlCrossReferences)
            {
                if (!guidSet.Contains(cr.ElementGUID))
                {
                    guidSet.Add(cr.ElementGUID);
                    this.relGridDataSource.Add(cr);
                }
            }
            this.relGrid.DataSource = this.relGridDataSource;

            // 要素のキーワードを更新する
            this.UpdateBehaviorKeyWords();

            // 操作の名前
            this.txtMethodName.Text = this.method.name;

            // 振る舞いの最新値を更新する
            this.txtBehavior.Text = this.method.behavior.Replace("\r\n", "\n");
            this.txtBehavior.InvokeTextChange();
//            this.method.behavior = Behavior;

            // 右クリックメニューをバインディングする
            this.txtBehavior.ContextMenuStrip = this.behaviorMenuStrip;
            this.relGrid.ContextMenuStrip = this.CrossReferenceMenuStrip;
        }
        #endregion


        #region "要素のキーワードの更新"
        /// <summary>
        /// 要素のキーワードを更新する
        /// </summary>
        private void UpdateBehaviorKeyWords()
        {
            // 相互参照要素の名前を取得する
            this.txtBehavior.CrossReferenceList = this.relGridDataSource;
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
            ElementsXmlReader elemReader = new ElementsXmlReader();

            
            foreach(ConnectorVO convo in this.element.connectors ) {
            	CrossReference cref = new CrossReference();
            	cref.Name = convo.targetObjName;
                cref.ElementGUID = convo.targetObjGuid;
                
                ElementVO destObject = elemReader.readElementByGUID(cref.ElementGUID);
                
                // タイプ
                cref.Type = convo.connectionType;

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

//                            EA.Element ele = repository.GetElementByGuid(row.ElementGUID);
//                            repository.ShowInProjectView(ele);

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
        }

        /// <summary>
        /// 保存処理
        /// </summary>
        private void SavaMethodChange()
        {
            // 前回保存の操作の名前
            this.oldMethodName = this.method.name;
            // 前回の振る舞いを保存する
            this.oldBehaviorValue = this.txtBehavior.Text;

            // 名前
            this.method.name = this.txtMethodName.Text;
            // 振る舞い
            this.method.behavior = this.txtBehavior.Text.Replace("\n", "\r\n");

            // 相互参照のタグ付き値の値を更新する
            // EA.MethodTag crMethodTag = GetCrossReferenceMethodTag(this.method);
//            crMethodTag.Notes = XMLUtil.ConverItemListToMethodTagValue(this.txtBehavior.GetUsedCrossReference());
//            crMethodTag.Update();
//
//            this.method.Update();

//            // 操作要素を選択する
//            this.repository.ShowInProjectView(this.method);
        }
        #endregion

        #region "要素に切り替え"
        /// <summary>
        /// 要素に切り替え
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SwitchReference_Click(object sender, EventArgs e)
        {
//            // 選択行がない場合、キャンセルする。
//            if (this.relGrid.SelectedRows.Count < 0)
//            {
//                MessageBox.Show("選択してください。", "インフォ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }
//
//            DataGridViewRow selectedRow = this.relGrid.SelectedRows[0];
//
//            CrossReference cr = this.relGridDataSource[selectedRow.Index];
//            EA.Element selectedEmement = null;
//            if (cr.classflg)
//            {
//                // クラス要素の場合
//                selectedEmement = this.repository.GetElementByGuid(cr.ElementGUID);
//            }
//            else
//            {
//                // 関連線の場合
//                EA.Connector cnn = this.repository.GetConnectorByGuid(cr.ElementGUID);
//                if (cnn.SupplierID == this.method.ParentID)
//                {
//                    // ソース側の要素
//                    selectedEmement = this.repository.GetElementByID(cnn.ClientID);
//                }
//                else
//                {
//                    // ターゲット側の要素
//                    selectedEmement = this.repository.GetElementByID(cnn.SupplierID);
//                }
//            }
//
//            if (selectedEmement != null)
//            {
//                // 該当要素は存在する場合、プロジェクトブラウザ内で選択状態にする
//                this.repository.ShowInProjectView(selectedEmement);
//            }
        }
        #endregion

        #region "相互参照の削除"
        /// <summary>
        /// 相互参照のデータを削除する
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void DelCrossReference_Click(object sender, EventArgs e)
        {
//            // 選択行がない場合、キャンセルする。
//            if (this.relGrid.SelectedRows.Count < 0)
//            {
//                MessageBox.Show("選択してください。", "インフォ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }
//
//            DataGridViewSelectedRowCollection selectedRows = this.relGrid.SelectedRows;
//
//            // 選択した行のリソースを取得する
//            bool updateFlg = false;
//            foreach (DataGridViewRow selectedRow in selectedRows)
//            {
//                CrossReference cr = this.relGridDataSource[selectedRow.Index];
//                if (cr.CanDelete)
//                {
//                    updateFlg = true;
//
//                    // 選択した相互参照要素を削除する
//                    this.relGridDataSource.Remove(cr);
//                }
//            }
//
//            if (updateFlg)
//            {
//                // 要素のキーワードを更新する
//                this.UpdateBehaviorKeyWords();
//
//                this.txtBehavior.InvokeTextChange();
//            }
//            else
//            {
//                MessageBox.Show("削除できません。", "インフォ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }
        }
        #endregion

        #region "相互参照の追加"
        /// <summary>
        /// 相互参照を追加する
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void AddCrossReference_Click(object sender, EventArgs e)
        {
//            // 分類子の指定ダイアログ
//            int elementid = repository.InvokeConstructPicker("IncludedTypes=Class,Component;");
//
//            // 選択されなかった場合、処理をキャンセルする
//            if (elementid == 0)
//            {
//                return;
//            }
//
//            // 選択された要素
//            EA.Element element = repository.GetElementByID(elementid);
//
//            // 既に存在する場合、処理をキャンセルする
//            foreach (var row in this.relGridDataSource)
//            {
//                if (element.ElementGUID.Equals(row.ElementGUID))
//                {
//                    return;
//                }
//            }
//
//            // 選択された要素の情報を一覧に追加する
//            CrossReference crossReference = new CrossReference()
//            {
//                // GUID
//                ElementGUID = element.ElementGUID,
//                // 名前
//                Name = element.Name,
//                // タイプ
//                Type = element.Type,
//                // コメント
//                Notes = element.Notes,
//                // パッケージ
//                PackageName = repository.GetPackageByID(element.PackageID).Name,
//                // 属性・操作一覧
//                AttributesMethods = this.GetAttributesMethodsFromElement(element),
//                // クラスフラグ
//                classflg = true,
//                // 削除権限フラグ
//                CanDelete = true
//            };
//
//            // 参照をリソースに追加する
//            this.relGridDataSource.Add(crossReference);
//
//            // 要素のキーワードを更新する
//            this.UpdateBehaviorKeyWords();
//
//            this.txtBehavior.InvokeTextChange();
        }
        #endregion

        #region "相互参照グリッドのセルのダブルクリックイベント"
        /// <summary>
        /// 相互参照グリッドのセルのダブルクリックイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void relGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
//            if (e.ColumnIndex != 1)
//            {
//                // 名前コラムのセルではない場合、キャンセルする
//                return;
//            }
//
//            if (e.RowIndex >= 0 && e.RowIndex < this.relGridDataSource.Count)
//            {
//                // 選択した名前の内容を振る舞いテキストエリアにセットする
//                this.txtBehavior.SelectedText = this.relGridDataSource[e.RowIndex].Name;
//            }
        }
        #endregion

        #region "振る舞いの文字列処理"
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
                Clipboard.SetText(this.txtBehavior.SelectedText);
                this.txtBehavior.SelectedText = "";
                this.txtBehavior.InvokeTextChange();
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
                MessageBox.Show(ex.ToString());
            }
        	
        }

        /// <summary>
        /// 振る舞いの貼り付け処理
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Clipboard.GetText()))
            {
                this.txtBehavior.SelectedText = Clipboard.GetText();
                this.txtBehavior.InvokeTextChange();
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

            if (!this.oldBehaviorValue.Equals(this.txtBehavior.Text) || !this.oldMethodName.Equals(this.txtMethodName.Text))
            {
                DialogResult rst = MessageBox.Show("変更データを保存しますか？\n・はい：保存します。\n・いいえ：保存しません。\n・キャンセイル：画面に戻ります。", "保存確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (rst == DialogResult.Yes)
                {
                    // 保存する
                    this.SavaMethodChange();
                }
                else if (rst == DialogResult.No)
                {
                    // 何もしないで、画面を終了する
                }
                else if (rst == DialogResult.Cancel)
                {
                    this.isFormClosing = false;

                    // クローズイベントをキャンセルする
                    e.Cancel = true;
                }
            }

            if (this.isFormClosing)
            {
                this.SavaDisplayConfig();
            }
        }
        #endregion

        #region "画面初期表示配置の保存"
        /// <summary>
        /// 画面サイズを保存する
        /// </summary>
        private void SavaDisplayConfig()
        {
//            DisplayConfig displayConfig = new DisplayConfig();
//
//            displayConfig.ViewWidth = this.Width;
//            displayConfig.ViewHeight = this.Height;
//
//            displayConfig.ViewLocationX = this.Location.X;
//            displayConfig.ViewLocationY = this.Location.Y;
//
//            displayConfig.NameColumnWidth = this.relGrid.Columns["name"].Width;
//            displayConfig.TypeColumnWidth = this.relGrid.Columns["type"].Width;
//            displayConfig.ComentColumnWidth = this.relGrid.Columns["coment"].Width;
//            displayConfig.PackageColumnWidth = this.relGrid.Columns["package"].Width;
//
//            ConfigUtil.SaveInitConfigObject<DisplayConfig>(displayConfig, DISPLAY_CONFIG_FILENAME);
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

            // 相互参照パネルの最新の座標を更新する
            int crossRefPanelPointX = 10;
            int crossRefPanelPointY = 300 + (newFormSize.Height - minFormSize.Height);
            this.crossRefPanel.Location = new Point(crossRefPanelPointX, crossRefPanelPointY);

            // 相互参照パネルのサイズの変更
            this.crossRefPanel.Width = this.crossRefPanel.MinimumSize.Width + (newFormSize.Width - minFormSize.Width);
            this.crossRefPanel.Height = this.crossRefPanel.MinimumSize.Height;

            int btnSavePointX = 615 + (newFormSize.Width - minFormSize.Width);
            int btnSavePointY = 0;
            // 更新ボタンの最新の座標を更新する
            this.btnSave.Location = new Point(btnSavePointX, btnSavePointY);

            // 相互参照グリッドのサイズの変更
            this.relGrid.Width = this.relGrid.MinimumSize.Width + (newFormSize.Width - minFormSize.Width);
            this.relGrid.Height = this.relGrid.MinimumSize.Height;

            int btmBtnPanalPointX = 480 + (newFormSize.Width - minFormSize.Width);
            int btmBtnPanalPointY = 250;
            // 更新ボタンの最新の座標を更新する
            this.buttomButtonPanel.Location = new Point(btmBtnPanalPointX, btmBtnPanalPointY);
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

        #region "画面を閉じる"
        /// <summary>
        /// 画面を閉じ
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region "ヘルプ表示"
        /// <summary>
        /// ヘルプを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelp_Click(object sender, EventArgs e)
        {
//            ProjectConfig projectConfig = ConfigUtil.GetInitConfigObject<ProjectConfig>(PROJECT_CONFIG_FILENAME);
//
//            if (projectConfig != null)
//            {
//                ProcessStartInfo sInfo = new ProcessStartInfo(projectConfig.HelpUrl);
//                Process.Start(sInfo);
//            }
        }
        #endregion

        #region "ベースラインのマージ"
        /// <summary>
        /// ベースラインとマージする
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void btnMergeBaseline_Click(object sender, EventArgs e)
        {
            string baseLineGuid = string.Empty;
            string packageGuid = string.Empty;

//            EA.Project project = repository.GetProjectInterface();
//            EA.Element parentElement = this.repository.GetElementByID(this.method.ParentID);
//
//            // 親パッケージ（二層まで）をループする
//            int parentPackageId = parentElement.PackageID;
//            for (int i = 0; i < 2; i++)
//            {
//                // 親パッケージがない場合、終止する
//                if (parentPackageId == 0) break;
//
//                EA.Package parentPackage = this.repository.GetPackageByID(parentPackageId);
//
//                // 当該パッケージのベースラインを取得する
//                string curBaselines = project.GetBaselines(parentPackage.PackageGUID, this.repository.ConnectionString);
//                IList<Baseline> baseLines = BaselineUtil.ConvertXmlToBaseLineList(curBaselines);
//
//                if (baseLines.Count > 0)
//                {
//                    if (baseLines.Count == 1)
//                    {
//                        // ベースラインが一つの場合
//                        baseLineGuid = baseLines[0].guid;
//                        packageGuid = parentPackage.PackageGUID;
//                    }
//                    else
//                    {
//                        // ベースラインが複数の場合
//                        BaselineSelect baselineSelectView = new BaselineSelect(baseLines);
//                        baselineSelectView.ShowDialog();
//                        if (baselineSelectView.DialogResult == DialogResult.OK)
//                        {
//                            baseLineGuid = baselineSelectView.ResultValue;
//                            packageGuid = parentPackage.PackageGUID;
//                        }
//                        else
//                        {
//                            // ユーザキャンセル
//                            return;
//                        }
//                    }
//                    break;
//                }
//
//                // 親パッケージへ
//                parentPackageId = parentPackage.ParentID;
//            }
//
//            if (string.IsNullOrEmpty(baseLineGuid))
//            {
//                // ベースラインがない場合
//                MessageBox.Show("ベースライン情報が見つかりません。", "ベースライン", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }
//            else
//            {
//                // ベースの振る舞い
//                BaseLineBehavior baseLineBehavior = new BaseLineBehavior()
//                {
//                    State = false,
//                    Behavior = string.Empty
//                };
//
//                // ベースラインがある場合
//                string cmpRst = project.DoBaselineCompare(packageGuid, baseLineGuid, this.repository.ConnectionString);
//                ComparePackageResult comparePackageResult = CompareUtil.GetComparePackageResult(cmpRst);
//
//                // 差分結果がある場合
//                if (comparePackageResult.hasChanges)
//                {
//                    CompareItem compareItem = null; 
//                    if (this.method.MethodGUID.Equals(comparePackageResult.compareItem.guid))
//                    {
//                        compareItem = comparePackageResult.compareItem;
//                    }
//                    else
//                    {
//                        // 差分結果のサブアイテムがある場合
//                        IList<CompareItem> compareItems = comparePackageResult.compareItem.compareItems;
//                        if (compareItems != null)
//                        {
//                            compareItem = this.GetCompareItem(this.method.MethodGUID, compareItems);
//                        }
//                    }
//
//                    if (compareItem != null && compareItem.comparePropertys != null)
//                    {
//                        CompareProperty compareProperty = null;
//                        foreach (CompareProperty cp in compareItem.comparePropertys)
//                        {
//                            if (PROPERTY_NAME.Equals(cp.name))
//                            {
//                                // 振る舞いのプロパティの場合
//                                compareProperty = cp;
//                                break;
//                            }
//                        }
//
//                        if (compareProperty != null)
//                        {
//                            baseLineBehavior.State = true;
//                            baseLineBehavior.Behavior = compareProperty.baseline;
//                        }
//                    }
//                }
//
//                // 変更がない場合、現在の振る舞いをセットする
//                if (!baseLineBehavior.State) baseLineBehavior.Behavior = this.oldBehaviorValue;
//                baseLineBehavior.Behavior = baseLineBehavior.Behavior.Replace("\r\n", "\n");
//
//                if (!baseLineBehavior.State && this.oldBehaviorValue.Equals(this.txtBehavior.Text))
//                {
//                    // 比較結果の中にがない場合
//                    MessageBox.Show("振る舞いの変更がありません。", "ベースライン", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                }
//                else
//                {
//                    // 外部マージツールを起動して、マージ結果をコピーして、振る舞いを保存する
//                    string currentDataFile = string.Empty;
//                    string baseLineDataFile = string.Empty;
//
//                    try
//                    {
//                        // 一時比較用ファイルを作成する
//                        currentDataFile = FileUtil.SavaCurrentDataToFile(this.txtBehavior.Text);
//                        baseLineDataFile = FileUtil.SavaBaseLineDataToFile(baseLineBehavior.Behavior);
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show("一時ファイルの作成処理に異常を発生しました。\nメッセージ：" + ex.Message, "一時ファイル作成", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                        return;
//                    }
//
//                    // 実行の当前パス
//                    string runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//                    // マージツールのフルパース
//                    string mergerToolPath = runPath + Constants.MERGE_TOOL_FILENAME;
//
//                    if (File.Exists(mergerToolPath))
//                    {
//                        // マージツールを起動する
//                        ProcessStartInfo startInfo = new ProcessStartInfo(mergerToolPath)
//                        {
//                            Arguments = "/wr /e " + currentDataFile + " " + baseLineDataFile
//                        };
//
//                        Process process = Process.Start(startInfo);
//                    }
//                    else
//                    {
//                        // マージツールがセットされない場合
//                        MessageBox.Show("マージツールが配置されません。", "マージツール", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    }
//                }
//            }
        }

        /// <summary>
        /// 差分結果アイテムを取得する
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="compareItems">アイテム</param>
        /// <returns>アイテム</returns>
        private CompareItem GetCompareItem(string guid, IList<CompareItem> compareItems)
        {
            CompareItem compareItem = null;

            foreach (CompareItem ci in compareItems)
            {
                if (guid.Equals(ci.guid))
                {
                    // マッチの場合
                    compareItem = ci;
                    break;
                }

                if (ci.compareItems != null)
                {
                    // サブアイテム結果がある場合、再帰的にループする
                    compareItem = this.GetCompareItem(guid, ci.compareItems);
                    if (compareItem != null)
                    {
                        break;
                    }
                }
            }

            return compareItem;
        }
        #endregion
    }
}

