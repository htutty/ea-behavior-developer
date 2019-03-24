using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    /// <summary>
    /// ふるまいのチャンクを保持する。
    /// ここで言うチャンクはふるまいテキストの１行を表す。
    /// </summary>
    public class BehaviorChunk
    {
        /// <summary>チャンクのID</summary>
        public int chunkId { get; set; }

        /// <summary>
        /// 操作のid
        /// </summary>
        public int methodId { get; set; }

        /// <summary>
        /// 振る舞いの中の行番号
        /// </summary>
        public int pos { get; set; }

        /// <summary>
        /// このチャンクの次処理を示すID
        /// </summary>
        public int nextChunkId { get; set; }

        /// <summary>
        /// このチャンクの親を示すID。
        /// </summary>
        public int parentChunkId { get; set; }

        /// <summary> １．２．３ のようにドットでつながれた連番 </summary>
        public string dottedNum { get; set; }

        public bool hasFollower { get; set; }

        public int followeeIdx { get; set; }

        /// <summary> インデント部分 </summary>
        public string indent { get; set; }

        /// <summary>振る舞いの１行分</summary>
        public string behavior { get; set; }

        public BehaviorChunk()
        {
            hasFollower = false;
            followeeIdx = 0;
        }


    }
}
