using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public class DiagramObjectVO
    {
        public int diagramId { get; set; }
        public int objectId { get; set; }
        public int rectTop { get; set; }
        public int rectLeft { get; set; }
        public int rectRight { get; set; }
        public int rectBottom { get; set; }
        public int sequence { get; set; }
        public string objectStyle { get; set; }
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
        public int CompareTo(DiagramObjectVO o)
        {
            return (this.sequence - o.sequence);
        }

    }

}
