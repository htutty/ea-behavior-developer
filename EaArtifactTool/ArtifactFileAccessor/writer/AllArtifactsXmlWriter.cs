using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ArtifactFileAccessor.writer
{
    public class AllArtifactsXmlWriter
    {

        public static void outputAllArtifactsFile(string outputDir, ArtifactsVO artifacts)
        {
            StreamWriter sw = null;

            try
            {
                //BOM無しのUTF8でテキストファイルを作成する
                sw = new StreamWriter(outputDir + "\\AllArtifacts.xml");
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                sw.WriteLine("");

                outputArtifactNodes(artifacts, sw);
            }
            catch (Exception ex)
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine("stacktrace: ");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (sw != null) sw.Close();
            }

        }

        /// <summary>
        /// 成果物を示すノード artifacts, artifact をXML出力する
        /// </summary>
        /// <param name="artifacts">成果物リスト要素</param>
        /// <param name="sw">StreamWriter</param>
        private static void outputArtifactNodes(ArtifactsVO artifacts, StreamWriter sw)
        {
            // Console.WriteLine("start outputArtifacts()")

            sw.Write("<artifacts ");
            sw.Write(" targetProject='" + artifacts.targetProject + "' ");
            //	sw.Write(" created='" + artifacts.Created + "' ");
            sw.Write(" lastUpdated='" + artifacts.lastUpdated + "' ");
            sw.Write(" targetModel='" + artifacts.targetModel + "' ");
            sw.WriteLine(" >");

            foreach (ArtifactVO atf in artifacts.artifactList ) {
                sw.Write("  <artifact ");
                sw.Write(" guid='" + atf.guid + "' ");
                sw.Write(" name='" + StringUtil.escapeXML(atf.name) + "' ");
                sw.Write(" project='" + StringUtil.escapeXML(atf.projectName) + "' ");
                sw.Write(" stereotype='" + StringUtil.escapeXML(atf.stereoType) + "' ");
                sw.WriteLine(">");
                sw.WriteLine("    <pathName>" + StringUtil.escapeXML(atf.pathName) + "</pathName>");
                sw.WriteLine("    <asciidocFile> " + StringUtil.escapeXML(atf.asciidocFilePath) + "</asciidocFile>");
                sw.WriteLine("  </artifact>");
            }

            sw.WriteLine("</artifacts>");
        }


    }
}
