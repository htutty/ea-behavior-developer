using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ArtifactFileAccessor.reader
{
    public class PackagesXmlReader
    {
        private string allPackagesXmlFilePath;
        private List<PackageVO> rootPackages = null; 
        private bool sortByPosFlg = true;

        public PackagesXmlReader(string allPackagesXmlFilePath)
        {
            this.allPackagesXmlFilePath = allPackagesXmlFilePath ;
            this.rootPackages = readAllPackage();
        }

        private List<PackageVO> readAllPackage()
        {
            List<PackageVO> packages = new List<PackageVO>();

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.allPackagesXmlFilePath);

            // AllPackageノードに移動する
            XmlNode allPackagesNode = xmlDoc.SelectSingleNode("AllPackage");

            foreach (XmlNode pkgNode in allPackagesNode.ChildNodes)
            {
                if (pkgNode.Name == "package")
                {
                    PackageVO rootPackage = readPackageAttributes(pkgNode);
                    readRootPackage(rootPackage, pkgNode);
                    packages.Add(rootPackage);
                }
            }

            return packages;
        }



        /// <summary>
        /// 成果物パッケージの読み込み
        /// </summary>
        /// <param name="parentNode">packageを子として持つ親のartifactノード</param>
        /// <returns>読み込まれた成果物のパッケージVO（常に１つ）</returns>
        private void readRootPackage(PackageVO rootPackage, XmlNode parentNode)
        {
            // 成果物のルートパッケージから再帰的に子パッケージを読み込み
            readPackages(rootPackage, parentNode);

            // ソート順指定フラグにより、ソート処理が分かれる
            if (this.sortByPosFlg)
            {
                rootPackage.sortChildNodes();
            }
            else
            {
                rootPackage.sortChildNodesGuid();
            }

            return;
        }

        /// <summary>
        /// パッケージの読み込み（再帰処理）
        /// 引数のパッケージノード以下を再帰的に読み込み、 Package, Element をVOに読み込む
        /// </summary>
        /// <param name="pkgvo">(out)パッケージのVO</param>
        /// <param name="parentNode">(in)対象のパッケージのXmlNode</param>
        private void readPackages(PackageVO pkgvo, XmlNode parentNode)
        {
            Console.WriteLine("enter readPackages(parent=" + pkgvo.name + ")");

            List<PackageVO> retList = new List<PackageVO>();
            List<ElementVO> retElementList = new List<ElementVO>();
            List<DiagramVO> retDiagramList = new List<DiagramVO>();

            foreach (XmlNode pkgNode in parentNode.ChildNodes)
            {
                if ("package".Equals(pkgNode.Name))
                {
                    PackageVO pkg = readPackageAttributes(pkgNode);

                    readPackages(pkg, pkgNode);
                    retList.Add(pkg);
                }

            }

            pkgvo.childPackageList = retList;
            pkgvo.elements = retElementList;
            pkgvo.diagrams = retDiagramList;
        }


        private PackageVO readPackageAttributes(XmlNode pkgNode)
        {
            PackageVO pkg = new PackageVO();

            foreach (XmlAttribute attr in pkgNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "PackageId":
                        pkg.packageId = readAttributeIntValue(attr);
                        break;
                    case "name":
                        pkg.name = attr.Value;
                        break;
                    case "guid":
                        pkg.guid = attr.Value;
                        break;
                    case "alias":
                        pkg.alias = attr.Value;
                        break;
                    case "stereotype":
                        pkg.stereoType = attr.Value;
                        break;
                    case "TPos":
                        pkg.treePos = readAttributeIntValue(attr);
                        break;
                    case "changed":
                        pkg.changed = readAttributeCharValue(attr);
                        break;
                }
            }

            return pkg;
        }


        public List<PackageVO> getRootPackages(  )
        {
            if( this.rootPackages == null)
            {
                return null;
            }

            List<PackageVO> retPackages = new List<PackageVO>();

            foreach(PackageVO package in this.rootPackages)
            {
                PackageVO clonePack = new PackageVO();
                clonePack.name = package.name;
                clonePack.guid = package.guid;
                clonePack.packageId = package.packageId;
                clonePack.treePos = package.treePos;

                retPackages.Add(clonePack);
            }

            return retPackages;
        }


        public List<PackageVO> getAllPackages()
        {
            return this.rootPackages;
        }



            private static string readAttributeStringValue(XmlAttribute attr)
        {
            return attr.Value;
        }


        private static int readAttributeIntValue(XmlAttribute attr)
        {
            int p;
            if (!Int32.TryParse(attr.Value, out p))
            {
                p = 0;
            }
            return p;
        }


        private static char readAttributeCharValue(XmlAttribute attr)
        {
            if (attr.Value != null && !"".Equals(attr.Value))
            {
                return attr.Value[0];
            }
            else
            {
                return ' ';
            }
        }


        private static bool readAttributeBooleanValue(XmlAttribute attr)
        {
            return "True".Equals(attr.Value);
        }


        private static DateTime readAttributeDateTimeValue(XmlAttribute attr)
        {
            if (attr.Value != null && !"".Equals(attr.Value))
            {
                return DateTime.Parse(attr.Value);
            }
            else
            {
                return new DateTime();
            }
        }
    }
}
