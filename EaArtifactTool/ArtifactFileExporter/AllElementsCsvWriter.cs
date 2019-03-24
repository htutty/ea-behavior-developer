using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArtifactFileExporter
{
    class AllElementsCsvWriter
    {
        private string outputDir;

        private const int OUTPUTTYPE_ELEMENT = 1;
        private const int OUTPUTTYPE_ATTRMTH = 2;

        private const string QUOTE = "\"";

        public AllElementsCsvWriter(string outputDir)
        {
            this.outputDir = outputDir;
        }


        #region 要素CSVファイルの出力
        /// <summary>
        /// 成果物配下のクラス（インタフェース、列挙）の一覧をCSVに出力する
        /// </summary>
        /// <param name="artifacts">全成果物リスト</param>
        public void outputElementsCsv(ArtifactsVO artifacts)
        {
            StreamWriter csvsw = null;

            try
            {
                csvsw = new StreamWriter(outputDir + "\\AllElements.csv", false, System.Text.Encoding.GetEncoding("shift_jis"));

                // 1行目に列タイトルをつける
                csvsw.WriteLine("artifactGuid,packStereotype,pathName,elemGuid,elemType,elemName,elemStereotype,fqcn");

                foreach (var atf in artifacts.artifactList)
                {
                    retrievePackage(atf.package, csvsw, OUTPUTTYPE_ELEMENT);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (csvsw != null) csvsw.Close();
            }

        }

        #endregion

        #region 属性・操作CSVファイルの出力
        public void outputMthAttrCsv(ArtifactsVO artifacts)
        {
            StreamWriter csvsw = null;

            try
            {
                csvsw = new StreamWriter(outputDir + "\\AllAttrMth.csv", false, System.Text.Encoding.GetEncoding("shift_jis"));

                // 1行目に列タイトルをつける
                csvsw.WriteLine("artifactGuid,packStereotype,pathName,elemGuid,elemType,elemName,elemStereotype,attrMthFlg,attrMthGuid,attrMthName,attrMthAlias,mthParamDesc");

                foreach (var atf in artifacts.artifactList)
                {
                    retrievePackage(atf.package, csvsw, OUTPUTTYPE_ATTRMTH);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (csvsw != null) csvsw.Close();
            }

        }
        #endregion


        #region 共通処理：パッケージを再帰的に読み込みしながら要素をなめる
        /// <summary>
        /// 引数のパッケージを再帰的に読み込み、配下のelementsをさらにretrieveする
        /// </summary>
        /// <param name="package"></param>
        /// <param name="csvsw"></param>
        /// <param name="outputType"></param>
        private void retrievePackage(PackageVO package, StreamWriter csvsw, short outputType)
        {
            if (package.elements != null)
            {
                foreach (var elem in package.elements)
                {
                    retrieveElement(package, elem, csvsw, outputType);
                }
            }

            if (package.childPackageList != null)
            {
                foreach (var pak in package.childPackageList)
                {
                    retrievePackage(pak, csvsw, outputType);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="elem"></param>
        private void retrieveElement(PackageVO pack, ElementVO elem, StreamWriter csvsw, short outputType)
        {
            // 論理モデルをターゲットとして要素のタイプ が クラス、インタフェース、列挙 の場合は独自のCSVファイルに出力
            if (elem.eaType == "Class" || elem.eaType == "Interface" || elem.eaType == "Enumeration")
            {
                if ( outputType == OUTPUTTYPE_ELEMENT)
                {
                    writeElementCsv(pack, elem, csvsw);
                }
                else if ( outputType == OUTPUTTYPE_ATTRMTH)
                {
                    foreach (AttributeVO attr in elem.attributes)
                    {
                        writeCsvFileAttr(pack, elem, attr, csvsw);
                    }

                    foreach (MethodVO mth in elem.methods)
                    {
                        writeCsvFileMethod(pack, elem, mth, csvsw);
                    }
                }

            }

        }

        #endregion


        /// <summary>
        /// 要素CSVファイルの明細行出力
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="elem"></param>
        /// <param name="csvsw"></param>
        private void writeElementCsv(PackageVO pack, ElementVO elem, StreamWriter csvsw)
        {

            csvsw.Write(addQuote(pack.guid));
            csvsw.Write("," + addQuote(pack.stereoType));
            csvsw.Write("," + addQuote(pack.pathName));
            csvsw.Write("," + addQuote(elem.guid));
            csvsw.Write("," + addQuote(elem.eaType));
            csvsw.Write("," + addQuote(elem.name));
            csvsw.Write("," + addQuote(elem.stereoType));
            csvsw.Write("," + addQuote(elem.tag));
            csvsw.Write("\r\n");

        }

        private static string addQuote(string origStr)
        {
            return QUOTE + origStr + QUOTE;
        }

        /// <summary>
        /// 全属性・操作CSVに属性行を追加
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="elem">要素VO</param>
        /// <param name="attr">属性VO</param>
        /// <param name="csvsw">出力ストリーム</param>
        private void writeCsvFileAttr(PackageVO pak, ElementVO elem, AttributeVO attr, StreamWriter csvsw)
        {
            csvsw.Write("" + addQuote(pak.guid));
            csvsw.Write("," + addQuote(pak.stereoType));
            csvsw.Write("," + addQuote(pak.pathName));
            csvsw.Write("," + addQuote(elem.guid));
            csvsw.Write("," + addQuote(elem.eaType));
            csvsw.Write("," + addQuote(elem.name));
            csvsw.Write("," + addQuote(elem.stereoType));
            // csvsw.Write("," + elem.tag);
            csvsw.Write("," + addQuote("a"));
            csvsw.Write("," + addQuote(attr.guid));
            csvsw.Write("," + addQuote(attr.name));
            csvsw.Write("," + addQuote(attr.alias));
            csvsw.Write("," + addQuote(""));
            csvsw.Write("\r\n");
        }

        /// <summary>
        /// 全属性・操作CSVに操作行を追加
        /// </summary>
        /// <param name="pak">パッケージVO</param>
        /// <param name="elem">要素VO</param>
        /// <param name="mth">操作VO</param>
        /// <param name="csvsw">出力ストリーム</param>
        private void writeCsvFileMethod(PackageVO pak, ElementVO elem, MethodVO mth, StreamWriter csvsw)
        {

            csvsw.Write("" + addQuote(pak.guid));
            csvsw.Write("," + addQuote(pak.stereoType));
            csvsw.Write("," + addQuote(pak.pathName));
            csvsw.Write("," + addQuote(elem.guid));
            csvsw.Write("," + addQuote(elem.eaType));
            csvsw.Write("," + addQuote(elem.name));
            csvsw.Write("," + addQuote(elem.stereoType));
            // csvsw.Write("," + elem.tag);
            csvsw.Write("," + addQuote("m"));
            csvsw.Write("," + addQuote(mth.guid));
            csvsw.Write("," + addQuote(mth.name));
            csvsw.Write("," + addQuote(mth.alias));
            csvsw.Write("," + addQuote(mth.getParamDesc()));
            csvsw.Write("\r\n");
        }
    }

}
