using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ArtifactFileExporter
{
    /// <summary>
    /// 全成果物リストを受け、配下の要素のノートと操作のふるまいをテキスト出力するためのクラス
    /// </summary>
    class AllBehaviorsWriter
    {
        private string outputDir;

        public AllBehaviorsWriter(string outputDir)
        {
            this.outputDir = outputDir;
        }


        /// <summary>
        /// 成果物配下のクラス（インタフェース、列挙）の操作のふるまい(method.behavior)をテキストに出力する
        /// </summary>
        /// <param name="artifacts">全成果物リスト</param>
        public void outputBehaviorsText(ArtifactsVO artifacts)
        {
            Console.WriteLine("start outputBehaviorsText():");

            StreamWriter csvsw = null;

            try
            {
                csvsw = new StreamWriter(outputDir + "\\AllBehaviors.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

                // 成果物のうち実装モデルを除いたものが対象
                foreach (var atf in artifacts.getArtifactsExcludeImplModel())
                {
                    retrieveElementsInArtifact(atf, csvsw);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (csvsw != null) csvsw.Close();
            }

        }

        private ElementVO parsingElement;
        private MethodVO parsingMethod;


        /// <summary>
        /// 要素リストを受け、要素のノートと操作のふるまいをテキスト出力する。
        /// テキスト解析に使用する。
        /// </summary>
        /// <param name="atf">対象成果物VO</param>
        /// <param name="elements">要素の</param>
        /// <param name="sw"></param>
        private void retrieveElementsInArtifact(ArtifactVO atf, StreamWriter sw)
        {
            foreach( ElementVO elem in atf.getOwnElements())
            {
                if( elem.methods != null )
                {
                    parsingElement = elem;
                    foreach ( MethodVO mth in elem.methods )
                    {
                        parsingMethod = mth;
                        outputText(mth.behavior, sw);
                    }
                }
            }
        }

        /// <summary>
        /// テキストファイルに出力する
        /// </summary>
        /// <param name="multiRowsText"></param>
        /// <param name="sw"></param>
        private void outputText(string multiRowsText, StreamWriter sw)
        {
            string[] delimiter = { "\r\n" };
            string[] lins = multiRowsText.Split(delimiter, StringSplitOptions.None);

            List<BehaviorChunk> chunkList = parseBehavior(lins, sw);

            Boolean dottedFlg = false;

            for(int i=0; i < chunkList.Count; i++)
            {
                var chunk = chunkList[i];

                if( chunk.dottedNum != null )
                {
                    dottedFlg = true;
                }

                if( dottedFlg )
                {
                    for(int j=i+1; j < chunkList.Count; j++)
                    {
                        BehaviorChunk nextChunk = chunkList[j];

                        if(nextChunk.dottedNum == null)
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

            }


            for (int i = 0; i < chunkList.Count; i++)
            {
                var chunk = chunkList[i];

                if (chunk.hasFollower)
                {
                    for (int j = i + 1; j < chunkList.Count; j++)
                    {
                        BehaviorChunk nextChunk = chunkList[j];
                        if (i == nextChunk.followeeIdx)
                        {
                            chunk.behavior = chunk.behavior + "#\\n#" + nextChunk.behavior;
                        }
                    }

                }

            }



            // sw.WriteLine(multiRowsText);

        }

        private int chunkCount = 1;

        /// <summary>
        ///
        /// </summary>
        /// <param name="tlin"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private List<BehaviorChunk> parseBehavior(string[] tlin, StreamWriter sw)
        {
            Console.WriteLine("start parsedBehavior()");

            List<BehaviorChunk> chunkList = new List<BehaviorChunk>();

            for (int idx = 0; idx < tlin.Length; idx++)
            {
                string l = tlin[idx];

                // ハイフンだけの行は出力外
                Match matche = Regex.Match(l, "^-----*");
                if(matche.Success)
                {
                    continue;
                }

                // "１．２．３"の部分のあるふるまい行は、行番号部分を抽出する
                string trm = l.Trim(' ', '　');
                matche = Regex.Match(trm, "^[０-９][０-９．]*");
                if (matche.Success)
                {
                    string bodyText = trm.Substring(matche.Value.Length, trm.Length - matche.Value.Length);

                    BehaviorChunk chunk = new BehaviorChunk();
                    chunk.pos = idx + 1;
                    chunk.behavior = bodyText;
                    chunk.dottedNum = matche.Value;
                    chunk.indent = null;
                    chunk.methodId = parsingMethod.methodId;

                    chunkList.Add(chunk);
                }
                else
                {
                    // そうでないふるまい行は先頭の空白を行番号とする
                    matche = Regex.Match(l, "^[　 ]*");

                    BehaviorChunk chunk = new BehaviorChunk();
                    chunk.pos = idx + 1;
                    chunk.behavior = trm;
                    chunk.dottedNum = null;
                    chunk.indent = matche.Value;
                    chunk.methodId = parsingMethod.methodId;

                    chunkList.Add(chunk);
                }

            }

            return chunkList;
        }


        private void writeLine(string str, StreamWriter sw)
        {
            string trm = str.Trim(' ', '　');

            if (trm.Length > 0)
            {
                sw.WriteLine("{0}\t{{ \"chunkId\": {1}, \"behavior\": \"{2}\" }}", chunkCount, chunkCount, escapeJson(trm));
                chunkCount++;
            }
        }


        private void writeLine(string dottedNum, string str, StreamWriter sw)
        {
            string trm = str.Trim(' ', '　');

            if (trm.Length > 0)
            {
                sw.WriteLine("{0}\t{{ \"chunkId\": {1}, \"methodId\": {2},  \"dottedNum\": \"{3}\",  \"behavior\": \"{4}\" }}",
                    chunkCount, chunkCount, parsingMethod.methodId, dottedNum, escapeJson(trm));
                chunkCount++;
            }

        }

        private string escapeJson(string orig)
        {
            StringBuilder sb = new StringBuilder(orig);
            sb.Replace("\\", "\\\\");
            sb.Replace("\"", "\\\"");
            sb.Replace("/", "\\/");

            return sb.ToString();
        }


    }
}
