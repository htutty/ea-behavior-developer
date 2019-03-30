using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ArtifactFileAccessor.util;

namespace ArtifactFileExporter
{
    /// <summary>
    /// 全成果物リストを受け、配下の要素のノートと操作のふるまいをテキスト出力するためのクラス
    /// </summary>
    class AllBehaviorsWriter
    {
        private string outputDir;

        private BehaviorParser behaviorParser = new BehaviorParser();

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
                    foreach ( MethodVO mth in elem.methods )
                    {
                        List<BehaviorChunk> chunks = behaviorParser.parseBehavior(elem, mth);

                        foreach (BehaviorChunk cnk in chunks)
                        {
                            writeChunkLine(cnk, sw);
                        }
                    }
                }
            }
        }


        private void writeChunkLine(BehaviorChunk cnk, StreamWriter sw)
        {
            sw.WriteLine("{0}\t{{ \"chunkId\": {1}, \"methodId\": {2},  \"dottedNum\": \"{3}\",  \"behavior\": \"{4}\""
                + ", \"pos\": {5}, \"parentId\": {6}, \"previousId\": {7} }}",
                cnk.chunkId, cnk.chunkId, cnk.methodId, cnk.dottedNum, escapeJson(cnk.behavior),
                cnk.pos, cnk.parentChunkId, cnk.previousChunkId);
            // chunkCount++;
        }

        private string escapeJson(string orig)
        {
            StringBuilder sb = new StringBuilder(orig);
            sb.Replace("\\", "\\\\");
            sb.Replace("\"", "\\\"");
            sb.Replace("/", "\\/");
            sb.Replace("\r", "\\r");
            sb.Replace("\n", "\\n");

            return sb.ToString();
        }


    }
}
