/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/06/04
 * Time: 14:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using VoidNish.Diff;
using BDFileReader.vo;

namespace BehaviorDevelop.util
{
	/// <summary>
	/// Description of ClassDocumentMaker.
	/// </summary>
	public class ClassDocumentMaker
	{
		StringBuilder diffBuffer;
		
		public ClassDocumentMaker()
		{
		}
		
        public void make(ElementVO changedElement, string docFilename)
        {
        	
			Document document  = null;
			Word.Application word = null;
        	
            try
            {
                // Word アプリケーションオブジェクトを作成
                word = new Word.Application();
                // Word の GUI を起動しないようにする
                word.Visible = false;

                // 新規文書を作成
                document = word.Documents.Add();

                // ヘッダーを編集
                editHeaderSample(ref document, 10, WdColorIndex.wdPink, "Header Area");

                // フッターを編集
                editFooterSample(ref document, 10, WdColorIndex.wdBlue, "Footer Area");

                // クラスの見出しを追加
                addHeadingClass(ref document, changedElement);

                // パラグラフを追加
                document.Content.Paragraphs.Add();

                foreach( AttributeVO a in changedElement.attributes ) {
                	outputAttribute(ref document, a);
                }

                foreach( MethodVO m in changedElement.methods ) {
                	outputMethod(ref document, m);
                }
                
                // 名前を付けて保存
                object filename = docFilename;
                document.SaveAs2(ref filename);

                Console.WriteLine("Document created successfully !");
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
            	// 文書を閉じる
            	if( document != null) {
	                document.Close();
	                document = null;
            	}

            	if( word != null ) {
	                word.Quit();
	                word = null;
            	}
            }
            
        }

        
        /// <summary>
        /// 文書のヘッダーを編集する.
        /// </summary>
        private void editHeaderSample(ref Document document, int fontSize, WdColorIndex color, string text)
        {
            foreach (Section section in document.Sections)
            {
                //Get the header range and add the header details.
                Range headerRange = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                headerRange.Fields.Add(headerRange, WdFieldType.wdFieldPage);
                headerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                headerRange.Font.ColorIndex = color;
                headerRange.Font.Size = fontSize;
                headerRange.Text = text;
            }
        }

        /// <summary>
        /// 文書のフッターを編集する.
        /// </summary>
        private void editFooterSample(ref Document document, int fontSize, WdColorIndex color, string text)
        {
            foreach (Section wordSection in document.Sections)
            {
                //Get the footer range and add the footer details.
                Range footerRange = wordSection.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                footerRange.Font.ColorIndex = color;
                footerRange.Font.Size = fontSize;
                footerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                footerRange.Text = text;
            }
        }

        /// <summary>
        /// 文書に見出しを追加する.
        /// </summary>
        private void addHeadingSample(ref Document document, string text)
        {
            Paragraph para = document.Content.Paragraphs.Add(System.Reflection.Missing.Value);
            object styleHeading1 = "見出し 1";
            para.Range.set_Style(ref styleHeading1);
            para.Range.Text = text;
            para.Range.InsertParagraphAfter();
        }

        
        /// <summary>
        /// クラス名(別名)で見出しを追加する.
        /// </summary>
        private void addHeadingClass(ref Document document, ElementVO origElement)
        {
            Paragraph para = document.Content.Paragraphs.Add(System.Reflection.Missing.Value);
            object styleHeading1 = "見出し 1";
            para.Range.set_Style(ref styleHeading1);
            para.Range.Text = origElement.name + " [" + origElement.alias + "]";
            para.Range.InsertParagraphAfter();
        }
        
        
        /// <summary>
        /// 文書の末尾にテキストを追加する.
        /// </summary>
        private void addTextSample(ref Document document, WdColorIndex color, string text)
        {
            int before = getLastPosition(ref document);
            Range rng = document.Range(document.Content.End - 1, document.Content.End - 1);
            rng.Text += text;
            int after = getLastPosition(ref document);

            document.Range(before, after).Font.ColorIndex = color;
        }

        /// <summary>
        /// 文書の末尾に属性の情報を出力する.
        /// </summary>
        private void outputAttribute(ref Document document, AttributeVO changedAttr)
        {
            // パラグラフを追加
            document.Content.Paragraphs.Add();

            addAttributeText(ref document, WdColorIndex.wdBlack, "□属性: " + changedAttr.name + "[" + changedAttr.alias + "]" + "\r\n");
        	addAttributeText_detail(ref document, WdColorIndex.wdBlack, "ID: " + changedAttr.guid + "\r\n" );
        	addAttributeText_detail(ref document, WdColorIndex.wdBlack, changedAttr.notes + "\r\n" );
        }
        
