/*
 * Created by SharpDevelop.
 * User: z2050275
 * Date: 08/29/2018
 * Time: 16:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using BDFileReader.vo;
using ElementEditor.util;
using ElementEditor.vo;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

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
                    behaviorEdit.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("XSHD読み込み処理でエラーが発生しました: " + ex.Message);
            }
            //イベントハンドラを登録
            behaviorEdit.TextArea.TextEntered += TextArea_TextEntered;
            behaviorEdit.TextArea.TextEntering += TextArea_TextEntering;

            // パラメータで取得した要素、メソッドを自オブジェクト内に保持
            element = elementVO;
            method = methodVO;
            oldBehaviorValue = methodVO.behavior;

            this.behaviorEdit.Text = methodVO.behavior;


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

            // Ctrl+スペース で入力補完Windowを表示する (クラスの一覧)
            if (e.Text == " " && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            { 
                //入力補完Windowを生成
                completionWindow = new CompletionWindow(behaviorEdit.TextArea);

                ElementSearcher elementSearcher = new ElementSearcher();

                // 補完リストに表示するアイテムをコレクションに追加する
                elementSearcher.searchCompletionDataByElement( element, completionWindow.CompletionList.CompletionData);

                // Windowを表示
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }

            // ピリオドを入力
            if (e.Text == ".")
            {
                //入力補完Windowを生成 (メソッド、属性の一覧)
                completionWindow = new CompletionWindow(behaviorEdit.TextArea);
                //補完リストに表示するアイテムをコレクションに追加する
                //---> ここは、編集内容に応じて適切な入力候補を追加するように書き換える！
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new CompletionData("item1", "item desc 1", null, 1.0, "foo"));
                data.Add(new CompletionData("item2", "item desc 2", null, 2.0, "bar"));
                data.Add(new CompletionData("item3", "item desc 3", null, 3.0, "hogehoge"));
                //<---
                //Windowを表示
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}