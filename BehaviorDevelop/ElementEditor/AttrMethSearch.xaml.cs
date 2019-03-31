using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
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

        public AttrMethSearch()
		{
//            ProjectSetting.load(@"D:\DesignHistory\aswea_20200401\project.bdprj");

            InitializeComponent();

            this.viewModel = new ViewModel();
            this.DataContext = this.viewModel;
            this.attrMethSearchResultList.ItemsSource = viewModel.AttrMethItems;
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
                string keyword = txtKeyword.Text;
                //AttrMethSearcher search = new AttrMethSearcher();
                //List <AttrMethItem> list = search.findByKeyword(keyword);

                this.viewModel.searchAttrMethFromIndex(keyword);

                this.attrMethSearchResultList.ItemsSource = viewModel.AttrMethItems;
            }
        }


        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            // this.SaveMethodChange();

            int RowCnt = attrMethSearchResultList.Items.IndexOf(attrMethSearchResultList.SelectedItem);
            AttrMthSearchItem nowRow = viewModel.AttrMethItems[RowCnt];
            string apendString = nowRow.elemName + "." + nowRow.attrMethName;
            // MessageBox.Show("now=" + apendString);

            if( this.Owner != null )
            {
                BehaviorEditor parent = (BehaviorEditor)this.Owner;
                parent.insertTextOnCaret(apendString);
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

    }
}