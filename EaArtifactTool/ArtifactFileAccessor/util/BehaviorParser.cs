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

        private MethodVO parsingMethod = null;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviorParser()
        {
            chunkCount = 1;
        }


        public List<BehaviorChunk> parseBehavior(string origBehavior)
        {
            string[] delimiter = { "\r\n" };
            string[] lins = origBehavior.Split(delimiter, StringSplitOptions.None);

            List<BehaviorChunk> retChunks = parseBehaviorLines(lins);

            foreach (BehaviorChunk c in retChunks)
            {
                c.behaviorToken = tryTokenize(c); 
            }

            return retChunks;
        }

        /// <summary>
        /// 受領した操作のふるまいを解析し、ふるまいのチャンクリストを返す
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public List<BehaviorChunk> parseBehavior(MethodVO method)
        {
            this.parsingMethod = method;
            return parseBehavior(method.behavior);
        }


        private BehaviorToken tryTokenize(BehaviorChunk c)
        {

            string origStr = c.behavior;
            string trimmedStr = getTrimmed(origStr);

            BehaviorToken tokenTop = new BehaviorToken();
            Match matche;

            // 変数とかインスタンスを生成しつつ初期値をセットする
            matche = Regex.Match(trimmedStr, "(.*)を生成し、?(.*)を(セット|設定)する");
            if (matche.Success)
            {
                tokenTop.token = "[define]";
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, "var", TokenType.TOKEN_DECLARE_LABEL);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "=", TokenType.TOKEN_EQUAL);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                return tokenTop;
            }

            // 型宣言と値代入を同時に行う場合
            // 例: OPEN用フライト情報にフライト情報を生成してセットする
            matche = Regex.Match(trimmedStr, "(.*)に、?(.*)を、?生成してセットする");
            if (matche.Success)
            {
                tokenTop.token = "[define-with-type]";
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_TYPE);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "=", TokenType.TOKEN_EQUAL);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                return tokenTop;
            }


            // 変数とかインスタンスの生成（名前＝型のやつ）
            matche = Regex.Match(trimmedStr, "(.*)を生成する");
            if (matche.Success)
            {
                tokenTop.token = "[declare]";
                var g1 = matche.Groups[1];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ELEMENT_NAME);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                return tokenTop;
            }


            // メソッド呼び出しで戻り値を取らないパターン
            matche = Regex.Match(trimmedStr, "(.*)を呼び出す");
            if (matche.Success)
            {
                tokenTop.token = "[call]";
                var g1 = matche.Groups[1];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_METHOD_NAME);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // メソッド呼び出しで戻り値を取るパターン
            matche = Regex.Match(trimmedStr, "(.*)を呼び?出し、?(.*)(に戻り値)?を(セット|取得)する");
            if (matche.Success)
            {
                tokenTop.token = "[receive_response]";
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_EQUAL);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // リターン文
            // 例:　表示可能運賃存在フラグをリターンする。
            matche = Regex.Match(trimmedStr, "(.*)をリターンする");
            if (matche.Success)
            {
                tokenTop.token = "[return]";
                var g1 = matche.Groups[1];

                addToken(tokenTop, "return ", TokenType.TOKEN_RETURN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // 型変換を伴う代入式のパターン
            matche = Regex.Match(trimmedStr, "(.*)に、?(.*)に変換した(.*)を(セット|設定)(する|し)");
            if (matche.Success)
            {
                tokenTop.token = "[let_with_conv]";

                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];
                var g3 = matche.Groups[3];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_EQUAL);

                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                addToken(tokenTop, g3.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // 代入式のパターン
            matche = Regex.Match(trimmedStr, "(.*)に、?(.*)を(セット|設定)(する|し)");
            if (matche.Success)
            {
                tokenTop.token = "[let]";
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_EQUAL);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // if文の条件式のパターン（「～の場合」で文が終わる）
            matche = Regex.Match(trimmedStr, "(.*)の場合[。、]?\\s*$");
            if (matche.Success)
            {
                tokenTop.token = "[if-cond]";
                var g1 = matche.Groups[1];

                addToken(tokenTop, "if", TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                return tokenTop;
            }

            // ループ 
            // 例： フライト情報リストの要素数分ループし、以下の処理を繰り返す(ループカウンタ:i)
            matche = Regex.Match(trimmedStr, "(.*)の要素数分(ループし)?、*以下の処理を繰り?返え?す。?\\(ループカウンタ:(.*)\\)");
            if (matche.Success)
            {
                tokenTop.token = "[foreach-collection]";
                Group g1 = matche.Groups[1];

                Group g2;
                if ( matche.Groups[2].ToString() == "ループし")
                {
                    g2 = matche.Groups[3];
                }
                else
                {
                    g2 = matche.Groups[2];
                }

                addToken(tokenTop, "for", TokenType.TOKEN_FOREACH);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                return tokenTop;
            }

            // ループ 
            // 例： フライト情報リストの要素数分ループし、以下の処理を繰り返す
            matche = Regex.Match(trimmedStr, "(.*)の要素数分(ループし)?、*以下の処理を繰り?返え?す");
            if (matche.Success)
            {
                tokenTop.token = "[foreach-collection]";
                var g1 = matche.Groups[1];

                addToken(tokenTop, "for", TokenType.TOKEN_FOREACH);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                return tokenTop;
            }

            // break(ループを抜ける)
            // ループを抜ける。
            matche = Regex.Match(trimmedStr, "ループを抜ける");
            if (matche.Success)
            {
                tokenTop.token = "[loop-break]";

                addToken(tokenTop, "break", TokenType.TOKEN_BREAK);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // continue(ループの先頭に戻る)
            // ループの先頭に戻る。
            matche = Regex.Match(trimmedStr, "ループの先頭に戻る");
            if (matche.Success)
            {
                tokenTop.token = "[loop-continue]";

                addToken(tokenTop, "continue", TokenType.TOKEN_CONTINUE);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            return null;
        }


        private string getTrimmed(string origStr)
        {
            string retStr = origStr;

            if( retStr != null && retStr != "")
            {
                //削除する文字の配列
                string[] removeChars = new string[] { " ", "　", "\\n", "\n", "\r" };

                //削除する文字を1文字ずつ削除する
                foreach (string s in removeChars)
                {
                    retStr = retStr.Replace(s, "");
                }
            }

            return retStr;
        }


        private BehaviorToken addToken(BehaviorToken token, string content, TokenType tokenTyp)
        {
            BehaviorToken targetToken = token;

            while(targetToken.NextToken != null)
            {
                targetToken = targetToken.NextToken;
            }

            BehaviorToken appendToken = new BehaviorToken();
            appendToken.token = content;
            appendToken.tokenType = tokenTyp;
            targetToken.NextToken = appendToken;

            return targetToken;
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
                    if(parsingMethod != null)
                    {
                        chunk.methodId = parsingMethod.methodId;
                    }

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

                    if (parsingMethod != null)
                    {
                        chunk.methodId = parsingMethod.methodId;
                    }

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
                            chunk.behavior = chunk.behavior + "\\n" + nextChunk.behavior;
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
