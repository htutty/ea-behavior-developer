using System;
using System.Collections.Generic;
using System.IO;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using IndexAccessor;

namespace AsciidocGenerator
{
    
    /// <summary>
    /// 成果物単位Asciidoc出力用クラス
    /// </summary>
    public class ArtifactAsciidocWriter
    {
        private ElementSearcher elementSearcher;
        private ConnectorSearcher connSearcher;
        private string asciidocDir;

        public ArtifactAsciidocWriter(string asciidocDir)
        {
            string dbFilePath = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;
            this.elementSearcher = new ElementSearcher(dbFilePath);
            this.connSearcher = new ConnectorSearcher(dbFilePath);

            this.asciidocDir = asciidocDir;

            // Asciidoc出力フォルダの存在チェック＆無ければ作成
            makeAsciidocDirIfNotExist(asciidocDir);
        }


        /// <summary>
        /// Asciidoc出力用フォルダを作る。
        /// 先に存在チェックし、なかった場合だけフォルダ作成を行う。
        /// </summary>
        /// <param name="asciidocDir">出力先asciidocパス</param>
        private static void makeAsciidocDirIfNotExist(string asciidocDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(asciidocDir))
            {
                Directory.CreateDirectory(asciidocDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + asciidocDir);
            }
        }



        /// <summary>
        /// デフォルトモード（PREVIEW）でのAsciiDoc出力
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="filepath"></param>
        public void outputAsciidocFile(ArtifactVO artifact, string adocFileName)
        {
            outputAsciidocFile(artifact, adocFileName, AsciidocOutputMode.MODE_PREVIEW);
        }


