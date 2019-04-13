using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectDiffMaker
{
    class ChangedArtifactXmlWriter
    {
        private ArtifactVO procArtifact;

        /// <summary>
        /// マージ済みの成果物情報を各ファイルに出力する
        /// </summary>
        public void writeChagedArtifacts(string outputDir, List<ArtifactVO> outArtifacts)
        {
            Console.WriteLine("outputMerged: outputDir=" + outputDir);

            string artifactDir =outputDir + @"\artifacts";

            // 差分のdetailファイルが出力されるフォルダを事前に作成する
            makeDetailDirIfNotExist(artifactDir + "\\detail");

            //BOM無しのUTF8でテキストファイルを作成する
            StreamWriter listsw = new StreamWriter(artifactDir + "\\ChangedArtifacts.xml");
            listsw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?> ");
            listsw.WriteLine("");

            listsw.Write("<artifacts ");
            listsw.Write(" targetProject='asw' ");
            //			listsw.Write( " lastUpdated='" + artifacts.LastUpdated + "' " );
            //			listsw.Write( " targetModel='" + artifacts.TargetModel + "' " );
            listsw.WriteLine(" >");

            foreach (ArtifactVO atf in outArtifacts)
            {
                // artifactのchangedが空白以外なら変更ありとみなす
                if (atf.changed != ' ')
                {
                    // このオブジェクト内で現在処理中の成果物を更新する
                    this.procArtifact = atf;
                    // ChangedArtifacts.xml に出力する
                    outputChangedArtifactList(atf, listsw);

                    // Artifact別のXMLファイルに出力する
                    //BOM無しのUTF8でテキストファイルを作成する
                    StreamWriter atfsw = new StreamWriter(artifactDir + "\\atf_" + atf.guid.Substring(1, 36) + ".xml");
                    atfsw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                    atfsw.WriteLine("");

                    if (atf.package != null)
                    {
                        atfsw.Write("<artifact ");
                        atfsw.Write(" changed='" + atf.changed + "' ");
                        atfsw.Write(" guid='" + atf.guid + "' ");
                        atfsw.Write(" name='" + StringUtil.escapeXML(atf.name) + "' ");
                        atfsw.Write(" path='" + StringUtil.escapeXML(atf.pathName) + "' ");
                        atfsw.Write(" project='asw' ");
                        atfsw.Write(" stereotype='" + atf.stereoType + "' ");
                        atfsw.WriteLine(">");

                        //sw.WriteLine("■成果物(" + atf.changed + "): GUID="+ atf.guid + ", Name=" + atf.name );
                        outputChangedPackage(atf.package, 2, atfsw);

                        atfsw.WriteLine("</artifact>");
                    }
                    atfsw.Close();
                }
            }

            listsw.WriteLine("</artifacts>");
            listsw.Close();
            

            // projectファイルの出力
            StreamWriter prjsw = new StreamWriter(outputDir + "\\project.bdprj");
            prjsw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?> ");
            prjsw.WriteLine("");

            prjsw.WriteLine("<project>");
            prjsw.WriteLine(@" <projectName>merged</projectName>
  <artifactsFile>ChangedArtifacts.xml</artifactsFile>
  <artifactsPath>artifacts</artifactsPath>
  <allConnector>AllConnectors.xml</allConnector>
  <dbName>merged.db</dbName>");

            prjsw.WriteLine("</project>");
            prjsw.Close();

        }

        private static void makeDetailDirIfNotExist(string detailDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(detailDir))
            {
                Directory.CreateDirectory(detailDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + detailDir);
            }
        }



        /// <summary>
        /// ChangedArtifacts.xml ファイルの成果物行出力
        /// </summary>
        /// <param name="atf">成果物</param>
        /// <param name="sw"></param>
        private void outputChangedArtifactList(ArtifactVO atf, StreamWriter sw)
        {
            sw.Write("  <artifact ");
            sw.Write(" changed='" + atf.changed + "' ");
            sw.Write(" guid='" + atf.guid + "' ");
            sw.Write(" name='" + StringUtil.escapeXML(atf.name) + "' ");
            sw.Write(" path='" + StringUtil.escapeXML(atf.pathName) + "' ");
            sw.Write(" project='asw' ");
            sw.Write(" stereotype='" + atf.stereoType + "' ");
            sw.WriteLine("/>");
        }


        /// <summary>
        /// マージXMLファイルの出力（パッケージ）
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="depth"></param>
        /// <param name="sw"></param>
        private void outputChangedPackage(PackageVO pkg, int depth, StreamWriter sw)
        {

            sw.Write(StringUtil.indent(depth) + "<package ");
            sw.Write(" guid='" + pkg.guid + "' ");
            sw.Write(" name='" + StringUtil.escapeXML(pkg.name) + "' ");
            sw.Write(" isControlled='" + pkg.isControlled + "' ");
            sw.Write(" changed='" + pkg.changed + "' ");
            sw.WriteLine(" >");

            if (pkg.childPackageList.Count > 0)
            {
                foreach (PackageVO p in pkg.childPackageList)
                {
                    outputChangedPackage(p, depth + 1, sw);
                }
            }

            if (pkg.elements.Count > 0)
            {
                outputElements(pkg.elements, depth + 1, sw);
            }

            sw.WriteLine(StringUtil.indent(depth) + "</package>");
        }


        /// <summary>
        /// マージXMLファイルの出力（要素）
        /// 2019年3月現在ではXMLの出力項目を統一するため、
        /// 共通処理の ElementXmlWriter を呼び出すようにしている
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="depth"></param>
        /// <param name="sw"></param>
        private void outputElements(List<ElementVO> elements, int depth, StreamWriter sw)
        {
            foreach (ElementVO elem in elements)
            {
                ElementXmlWriter.writeElementXml(elem, depth, sw);
            }
        }



    }
}
