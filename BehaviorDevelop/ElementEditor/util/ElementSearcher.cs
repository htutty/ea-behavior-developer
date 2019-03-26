using BDFileReader.vo;
using ElementEditor.vo;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ElementEditor.util
{
    class ElementSearcher
    {
        public void searchCompletionDataByElement(ElementVO elementVO, IList<ICompletionData> completionDatas)
        {
            IList<ICompletionData> data = completionDatas;
            double priority = 1.0;

            // リソースで管理されている属性アイコン、操作アイコンをWPFで使用するImageSourceに変換
            //var bitmapAttr = convertBitmapToImageSource(Properties.Resources.ICON_SYMBOL_ATTRIBUTE);
            //var bitmapMth = convertBitmapToImageSource(Properties.Resources.ICON_SYMBOL_METHOD);

            BitmapImage bitmapAttr = new BitmapImage(new Uri("Resources/ICON_SYMBOL_ATTRIBUTE.png", UriKind.Relative));
            BitmapImage bitmapMth = new BitmapImage(new Uri("Resources/ICON_SYMBOL_METHOD.png", UriKind.Relative));

            // 自要素が保持する属性を取得して追加
            foreach (AttributeVO attr in elementVO.attributes)
            {
                string content = attr.name;
                string description = attr.notes;

                string text = attr.name;
                data.Add(new CompletionData(content, description, bitmapAttr, priority++, text));
            }

            // 自要素が保持する操作を取得して追加
            foreach (MethodVO mth in elementVO.methods)
            {
                string content = mth.name;
                string description = mth.notes;

                string text = mth.name + "(" + getParameterString(mth.parameters) + ")";
                data.Add(new CompletionData(content, description, bitmapMth, priority++, text));
            }

            return;
        }


        private string getParameterString(List<ParameterVO> parameters)
        {
            string paramStr = "";

            foreach(ParameterVO param in parameters)
            {
                if( paramStr != "" )
                {
                    paramStr = paramStr + ", ";
                }
                paramStr = paramStr + "["+ param.name + ":" + param.eaType + "]";
            }

            return paramStr;
        }


        public IList<ICompletionData> searchCompletionDataByString(string keyword, IList<ICompletionData> completionDatas)
        {
            IList<ICompletionData> data = new List<ICompletionData>();

            data.Add(new CompletionData("foo", "item desc 1", null, 1.0, "foo"));
            data.Add(new CompletionData("bar", "item desc 2", null, 2.0, "bar"));
            data.Add(new CompletionData("item3", "item desc 3", null, 3.0, "hogehoge"));

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap">変換元の画像(Bitmap型)</param>
        /// <returns></returns>
        private ImageSource convertBitmapToImageSource(Bitmap bitmap)
        {
            // MemoryStreamを利用した変換処理
            using (var ms = new System.IO.MemoryStream())
            {
                // MemoryStreamに書き出す
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                // MemoryStreamをシーク
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                // MemoryStreamからBitmapFrameを作成
                // (BitmapFrameはBitmapSourceを継承しているのでそのまま渡せばOK)
                System.Windows.Media.Imaging.BitmapSource bitmapSource =
                    System.Windows.Media.Imaging.BitmapFrame.Create(
                        ms,
                        System.Windows.Media.Imaging.BitmapCreateOptions.None,
                        System.Windows.Media.Imaging.BitmapCacheOption.OnLoad
                    );

                return bitmapSource;
            }
        }


    }
}
