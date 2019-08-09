using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.reader;

using System.IO;

namespace BehaviorTextExportImport
{
    class BehaviorTextExporter
    {
        private int cumulativeFileSize = 0;
        private int blockNum = 0;
        //        private const int FILE_SIZE_LIMIT = 1040000;
        private const int FILE_SIZE_LIMIT = 500000;

        public void exportBehaviorText()
        {
            // 出力フォルダはプロジェクトファイルの配置パスとする
            string outputDir = ProjectSetting.getVO().projectPath;

            // 全成果物リストの読み込み
            string artifactDir = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().artifactsPath;
            ArtifactsVO allArtifacts = ArtifactsXmlReader.readAllArtifacts(artifactDir);

            // 成果物パッケージXMLファイル読み込み
            ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactDir, true);

            for (int i = 0; i < allArtifacts.artifactList.Count; i++)
            {
                ArtifactVO atf = allArtifacts.artifactList[i];

                // 成果物VOのPathNameより出力が必要と判断された場合
                if (artifactPathNameIsValid(atf.pathName) )
                {
                    // ルールに従い、GUIDから成果物ファイルの名称を取得
                    string artifactFileName = "atf_" + atf.guid.Substring(1, 36) + ".xml";
                    // 成果物ファイル名と出力フォルダを渡し、ふるまいテキストを出力させる
                    outputIndexedBehaviorText(atfReader, artifactFileName, outputDir);

                    Console.WriteLine("{0}:テキスト出力中 {1}", i + 1, artifactFileName);
                }

            }

        }

        /// <summary>
        /// 成果物VOのPathNameから、出力が必要か判断する
        /// </summary>
        /// <param name="artifactVO"></param>
        /// <returns></returns>
        private bool artifactPathNameIsValid(string targetPath)
        {
            // 出力しないパッケージを設定
            string[] denyPaths = {
                "/次期ASWプロジェクト-本開発/論理モデル/レイヤ別ビュー/フレームワーク_PL移管",
                "/次期ASWプロジェクト-本開発/論理モデル/レイヤ別ビュー/PL層設計モデル/プレゼンテーション/PLアプリケーション/Webビュー",
                "/次期ASWプロジェクト-本開発/実装モデル/",
                "/次期＠desk開発/",
                "/提携APIプロジェクト/"
            };

            foreach (string path in denyPaths)
            {
                // 対象のパスが不許可のパスと前方一致でマッチしたら
                if( targetPath.StartsWith(path) )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="atfReader"></param>
        /// <param name="artifactFile"></param>
        /// <param name="outputDir"></param>
        private void outputIndexedBehaviorText(ArtifactXmlReader atfReader, string artifactFile, string outputDir)
        {
            // 成果物XMLファイルの読み込み
            ArtifactVO artifact = atfReader.readArtifactFile(artifactFile);
            string textFileName = "BehaviorText";

            List<ElementVO> elements = artifact.getOwnElements();
            foreach (ElementVO elem in elements)
            {
                if(elementPathNameIsValid(elem.elementPath) )
                {
                    if (elem.eaType == "Class" || elem.eaType == "Interface" || elem.eaType == "Enumeration" || elem.eaType == "Notes")
                    {
                        outputBehaviorTextFile(elem, outputDir + "\\" + textFileName);
                    }
                }

            }

            return;
        }


        private bool elementPathNameIsValid(string targetPath)
        {
            // 出力しないパッケージを設定
            string[] denyPaths = {
                "/次期ASWプロジェクト-本開発/要求モデル/フレームワーク",
                "/次期ASWプロジェクト-本開発/要求モデル/外部IFモデル/",
            };

            foreach (string path in denyPaths)
            {
                // 対象のパスが不許可のパスと前方一致でマッチしたら
                if (targetPath.StartsWith(path))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="filepath"></param>
        private void outputBehaviorTextFile(ElementVO element, string filepath)
        {
            StringWriter bhtxtWriter = new StringWriter();

            if (element.notes != null && element.notes != "")
            {
                bhtxtWriter.WriteLine("[en " + element.elementId + "]");
                bhtxtWriter.WriteLine("");
                bhtxtWriter.WriteLine(element.notes);
                bhtxtWriter.WriteLine("");
            }

            foreach (AttributeVO attr in element.attributes)
            {
                if (attr.notes != null && attr.notes != "")
                {
                    bhtxtWriter.WriteLine("[an " + attr.attributeId + "]");
                    bhtxtWriter.WriteLine("");
                    bhtxtWriter.WriteLine(attr.notes);
                    bhtxtWriter.WriteLine("");
                }
            }

            foreach (MethodVO mth in element.methods)
            {
                if (mth.notes != null && mth.notes != "")
                {
                    bhtxtWriter.WriteLine("[mn " + mth.methodId + "]");
                    bhtxtWriter.WriteLine("");
                    bhtxtWriter.WriteLine(mth.notes);
                    bhtxtWriter.WriteLine("");
                }

                if (mth.behavior != null && mth.behavior != "")
                {
                    bhtxtWriter.WriteLine("[mb " + mth.methodId + "]");
                    bhtxtWriter.WriteLine("");
                    bhtxtWriter.WriteLine(mth.behavior);
                    bhtxtWriter.WriteLine("");
                }

                foreach( ParameterVO prm in mth.parameters)
                {
                    bhtxtWriter.WriteLine("[pn " + mth.methodId + "-" + prm.pos + "]");
                    bhtxtWriter.WriteLine("");
                    bhtxtWriter.WriteLine("{0} {1}", prm.eaType, prm.name);
                    bhtxtWriter.WriteLine(prm.notes);
                    bhtxtWriter.WriteLine("");
                }

            }

            bhtxtWriter.Close();

            string bhText = bhtxtWriter.ToString();

            try
            {
                if (bhText.Length > 0)
                {
                    StringWriter elemPropWriter = new StringWriter();
                    elemPropWriter.WriteLine("[ep " + element.elementId + "]");
                    elemPropWriter.WriteLine("");
                    elemPropWriter.WriteLine("要素パス: " + element.elementPath);
                    elemPropWriter.WriteLine("要素名: " + element.name);
                    elemPropWriter.WriteLine("");

                    Encoding utf8Enc = Encoding.GetEncoding("utf-8");

                    int bytesize = utf8Enc.GetByteCount(elemPropWriter.ToString())
                         + utf8Enc.GetByteCount(bhtxtWriter.ToString());

                    if (cumulativeFileSize + bytesize > FILE_SIZE_LIMIT)
                    {
                        blockNum++;
                        cumulativeFileSize = bytesize;
                    } else
                    {
                        cumulativeFileSize = cumulativeFileSize + bytesize;
                    }


                    //BOM無しのUTF8でテキストファイルを作成する
                    string txtFileName = filepath + "_" + (blockNum + 1) + ".txt";
                    StreamWriter txtFileWriter = new StreamWriter(txtFileName, true);

                    txtFileWriter.Write(elemPropWriter.ToString());
                    txtFileWriter.Write(bhtxtWriter.ToString());
                    txtFileWriter.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
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


    }
}
