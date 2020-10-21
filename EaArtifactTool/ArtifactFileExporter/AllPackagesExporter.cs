using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;

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
            // 子の分の要素数とダイアグラム数をカウントしてセットする処理を呼び出し
            countPackageElementsAndDiagrams(allPackages);

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

        /// <summary>
        /// 子の分の要素数とダイアグラム数をカウントしてPackageVOにセットする
        /// </summary>
        /// <param name="allPackages"></param>
        private void countPackageElementsAndDiagrams(List<PackageVO> allPackages)
        {

            for (int i = 0; i < allPackages.Count; i++)
            {
                getChildrenNodeCount(allPackages[i]);
            }

        }


        private ChildrenCounters getChildrenNodeCount(PackageVO packageNode)
        {
            // 自分と子の分のカウント数を入れる入れ物を用意
            ChildrenCounters counters = new ChildrenCounters();

            // まず自分が保持する要素の数とダイアグラムの数をセット
            if (packageNode.elements != null)
            {
                counters.elementsCount = packageNode.elements.Count;
            }
            else
            {
                packageNode.elementsCount = 0;
            }

            if (packageNode.diagrams != null)
            {
                counters.diagramsCount = packageNode.diagrams.Count;
            }
            else
            {
                packageNode.diagramsCount = 0;
            }

            // 子パッケージがある場合、子の分の数も自分に集計する
            if (packageNode.childPackageList != null && packageNode.childPackageList.Count > 0)
            {
                // 子の分の要素数とダイアグラム数を取得し、それぞれ自分のカウンタに足しこむ
                for (int i = 0; i < packageNode.childPackageList.Count; i++)
                {
                    ChildrenCounters childCount = getChildrenNodeCount(packageNode.childPackageList[i]);

                    counters.elementsCount += childCount.elementsCount;
                    counters.diagramsCount += childCount.diagramsCount;
                }
            }


            // 最後に、要素数とダイアグラム数を自ノードにセット
            packageNode.elementsCount = counters.elementsCount;
            packageNode.diagramsCount = counters.diagramsCount;

            return counters;
        }

        /// <summary>
        /// 指定されたStreamWriterにPackageTreeを書き込む。
        /// リスト内のPackageが子を持っている場合は自メソッドを再帰的に呼び出し
        /// 末端の子まで全て記録する。
        /// </summary>
        /// <param name="packageList">記録対象のPackageリスト</param>
        /// <param name="depth">深さ(インデントに使用)</param>
        /// <param name="ppath">親パッケージパス</param>
        /// <param name="sw"></param>
        private void outputPackageTree(List<PackageVO> packageList, int depth, string ppath, StreamWriter sw)
        {

            foreach (PackageVO pac in packageList)
            {
                pac.pathName = ppath + "/" + pac.name;

                sw.Write(StringUtil.indent(depth) + "<package");
                sw.Write(" PackageID='" + pac.packageId + "'");
                sw.Write(" guid='" + StringUtil.escapeXML(pac.guid) + "'");
                sw.Write(" parentPackageId='" + pac.parentPackageId + "'");
                sw.Write(" TPos='" + pac.treePos + "'");
                sw.Write(" name='" + StringUtil.escapeXML(pac.name) + "'");
                sw.Write(" Alias='" + StringUtil.escapeXML(pac.alias) + "'");
                sw.Write(" stereoType='" + StringUtil.escapeXML(pac.stereoType) + "'");
                sw.Write(" elementsCount='" + pac.elementsCount + "'");
                sw.Write(" diagramsCount='" + pac.diagramsCount + "'");
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

    class ChildrenCounters
    {
        public int elementsCount { get; set; }
        public int diagramsCount { get; set; }
        public int changedElemensCount { get; set; }
    }

}
