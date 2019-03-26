using System.Collections.Generic;

namespace ElementEditor.vo
{
    /// <summary>
    /// アイテム差分結果
    /// </summary>
    public class CompareItem
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// タイプ
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// 変更状態
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 属性の差分結果
        /// </summary>
        public IList<CompareProperty> comparePropertys { get; set; }

        /// <summary>
        /// アイテム差分結果
        /// </summary>
        public IList<CompareItem> compareItems { get; set; }
    }
}