using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using IndexAccessor;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportGraphData
{
    class GraphDataExporter
    {
        private string outputDir;

        private Dictionary<string, MethodVO> allMethodMap;

        private Dictionary<int, List<MethodVO>> elementOwnMethodsMap;

        public GraphDataExporter(string outputDir_)
        {
            this.outputDir = outputDir_;
            this.allMethodMap = new Dictionary<string, MethodVO>();
            this.elementOwnMethodsMap = new Dictionary<int, List<MethodVO>>();
        }


        public void doExport()
        {
//            exportElementGraph();
            exportAttributesAndMethodsGraph();
        }

        /// <summary>
        /// 
        /// </summary>
        private void exportElementGraph()
        {
            StreamWriter grphsw = null;

            try
            {
                grphsw = new StreamWriter(outputDir + "\\" + "graphdata.dat", false, System.Text.Encoding.GetEncoding("utf-8"));

                // Elementノードの出力
                List<ElementSearchItem>  elementSearches = getElementSearcheItems();

                foreach (ElementSearchItem elemSrch in elementSearches)
                {
                    grphsw.Write("CREATE (" + "E" + elemSrch.elementId + ":Element ");
                    grphsw.Write("{ name:\"" + elemSrch.elemName + "\",");
                    grphsw.Write(" id:" + elemSrch.elementId + ",");
                    grphsw.Write(" path:\"" + elemSrch.elemPath + "\" ");
                    grphsw.WriteLine("});");

                }


                // Element間の接続情報を追加
                ConnectorSearcher connectorSearcher = new ConnectorSearcher();

                // 抽出されたElementSearchItem のGUIDをキーとして、Connector情報を調べる
                foreach (ElementSearchItem elemSrch in elementSearches)
                {
                    List<ConnectorVO> elemConnectors = connectorSearcher.findByObjectGuid(elemSrch.elemGuid);

                    foreach(ConnectorVO conn in elemConnectors )
                    {
                    
                        // 自オブジェクトがsrcの場合のみ
                        if( conn.srcObjId == elemSrch.elementId )
                        {
                            // ２ノードのMATCHと、MATCHされた２ノード間にリレーションを設定する以下のようなCypher文を出力：
                            // MATCH (src:Element{id:9999}),(dst:Element{id:9999}) CREATE (src)-[:Association]->(dst);
                            grphsw.Write("MATCH (src:Element{id:" + conn.srcObjId + "}), ");
                            grphsw.Write("(dst:Element{id:" + conn.destObjId + "}) ");
                            grphsw.WriteLine("CREATE (src)-[:" + conn.connectorType + "]->(dst)");
                            grphsw.WriteLine(";");

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (grphsw != null) grphsw.Close();
            }

        }


        /// <summary>
        /// SQLiteに格納されたt_elementテーブルから、以下の条件でマッチしたレコードを取得し
        /// ElementSearchItemに格納して返す。
        ///   ・要素のタイプ が 'Class', 'Interface', 'Enumeration'のいずれか
        ///   ・要素パスが '/論理モデル/' に部分一致する 
        /// </summary>
        /// <returns>取得</returns>
        private List<ElementSearchItem> getElementSearcheItems()
        {

            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + dbFileName);

            conn.Open();


            List<ElementSearchItem> retList = new List<ElementSearchItem>();

            string sql =
                @"select objectId, elemGuid, elemName, elemAlias, elemType, ifnull(elemStereotype, ''),
                         elementPath, artifactGuid,  artifactName
				  from   t_element 
                  where  elemType in ('Class', 'Interface', 'Enumeration')
                   and   elementPath like '%/論理モデル/%'  ";

            using (var command = conn.CreateCommand())
            {
                //クエリの実行
                command.CommandText = sql;
                using (var sdr = command.ExecuteReader())
                {
                    //
                    while (sdr.Read())
                    {
                        ElementSearchItem elemSrchItem = new ElementSearchItem();

                        elemSrchItem.elementId = sdr.GetInt32(0);
                        elemSrchItem.elemGuid = sdr.GetString(1);
                        elemSrchItem.elemName = StringUtil.removeQuoteChar(sdr.GetString(2));
                        elemSrchItem.elemAlias = sdr.GetString(3);
                        elemSrchItem.elemType = sdr.GetString(4);
                        elemSrchItem.elemStereotype = sdr.GetString(5);
                        elemSrchItem.elemPath = sdr.GetString(6);
                        elemSrchItem.artifactGuid = sdr.GetString(7);
                        elemSrchItem.artifactName = sdr.GetString(8);

                        retList.Add(elemSrchItem);
                    }
                }
            }

            conn.Close();

            // 一件でも取得できたらtrueを返す
            return retList;
        }



        /// <summary>
        /// 
        /// </summary>
        private void exportAttributesAndMethodsGraph()
        {
            StreamWriter grphsw = null;

            try
            {
                grphsw = new StreamWriter(outputDir + "\\" + "graphdata_d.dat", false, System.Text.Encoding.GetEncoding("utf-8"));

                // AttributeノードおよびMethodノードの出力
                List<AttrMthSearchItem> attrMethSearchItems = readAttributesAndMethods();
                List<MethodVO> methods = new List<MethodVO>();
                List<MethodVO> elemOwnedMethods = new List<MethodVO>();
                int savedElementId = -1;

                foreach (AttrMthSearchItem attrMethItem in attrMethSearchItems)
                {
                    if(attrMethItem.isAttribute())
                    {
                        // AttributeノードのCREATE
                        grphsw.Write("CREATE (" + "A" + attrMethItem.attrMethId + ":Attribute ");
                        grphsw.Write("{ name:\"" + attrMethItem.attrMethName + "\",");
                        grphsw.Write(" id:" + attrMethItem.attrMethId + ",");
                        grphsw.Write(" alias:\"" + attrMethItem.attrMethAlias + "\" ");
                        grphsw.WriteLine(" })");
                        grphsw.WriteLine(";");

                        // Elementノードとのリレーション(:Own) の追加
                        grphsw.Write("MATCH (e:Element{id:" + attrMethItem.elemId + "}), ");
                        grphsw.Write("(attr:Attribute{id:" + attrMethItem.attrMethId + "}) ");
                        grphsw.WriteLine("CREATE (e)-[:Own]->(attr)");
                        grphsw.WriteLine(";");

                    }
                    else
                    {
                        // MethodノードのCREATE
                        grphsw.Write("CREATE (" + "M" + attrMethItem.attrMethId + ":Method ");
                        grphsw.Write("{ name:\"" + attrMethItem.attrMethName + "\",");
                        grphsw.Write(" id:" + attrMethItem.attrMethId + ",");
                        grphsw.Write(" alias:\"" + attrMethItem.attrMethAlias + "\" ");
                        grphsw.WriteLine(" })");
                        grphsw.WriteLine(";");

                        // Elementノードとのリレーション(:Own) の追加
                        grphsw.Write("MATCH (e:Element{id:" + attrMethItem.elemId + "}), ");
                        grphsw.Write("(mth:Method{id:" + attrMethItem.attrMethId + "}) ");
                        grphsw.WriteLine("CREATE (e)-[:Own]->(mth)");
                        grphsw.WriteLine(";");


                        // Methodのみ、メソッドのCalling Tree Graph出力のための材料するため
                        // ターゲットのメソッドリストに追加する
                        MethodVO mth = new MethodVO();
                        mth.methodId = attrMethItem.attrMethId;
                        mth.guid = attrMethItem.attrMethGuid;
                        mth.name = attrMethItem.attrMethName;
                        mth.alias = attrMethItem.attrMethAlias;
                        methods.Add(mth);


                        // 要素IDが変わったら、今溜まっているメソッドリストを全要素分のMapに移動
                        if( savedElementId < attrMethItem.elemId)
                        {
                            if( savedElementId > 0) { 
                                // 要素毎のメソッドリストMapにメソッドリストを登録
                                this.elementOwnMethodsMap.Add(savedElementId, elemOwnedMethods);
                                elemOwnedMethods = new List<MethodVO>();
                            }

                            savedElementId = attrMethItem.elemId;
                        }


                        if (attrMethItem.elemName != null && attrMethItem.attrMethName != null )
                        {
                            // 全メソッドマップに登録する
                            string mapKeyStr = attrMethItem.elemName + "." + attrMethItem.attrMethName;
                            if( !allMethodMap.ContainsKey(mapKeyStr) )
                            {
                                this.allMethodMap.Add(mapKeyStr, mth);
                            }

                            // 合わせて要素毎のメソッド名リストに登録する
                            elemOwnedMethods.Add(mth);

                        }

                        Console.WriteLine("Method {0}.{1}({2}) 処理済み", attrMethItem.elemName , attrMethItem.attrMethName, attrMethItem.elemId);
                    }

                } // loop end

                // 要素毎のメソッドリストMapにメソッド名リストを登録
                this.elementOwnMethodsMap.Add(savedElementId, elemOwnedMethods);

                // メソッドのコーリングツリー用グラフデータ出力
                exportMethodCallingGraph(methods);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);

                Console.WriteLine("-------改行の入力で終了します");

                Console.ReadLine();
            }
            finally
            {
                if (grphsw != null) grphsw.Close();
            }

        }


        private void exportMethodCallingGraph(List<MethodVO> methods)
        {
            StreamWriter grphsw = null;
            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + dbFileName);

            try
            {
                grphsw = new StreamWriter(outputDir + "\\" + "graphdata_l.dat", false, System.Text.Encoding.GetEncoding("utf-8"));


                conn.Open();

                BehaviorParser parser = new BehaviorParser();

                foreach (MethodVO mth in methods)
                {
                    List<MethodCallingInfo> callingInfos = searchForMethodLinks(mth, conn, parser);

                    foreach(MethodCallingInfo ci in callingInfos)
                    {
                        // Methodノードとのリレーション(:Call) の追加
                        grphsw.Write("MATCH (src:Method{id:" + ci.methodId + "}), ");
                        grphsw.Write("(dst:Method{id:" + ci.destMethodId + "}) ");
                        grphsw.WriteLine("CREATE (src)-[:Call{ row:" + ci.row + " }]->(dst)");
                        grphsw.WriteLine(";");
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);

                Console.WriteLine("-------改行の入力で終了します");

                Console.ReadLine();
            }
            finally
            {
                if (grphsw != null) grphsw.Close();
                if (conn!=null) conn.Close();

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="methods"></param>
        private List<MethodCallingInfo> searchForMethodLinks(MethodVO mth, SQLiteConnection conn, BehaviorParser parser)
        {
            List<BehaviorChunk> chunks = readBehaviorChunks(conn, mth.methodId);
            List<BehaviorChunk> tokenizedChunks = parser.tokenizeChunk(chunks);

            List<MethodCallingInfo> retCallingInfos = new List<MethodCallingInfo>();

            Console.WriteLine("■■MethodID:" + mth.methodId);
            foreach (BehaviorChunk tchk in tokenizedChunks)
            {
                MethodVO hitMethod = null;
                Console.Write(tchk.dottedNum + "　");
                if (tchk.behaviorToken != null)
                {
                    Console.Write(tchk.behaviorToken.token + tchk.behavior);

                    BehaviorToken tkn = tchk.behaviorToken;

                    // 最初のトークン（命令タイプ識別子）による判断分岐
                    switch (tkn.token)
                    {
                        // call-method 文なら
                        case BehaviorParser.STMT_TYPE_CALL_METHOD:
                            hitMethod = tryGetMethodIdFromCandidateString(getNumberedTokenValue(tkn, 1), mth.elementId);
                            break;

                        // call-method-take-return 文なら
                        case BehaviorParser.STMT_TYPE_CALL_METHOD_TAKE_RETURN:
                            hitMethod = tryGetMethodIdFromCandidateString(getNumberedTokenValue(tkn, 3), mth.elementId);
                            break;

                        // let 文なら
                        case BehaviorParser.STMT_TYPE_LET:
                            hitMethod = tryGetMethodIdFromCandidateString(getNumberedTokenValue(tkn, 1), mth.elementId);
                            if (hitMethod == null)
                            {
                                hitMethod = tryGetMethodIdFromCandidateString(getNumberedTokenValue(tkn, 3), mth.elementId);
                            }
                            break;

                        // let-with-cast 文なら
                        case BehaviorParser.STMT_TYPE_LET_WITH_CAST:
                            hitMethod = tryGetMethodIdFromCandidateString(getNumberedTokenValue(tkn, 6), mth.elementId);
                            break;
                    }

                }
                else
                {
                    Console.Write("[cant-tokenize]" + tchk.behavior);
                }

                // 
                if (hitMethod != null)
                {
                    MethodCallingInfo ci = new MethodCallingInfo();
                    ci.chunkId = tchk.chunkId;
                    ci.methodId = mth.methodId;
                    ci.row = tchk.pos;
                    ci.matchedChunk = tchk.behavior;
                    ci.destMethodId = hitMethod.methodId;
                    retCallingInfos.Add(ci);

                    Console.WriteLine("\t" + hitMethod.name);
                }
                else
                {
                    Console.WriteLine("");
                }

            }

            return retCallingInfos;
        }

        /// <summary>
        /// 指定された回数だけ先頭から次トークンを辿った結果を返却する
        /// （つまり、N個目のトークンを返す）
        /// </summary>
        /// <param name="firstToken"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        private string getNumberedTokenValue(BehaviorToken firstToken, int times)
        {
            BehaviorToken p = firstToken;
            for (int i=0; i < times; i++)
            {
                if (p.NextToken != null)
                    p = p.NextToken;
                else
                    return null;
            }

            return p.token;
        }

        /// <summary>
        /// メソッド名の候補となる文字列から、メソッド名として取り出せるかを
        /// </summary>
        /// <param name="candidateStr"></param>
        /// <param name="elementId"></param>
        /// <returns></returns>
        private MethodVO tryGetMethodIdFromCandidateString(string candidateStr, int elementId)
        {
            if (candidateStr == null)
            {
                return null;
            }

            int posOfDot = candidateStr.IndexOf(".");
            int posOfLeftParen = candidateStr.IndexOf("(");

            // "."文字が含まれる、かつ "."の位置が "(" よりも左(小さい)
            //  ⇒ クラス名.メソッド名(パラメータ)形式と解釈
            if (posOfDot >= 0 && posOfDot < posOfLeftParen)
            {
                // 文字列の最初から "(" の前までの名前をキーとして全メソッドのMapを探索
                string methodNameCandidate = candidateStr.Substring(0, posOfLeftParen);
                if (allMethodMap.ContainsKey(methodNameCandidate))
                {
                    // 見つかったメソッドVOを返却
                    return allMethodMap[methodNameCandidate];
                }
            }


            // "."文字が含まれる、かつ "."の位置が最後２文字よりも左
            //  ⇒ クラス名.メソッド名 形式と解釈（たまにメソッド呼び出しで()を付けてくれない奴もいるので）
            if (posOfDot >= 0 && posOfDot < candidateStr.Length - 2)
            {
                // 文字列の最初から "(" の前までの名前をキーとして全メソッドのMapを探索
                string methodNameCandidate = candidateStr;
                if (allMethodMap.ContainsKey(methodNameCandidate))
                {
                    // 見つかったメソッドVOを返却
                    return allMethodMap[methodNameCandidate];
                }
            }


            // "." が含まれず、かつ "(" が含まれる場合
            //  ⇒ 同クラス内の呼び出しでメソッド名のみでの呼び出しを行うと解釈
            else if (posOfDot < 0 && posOfLeftParen >= 0)
            {
                if (this.elementOwnMethodsMap.ContainsKey(elementId))
                {
                    List<MethodVO> MethodsInSameElement = this.elementOwnMethodsMap[elementId];

                    string methodNameCandidate = candidateStr.Substring(0, posOfLeftParen);
                    foreach( MethodVO neighborMethod in MethodsInSameElement)
                    {
                        if( methodNameCandidate == neighborMethod.name)
                        {
                            return neighborMethod; 
                        }
                    }
                }

                return null;
            }


            return null;
        }




        /// <summary>
        /// SQLiteのDBから属性、操作の値を取得
        /// </summary>
        /// <returns></returns>
        private List<BehaviorChunk> readBehaviorChunks(SQLiteConnection conn, int methodId)
        {

            List<BehaviorChunk> retList = new List<BehaviorChunk>();

            string fields = @"chunkId, methodId, pos, parentId, previousId, indLv, dottedNum, indent, behavior";
            string whereCond = @" methodId = " + methodId ;
            string sql =
                @"select " + fields +
                 " from t_parsed_behavior " +
                 " where " + whereCond +
                 " order by pos ";

            using (var command = conn.CreateCommand())
            {
                //クエリの実行
                command.CommandText = sql;
                using (var sdr = command.ExecuteReader())
                {
                    //
                    while (sdr.Read())
                    {
                        BehaviorChunk chk = new BehaviorChunk();

                        chk.chunkId = sdr.GetInt32(0);
                        chk.methodId = sdr.GetInt32(1);
                        chk.pos = sdr.GetInt32(2);
                        chk.parentChunkId = sdr.GetInt32(3);
                        chk.previousChunkId = sdr.GetInt32(4);
                        chk.indLv = sdr.GetInt32(5);
                        chk.dottedNum = sdr.GetString(6);
                        chk.indent = sdr.GetString(7);
                        chk.behavior = sdr.GetString(8);

                        retList.Add(chk);
                    }
                }
            }

            // 一件でも取得できたらtrueを返す
            return retList;
        }



        /// <summary>
        /// SQLiteのDBから属性、操作の値を取得
        /// </summary>
        /// <returns></returns>
        private List<AttrMthSearchItem> readAttributesAndMethods()
        {

            string dbFileName = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + dbFileName);

            conn.Open();


            List<AttrMthSearchItem> retList = new List<AttrMthSearchItem>();

            string fields = @"atmt.elemId, elm.elemName, elm.elemAlias, elm.elemType, elm.elemStereotype,
                atmt.elemGuid, atmt.attrMthFlg, atmt.attrMthId, atmt.attrMthType,
                atmt.attrMthGuid, atmt.attrMthName, atmt.attrMthAlias, atmt.attrMthNotes,
                ifnull(atmt.mthParamDesc, ''), elm.elementPath";

            string whereCond = @" elm.elemType in ('Class', 'Interface', 'Enumeration')
                   and  elm.elementPath like '%/論理モデル/%' ";

            string sql =
                @"select " + fields +
                   " from t_attr_mth atmt inner join t_element elm on atmt.elemId = elm.objectId " +
                   " where  " + whereCond +
                   " order by atmt.elemId, atmt.attrMthId ";

            using (var command = conn.CreateCommand())
            {
                //クエリの実行
                command.CommandText = sql;
                using (var sdr = command.ExecuteReader())
                {
                    //
                    while (sdr.Read())
                    {
                        AttrMthSearchItem attrMth = new AttrMthSearchItem();

                        attrMth.elemId = sdr.GetInt32(0);
                        attrMth.elemName = StringUtil.removeQuoteChar(sdr.GetString(1));
                        attrMth.elemAlias = sdr.GetString(2);
                        attrMth.elemType = sdr.GetString(3);
                        attrMth.elemStereotype = sdr.GetString(4);
                        attrMth.elemGuid = sdr.GetString(5);
                        attrMth.attrMethFlg = sdr.GetString(6);

                        if (attrMth.attrMethFlg == "a")
                            attrMth.attrMethId = sdr.GetInt32(7) * -1;
                        else
                            attrMth.attrMethId = sdr.GetInt32(7);

                        attrMth.attrMethType = sdr.GetString(8);
                        attrMth.attrMethGuid = sdr.GetString(9);
                        attrMth.attrMethName = StringUtil.removeQuoteChar(sdr.GetString(10));
                        attrMth.attrMethAlias = sdr.GetString(11);
                        attrMth.attrMethNotes = sdr.GetString(12);
                        attrMth.methParameterDesc = sdr.GetString(13);
                        attrMth.elementPath = sdr.GetString(14);

                        retList.Add(attrMth);
                    }
                }
            }

            conn.Close();

            // 一件でも取得できたらtrueを返す
            return retList;
        }


    }
}
