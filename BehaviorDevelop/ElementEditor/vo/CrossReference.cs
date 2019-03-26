using System.Collections.Generic;

namespace ElementEditor.vo
{
    /// <summary>
    /// 相互参照要素の情報クラス
    /// </summary>
    public class CrossReference
    {
        /// <summary>
        /// GUID
        /// </summary>
        public string ElementGUID { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// タイプ
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// パッケージ
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 属性・操作一覧
        /// </summary>
        public IList<AttributeMethod> AttributesMethods { get; set; }

        /// <summary>
        /// クラスフラグ
        /// 　ture：カラス
        /// 　false：関連
        /// </summary>
        public bool classflg { get; set; }

        /// <summary>
        /// 削除権限フラグ
        /// </summary>
        public bool CanDelete { get; set; }
    }
}