        /// <summary>
        /// 文書の末尾に属性テキストを追加する.
        /// </summary>
        private void addAttributeText(ref Document document, WdColorIndex color, string text)
        {
            int before = getLastPosition(ref document);
            Range rng = document.Range(document.Content.End - 1, document.Content.End - 1);
            rng.Text += text;
            int after = getLastPosition(ref document);

            document.Range(before, after).Font.ColorIndex = color;
            document.Range(before, after).Font.Size = 9;            
        }
        
        /// <summary>
        /// 文書の末尾に属性テキストを追加する.
        /// </summary>
        private void addAttributeText_detail(ref Document document, WdColorIndex color, string text)
        {
            int before = getLastPosition(ref document);
            Range rng = document.Range(document.Content.End - 1, document.Content.End - 1);
            rng.Text += text;
            int after = getLastPosition(ref document);

            document.Range(before, after).Font.ColorIndex = color;
            document.Range(before, after).Font.Size = 8;
        }
        
        
      /// <summary>
        /// 文書の末尾に属性テキストを追加する.
        /// </summary>
        private void addAttributeText_Deleted(ref Document document, string text)
        {
            int before = getLastPosition(ref document);
            Range rng = document.Range(document.Content.End - 1, document.Content.End - 1);
            rng.Text += text;
            int after = getLastPosition(ref document);

            document.Range(before, after).Font.ColorIndex = WdColorIndex.wdRed ;
            document.Range(before, after).Font.Size = 8;
            document.Range(before, after).Font.StrikeThrough = 1;
        }
        
        
        /// <summary>
        /// 文書の末尾に属性の情報を出力する.
        /// </summary>
        private void outputMethod(ref Document document, MethodVO changedMethod)
        {
        	// パラグラフを追加
            document.Content.Paragraphs.Add();

        	addAttributeText(ref document, WdColorIndex.wdBlack, "■操作: " + changedMethod.name + "[" + changedMethod.alias + "]" + "\r\n");
        	addAttributeText_detail(ref document, WdColorIndex.wdBlack, "ID: " + changedMethod.guid + "\r\n");
        	addAttributeText_detail(ref document, WdColorIndex.wdBlack, changedMethod.notes + "\r\n" );
        	
        	this.diffBuffer = new StringBuilder();
			
			string leftText, rightText;
			leftText = changedMethod.srcMethod.behavior;
			rightText = changedMethod.destMethod.behavior;
			var simpleDiff = new SimpleDiff<string>(leftText.Split('\n'), rightText.Split('\n'));
			simpleDiff.LineUpdate += new EventHandler<DiffEventArgs<string>> (BehaviorDiff_LineUpdate);
			simpleDiff.RunDiff();
        	
        	// パラグラフを追加
        	document.Content.Paragraphs.Add();
        	addAttributeText(ref document, WdColorIndex.wdBlack, "[ふるまい]------------------" + "\r\n" );
        	
       		outputTextWithDiffMarked(ref document, diffBuffer.ToString().Split('\n'));

       		addAttributeText(ref document, WdColorIndex.wdBlack, "------------------" + "\r\n" );
        }
        
        void outputTextWithDiffMarked(ref Document document, string[] markedTextList ) {
        	foreach( string txt in markedTextList ) {
        		if ( txt.Length > 0 ) {
	        		string mark = txt.Substring(0,1);
	        		
	        		switch( mark ) {
	        			case "-":
	        				addAttributeText_Deleted(ref document, txt + "\n" );
	        				break;
	        			case "+":
	        				addAttributeText_detail(ref document, WdColorIndex.wdBlue, txt + "\n" );
	        				break;
	        			default:
	        				addAttributeText_detail(ref document, WdColorIndex.wdBlack, txt + "\n" );
	        				break;
	        		}
        		}
        		
        	}
        	
        	
        }
        
        
        /// <summary>
        /// 文書の末尾位置を取得する.
        /// </summary>
        /// <returns></returns>
        private static int getLastPosition(ref Document document)
        {
            return document.Content.End - 1;
        }

        
        
        private void BehaviorDiff_LineUpdate(object sender, DiffEventArgs<string> e)
        {
            String indicator = " ";
            switch (e.DiffType)
            {
                case DiffType.Add:
                    indicator = "+";
                    break;

                case DiffType.Subtract:
                    indicator = "-";
                    break;
               
                default:
                    indicator = " ";
		            break;
            }

            this.diffBuffer.Append(indicator + e.LineValue + "\n");
//            Console.WriteLine("{0}{1}", indicator, e.LineValue);
        }
        
        
        
	}
}
