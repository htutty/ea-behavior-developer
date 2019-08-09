using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.util
{
    public class BehaviorParser
    {
        // パース結果で判断されたステートメントのタイプ指定
        public const string STMT_TYPE_DEFINE = "[define]";
        public const string STMT_TYPE_DEFINE_WITH_TYPE = "[define-with-type]";
        public const string STMT_TYPE_DECLARE = "[declare]";
        public const string STMT_TYPE_CALL_METHOD = "[call-method]";
        public const string STMT_TYPE_CALL_METHOD_TAKE_RETURN = "[call-method-take-return]";
        public const string STMT_TYPE_RETURN = "[return]";
        public const string STMT_TYPE_LET_WITH_CAST = "[let-with-cast]";
        public const string STMT_TYPE_LET = "[let]";
        public const string STMT_TYPE_IF_COND = "[if-condition]";
        public const string STMT_TYPE_FOREACH_COLLECTION_W_INDEX = "[foreach-collection-with-index]";
        public const string STMT_TYPE_FOREACH_COLLECTION = "[foreach-collection]";
        public const string STMT_TYPE_FOREACH_PICKUP = "[foreach-pickup]";
        public const string STMT_TYPE_LOOP_BREAK = "[loop-break]";
        public const string STMT_TYPE_LOOP_CONTINUE = "[loop-continue]";
        public const string STMT_TYPE_COMMENT = "[comment]";

        private int chunkCount;

        private MethodVO parsingMethod = null;

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
            return parseBehavior(method.behavior);
        }

        /// <summary>
        /// ふるまいのパースを行う。チャンクに切り分けた上で、チャンク毎にトークンを抽出する
        /// </summary>
        /// <param name="origBehavior">ふるまい</param>
        /// <returns>ふるまいチャンクのリスト</returns>
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
        /// 既にチャンク化された
        /// </summary>
        /// <param name="chunks"></param>
        /// <returns></returns>
        public List<BehaviorChunk> tokenizeChunk(List<BehaviorChunk> chunks)
        {

            foreach (BehaviorChunk c in chunks)
            {
                c.behaviorToken = tryTokenize(c);
            }

            return chunks;
        }


        /// <summary>
        /// トークナイズを試みる（tryを付けているのはインプットのふるまいの記述ルールに
        /// 揺れがあり実効性が不明なため）
        /// </summary>
        /// <param name="c">ふるまいチャンク</param>
        /// <returns>取得されたトークン</returns>
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
                tokenTop.token = STMT_TYPE_DEFINE;
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, "var", TokenType.TOKEN_DECLARE_LABEL);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                return tokenTop;
            }

            // 型宣言と値代入を同時に行う場合
            // 例: OPEN用フライト情報にフライト情報を生成してセットする
            matche = Regex.Match(trimmedStr, "(.*)に、?(.*)を、?生成してセットする");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_DEFINE_WITH_TYPE;
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_TYPE);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                return tokenTop;
            }


            // 変数とかインスタンスの生成（名前＝型のやつ）
            matche = Regex.Match(trimmedStr, "(.*)を生成する");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_DECLARE;
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
                tokenTop.token = STMT_TYPE_CALL_METHOD;
                var g1 = matche.Groups[1];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_METHOD_NAME);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // メソッド呼び出しで戻り値を取るパターン
            matche = Regex.Match(trimmedStr, "(.*)を呼び?出し、?(.*)(に戻り値)?を(セット|取得)する");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_CALL_METHOD_TAKE_RETURN;
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // メソッド呼び出しで戻り値を取るパターン（別バージョン）
            // 日付操作ユーティリティ.日付分割処理(空席照会条件入力・空席照会検索条件.往路搭乗日,\n定数クラス.日付セパレートキー（年）)を呼び出し、取得した分割搭乗年を(変数)搭乗年 にセットする。
            matche = Regex.Match(trimmedStr, "(.*)を呼び?出し、?取得した(.*)を(.*)にセットする");
            if (matche.Success)
            {
                tokenTop.token = tokenTop.token = STMT_TYPE_CALL_METHOD_TAKE_RETURN;

                var g1 = matche.Groups[1];
                var g3 = matche.Groups[3];

                addToken(tokenTop, g3.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // リターン文
            // 例:　表示可能運賃存在フラグをリターンする。
            matche = Regex.Match(trimmedStr, "(.*)をリターンする");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_RETURN;
                var g1 = matche.Groups[1];

                addToken(tokenTop, "return ", TokenType.TOKEN_RETURN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // 型変換(Class Cast)を伴う代入式のパターン
            matche = Regex.Match(trimmedStr, "(.*)に、?(.*)に変換した(.*)を(セット|設定)(する|し)");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_LET_WITH_CAST;

                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];
                var g3 = matche.Groups[3];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);

                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ELEMENT_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                addToken(tokenTop, g3.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // 代入式のパターン
            matche = Regex.Match(trimmedStr, "(.*)に、?(.*)を(セット|設定)(する|し)");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_LET; 
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_EXPR_IDENTIFIER);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // 代入式のパターン（その２）　※ルール的にはダメなやつ
            matche = Regex.Match(trimmedStr, "(.*)を、?(.*)に(セット|設定)(する|し)");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_LET;
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                return evaluateAssignmentExpression(tokenTop);
            }


            // if文の条件式のパターン（「～の場合」で文が終わる）
            matche = Regex.Match(trimmedStr, "(.*)の場合[。、]?\\s*$");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_IF_COND;
                var g1 = matche.Groups[1];

                addToken(tokenTop, "if", TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                return tokenTop;
            }

            // ループ
            // 例： フライト情報リストの要素数分ループし、以下の処理を繰り返す(ループカウンタ:i)
            matche = Regex.Match(trimmedStr, "(.*)の要素数分(ループし)?、*以下(の処理)?を繰り?返え?す。?\\(ループカウンタ[:：](.*)\\)");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_FOREACH_COLLECTION_W_INDEX;
                Group g1 = matche.Groups[1];
                Group g2 = matche.Groups[4];

                addToken(tokenTop, "for", TokenType.TOKEN_FOREACH);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, "int", TokenType.TOKEN_INSTANCE_TYPE);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);
                addToken(tokenTop, "0", TokenType.TOKEN_LITERAL);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "<", TokenType.TOKEN_OPER_LESSTHAN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, ".", TokenType.TOKEN_OPER_DOT);
                addToken(tokenTop, "Count", TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);

                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_INSTANCE_LABEL);
                addToken(tokenTop, "++", TokenType.TOKEN_OPER_INCREMENT);

                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                return tokenTop;
            }

            // ループ
            // 例： フライト情報リストの要素数分ループし、以下の処理を繰り返す
            matche = Regex.Match(trimmedStr, "(.*)の要素数分(ループし)?、*以下の処理を繰り?返え?す");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_FOREACH_COLLECTION;
                var g1 = matche.Groups[1];

                addToken(tokenTop, "for", TokenType.TOKEN_FOREACH);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);

                return tokenTop;
            }

            // コレクションをなめるループをしている中で、コレクションから１要素を取り出す記述
            // 合わせて、変数名などに「〇〇リスト」と名付けておくだけで
            // 「〇〇」という型のコレクションであるという宣言をしていることになるらしい。
            // 例： フライト情報リスト(i)をフライト情報として取得する。
            matche = Regex.Match(trimmedStr, "(.*)\\(([i-n])\\)を(.*)として取得する");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_FOREACH_PICKUP;
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];
                var g3 = matche.Groups[3];

                addToken(tokenTop, g3.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, "=", TokenType.TOKEN_OPER_EQUAL);

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, "[", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_COLLECTION_NAME);
                addToken(tokenTop, "]", TokenType.TOKEN_PARENTHESIS_END);

                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // break(ループを抜ける)
            // ループを抜ける。
            matche = Regex.Match(trimmedStr, "ループを抜ける");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_LOOP_BREAK;

                addToken(tokenTop, "break", TokenType.TOKEN_BREAK);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }


            // continue(ループの先頭に戻る)
            // ループの先頭に戻る。
            matche = Regex.Match(trimmedStr, "ループの先頭に戻る");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_LOOP_CONTINUE;

                addToken(tokenTop, "continue", TokenType.TOKEN_CONTINUE);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // メソッド呼び出しで戻り値を取らないパターン（別パターン）
            // 単にメソッド名だけを書いた文章は、そのメソッドを呼び出し戻り値は無視する、という意味になるらしい。
            matche = Regex.Match(trimmedStr, "(.*)\\((.*)\\)$");
            if (matche.Success)
            {
                tokenTop.token = STMT_TYPE_CALL_METHOD;
                var g1 = matche.Groups[1];
                var g2 = matche.Groups[2];

                addToken(tokenTop, g1.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, "(", TokenType.TOKEN_PARENTHESIS_BEGIN);
                addToken(tokenTop, g2.ToString(), TokenType.TOKEN_ATTRIBUTE_NAME);
                addToken(tokenTop, ")", TokenType.TOKEN_PARENTHESIS_END);
                addToken(tokenTop, ";", TokenType.TOKEN_SEMICOLON);
                return tokenTop;
            }

            // それ以外はコメント扱いとしてそのまま出力
            tokenTop.token = STMT_TYPE_COMMENT;

            addToken(tokenTop, trimmedStr, TokenType.TOKEN_COMMENT);
            return tokenTop;

            // return null;
        }

        /// <summary>
        /// 代入式の評価
        /// </summary>
        /// <param name="origToken">材料のトークン</param>
        /// <returns></returns>
        private BehaviorToken evaluateAssignmentExpression(BehaviorToken origToken)
        {
            // 代入式の場合
            if( origToken.token == STMT_TYPE_LET )
            {
                // left sideは Top の次
                BehaviorToken lhsToken = origToken.NextToken;
                // right side は lhs の次の次
                BehaviorToken rhsToken = lhsToken.NextToken.NextToken;

                // 変数名と思しき場所を、さらにパースする
                // 　演算子 "." で区切られた文字列の場合は要素名と属性名に分ける
                // 　なおどのような場合でもメソッドには非対応
                BehaviorToken lhsParsed = parseIdentifier(lhsToken.token);
                BehaviorToken rhsParsed = parseIdentifier(rhsToken.token);

                BehaviorToken retToken = new BehaviorToken();
                retToken.token = "[let]";

                addAllTokens(retToken, lhsParsed);
                addToken(retToken, "=", TokenType.TOKEN_OPER_EQUAL);
                addAllTokens(retToken, rhsParsed);
                addToken(retToken, ";", TokenType.TOKEN_SEMICOLON);
            }

            return null;

        }


        /// <summary>
        /// 識別子を取り出す（要素名.属性名 もしくは 属性名のみ）
        /// </summary>
        /// <param name="origValue">取得元の文字列</param>
        /// <returns>トークン</returns>
        private BehaviorToken parseIdentifier(string origValue)
        {
            BehaviorToken retToken = new BehaviorToken();

            // "." によって２つ以上に区切られた式の場合
            Match matche = Regex.Match(origValue, @"[^\.]+\.[^\.]+");
            if (matche.Success)
            {
                string[] splitted = origValue.Split( new string[] { "." }, StringSplitOptions.None);

                retToken.tokenType = TokenType.TOKEN_ELEMENT_NAME;
                retToken.token = splitted[0];
                for(int i=1; i < splitted.Length; i++)
                {
                    addToken(retToken, ".", TokenType.TOKEN_OPER_DOT);
                    addToken(retToken, splitted[i], TokenType.TOKEN_ATTRIBUTE_NAME);
                }
                return retToken;
            }
            else
            {
                retToken.tokenType = TokenType.TOKEN_ATTRIBUTE_NAME;
                retToken.token = origValue;
                return retToken;
            }

        }


        /// <summary>
        /// 引数の文字列が操作かどうかを判断する。
        /// </summary>
        /// <param name="expr">識別したい文字列</param>
        /// <returns>true:操作、false:操作でない</returns>
        private bool isMethod( string expr )
        {
            // ( ) を含んだ文字列が識別子中に有れば
            Match matche = Regex.Match(expr, @"[^\.\(]+\((.*)\)$");
            return matche.Success;
        }

        /// <summary>
        /// 空白、改行を取り除いた文字列を返却する
        /// </summary>
        /// <param name="origStr">元の文字列</param>
        /// <returns>空白改行を取り除いた文字列</returns>
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

        /// <summary>
        /// 引数からトークンを作成し、渡されたトークンの末尾に付与して返却する
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <param name="tokenTyp"></param>
        /// <returns></returns>
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



        private BehaviorToken addAllTokens(BehaviorToken origToken, BehaviorToken appendToken)
        {
            BehaviorToken targetToken = origToken;

            while (targetToken.NextToken != null)
            {
                targetToken = targetToken.NextToken;
            }

            while (appendToken != null)
            {
                targetToken.NextToken = appendToken;
                appendToken = appendToken.NextToken;
            }

            return targetToken;
        }

        /// <summary>
        /// ふるまいの各行を解析し、各行の処理内容を評価する前段階として１行ないし複数行の
        /// 命令に分ける。
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
            for (int i = 0; i < chunkList.Count; i++)
            {
                bool dottedFlg = false;

                // １．１のようなドットでつながれた番号を持つ行かを判断
                var chunk = chunkList[i];
                if (chunk.dottedNum != null && chunk.dottedNum != "")
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

        /// <summary>
        /// 引数で指定された文字が文字列の中に何回出てくるかを数えて返却する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private int countChar(string s, string c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }

    }
}
