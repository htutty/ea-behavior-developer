using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.util
{
    public class BehaviorParser
    {

        private int chunkCount;

        private MethodVO parsingMethod;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviorParser()
        {
            chunkCount = 1;
        }

        /// <summary>
        /// 受領した操作のふるまいを解析し、ふるまいのチャンクリストを返す
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public List<BehaviorChunk> parseBehavior(MethodVO method)
        {
            this.parsingMethod = method;

            string[] delimiter = { "\r\n" };
            string[] lins = method.behavior.Split(delimiter, StringSplitOptions.None);

            return parseBehaviorLines(lins);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="tlin"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private List<BehaviorChunk> parseBehaviorLines(string[] tlin)
        {
            //Console.WriteLine("start parsedBehavior()");

            List<BehaviorChunk> chunkList = new List<BehaviorChunk>();

            for (int idx = 0; idx < tlin.Length; idx++)
            {
                string l = tlin[idx];

                // ハイフンだけの行は出力外
                Match matche = Regex.Match(l, "^-----*");
                if (matche.Success)
                {
                    continue;
                }

                // 全角・半角空白のtrim()結果が空文字列なら出力外
                string trm = l.Trim(' ', '　');
                if (trm == "")
                {
                    continue;
                }

                // "１．２．３"の部分のあるふるまい行は、行番号部分を抽出する
                matche = Regex.Match(trm, "^[０-９][０-９．]*");
                if (matche.Success)
                {
                    string bodyText = trm.Substring(matche.Value.Length, trm.Length - matche.Value.Length);

                    BehaviorChunk chunk = new BehaviorChunk();
                    chunk.pos = idx + 1;
                    chunk.behavior = bodyText.Trim(' ', '　');
                    chunk.dottedNum = matche.Value;
                    chunk.indent = "";
                    chunk.methodId = parsingMethod.methodId;

                    chunkList.Add(chunk);
                }
                else
                {
                    // そうでないふるまい行は先頭の空白をインデントとして保持し、
                    // 項番はNULLにする
                    matche = Regex.Match(l, "^[　 ]*");

                    BehaviorChunk chunk = new BehaviorChunk();
                    chunk.pos = idx + 1;
                    chunk.behavior = trm;
                    chunk.dottedNum = "";
                    chunk.indent = matche.Value;
                    chunk.methodId = parsingMethod.methodId;

                    chunkList.Add(chunk);
                }

            }

            // 1パスで作成されたふるまいチャンクリストを再度解析（2パス）
            bool dottedFlg = false;
            for (int i = 0; i < chunkList.Count; i++)
            {
                // １．１のようなドットでつながれた番号を持つ行かを判断
                var chunk = chunkList[i];
                if (chunk.dottedNum != null)
                {
                    dottedFlg = true;
                }

                if (dottedFlg)
                {
                    // 全角ドットの数を数え、インデントレベルを取得
                    chunk.indLv = countChar(chunk.dottedNum, "．") + 1;

                    for (int j = i + 1; j < chunkList.Count; j++)
                    {
                        BehaviorChunk nextChunk = chunkList[j];

                        if (nextChunk.dottedNum == "")
                        {
                            chunk.hasFollower = true;
                            nextChunk.followeeIdx = i;
                        }
                        else
                        {
                            // ループを抜ける
                            break;
                        }

                    }
                }
                else
                {
                    // 空白の数を数え、インデントレベルを取得
                    chunk.indLv = countChar(chunk.indent, "　") * 2 + countChar(chunk.indent, " ");

                }

            }


            // ふるまいチャンクリストを再度解析し、返却用リストに詰める（３パス）
            List<BehaviorChunk> retList = new List<BehaviorChunk>();
            int saveIndLv = 0;
            for (int i = 0; i < chunkList.Count; i++)
            {
                var chunk = chunkList[i];

                // 後続チャンクが存在したら
                if (chunk.hasFollower)
                {
                    // 後続チャンクが続く限り、ふるまいの内容を自チャンクに付加する（仮想改行文字付き）
                    for (int j = i + 1; j < chunkList.Count; j++)
                    {
                        BehaviorChunk nextChunk = chunkList[j];
                        if (i == nextChunk.followeeIdx)
                        {
                            // 続く処理でインデントレベルがぶつからないように後続チャンクには大きな数をセット
                            nextChunk.indLv = 999;
                            chunk.behavior = chunk.behavior + "#\\n#" + nextChunk.behavior;
                        }
                        else
                        {
                            // forを抜ける
                            break;
                        }
                    }
                }

                if (chunk.followeeIdx < 0)
                {
                    chunk.chunkId = chunkCount++;
                    retList.Add(chunk);
                }
            }

            for (int i = 0; i < retList.Count; i++)
            {
                var chunk = retList[i];

                // 2行目以降：　親チャンク、兄弟（インデントレベルが同じ）をセット
                if (i > 0)
                {
                    // 1つ前のインデントレベルと比較し同じだったら
                    if (chunk.indLv == saveIndLv)
                    {
                        // 自分のインデントレベルより小さい最後のチャンクを探し、それを親とみなす
                        chunk.parentChunkId = searchParentChunkId(chunkList, chunk.indLv, i);

                        // 前チャンクID＝１つ前の行のチャンクIDをセット
                        chunk.previousChunkId = chunkList[i - 1].chunkId;
                    }
                    // 1つ前のインデントレベルと比較し自分が大きい、もしくは小さい場合で、
                    // かつインデントレベルが０より大きい場合（何かの子になるはず）
                    else if (chunk.indLv > 0)
                    {
                        // 自分のインデントレベルより小さい最後のチャンクを探し、それを親とみなす
                        chunk.parentChunkId = searchParentChunkId(chunkList, chunk.indLv, i);

                        // 自分のインデントレベルと等しい最後のチャンクを探す。かつ小さくなる前にマッチしたものを返す
                        chunk.previousChunkId = searchPreviousChunkId(chunkList, chunk.indLv, i);
                    }
                    // 1つ前のインデントレベルと比較し自分が大きい、もしくは小さい場合で、
                    // かつインデントレベルが０の場合
                    else
                    {
                        // 親の検索は不要で親無しになる
                        chunk.parentChunkId = 0;
                        // 自分のインデントレベルと等しい最後のチャンクを探す。かつ小さくなる前にマッチしたものを返す
                        chunk.previousChunkId = searchPreviousChunkId(chunkList, chunk.indLv, i);
                    }

                }
                // 1行目は親も兄弟もなし
                else
                {
                    chunk.parentChunkId = 0;
                    chunk.previousChunkId = 0;
                }

                saveIndLv = chunk.indLv;
            }


            return retList;
        }


        private int searchParentChunkId(List<BehaviorChunk> chunkList, int targetIndLv, int childIdx)
        {
            if (childIdx > 0 && targetIndLv > 0)
            {
                // 子のインデックスの１つ上から上になめる
                for (int i = childIdx - 1; i >= 0; i--)
                {
                    // より小さなインデックスレベルのチャンクを見つけたら
                    var chunk = chunkList[i];
                    if (chunk.indLv < targetIndLv)
                    {
                        // 該当のチャンクIDを返却
                        return chunk.chunkId;
                    }
                }

                // 先頭まで戻っても、より小さいインデントレベルが見つからなかったら
                // 親無しの扱い(0)にする
                return 0;
            }
            else
            {
                // 親無しの扱いにする
                return 0;
            }

        }


        /// <summary>
        /// 自分のインデントレベルと同じ最初のチャンクを探す。かつ小さくなる前にマッチしたものを返す
        ///
        /// 例：
        /// [101]１
        /// [102]□１．１　　　　　　　← previous は 0
        /// [103]□□１．１．１　　　　← previous は 0
        /// [104]□□□１．１．１．１　← previous は 0
        /// [105]□１．２　　　　　　　← previous は 102
        /// </summary>
        /// <param name="chunkList"></param>
        /// <param name="targetIndLv"></param>
        /// <param name="childIdx"></param>
        /// <returns></returns>
        private int searchPreviousChunkId(List<BehaviorChunk> chunkList, int targetIndLv, int childIdx)
        {
            if (childIdx > 0 && targetIndLv > 0)
            {
                // 子のインデックスの１つ上から上になめる
                for (int i = childIdx - 1; i >= 0; i--)
                {
                    var chunk = chunkList[i];
                    // 自分と同じインデックスレベルのチャンクを(先に)見つけたら
                    if (chunk.indLv == targetIndLv)
                    {
                        // 該当のチャンクIDを返却
                        return chunk.chunkId;
                    }
                    // より小さなインデックスレベルのチャンクを見つけたら
                    else if (chunk.indLv < targetIndLv)
                    {
                        return 0;
                    }
                }

                // 先頭まで戻っても、同じインデントレベルが見つからなかったら
                // 親無しの扱い(0)にする
                return 0;
            }
            else
            {
                // 親無しの扱いにする
                return 0;
            }

        }

        // 文字の出現回数をカウント
        private int countChar(string s, string c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }

    }
}
