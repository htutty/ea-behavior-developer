using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System;

namespace BehaviorDevelop.util
{
    /// <summary>
    /// 文字列共通処理
    /// </summary>
    public static class StringUtil
    {
        #region "定数"
        /// <summary>
        /// 全角ドット：[．]
        /// </summary>
        public const string FULLWIDTH_DOT= "．";

        /// <summary>
        /// 全角スペース：[　]
        /// </summary>
        public const string FULLWIDTH_SPACE = "　";

        /// <summary>
        /// 段落番号の正規表現パターン
        /// </summary>
        private const string HEADER_NUMBER_PATTERN = "^[　]*[１２３４５６７８９０]+(．[１２３４５６７８９０]+)*";
        #endregion

        #region "WIN32API:LCMapStringW"
        /// <summary>
        /// WIN32API:LCMapStringWの宣言
        /// </summary>
        /// <param name="Locale">カルチャ</param>
        /// <param name="dwMapFlags">マップフラグ</param>
        /// <param name="lpSrcStr">変換文字列</param>
        /// <param name="cchSrc">変換文字列の長さ</param>
        /// <param name="lpDestStr">変換結果</param>
        /// <param name="cchDest">変換結果の長さ</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern private int LCMapStringW(int Locale, uint dwMapFlags,
            [MarshalAs(UnmanagedType.LPWStr)]string lpSrcStr, int cchSrc,
            [MarshalAs(UnmanagedType.LPWStr)]string lpDestStr, int cchDest);
        #endregion

        #region "マップフラグ"
        /// <summary>
        /// マップフラグ
        /// </summary>
        public enum dwMapFlags : uint
        {
            NORM_IGNORECASE = 0x00000001,           // 大文字と小文字を区別しません。
            NORM_IGNORENONSPACE = 0x00000002,       // 送りなし文字を無視します。このフラグをセットすると、日本語アクセント文字も削除されます。
            NORM_IGNORESYMBOLS = 0x00000004,        // 記号を無視します。
            LCMAP_LOWERCASE = 0x00000100,           // 小文字を使います。
            LCMAP_UPPERCASE = 0x00000200,           // 大文字を使います。
            LCMAP_SORTKEY = 0x00000400,             // 正規化されたワイド文字並び替えキーを作成します。
            LCMAP_BYTEREV = 0x00000800,             // Windows NT のみ : バイト順序を反転します。たとえば 0x3450 と 0x4822 を渡すと、結果は 0x5034 と 0x2248 になります。
            SORT_STRINGSORT = 0x00001000,           // 区切り記号を記号と同じものとして扱います。
            NORM_IGNOREKANATYPE = 0x00010000,       // ひらがなとカタカナを区別しません。ひらがなとカタカナを同じと見なします。
            NORM_IGNOREWIDTH = 0x00020000,          // シングルバイト文字と、ダブルバイトの同じ文字とを区別しません。
            LCMAP_HIRAGANA = 0x00100000,            // ひらがなにします。
            LCMAP_KATAKANA = 0x00200000,            // カタカナにします。
            LCMAP_HALFWIDTH = 0x00400000,           // 半角文字にします（適用される場合）。
            LCMAP_FULLWIDTH = 0x00800000,           // 全角文字にします（適用される場合）。
            LCMAP_LINGUISTIC_CASING = 0x01000000,   // 大文字と小文字の区別に、ファイルシステムの規則（既定値）ではなく、言語上の規則を使います。LCMAP_LOWERCASE、または LCMAP_UPPERCASE とのみ組み合わせて使えます。
            LCMAP_SIMPLIFIED_CHINESE = 0x02000000,  // 中国語の簡体字を繁体字にマップします。
            LCMAP_TRADITIONAL_CHINESE = 0x04000000, // 中国語の繁体字を簡体字にマップします。
        }
        #endregion

        #region "文字列の変換"
        /// <summary>
        /// 文字列を変換する
        /// </summary>
        /// <param name="str">変換文字列</param>
        /// <param name="flags">フラグ</param>
        /// <returns>変換結果</returns>
        public static string StringConvert(this string str, dwMapFlags flags)
        {
            var ci = CultureInfo.CurrentCulture;
            string result = new string(' ', str.Length);
            LCMapStringW(ci.LCID, (uint)flags, str, str.Length, result, result.Length);
            return result;
        }
        #endregion

