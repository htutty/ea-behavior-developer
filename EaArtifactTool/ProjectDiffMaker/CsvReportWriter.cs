using ArtifactFileAccessor.vo;
using System.Collections.Generic;
using System.IO;

namespace ProjectDiffMaker
{
    class CsvReportWriter
    {
        /// <summary>
        /// 変更サマリーのCSVファイルを出力する
        /// </summary>
        /// <param name="outputDir">出力フォルダ</param>
        /// <param name="outArtifacts">出力対象のマージ後成果物リスト</param>
        public void writeSummaryCsvFile(string outputDir, List<ArtifactVO> outArtifacts)
        {
            //BOM無しのUTF8でテキストファイルを作成する
            StreamWriter sumsw = new StreamWriter(outputDir + "\\ChangedSummary.csv",
                                                  false, System.Text.Encoding.GetEncoding("shift_jis"));
            // ヘッダ行出力
            sumsw.WriteLine("\"変更内容(CUD)\",\"パッケージGUID\",\"パッケージ名\",\"パッケージパス\",\"変更内容(CUD)\",\"要素GUID\",\"要素名\",\"差分詳細\",\"\"");
            // 
            foreach (ArtifactVO atf in outArtifacts)
            {
                if (atf.changed != ' ')
                {
                    outputChangedSummaryArtifact(atf, sumsw);
                }
            }

            sumsw.Close();
        }


        /// <summary>
        /// サマリーのCSVファイルを出力する（成果物）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputChangedSummaryArtifact(ArtifactVO atf, StreamWriter sw)
        {
            outputChangedSummaryPackage(atf, atf.package, sw);
        }

        /// <summary>
        /// サマリーのCSVファイルを出力する（パッケージ）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="pkg">パッケージ</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputChangedSummaryPackage(ArtifactVO atf, PackageVO pkg, StreamWriter sw)
        {
            foreach (ElementVO e in pkg.elements)
            {
                outputChangedSummaryElement(atf, e, sw);
            }

            foreach (PackageVO p in pkg.childPackageList)
            {
                outputChangedSummaryPackage(atf, p, sw);
            }
        }

        /// <summary>
        /// サマリーのCSVファイルを出力する（要素）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="elem">要素</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputChangedSummaryElement(ArtifactVO atf, ElementVO elem, StreamWriter sw)
        {
            string chdesc;

            chdesc = "";
            chdesc = chdesc + "attribute:" + elem.attributes.Count + "/";
            chdesc = chdesc + "method:" + elem.methods.Count + "/";
            chdesc = chdesc + "taggedvalue:" + elem.taggedValues.Count;

            sw.Write("\"" + atf.changed + "\"");
            sw.Write(",\"" + atf.guid + "\"");
            sw.Write(",\"" + atf.name + "\"");
            sw.Write(",\"" + atf.pathName + "\"");
            sw.Write(",\"" + elem.changed + "\"");
            sw.Write(",\"" + elem.guid + "\"");
            sw.Write(",\"" + elem.name + "\"");
            sw.Write(",\"" + chdesc + "\"");
            sw.WriteLine("");
        }



        /// <summary>
        /// 変更詳細のCSVファイルを出力する
        /// </summary>
        /// <param name="outputDir">出力フォルダ</param>
        /// <param name="outArtifacts">出力対象のマージ後成果物リスト</param>
        public void writeDetailCsvFile(string outputDir, List<ArtifactVO> outArtifacts)
        {
            //BOM無しのUTF8でテキストファイルを作成する
            StreamWriter detailsw = new StreamWriter(outputDir + "\\ChangedDetail.csv",
                                                     false, System.Text.Encoding.GetEncoding("shift_jis"));
            detailsw.WriteLine("\"変更内容(CUD)\",\"パッケージGUID\",\"パッケージ名\",\"パッケージパス\",\"変更内容(CUD)\",\"要素GUID\",\"要素名\",\"属性/操作\",\"変更内容(CUD)\",\"属性操作GUID\",\"属性操作名称\",\"左ファイル\",\"右ファイル\"");
            foreach (ArtifactVO atf in outArtifacts)
            {
                if (atf.changed != ' ')
                {
                    outputChangedDetailArtifact(atf, detailsw);
                }
            }

            detailsw.Close();


        }



        /// <summary>
        /// 詳細のCSVファイルを出力する（成果物）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputChangedDetailArtifact(ArtifactVO atf, StreamWriter sw)
        {
            outputChangedDetailPackage(atf, atf.package, sw);
        }

