using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElementEditor.util;

namespace ElementEditor.usercontrol.vo
{
    /// <summary>
    /// 段落番号のクラス
    /// </summary>
    public class HeaderNumber
    {
        /// <summary>
        /// 段落番号
        /// </summary>
        private IList<int> HeaderNumberList { get; set; }

        /// <summary>
        /// 段落番号のディープ
        /// </summary>
        public int HeaderNumberDeep {
            get
            {
                if (HeaderNumberList == null)
                {
                    return -1;
                }
                return HeaderNumberList.Count - 1;
            }
        }

        /// <summary>
        /// 最後位置の段落番号を取得する
        /// </summary>
        /// <returns>段落番号</returns>
        public int LastHeaderNumber()
        {
            return this.IndexHeaderNumber(HeaderNumberList.Count - 1);
        }

        /// <summary>
        /// 指定位置の段落番号を取得する
        /// </summary>
        /// <param name="index">位置</param>
        /// <returns>段落番号</returns>
        public int IndexHeaderNumber(int index)
        {
            return HeaderNumberList[index];
        }

        /// <summary>
        /// 指定位置の段落番号を更新する
        /// </summary>
        /// <param name="deep">段落番号のディープ</param>
        /// <param name="number">段落番号</param>
        public void UpdateHeaderNumber(int deep, int number)
        {
            HeaderNumberList[deep] = number;
        }

        /// <summary>
        /// 段落番号を取得する
        /// </summary>
        /// <returns></returns>
        public string GetHeaderNumber()
        {
            if (HeaderNumberList == null || HeaderNumberList.Count == 0)
            {
                return string.Empty;
            }

            string rlt = new string(' ', HeaderNumberList.Count - 1);

            rlt += string.Join(".", HeaderNumberList);

            return StringUtil.HanToZen(rlt);
        }

        /// <summary>
        /// 段落番号をセットする
        /// </summary>
        /// <returns></returns>
        public void SetHeaderNumber(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }

            HeaderNumberList = new List<int>();

            
            str = StringUtil.ZenToHan(str).Trim();
//            str =  StringUtil.HanToZen(str).Trim();
            // str = Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Narrow, 0x411);
            
            var splitStrs = str.Split('.');
            foreach (string splitStr in splitStrs)
            {
                HeaderNumberList.Add(Convert.ToInt32(splitStr));
            }
        }
    }
}
