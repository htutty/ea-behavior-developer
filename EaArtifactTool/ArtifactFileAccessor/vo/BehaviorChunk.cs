using System;
using System.Collections.Generic;
using System.IO;
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
        /// このチャンクの親を示すID。
        /// </summary>
        public int parentChunkId { get; set; }

        /// <summary>
        /// このチャンクと同レベルで１つ前のチャンクを示すID
        /// </summary>
        public int previousChunkId { get; set; }

        /// <summary> １．２．３ のようにドットでつながれた連番 </summary>
        public string dottedNum { get; set; }

        /// <summary>
        /// インデントのレベル。最上位は０
        /// </summary>
        public int indLv { get; set; }

        /// <summary>
        /// 自分が後続チャンクを持っているか。
        /// 後続チャンクとは、以下のようなふるまい行における2行目以降のこと。
        /// 「１．２．１　・・・の場合、・・・・
        ///   　　　　　　かつ、・・・の場合、・・・・
        ///   　　　　　　もしくは、・・・の場合、・・・・」
        /// 
        /// </summary>
        public bool hasFollower { get; set; }

        /// <summary>
        /// 自分に先行するチャンクのID
        /// </summary>
        public int followeeIdx { get; set; }

        /// <summary> インデント部分 </summary>
        public string indent { get; set; }

        /// <summary>振る舞いの１行分</summary>
        public string behavior { get; set; }

        /// <summary>ふるまいをトークン化したもの（NULLの場合もある）</summary>
        public BehaviorToken behaviorToken { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviorChunk()
        {
            hasFollower = false;
            followeeIdx = -1;
            indent = "";
            behavior = "";
        }


        public string getTokenRepresented()
        {
            if(behaviorToken != null)
            {
                BehaviorToken nowToken = behaviorToken;
                StringWriter sw = new StringWriter();
                while(nowToken.NextToken != null)
                {
                    if(nowToken != behaviorToken)
                    {
                        sw.Write(" ");
                    }
                    sw.Write(nowToken.token);
                    nowToken = nowToken.NextToken;
                }

                sw.Write(" ");
                sw.Write(nowToken.token);
                nowToken = nowToken.NextToken;

            return sw.ToString();
            }
            else
            {
                return "";
            }
        }

    }
}
