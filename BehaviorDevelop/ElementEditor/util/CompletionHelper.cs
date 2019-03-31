using ICSharpCode.AvalonEdit.CodeCompletion;
using IndexAccessor;
using ArtifactFileAccessor.vo;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ElementEditor.vo;

namespace ElementEditor.util
{
    public class CompletionHelper
    {

        // リソースで管理されている属性アイコン、操作アイコンをWPFで使用するImageSourceに変換
        private ImageSource bitmapAttr;
        private ImageSource bitmapMth;

        public CompletionHelper()
        {
            bitmapAttr = convertBitmapToImageSource(Properties.Resources.ICON_SYMBOL_ATTRIBUTE);
            bitmapMth = convertBitmapToImageSource(Properties.Resources.ICON_SYMBOL_METHOD);

        }


        public IList<ICompletionData> searchCompletionDataByElement(ElementVO elementVO)
        {
            IList<ICompletionData> retDatas = new List<ICompletionData>();
            double priority = 1.0;

            // 自要素が保持する属性を取得して追加
            foreach (AttributeVO attr in elementVO.attributes)
            {
                string content = attr.name;
                string description = attr.notes;

                string text = attr.name;
                retDatas.Add(new CompletionData(content, description, this.bitmapAttr, priority++, text));
            }

            // 自要素が保持する操作を取得して追加
            foreach (MethodVO mth in elementVO.methods)
            {
                string content = mth.name;
                string description = mth.notes;

                string text = mth.name + "(" + getParameterString(mth.parameters) + ")";
                retDatas.Add(new CompletionData(content, description, this.bitmapMth, priority++, text));
            }

            return retDatas;
        }

        /// <summary>
        /// 自クラスから参照できる識別子を検索し、補完ウィンドウに出力する。
        /// </summary>
        /// <param name="elementVO"></param>
        /// <returns></returns>
        public IList<ICompletionData> searchCompletionDataFromMyOwn(ElementVO elementVO)
        {

            IList<ICompletionData> retDatas = new List<ICompletionData>();
            double priority = 1.0;

            // 自要素が保持する属性を取得して追加
            foreach (AttributeVO attr in elementVO.attributes)
            {
                string content = "this." + attr.name;
                string description = attr.notes;

                string text = "this." + attr.name;
                retDatas.Add(new CompletionData(content, description, bitmapAttr, priority++, text));
            }

            // 自要素が保持する操作を取得して追加
            foreach (MethodVO mth in elementVO.methods)
            {
                string content = "this." + mth.name;
                string description = mth.notes;

                string text = "this." + mth.name + "(" + getParameterString(mth.parameters) + ")";
                retDatas.Add(new CompletionData(content, description, bitmapMth, priority++, text));
            }

            // 自要素の接続先要素を全て抽出
            ConnectorSearcher connSearcher = new ConnectorSearcher();
            List<ConnectorVO> retConns = connSearcher.findByObjectGuid(elementVO.guid);
            foreach ( ConnectorVO cn in retConns)
            {
                string targetName = "";

                switch(cn.connectorType)
                {
                    // 依存線、関連線で自分が src側の場合は destのオブジェクト名を取得
                    case "Dependency":
                    case "Association":
                        if (cn.srcObjGuid == elementVO.guid) targetName = cn.destObjName;
                        break;

                    // 集約線で自分が dest側の場合は srcのオブジェクト名を取得
                    case "Aggregation":
                        if (cn.destObjGuid == elementVO.guid) targetName = cn.srcObjName;
                        break;

                }

                // 得られた名前が空でなかったら、候補として追加
                if( targetName != "")
                {
                    retDatas.Add(new CompletionData(targetName, targetName, null, priority++, targetName));
                }

            }

            return retDatas;
        }

        /// <summary>
        /// 自クラスから参照できる識別子を検索し、補完ウィンドウに出力する。
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public IList<ICompletionData> searchCompletionDataFromClassName(string className)
        {
            IList<ICompletionData> retDatas = new List<ICompletionData>();
            double priority = 1.0;

            // 自要素の接続先要素を全て抽出
            AttrMthSearcher attrMthSearcher = new AttrMthSearcher();
            List<AttrMthSearchItem> items = attrMthSearcher.findByClassName(className);
            foreach (AttrMthSearchItem amItem in items)
            {
                string content = amItem.attrMethName;
                string description = amItem.attrMethNotes;
                string text;

                if (amItem.attrMethFlg == "a")
                {
                    // 展開後の文字列
                    text = amItem.attrMethName;
                    retDatas.Add(new CompletionData(content, description, bitmapAttr, priority++, text));
                }
                else
                {
                    // 展開後の文字列 に "メソッド名 + ( パラメータ )" をセットする
                    text = "this." + amItem.attrMethName + "(" + amItem.methParameterDesc + ")";
                    retDatas.Add(new CompletionData(content, description, bitmapMth, priority++, text));
                }

            }

            return retDatas;
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
