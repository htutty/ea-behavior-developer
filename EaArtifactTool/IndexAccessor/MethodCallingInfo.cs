using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexAccessor
{
    public class MethodCallingInfo
    {
        /// <summary>操作のid</summary>
        public int methodId { get; set; }

        /// <summary>チャンクのID</summary>
        public int chunkId { get; set; }

        /// <summary>呼び出し処理の行番号</summary>
        public int row { get; set; }

        /// <summary>呼び出し先操作のid</summary>
        public int destMethodId { get; set; }

        /// <summary>チャンク内の呼び出し処理に合致した部分</summary>
        public string matchedChunk { get; set; }
    }
}
