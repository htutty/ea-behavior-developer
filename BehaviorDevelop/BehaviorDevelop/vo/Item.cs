using System.Collections.Generic;

namespace BehaviorDevelop.vo
{
    /// <summary>
    /// 相互参照一覧の保存項目
    /// </summary>
    public class Item
    {
        /// <summary>
        /// 古い名前
        /// </summary>
        public string oldname { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// クラスフラグ
        /// 　ture：カラス
        /// 　false：関連
        /// </summary>
        public bool classflg { get; set; }

        /// <summary>
        /// 相互参照の属性・操作
        /// </summary>
        public IList<ElementVO> elements { get; set; }
    }
}