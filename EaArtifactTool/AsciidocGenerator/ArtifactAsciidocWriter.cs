using System;
using System.Collections.Generic;
using System.IO;
using ArtifactFileAccessor.vo;
using IndexAccessor;

namespace AsciidocGenerator
{
    public class ArtifactAsciidocWriter
    {
        private ArtifactVO Artifact;

        public ArtifactAsciidocWriter(ArtifactVO artifact)
        {
            this.Artifact = artifact;
        }

        public void writeFile(string filepath)
        {

            try
            {
                //BOM無しのUTF8でテキストファイルを作成する
                StreamWriter atfsw = new StreamWriter(filepath);
                atfsw.WriteLine(@":sectnums:
:chapter-label:
:toc: left
:toclevels: 2
:table-caption: 表
:stylesdir: stylesheets/
:stylesheet: asciidoctor-custom.css
// :pdf-fontsdir: fonts
// :pdf-stylesdir: theme
// :pdf-style: public

");

                atfsw.WriteLine("# モデル成果物詳細 " + Artifact.name);
                atfsw.WriteLine("");

                // atfsw.WriteLine("## パッケージ: " + pathName);
                // atfsw.WriteLine("// GUID: " + package.guid);
                // atfsw.WriteLine("");


                // 配下のパッケージを出力
                writePackage(Artifact.package, atfsw, "");

                atfsw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="sw"></param>
        /// <param name="packagePath"></param>
        private void writePackage(PackageVO package, StreamWriter sw, string packagePath)
        {
            string pathName = "";
            if (packagePath != "")
            {
                pathName = packagePath + "/" + package.name;
            }
            else
            {
                pathName = package.name;
            }

            // 直下に出力すべき要素が１つも存在しないパッケージは出力の対象外とする（ドキュメント内での項番も不要）
            if (countPrintableElement(package) > 0)
            {
                sw.WriteLine("## パッケージ: " + pathName);
                sw.WriteLine("// GUID: " + package.guid);
                sw.WriteLine("");

                foreach (ElementVO elem in package.elements)
                {
                    if (elem.eaType == "Class" || elem.eaType == "Interface" || elem.eaType == "Enumeration")
                    {
                        ElementAsciidocWriter.writeElement(elem, sw);
                    }
                }

                sw.WriteLine("<<<");
                sw.WriteLine("");
            }

            if (package.childPackageList != null && package.childPackageList.Count > 0)
            {
                foreach (PackageVO pkg in package.childPackageList)
                {
                    writePackage(pkg, sw, pathName);
                }
            }
        }

        /// <summary>
        /// ドキュメント出力対象の要素を数える
        /// </summary>
        /// <param name="package">対象パッケージ</param>
        /// <returns>パッケージ直下の要素数</returns>
        private int countPrintableElement(PackageVO package)
        {
            int elemCount = 0;
            if (package.elements != null && package.elements.Count > 0)
            {
                foreach (ElementVO elem in package.elements)
                {
                    if (elem.eaType == "Class" || elem.eaType == "Interface" || elem.eaType == "Enumeration")
                    {
                        elemCount++;
                    }
                }
                return elemCount;
            }
            else
            {
                return 0;
            }

        }


        private string filterSpecialChar(string orig)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
            sb.Replace("(", "（");
            sb.Replace(")", "）");
            // sb.Replace("（", "〈");
            // sb.Replace("）", "〉");
            sb.Replace(" ", "_");
            // sb.Replace("-", "_");
            sb.Replace("^", "＾");
            sb.Replace("？", "");

            return sb.ToString();
        }


        private string getVisibilitySymbol(string visibility)
        {
            switch (visibility)
            {
                case "Public": return "+";
                case "Private": return "-";
                case "Protected": return "#";
                default: return " ";
            }
        }


        private string getStereotypeStr(string stereoType)
        {
            if (stereoType != null && stereoType != "")
            {
                return " << " + stereoType + " >> ";
            }
            else
            {
                return "";
            }
        }


    }


}
