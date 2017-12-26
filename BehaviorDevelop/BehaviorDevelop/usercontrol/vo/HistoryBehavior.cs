using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDevelop.util;

namespace BehaviorDevelop.usercontrol.vo
{
    /// <summary>
    /// 段落番号のクラス
    /// </summary>
    public class HistoryBehavior
    {
        /// <summary>
        /// 選択の開始位置
        /// </summary>
        public int SelectionStart { get; set; }

        /// <summary>
        /// テキスト
        /// </summary>
        public  string Text { get; set; }
    }
}
