using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EaArtifactTool
{
    public partial class ArtifactToolStartForm : Form
    {
        public ArtifactToolStartForm()
        {
            InitializeComponent();
        }

        private void BtnSelectEapFile_Click(object sender, EventArgs e)
        {
            //はじめに「ファイル名」で表示される文字列を指定する
            this.openFileDialog1.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            this.openFileDialog1.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            this.openFileDialog1.Filter = "eapファイル(*.eap;*.eapx)|*.eap;*.eapx|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            this.openFileDialog1.FilterIndex = 1;
            //タイトルを設定する
            this.openFileDialog1.Title = "EAPファイルを選択してください";

            //ダイアログを表示する
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                this.TxtEapFile.Text = this.openFileDialog1.FileName;
            }

        }

        private void BtnOutputFolderSelect_Click(object sender, EventArgs e)
        {

            //上部に表示する説明テキストを指定する
            this.folderBrowserDialog1.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            // this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            this.folderBrowserDialog1.SelectedPath = @"C:\";
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            this.folderBrowserDialog1.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                TxtOutputFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }



        }

        private void BtnExecOutput_Click(object sender, EventArgs e)
        {

            if (TxtEapFile.Text == "")
            {
                MessageBox.Show("EAPファイルを入力してください");
                return;
            }

            if (TxtProjectName.Text == "")
            {
                MessageBox.Show("プロジェクト名を入力してください");
                return;
            }

            if (TxtOutputFolder.Text == "")
            {
                MessageBox.Show("出力フォルダを入力してください");
                return;
            }

            string arguments = TxtEapFile.Text + " " + TxtProjectName.Text + " " + TxtOutputFolder.Text;
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            System.Diagnostics.Process.Start(appPath + "\\ArtifactFileExporter.exe", arguments);

        }


        private void BtnSelectSrcProject_Click(object sender, EventArgs e)
        {
            //はじめに「ファイル名」で表示される文字列を指定する
            this.openFileDialog1.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            this.openFileDialog1.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            this.openFileDialog1.Filter = "bdprjファイル(*.bdprj)|*.bdprj|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            this.openFileDialog1.FilterIndex = 1;
            //タイトルを設定する
            this.openFileDialog1.Title = "成果物のプロジェクトファイルを選択してください";

            //ダイアログを表示する
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                this.TxtCmpSrcProject.Text = this.openFileDialog1.FileName;
            }
        }

        private void BtnSelectDestProject_Click(object sender, EventArgs e)
        {
            //はじめに「ファイル名」で表示される文字列を指定する
            this.openFileDialog1.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            this.openFileDialog1.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            this.openFileDialog1.Filter = "プロジェクトファイル(*.bdprj)|*.bdprj|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            this.openFileDialog1.FilterIndex = 1;
            //タイトルを設定する
            this.openFileDialog1.Title = "成果物のプロジェクトファイルを選択してください";

            //ダイアログを表示する
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                this.TxtCmpDestProject.Text = this.openFileDialog1.FileName;
            }
        }



        private void BtnSelectResultDir_Click(object sender, EventArgs e)
        {

            //上部に表示する説明テキストを指定する
            this.folderBrowserDialog1.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            // this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            this.folderBrowserDialog1.SelectedPath = @"C:\";
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            this.folderBrowserDialog1.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                TxtOutputFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }

        }

        private void BtnCompareStart_Click(object sender, EventArgs e)
        {

        }
    }
}
