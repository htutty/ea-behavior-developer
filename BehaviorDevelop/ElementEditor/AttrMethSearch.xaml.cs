using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using IndexAccessor;

namespace ElementEditor
{
	/// <summary>
	/// Interaction logic for BehaviorEditor.xaml
	/// </summary>
	public partial class AttrMethSearch : Window
	{
        private ViewModel viewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AttrMethSearch()
		{
            InitializeComponent();

            this.viewModel = new ViewModel();
            this.DataContext = this.viewModel;
            this.attrMethSearchResultList.ItemsSource = viewModel.AttrMethItems;
        }

        /// <summary>
        /// コンストラクタ(デフォルト文字列付き)
        /// </summary>
        public AttrMethSearch(string defaultKeyword)
        {
            InitializeComponent();

            this.viewModel = new ViewModel();
            this.DataContext = this.viewModel;

            this.txtKeyword.Text = defaultKeyword;
        }

        /// <summary>
        /// 検索ボタンクリック時のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtKeyword.Text != "")
            {
                //マウスカーソルを砂時計にする
                this.Cursor = Cursors.Wait;

                // テキストで入力されたキーワードをViewModelの検索機能に渡し、結果を反映する
                string keyword = txtKeyword.Text;
                this.viewModel.searchAttrMethFromIndex(keyword);
                this.attrMethSearchResultList.ItemsSource = viewModel.AttrMethItems;

                //マウスカーソルを矢印に戻す
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                MessageBox.Show("検索キーワードを何か入力してください（要素名、属性・操作名の両方の部分一致で検索します）");
            }
        }

        /// <summary>
        /// OKボタン押下時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            // this.SaveMethodChange();

            int RowCnt = attrMethSearchResultList.Items.IndexOf(attrMethSearchResultList.SelectedItem);
            AttrMthSearchItem nowRow = viewModel.AttrMethItems[RowCnt];

            string appendString = "";
            if ( nowRow.attrMethFlg == "m" )
            {
                if( nowRow.methParameterDesc != null )
                {
                    appendString = nowRow.elemName + "." + nowRow.attrMethName + "(" + nowRow.methParameterDesc + ")" ;
                }
                else
                {
                    appendString = nowRow.elemName + "." + nowRow.attrMethName + "( )" ;
                }
            }
            else
            {
                appendString = nowRow.elemName + "." + nowRow.attrMethName;
            }


            // 親画面(BehaviorEditor)から呼ばれた場合
            if( this.Owner != null )
            {
                BehaviorEditor parent = (BehaviorEditor)this.Owner;
                parent.insertTextOnCaret(appendString);
            }

            this.Close();
        }

        /// <summary>
        /// キャンセルボタン押下時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }




}