        /// <summary>
        /// 詳細のCSVファイルを出力する（パッケージ）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="pkg">パッケージ</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputChangedDetailPackage(ArtifactVO atf, PackageVO pkg, StreamWriter sw)
        {
            foreach (ElementVO e in pkg.elements)
            {
                outputChangedDetailElement(atf, e, sw);
            }

            foreach (PackageVO p in pkg.childPackageList)
            {
                outputChangedDetailPackage(atf, p, sw);
            }
        }

        /// <summary>
        /// 詳細のCSVファイルを出力する（要素）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="elem">要素</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputChangedDetailElement(ArtifactVO atf, ElementVO elem, StreamWriter sw)
        {
            foreach (AttributeVO a in elem.attributes)
            {
                outputChangedDetailAttribute(atf, elem, a, sw);
            }

            foreach (MethodVO m in elem.methods)
            {
                outputChangedDetailMethod(atf, elem, m, sw);
            }

            foreach (TaggedValueVO tv in elem.taggedValues)
            {
                outputChangedDetailTaggedValue(atf, elem, tv, sw);
            }
        }

        /// <summary>
        /// 詳細のCSVファイルを出力する（タグ付き値）
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        /// <param name="tv"></param>
        /// <param name="sw"></param>
        private void outputChangedDetailTaggedValue(ArtifactVO atf, ElementVO elem, TaggedValueVO tv, StreamWriter sw)
        {
            sw.Write("\"" + atf.changed + "\"");
            sw.Write(",\"" + atf.guid + "\"");
            sw.Write(",\"" + atf.name + "\"");
            sw.Write(",\"" + atf.pathName + "\"");
            sw.Write(",\"" + elem.changed + "\"");
            sw.Write(",\"" + elem.guid + "\"");
            sw.Write(",\"" + elem.name + "\"");
            sw.Write(",\"タグ\"");
            sw.Write(",\"" + tv.changed + "\"");
            sw.Write(",\"" + tv.guid + "\"");
            sw.Write(",\"" + tv.name + "\"");

            if (tv.changed == 'D' || tv.changed == 'U')
            {
                sw.Write(",\"#taggedvalue_" + tv.guid.Substring(1, 36) + "_L.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            if (tv.changed == 'C' || tv.changed == 'U')
            {
                sw.Write(",\"#taggedvalue_" + tv.guid.Substring(1, 36) + "_R.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            sw.WriteLine("");
        }

        /// <summary>
        /// 詳細のCSVファイルを出力する（属性）
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        /// <param name="attr"></param>
        /// <param name="sw"></param>
        private void outputChangedDetailAttribute(ArtifactVO atf, ElementVO elem, AttributeVO attr, StreamWriter sw)
        {
            sw.Write("\"" + atf.changed + "\"");
            sw.Write(",\"" + atf.guid + "\"");
            sw.Write(",\"" + atf.name + "\"");
            sw.Write(",\"" + atf.pathName + "\"");
            sw.Write(",\"" + elem.changed + "\"");
            sw.Write(",\"" + elem.guid + "\"");
            sw.Write(",\"" + elem.name + "\"");
            sw.Write(",\"属性\"");
            sw.Write(",\"" + attr.changed + "\"");
            sw.Write(",\"" + attr.guid + "\"");
            sw.Write(",\"" + attr.name + "\"");

            if (attr.changed == 'D' || attr.changed == 'U')
            {
                sw.Write(",\"#attribute_" + attr.guid.Substring(1, 36) + "_L.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            if (attr.changed == 'C' || attr.changed == 'U')
            {
                sw.Write(",\"#attribute_" + attr.guid.Substring(1, 36) + "_R.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            sw.WriteLine("");
        }

        /// <summary>
        /// 詳細のCSVファイルを出力する（操作）
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        /// <param name="mth"></param>
        /// <param name="sw"></param>
        private void outputChangedDetailMethod(ArtifactVO atf, ElementVO elem, MethodVO mth, StreamWriter sw)
        {
            sw.Write("\"" + atf.changed + "\"");
            sw.Write(",\"" + atf.guid + "\"");
            sw.Write(",\"" + atf.name + "\"");
            sw.Write(",\"" + atf.pathName + "\"");
            sw.Write(",\"" + elem.changed + "\"");
            sw.Write(",\"" + elem.guid + "\"");
            sw.Write(",\"" + elem.name + "\"");
            sw.Write(",\"操作\"");
            sw.Write(",\"" + mth.changed + "\"");
            sw.Write(",\"" + mth.guid + "\"");
            sw.Write(",\"" + mth.name + "\"");

            if (mth.changed == 'D' || mth.changed == 'U')
            {
                sw.Write(",\"#method_" + mth.guid.Substring(1, 36) + "_L.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            if (mth.changed == 'C' || mth.changed == 'U')
            {
                sw.Write(",\"#method_" + mth.guid.Substring(1, 36) + "_R.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            sw.WriteLine("");
        }


    }
}
