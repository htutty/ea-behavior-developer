using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ElementEditor.vo
{
    public class CompletionData : ICompletionData
    {
        //入力候補一覧(UI)に表示する内容
        public object Content { get; set; }
        //ツールチップに表示する説明文
        public object Description { get; set; }
        //入力候補の左側に表示するアイコン
        public ImageSource Image { get; set; }
        //表示順の優先度？
        public double Priority { get; set; }
        //アイテム選択時に挿入される文字列
        public string Text { get; set; }

        //アイテム選択後の処理
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">入力候補一覧(UI)に表示する内容</param>
        /// <param name="Description">ツールチップに表示する説明文</param>
        /// <param name="image">入力候補の左側に表示するアイコン</param>
        /// <param name="Priority">表示順</param>
        /// <param name="Text">アイテム選択時に挿入される文字列</param>
        public CompletionData(object content, object description, ImageSource image, double priority, string text)
        {
            this.Content = content;
            this.Description = description;
            this.Image = image;
            this.Priority = priority;
            this.Text = text;
        }

    }
}