        /// <summary>
        /// AsciiDocファイルの出力（出力モード指定付き）
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="filepath"></param>
        /// <param name="mode">出力モード</param>
        public void outputAsciidocFile(ArtifactVO artifact, string adocFileName, AsciidocOutputMode mode)
        {

            try
            {
                string filepath = this.asciidocDir + "\\" + adocFileName;

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
        private static string getAsciiDocHeader(AsciidocOutputMode mode)
        {
            string defaultHeader = ":sectnums: \r\n:chapter-label: \r\n:toc: left\r\n:toclevels: 2\r\n:table - caption: 表\r\n";
            string pdfHeader = ":pdf-fontsdir: fonts\r\n:pdf-stylesdir: theme\r\n:pdf-style: public\r\n";
            string htmlHeader = ":stylesdir: stylesheets/\r\n:stylesheet: asciidoctor-custom.css\r\n";

            switch (mode)
            {
                case AsciidocOutputMode.MODE_PREVIEW:
                    return defaultHeader + "\r\n" ;
                case AsciidocOutputMode.MODE_HTML:
                    return defaultHeader + htmlHeader + "\r\n";
                case AsciidocOutputMode.MODE_PDF:
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

            // 要素別AsciiDocファイルの出力先を指定
            string elementsDir = ProjectSetting.getVO().projectPath + "\\elements";

            // 直下に出力すべき要素が１つも存在しないパッケージは出力の対象外とする（ドキュメント内での項番も不要）
            if (countPrintableElement(package) > 0)
            {
                sw.WriteLine("## パッケージ: " + pathName);
                sw.WriteLine("// GUID: " + package.guid);
                sw.WriteLine("");

                // このパッケージ直下にダイアグラムを持っていたら
                if (package.diagrams != null && package.diagrams.Count > 0)
                {
                    foreach (DiagramVO diag in package.diagrams)
                    {
                        // writeDiagramData(diag, sw);
                        writeDiagramPlantUml(diag, sw);
                    }
                }

                foreach (ElementVO elem in package.elements)
                {
                    if (elem.eaType == "Class" || elem.eaType == "Interface" || elem.eaType == "Enumeration" || elem.eaType == "UseCase" || elem.eaType == "Actor")
                    {
                        //ElementAsciidocWriter.writeElement(elem, sw);
                        string relPath = ElementAsciidocWriter.doWrite(elementsDir, elem);
                        writeElementInclude(sw, relPath);
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
        /// ダイアグラム情報を出力する
        /// </summary>
        /// <param name="diag"></param>
        /// <param name="sw"></param>
        private void writeDiagramData(DiagramVO diag, StreamWriter sw)
        {
            sw.WriteLine("### ダイアグラム: " + diag.name);

            foreach(DiagramObjectVO diaObj in diag.diagramObjects)
            {
                ElementSearchItem searchItem = elementSearcher.findByElementId(diaObj.objectId);

                if (searchItem != null)
                {
                    sw.WriteLine(" - " + searchItem.elemName + " " + searchItem.elemGuid);
                }
            }

            sw.WriteLine("\r\n");
        }


        /// <summary>
        /// 要素単位のasciidocを参照するinclude文を成果物asciidoc内に出力する
        /// </summary>
        /// <param name="sw">出力StreamWriter</param>
        /// <param name="relPath">要素asciidocへの相対パス(成果物asciidocの場所をカレントとする)</param>
        private static void writeElementInclude(StreamWriter sw, string relPath)
        {
            sw.WriteLine("include::" + relPath + "[]");
            sw.WriteLine("");
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
                    if (elem.eaType == "Class" || elem.eaType == "Interface" || elem.eaType == "Enumeration" || elem.eaType == "UseCase" || elem.eaType == "Actor")
                    {
                        elemCount++;
                    }
                }
                return elemCount;
            }
            else
            {
                if (package.diagrams != null && package.diagrams.Count > 0)
                {
                    return package.diagrams.Count;
                }

                return 0;
            }

        }



        private void writeDiagramPlantUml(DiagramVO diag, StreamWriter sw)
        {
            sw.WriteLine("### ダイアグラム: " + diag.name);

            sw.WriteLine("");
            sw.WriteLine("[plantuml, img-diag-" + diag.guid.Substring(1, 13) + ", svg]");
            sw.WriteLine("--");
            sw.WriteLine("@startuml");

            var elemNameHash = new Dictionary<int, string>();

            foreach (DiagramObjectVO diaObj in diag.diagramObjects)
            {
                // ダイアグラムオブジェクトのobjectIdをキーとしてt_elementから要素を取りに行く
                ElementSearchItem elemSearchItem = this.elementSearcher.findByElementId(diaObj.objectId);
                // 取得できなかったらインデックスに登録される対象の要素タイプではないので、class文を出す必要はない
                if (elemSearchItem != null)
                {
                    // クラス図の中で表示されるクラス名を取得(空白を別の文字に置換するなど)
                    string normalizedName = filterSpecialChar(elemSearchItem.elemName);
                    // 接続線を引く時にこのクラス名を使う必要があるので、ID毎に名前をキャッシュしておく
                    elemNameHash.Add(elemSearchItem.elementId, normalizedName);

                    // class 文の出力
                    sw.WriteLine("class \"" + normalizedName + "\" " + getStereotypeStr(elemSearchItem.elemStereotype) + " {");
                    sw.WriteLine("}");
                }

            }

            // ダイアグラムリンク（ダイアグラム上で有効な接続）情報を元にPlantUMLの接続線を引く
            foreach (DiagramLinkVO diaLink in diag.diagramLinks) {
                // diagramLink の connectorId をキーとして Connector情報を取得
                List<ConnectorVO> conns = this.connSearcher.findByConnectorId(diaLink.connectorId);
                ConnectorVO targetConn = null;
                if (conns.Count > 0)
                {
                    targetConn = conns[0];
                    outputSrcConnectLine(targetConn, sw);
                }

            }

            sw.WriteLine("@enduml");
            sw.WriteLine("--");
            sw.WriteLine("");
        }


        private static void outputConnDestClass(string objGuid, ElementSearcher elemSrch, Dictionary<string, ElementSearchItem> nameHash, StreamWriter sw)
        {
            ElementSearchItem elemSrcItem = elemSrch.findByGuid(objGuid);
            if (elemSrcItem != null)
            {
                string identifiedName = filterSpecialChar(elemSrcItem.elemName);
                if (!nameHash.ContainsKey(identifiedName))
                {
                    sw.WriteLine("class \"" + identifiedName + "\" " + getStereotypeStr(elemSrcItem.elemStereotype) + " {");
                    sw.WriteLine("}");
                    nameHash.Add(identifiedName, elemSrcItem);
                }
            }
        }


        private static void outputSrcConnectLine(ConnectorVO cn, StreamWriter sw)
        {
            string leftSideName, rightSideName, lineStyle;

            leftSideName = cn.srcObjName;
            rightSideName = cn.destObjName;

            switch (cn.connectorType)
            {
                case "Association":
                    lineStyle = " --> ";
                    break;
                case "Dependency":
                    lineStyle = " ..> ";
                    break;
                case "Generalization":
                    leftSideName = cn.destObjName;
                    rightSideName = cn.srcObjName;
                    lineStyle = " <|-- ";
                    break;
                case "Aggregation":
                    leftSideName = cn.destObjName;
                    rightSideName = cn.srcObjName;
                    lineStyle = " o-- ";
                    break;
                case "Realisation":
                    leftSideName = cn.destObjName;
                    rightSideName = cn.srcObjName;
                    lineStyle = " <|.. ";
                    break;
                case "NoteLink": // ノートリンクは出力の価値が無いので出力しないでリターン
                    return;

                default:
                    lineStyle = " --- ";
                    break;
            }

            sw.WriteLine("\"" + filterSpecialChar(leftSideName) + "\" " + lineStyle + "\"" + filterSpecialChar(rightSideName) + "\" ");
        }


        private static void outputDestConnectLine(ConnectorVO cn, StreamWriter sw)
        {
            string leftSideName, rightSideName, lineStyle;

            leftSideName = cn.srcObjName;
            rightSideName = cn.destObjName;

            switch (cn.connectorType)
            {
                case "Association":
                    lineStyle = " --> ";
                    break;
                case "Dependency":
                    lineStyle = " ..> ";
                    break;
                case "Generalization":
                    leftSideName = cn.destObjName;
                    rightSideName = cn.srcObjName;
                    lineStyle = " <|-- ";
                    break;
                case "Aggregation":
                    leftSideName = cn.destObjName;
                    rightSideName = cn.srcObjName;
                    lineStyle = " o-- ";
                    break;
                case "Realisation":
                    leftSideName = cn.destObjName;
                    rightSideName = cn.srcObjName;
                    lineStyle = " <|.. ";
                    break;
                case "NoteLink": // ノートリンクは出力の価値が無いので出力しないでリターン
                    return;

                default:
                    lineStyle = " --- ";
                    break;
            }

            sw.WriteLine("\"" + filterSpecialChar(leftSideName) + "\" " + lineStyle + "\"" + filterSpecialChar(rightSideName) + "\" ");
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
            sb.Replace("\r", "");
            sb.Replace("\n", "");

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
