using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDFileReader.vo
{
    public class DiagramLinkVO
    {
        public int diagramId { get; set; }
        public int connectorId { get; set; }
        public string geometry { get; set; }
        public string style { get; set; }
        public bool hidden { get; set; }
        public string path { get; set; }
        public int instanceId { get; set; }

        /// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

        /// <summary>
        /// ソートで呼び出される比較処理
        /// </summary>
        /// <param name="o">比較対象</param>
        /// <returns></returns>
        public int CompareTo(DiagramLinkVO o)
        {
            return (this.instanceId - o.instanceId);
        }
    }
}
