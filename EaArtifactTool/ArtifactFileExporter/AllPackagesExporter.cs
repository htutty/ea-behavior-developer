using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArtifactFileExporter
{
    class AllPackagesExporter
    {

        private string outputDir;

        public AllPackagesExporter(string outputDir)
        {
            this.outputDir = outputDir;
        }


        public void outputPackageXml(List<PackageVO> allPackages)
        {

            StreamWriter sw = null;

            try
            {
                //BOM無しのUTF8でテキストファイルを作成する
                sw = new StreamWriter(outputDir + "\\AllPackageTree.xml");
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                sw.WriteLine("");

                sw.WriteLine("<AllPackage>");
                outputPackageTree(allPackages, 1, "", sw);
                sw.WriteLine("</AllPackage>");
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



        private void outputPackageTree(List<PackageVO> packageList, int depth, string ppath, StreamWriter sw)
        {

            foreach (PackageVO pac in packageList)
            {
                pac.pathName = ppath + "/" + pac.name;

                sw.Write(StringUtil.indent(depth) + "<package");
                sw.Write(" PackageID='" + pac.packageId + "'");
                sw.Write(" guid='" + StringUtil.escapeXML(pac.guid) + "'");
                sw.Write(" TPos='" + pac.treePos + "'");
                sw.Write(" name='" + StringUtil.escapeXML(pac.name) + "'");
                sw.Write(" Alias='" + StringUtil.escapeXML(pac.alias) + "'");
                sw.WriteLine(">");

                sw.WriteLine(StringUtil.indent(depth + 1) + StringUtil.escapeXML(pac.pathName) );

                if (pac.childPackageList != null)
                {
                    outputPackageTree(pac.childPackageList, depth + 1, pac.pathName, sw);
                }

                sw.WriteLine(StringUtil.indent(depth) + "</package>" );
            }

        }





    }
}
