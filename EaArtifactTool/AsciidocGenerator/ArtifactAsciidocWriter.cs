using System;
using System.Collections.Generic;
using System.IO;
using ArtifactFileAccessor.vo;
using IndexAccessor;

namespace AsciidocGenerator
{
    /// <summary>
    /// AsciiDocの出力モード
    /// </summary>
    public enum OutputMode
    {
        MODE_PDF,
        MODE_HTML,
        MODE_PREVIEW
    }


    public class ArtifactAsciidocWriter
    {

        /// <summary>
        /// デフォルトモード（PREVIEW）でのAsciiDoc出力
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="filepath"></param>
        public static void outputAsciidocFile(ArtifactVO artifact, string filepath)
        {
            outputAsciidocFile(artifact, filepath, OutputMode.MODE_PREVIEW);
        }


        /// <summary>
        /// AsciiDocファイルの出力
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="filepath"></param>
        public static void outputAsciidocFile(ArtifactVO artifact, string filepath, OutputMode mode)
        {

            try
            {
                //BOM無しのUTF8でテキストファイルを作成する
                StreamWriter atfsw = new StreamWriter(filepath);

                // 出力モードによって異なるAsciiDocのヘッダーを出力する
                atfsw.WriteLine(getAsciiDocHeader(mode));

                atfsw.WriteLine("# モデル成果物詳細 " + artifact.name);
                atfsw.WriteLine("");

                // 配下のパッケージを出力
                writePackage(artifact.package, atfsw, "");

                atfsw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 出力モードに応じて適切なAsciiDocのヘッダー文字列を返却する
        /// </summary>
        /// <param name="mode">OutputMode.MODE_PREVIEW/MODE_HTML/MODE_PDFから選択</param>
        /// <returns>ヘッダー文字列</returns>
        private static string getAsciiDocHeader(OutputMode mode)
        {
            string defaultHeader = ":sectnums: \r\n:chapter-label: \r\n:toc: left\r\n:toclevels: 2\r\n:table - caption: 表\r\n";
            string pdfHeader = ":pdf-fontsdir: fonts\r\n:pdf-stylesdir: theme\r\n:pdf-style: public\r\n";
            string htmlHeader = ":stylesdir: stylesheets/\r\n:stylesheet: asciidoctor-custom.css\r\n";

            switch (mode)
            {
                case OutputMode.MODE_PREVIEW:
                    return defaultHeader + "\r\n" ;
                case OutputMode.MODE_HTML:
                    return defaultHeader + htmlHeader + "\r\n";
                case OutputMode.MODE_PDF:
                    return defaultHeader + pdfHeader + "\r\n";
                default:
                    return "";
            }
         
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="sw"></param>
        /// <param name="packagePath"></param>
        private static void writePackage(PackageVO package, StreamWriter sw, string packagePath)
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
        private static int countPrintableElement(PackageVO package)
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

        /// <summary>
        /// ファイル名などに使えない特殊文字を別の文字に置き換える
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static string filterSpecialChar(string orig)
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

        /// <summary>
        /// visibilityの文字列からPlantUMLのvisibility指定 "+", "-", "#" に変換する
        /// </summary>
        /// <param name="visibility"></param>
        /// <returns></returns>
        private static string getUmlVisibilitySymbol(string visibility)
        {
            switch (visibility)
            {
                case "Public": return "+";
                case "Private": return "-";
                case "Protected": return "#";
                default: return " ";
            }
        }


        private static string getStereotypeStr(string stereoType)
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
