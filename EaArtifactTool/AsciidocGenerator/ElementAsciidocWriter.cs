using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using IndexAccessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AsciidocGenerator
{
    public class ElementAsciidocWriter
    {
        /// <summary>
        /// 要素毎のAsciidoc出力
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="elem"></param>
        public static void doWrite(string outputDir, ElementVO elem)
        {
            string filepath = outputDir + "\\elements\\" + elem.guid.Substring(1, 1) + "\\"
                + elem.guid.Substring(2, 1) + "\\" + elem.guid.Substring(1, 36) + ".adoc";

            // 要素XMLを出力するフォルダ (projectDir + /elements/ 配下) が存在するかを調べ、なければ作る
            string edir = outputDir + @"\elements\" + elem.guid.Substring(1, 1) + @"\" + elem.guid.Substring(2, 1);
            checkAndMakeElementDir(edir);

            try
            {
                //BOM無しのUTF8でテキストファイルを作成する
                StreamWriter sw = new StreamWriter(filepath);
                // atfsw.WriteLine(@":sectnums:");

                //
                writeElement(elem, sw);

                sw.Close();
            } catch( Exception ex ) {
                Console.WriteLine(ex.Message);
            }

        }


        private static void checkAndMakeElementDir(string edir)
        {
            if (!Directory.Exists(edir))
            {
                Directory.CreateDirectory(edir);
            }
        }


        /// <summary>
        /// 要素のAsciidoc出力(既に開いているストリームに出力)
        /// 成果物単位やパッケージ単位でのAsciidoc出力に対応
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sw"></param>
        public static void writeElement(ElementVO element, StreamWriter sw)
        {
            sw.WriteLine("//##BEGIN element GUID=" + element.guid + "##");
            string stereotypeStr = "";
            if (element.stereoType != null && element.stereoType != "")
            {
                //                stereotypeStr = "&#x00AB;" + element.stereoType + "&#x00BB;";
                stereotypeStr = "≪" + element.stereoType + "≫";
            }

            // sw.WriteLine("### 要素" + " (" + element.eaType + ") " + stereotypeStr + element.name );
            sw.WriteLine("### 要素:" + element.name);

            //sw.WriteLine("[horizontal]");
            sw.WriteLine("クラス宣言:: " + element.visibility + " " + element.eaType + " " + stereotypeStr + element.name + "[" + element.alias + "]");
            //sw.WriteLine("ステレオタイプ: " + element.stereoType);
            sw.WriteLine("");

            // ノートの出力
            if (element.notes != null && element.notes != "")
            {
                sw.WriteLine("- クラス概要 +");
                sw.WriteLine("[%hardbreaks]");
                sw.WriteLine("クラス概要 + " + element.notes + "\r\n");
            }

            // plantUMLでクラス図を出力するための情報を書き込む
            writePlantUml(element, sw);

            // タグ付き値の出力
            if (element.taggedValues != null && element.taggedValues.Count > 0)
            {
                writeTaggedValueElem(element.taggedValues, sw);
            }

            // 属性の内容の出力
            if (element.attributes != null && element.attributes.Count > 0)
            {
                // sw.WriteLine("- 属性 +");
                foreach (AttributeVO attr in element.attributes)
                {
                    writeAttribute(attr, sw);
                }
            }

            // 操作の内容の出力
            if (element.methods != null && element.methods.Count > 0)
            {
                // sw.WriteLine("- 操作 +");
                foreach (MethodVO mth in element.methods)
                {
                    writeMethod(mth, sw);
                }
            }
            sw.WriteLine("//##END element" + "##");
        }

        private static void writeTaggedValueElem(List<TaggedValueVO> elemTags, StreamWriter sw)
        {
            sw.WriteLine("//##BEGIN element-taggedvalues##");
            sw.WriteLine(".要素のタグ付き値");
            sw.WriteLine("[cols = \"1,3\"]");
            sw.WriteLine("|===");
            sw.WriteLine("| キー | 値");

            foreach (TaggedValueVO tv in elemTags)
            {
                sw.WriteLine("//##BEGIN taggedvalue GUID=" + tv.guid + "##");
                sw.Write("|" + tv.name);

                if (tv.tagValue == "<memo>")
                {
                    sw.Write(" |" + tv.notes);
                }
                else
                {
                    sw.Write(" |" + tv.tagValue);
                }

                sw.WriteLine("");
                sw.WriteLine("//##END taggedvalue##");
            }
            sw.WriteLine("|===");
            sw.WriteLine("");
            sw.WriteLine("//##END element-taggedvalues##");
        }


        private static void writeAttribute(AttributeVO attribute, StreamWriter sw)
        {
            sw.WriteLine("//##BEGIN attribute GUID=" + attribute.guid + "##");

            string stereotypeStr = "", staticFinalStr, defaultStr;
            if (attribute.stereoType != null && attribute.stereoType != "")
            {
                // stereotypeStr = "&#x00AB;" + attribute.stereoType + "&#x00BB;" ;
                stereotypeStr = "≪" + attribute.stereoType + "≫";
            }
            // sw.WriteLine("#### 属性 " + stereotypeStr + attribute.name + " [" + attribute.alias + "]");

            sw.WriteLine("#### 属性 " + attribute.name + " [" + attribute.alias + "]");
            sw.WriteLine("// +GUID: " + attribute.guid + "+");
            sw.WriteLine("");
            sw.WriteLine("##### 宣言");
            sw.WriteLine("");


            staticFinalStr = (attribute.isStatic ? "static" : "");
            if (attribute.isConst)
            {
                staticFinalStr = staticFinalStr + (staticFinalStr != "" ? " " : "");
                staticFinalStr = staticFinalStr + (staticFinalStr != "" ? " final" : "final"); ;
            }
            staticFinalStr = staticFinalStr + (staticFinalStr != "" ? " " : "");

            if (attribute.defaultValue != null && attribute.defaultValue != "")
            {
                defaultStr = " = " + attribute.defaultValue;
            }
            else
            {
                defaultStr = "";
            }

            sw.WriteLine("  " + stereotypeStr + " " + attribute.visibility + " " + staticFinalStr + attribute.eaType +
                          " " + attribute.name + defaultStr);
            // sw.WriteLine("可視性:" + attribute.visibility + " 型:" + attribute.eaType );
            // sw.WriteLine("ステレオタイプ: " + attribute.stereoType);
            sw.WriteLine("");

            // ノートの出力
            if (attribute.notes != null && attribute.notes != "")
            {
                sw.WriteLine("##### 属性概要");
                sw.WriteLine("");
                sw.WriteLine("[%hardbreaks]");
                sw.WriteLine(attribute.notes);
                // sw.WriteLine("");
            }


            // タグ付き値の出力
            if (attribute.taggedValues != null && attribute.taggedValues.Count > 0)
            {
                writeTaggedValueAttr(attribute.taggedValues, sw);
            }

            sw.WriteLine("");
            sw.WriteLine("//##END attribute##");
        }

        private static void writeTaggedValueAttr(List<TaggedValueVO> attributeTags, StreamWriter sw)
        {
            sw.WriteLine("##### タグ付き値");
            sw.WriteLine("");

            sw.WriteLine("[cols = \"1,3\"]");
            sw.WriteLine("|===");
            sw.WriteLine("| キー | 値");

            foreach (TaggedValueVO tv in attributeTags)
            {
                sw.Write("|" + tv.name);

                if (tv.tagValue == "<memo>")
                {
                    sw.Write(" |" + tv.notes);
                }
                else
                {
                    sw.Write(" |" + tv.tagValue);
                }
                sw.WriteLine("");
            }
            sw.WriteLine("|===");
        }


        private static void writeMethod(MethodVO method, StreamWriter sw)
        {
            sw.WriteLine("//##BEGIN method GUID=" + method.guid + "##");

            string stereotypeStr = "";
            if (method.stereoType != null && method.stereoType != "")
            {
                // stereotypeStr = "&#x00AB;" + method.stereoType + "&#x00BB;";
                stereotypeStr = "≪" + method.stereoType + "≫";
            }

            sw.WriteLine("#### 操作 " + stereotypeStr + method.name + "[" + method.alias + "]");
            sw.WriteLine("// +GUID: " + method.guid + "+");
            sw.WriteLine("");
            sw.WriteLine("##### 操作宣言");
            sw.WriteLine("");
            sw.WriteLine("  " + method.visibility + " " + method.returnType + " " + stereotypeStr + method.name + " (" + method.getParamDesc() + ")");
            sw.WriteLine("");

            // パラメータの出力
            if (method.parameters != null && method.parameters.Count > 0)
            {
                writeParameters(method.parameters, sw);
            }

            // ノートの出力
            if (method.notes != null && method.notes != "")
            {
                sw.WriteLine("#####  メソッドの概要");
                sw.WriteLine("");
                sw.WriteLine("[%hardbreaks]");
                sw.WriteLine(method.notes);
                sw.WriteLine("");
                // sw.WriteLine("[%hardbreaks]");
                // sw.WriteLine("メソッド概要:: " + method.notes + "\r\n");
            }

            // タグ付き値の出力
            if (method.taggedValues != null && method.taggedValues.Count > 0)
            {
                writeTaggedValueMth(method.taggedValues, sw);
            }

            // ふるまいの出力
            if (method.behavior != null && method.behavior != "")
            {
                sw.WriteLine("##### ふるまい");
                sw.WriteLine("");
                sw.WriteLine("------");
                sw.WriteLine(getParsedBehavior(method));
                sw.WriteLine("------");

                sw.WriteLine("");
            }

            sw.WriteLine("//##END method##");
        }


        private static string getParsedBehavior(MethodVO mth)
        {
            BehaviorParser parser = new BehaviorParser();
            List<BehaviorChunk> chunkList = parser.parseBehavior(mth);

            StringWriter outsw = new StringWriter();

            foreach (var bc in chunkList)
            {
                outsw.WriteLine(generateIndentStr(bc.indLv) + bc.behavior);
            }

            return outsw.ToString();
        }


        private static string generateIndentStr(int indentLevel)
        {
            switch( indentLevel )
            {
                case 0:
                    return "";
                case 1:
                    return "# ";
                case 2:
                    return "## ";
                case 3:
                    return "### ";
                case 4:
                    return "#### ";
                case 5:
                    return "##### ";
                case 6:
                    return "###### ";
                case 7:
                    return "####### ";
                case 8:
                    return "######## ";
                case 9:
                    return "######### ";
                case 10:
                    return "########## ";
                default:
                    return "##########" + indentLevel + " ";
            }

        }



        private static void writeParameters(List<ParameterVO> parameters, StreamWriter sw)
        {
            sw.WriteLine("##### パラメータ");
            sw.WriteLine("");
            // sw.WriteLine(".パラメータ");
            sw.WriteLine("[cols = \"1,1,3\"]");
            sw.WriteLine("|===");
            sw.WriteLine("| パラメータ型 | 名前(別名) | ノート");

            foreach (ParameterVO param in parameters)
            {
                sw.Write("|" + param.eaType);
                sw.Write("|" + param.name);
                sw.Write("|" + param.notes);
                sw.WriteLine("");
            }
            sw.WriteLine("|===");
            sw.WriteLine("");
        }


        private static void writeTaggedValueMth(List<TaggedValueVO> methodTags, StreamWriter sw)
        {
            sw.WriteLine("##### タグ付き値");
            sw.WriteLine("");
            // sw.WriteLine(".操作のタグ付き値");

            sw.WriteLine("[cols = \"1,3\"]");
            sw.WriteLine("|===");
            sw.WriteLine("| キー | 値");

            foreach (TaggedValueVO tv in methodTags)
            {
                sw.Write("|" + tv.name);

                if (tv.tagValue == "<memo>")
                {
                    sw.Write(" |" + tv.notes);
                }
                else
                {
                    sw.Write(" |" + tv.tagValue);
                }
                sw.WriteLine("");
            }
            sw.WriteLine("|===");
            sw.WriteLine("");
        }


        private static void writePlantUml(ElementVO element, StreamWriter sw)
        {
            sw.WriteLine("");
            // sw.WriteLine("[plantuml, images/" + element.guid.Substring(1,8) + ", svg]");
            sw.WriteLine("[plantuml, img-" + element.guid.Substring(1, 13) + ", png]");
            sw.WriteLine("--");
            sw.WriteLine("@startuml");

            sw.WriteLine("class \"" + filterSpecialChar(element.name) + "\" " + getStereotypeStr(element.stereoType) + " {");

            if (element.attributes != null)
            {
                foreach (AttributeVO attr in element.attributes)
                {
                    sw.WriteLine("  " + getVisibilitySymbol(attr.visibility) + " " + attr.name + "");
                }
            }

            if (element.methods != null)
            {
                foreach (MethodVO mth in element.methods)
                {
                    sw.WriteLine("  " + getVisibilitySymbol(mth.visibility) + " " + mth.name + "()");
                }
            }

            sw.WriteLine("}");

            ElementSearcher elemSrch = new ElementSearcher();

            // DBからこの要素につながっている接続情報を取得
            ConnectorSearcher searcher = new ConnectorSearcher();
            List<ConnectorVO> conns = searcher.findByObjectGuid(element.guid);

            Dictionary<string, ElementSearchItem> nameHash = new Dictionary<string, ElementSearchItem>();

            List<ConnectorVO> srcConnList = new List<ConnectorVO>();
            List<ConnectorVO> destConnList = new List<ConnectorVO>();
            foreach (ConnectorVO cn in conns)
            {
                if (cn.srcObjGuid == element.guid)
                {
                    srcConnList.Add(cn);
                    outputConnDestClass(cn.destObjGuid, elemSrch, nameHash, sw);
                }

                if (cn.destObjGuid == element.guid)
                {
                    destConnList.Add(cn);
                    outputConnDestClass(cn.srcObjGuid, elemSrch, nameHash, sw);
                }
            }


            for (var i = 0; i < srcConnList.Count; i++)
            {
                outputSrcConnectLine(srcConnList[i], sw);
            }

            for (var i = 0; i < destConnList.Count; i++)
            {
                outputDestConnectLine(destConnList[i], sw);
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


        private static string getVisibilitySymbol(string visibility)
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