        #region "半角⇒全角"
        /// <summary>
        /// 半角を全角に変換
        /// </summary>
        /// <param name="str">変換文字列</param>
        /// <returns>変換結果</returns>
        public static string HanToZen(this string str)
        {
            return StringConvert(str, dwMapFlags.LCMAP_FULLWIDTH);
        }
        #endregion

        #region "全角⇒半角"
        /// <summary>
        /// 全角を半角に変換
        /// </summary>
        /// <param name="str">変換文字列</param>
        /// <returns>変換結果</returns>
        public static string ZenToHan(this string str)
        {
            return StringConvert(str, dwMapFlags.LCMAP_HALFWIDTH | dwMapFlags.LCMAP_KATAKANA);
        }
        #endregion

        #region "段落番号の取得"
        /// <summary>
        /// 段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static string GetHeaderNumber(this string str)
        {
            string rlt = string.Empty;

            MatchCollection mc = Regex.Matches(str, HEADER_NUMBER_PATTERN);
            foreach (Match m in mc)
            {
                rlt = m.Value;
            }

            return rlt;
        }
        #endregion

        #region "サブ段落番号の取得"
        /// <summary>
        /// サブ段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static string GetSubHeaderNumber(this string str)
        {
            string rlt = GetHeaderNumber(str);

            if (!string.IsNullOrEmpty(rlt))
            {
                rlt += "．１";
            }

            return rlt;
        }
        #endregion

        #region "前の段落番号の取得"
        /// <summary>
        /// 前の段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static string GetPreHeaderNumber(this string str)
        {
            string rlt = GetHeaderNumber(str);

            if (!string.IsNullOrEmpty(rlt))
            {
                int lastIndexOfDot = rlt.LastIndexOf(FULLWIDTH_DOT);
                short lastNumber = Convert.ToInt16(ZenToHan(rlt.Substring(lastIndexOfDot + 1)));
                rlt = rlt.Substring(0, lastIndexOfDot + 1) + HanToZen((lastNumber - 1).ToString());
            }

            return rlt;
        }
        #endregion

        #region "次の段落番号の取得"
        /// <summary>
        /// 次の段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static string GetNextHeaderNumber(this string str)
        {
            string rlt = GetHeaderNumber(str);

            if (!string.IsNullOrEmpty(rlt))
            {
                int lastIndexOfDot = rlt.LastIndexOf(FULLWIDTH_DOT);
                short lastNumber = Convert.ToInt16(ZenToHan(rlt.Substring(lastIndexOfDot + 1)));
                rlt = rlt.Substring(0, lastIndexOfDot + 1) + HanToZen((lastNumber + 1).ToString());
            }

            return rlt;
        }
        #endregion

        #region "親の段落番号の取得"
        /// <summary>
        /// 親の段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static string GetParentHeaderNumber(this string str)
        {
            string rlt = GetHeaderNumber(str);

            if (!string.IsNullOrEmpty(rlt) && rlt.IndexOf(FULLWIDTH_DOT) != -1)
            {
                rlt = rlt.Substring(0, rlt.LastIndexOf(FULLWIDTH_DOT));

                if (rlt.StartsWith(FULLWIDTH_SPACE))
                {
                    rlt = rlt.Substring(1);
                }
            }
            else
            {
                rlt = string.Empty;
            }

            return rlt;
        }
        #endregion

        #region "親の次の段落番号の取得"
        /// <summary>
        /// 親の次の段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static string GetParentNextHeaderNumber(this string str)
        {
            string rlt = GetHeaderNumber(str);

            if (!string.IsNullOrEmpty(rlt) && rlt.IndexOf(FULLWIDTH_DOT) != -1)
            {
                rlt = rlt.Substring(0, rlt.LastIndexOf(FULLWIDTH_DOT));

                rlt = GetNextHeaderNumber(rlt);

                if (rlt.StartsWith(FULLWIDTH_SPACE))
                {
                    rlt = rlt.Substring(1);
                }
            }
            else
            {
                rlt = string.Empty;
            }

            return rlt;
        }
        #endregion

        #region "親の次の段落番号の取得"
        /// <summary>
        /// 親の次の段落番号を取得する
        /// </summary>
        /// <param name="str">取得先文字列</param>
        /// <returns>段落番号</returns>
        public static bool IsRootHeaderNumber(this string str)
        {
            bool rlt = false;

            string parentHeaderNumber = GetParentHeaderNumber(str);

            if (string.IsNullOrEmpty(parentHeaderNumber))
            {
                rlt = true;
            }

            return rlt;
        }
        #endregion
    }
}
