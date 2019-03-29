using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Xml;
using ElementEditor.viewmodel;

namespace ElementEditor
{
	/// <summary>
	/// Interaction logic for BehaviorEditor.xaml
	/// </summary>
	public partial class AttrMethSearch : Window
	{
        
        public AttrMethSearch()
		{
            this.DataContext = new AttrMethSearchViewModel();
            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}