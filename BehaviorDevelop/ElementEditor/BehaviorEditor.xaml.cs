using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Xml;
using ElementEditor.util;
using ArtifactFileAccessor.vo;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Windows.Input;

namespace ElementEditor
{
	/// <summary>
	/// Interaction logic for BehaviorEditor.xaml
	/// </summary>
	public partial class BehaviorEditor : Window
	{
        // Element, Methodオブジェクト（ツールの内部型）
        private ElementVO element = null;
        private MethodVO method = null;

        // 前回保存の操作の名前
        private string oldMethodName = string.Empty;

        /// <summary>
        /// 前回保存の振る舞いの値
        /// </summary>
        private string oldBehaviorValue = string.Empty;

        private CompletionWindow completionWindow;

        private AttrMethSearch attrMethSearch;

        private BehaviorEditor()
		{
			InitializeComponent();
		}

        public BehaviorEditor(ElementVO elementVO, MethodVO methodVO)
        {
            InitializeComponent();

            try
            {
                using (var reader = new XmlTextReader("jbdl.xshd"))
                {
                    jpBehaviorEdit.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("XSHD読み込み処理でエラーが発生しました: " + ex.Message);
            }

            //イベントハンドラを登録
            jpBehaviorEdit.TextArea.TextEntered += TextArea_TextEntered;
            jpBehaviorEdit.TextArea.TextEntering += TextArea_TextEntering;

            // パラメータで取得した要素、メソッドを自オブジェクト内に保持
            element = elementVO;
            method = methodVO;
            oldBehaviorValue = methodVO.behavior;

            this.jpBehaviorEdit.Text = methodVO.behavior;


        }

        //文字入力中
        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Console.WriteLine("TextArea_TextEntering: " + e.ToString());

            //補完Windowが開いている間
            if (e.Text.Length > 0 && completionWindow != null)
            {
                //英数字以外が入力された場合
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    //選択中のリストの項目をエディタに挿入する                
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // HandledはTrueにしない
            // e.Handled=true;
        }

        //文字入力後
        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Console.WriteLine("TextArea_TextEntered: " + e.ToString());

            CompletionHelper completionHelpler = new CompletionHelper();


            // Ctrl+スペース で入力補完Windowを表示する 
            // 自クラスが保持する属性・操作の一覧("this."付き)と、集約・参照・依存の接続先クラス名が対象
            if (e.Text == " " && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            { 
                //入力補完Windowを生成
                completionWindow = new CompletionWindow(jpBehaviorEdit.TextArea);

                // 補完リストに表示するアイテムをコレクションに追加する
                IList<ICompletionData> cmplList = completionHelpler.searchCompletionDataFromMyOwn(this.element);
                foreach(ICompletionData cmp in cmplList)
                {
                    completionWindow.CompletionList.CompletionData.Add(cmp);
                }

                // Windowを表示
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }

            // "$"入力で別Windowを表示する（全クラスから属性・メソッドを一覧検索する）
            if (e.Text == "$")
            {
                attrMethSearch = new AttrMethSearch();

                // Windowを表示
                attrMethSearch.Show();

            }

            // ピリオドを入力 -> キャレットの前のキーワードをクラス名とし、配下の属性・要素の一覧を表示する
            if (e.Text == ".")
            {
                string className = getClassNameFromText(jpBehaviorEdit.Text, jpBehaviorEdit.TextArea.Caret.Offset-1);

                Console.WriteLine("探しに行くクラス名 = " + className);

                // 補完リストに表示するアイテムをコレクションに追加する
                //IList<ICompletionData> cmplList = elementSearcher.searchCompletionDataByElement(this.element);

                //foreach (ICompletionData cmp in cmplList)
                //{
                //    completionWindow.CompletionList.CompletionData.Add(cmp);
                //}

                //// Windowを表示
                //completionWindow.Show();
                //completionWindow.Closed += delegate
                //{
                //    completionWindow = null;
                //};
            }
        }

        private string getClassNameFromText(string alltext, int offsetOnCaret)
        {

            // キャレットの前の文字列を全てchar配列に刻む
            char[] sb = alltext.Substring(0, offsetOnCaret).ToCharArray();
            StringBuilder namepick = new StringBuilder();

            // キャレット前の文字から順に1文字づつ前になめる
            for(int i=sb.Length-1; i>=0; i--)
            {
                // デリミタとして定めた文字(空白(半角、全角), '$', 改行)が来たらループを抜ける
                if( sb[i] == ' ' || sb[i] == '$' || sb[i] == '　' || sb[i] == '\n' || sb[i] == '、' || sb[i] == '。')
                {
                    break;
                }
                // それ以外ならクラス名となるバッファに格納
                else
                {
                    namepick.Insert(0, sb[i]);
                }
            }

            return namepick.ToString();
        }



        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveMethodChange();
            this.Close();
        }

        /// <summary>
        /// 保存処理
        /// </summary>
        private void SaveMethodChange()
        {

            // 振る舞い
            this.method.behavior = this.jpBehaviorEdit.Text;

            this.method.changed = 'U';
            this.element.changed = 'U';

            // ElementForm parentForm = (ElementForm)(this.Owner);
            // parentForm.repaintFormMethod(this.method);

        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
    }
}