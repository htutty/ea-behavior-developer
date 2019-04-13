using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjectDiffMaker
{
    class DetailXmlWriter
    {
        private ArtifactVO procArtifact;
        private string fromArtifactDir;
        private string toArtifactDir;
        private string outputDir= null;

        public DetailXmlWriter(string fromArtifactDir, string toArtifactDir)
        {
            this.fromArtifactDir = fromArtifactDir;
            this.toArtifactDir = toArtifactDir;
        }

        /// <summary>
        /// マージ済みの成果物情報を各ファイルに出力する
        /// </summary>
        public void writeDetailFiles(string outputDir, List<ArtifactVO> outArtifacts)
        {
            Console.WriteLine("outputMerged: outputDir=" + outputDir);

            this.outputDir = outputDir;

            // 差分のdetailファイルが出力されるフォルダを事前に作成する
            makeDetailDirIfNotExist(outputDir + "\\detail");

            foreach (ArtifactVO atf in outArtifacts)
            {
                // artifactのchangedが空白以外なら変更ありとみなす
                if (atf.changed != ' ')
                {
                    // この成果物配下の詳細ファイルを出力する
                    outputDetailXmlArtifact(atf);
                }
            }

        }


        /// <summary>
        /// 詳細のXMLファイルを出力する（成果物）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputDetailXmlArtifact(ArtifactVO atf)
        {
            this.procArtifact = atf;

            outputDetailXmlPackage(atf.package);
        }

        /// <summary>
        /// 詳細のXMLファイルを出力する（パッケージ）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="pkg">パッケージ</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputDetailXmlPackage(PackageVO pkg)
        {
            foreach (ElementVO e in pkg.elements)
            {
                outputDetailXmlElement(e);
            }

            foreach (PackageVO p in pkg.childPackageList)
            {
                outputDetailXmlPackage(p);
            }
        }


        /// <summary>
        /// 詳細のXMLファイルを出力する（要素）
        /// </summary>
        /// <param name="atf">成果物VO</param>
        /// <param name="elem">要素</param>
        /// <param name="sw">出力ストリームライター</param>
        private void outputDetailXmlElement(ElementVO elem)
        {
            outputDetailXmlElementTags(elem);
            outputDetailXmlAttributes(elem);
            outputDetailXmlMethods(elem);

        }


        /// <summary>
        /// 詳細XMLファイルのタグ付き値を出力する
        /// </summary>
        /// <param name="currentElement"></param>
        private void outputDetailXmlElementTags(ElementVO currentElement)
        {
            if (currentElement.taggedValues.Count <= 0)
            {
                return;
            }


            // 　取得できたタグ付き値の情報をファイルに展開する
            foreach (TaggedValueVO tagv in currentElement.taggedValues)
            {

                switch (tagv.changed)
                {
                    case 'U':
                        outputOriginalTaggedValueFile(tagv, "L");
                        outputOriginalTaggedValueFile(tagv, "R");
                        break;

                    case 'C':
                        outputOriginalTaggedValueFile(tagv, "R");
                        break;
                    case 'D':
                        outputOriginalTaggedValueFile(tagv, "L");
                        break;
                }

            }

        }


        /// <summary>
        /// 比較して差異があったメソッドに対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
        /// </summary>
        /// <param name="tagVal">該当メソッド</param>
        /// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
        private void outputOriginalTaggedValueFile(TaggedValueVO tagVal, string leftRight)
        {

            XmlDocument xmlDocument = new XmlDocument();

            string xmlDir = null;
            if ("L".Equals(leftRight))
            {
                xmlDir = this.fromArtifactDir;
            }
            else
            {
                xmlDir = this.toArtifactDir;
            }
            ArtifactXmlReader atfReader = new ArtifactXmlReader(xmlDir);

            // 現在処理中の成果物のGUID、およびメソッドのGUIDから（左もしくは右の）XMLノードを取得
            XmlNode taggedValueNode = atfReader.readTaggedValueNode(procArtifact.guid, tagVal.guid);

            // 成果物のGUID, メソッドのGUIDから読み取ったメソッドNodeがNULLだったらスキップ
            if (taggedValueNode != null)
            {
                // 新しいrootノードを、成果物XMLからインポートする形で作成
                XmlNode root = xmlDocument.ImportNode(taggedValueNode, true);

                // ルート配下に引数のドキュメントを追加
                xmlDocument.AppendChild(root);

                // この内容で操作の詳細に記録する
                xmlDocument.Save(this.outputDir + "\\detail\\#taggedValue_" + tagVal.guid.Substring(1, 36) + "_" + leftRight + ".xml");
            }
        }


        /// <summary>
        /// 詳細XMLファイルの属性を出力する
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="sw"></param>
        private void outputDetailXmlAttributes(ElementVO elem)
        {
            if (elem.attributes.Count <= 0)
            {
                return;
            }

            //　取得できた属性の情報をファイルに展開する
            foreach (AttributeVO attr in elem.attributes)
            {

                switch (attr.changed)
                {
                    case 'U':
                        outputOriginalAttributeFile(attr, "L");
                        outputOriginalAttributeFile(attr, "R");
                        break;

                    case 'C':
                        outputOriginalAttributeFile(attr, "R");
                        break;
                    case 'D':
                        outputOriginalAttributeFile(attr, "L");
                        break;
                }
            }

        }



        /// <summary>
        /// 比較して差異があった属性に対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
        /// </summary>
        /// <param name="attr">該当属性</param>
        /// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
        private void outputOriginalAttributeFile(AttributeVO attr, string leftRight)
        {
            XmlDocument xmlDocument = new XmlDocument();
            //	' XML宣言部分生成
            // 	Set head = xmlDocument.createProcessingInstruction("xml", "version='1.0'")
            XmlProcessingInstruction head = xmlDocument.CreateProcessingInstruction("xml", "version='1.0'");

            //' XML宣言部分設定
            xmlDocument.AppendChild(head);

            string xmlDir = null;
            if ("L".Equals(leftRight))
            {
                xmlDir = this.fromArtifactDir;
            }
            else
            {
                xmlDir = this.toArtifactDir;
            }
            ArtifactXmlReader atfReader = new ArtifactXmlReader(xmlDir);

            // 現在処理中の成果物のGUID、および属性のGUIDから（左もしくは右の）XMLノードを取得
            XmlNode attrNode = atfReader.readAttributeNode(procArtifact.guid, attr.guid);

            // 成果物のGUID,属性のGUIDから取得した属性NodeがNullなら出力をスキップする
            if (attrNode != null)
            {
                // 新しいrootノードを、成果物XMLからインポートする形で作成
                XmlNode root = xmlDocument.ImportNode(attrNode, true);

                // ルート配下に引数のドキュメントを追加
                xmlDocument.AppendChild(root);

                // この内容で操作の詳細に記録する
                xmlDocument.Save(outputDir + "\\detail\\#attribute_" + attr.guid.Substring(1, 36) + "_" + leftRight + ".xml");
            }

        }


        /// <summary>
        /// 詳細XMLファイルの操作を出力する
        /// </summary>
        /// <param name="currentElement"></param>
        /// <param name="depth"></param>
        /// <param name="sw"></param>
        private void outputDetailXmlMethods(ElementVO currentElement)
        {

            if (currentElement.methods.Count <= 0)
            {
                return;
            }

            //　取得できた操作の情報をファイルに展開する
            foreach (MethodVO mth in currentElement.methods)
            {
                switch (mth.changed)
                {
                    case 'U':
                        outputOriginalMethodFile(mth, "L");
                        outputOriginalMethodFile(mth, "R");
                        break;
                    case 'C':
                        outputOriginalMethodFile(mth, "R");
                        break;
                    case 'D':
                        outputOriginalMethodFile(mth, "L");
                        break;
                }
            }
        }


        /// <summary>
        /// 比較して差異があったメソッドに対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
        /// </summary>
        /// <param name="mth">該当メソッド</param>
        /// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
        private void outputOriginalMethodFile(MethodVO mth, string leftRight)
        {

            XmlDocument xmlDocument = new XmlDocument();

            //	' XML宣言部分生成
            // 	Set head = xmlDocument.createProcessingInstruction("xml", "version='1.0'")
            //			XmlProcessingInstruction head = xmlDocument.CreateProcessingInstruction("xml", "version='1.0'");

            //' XML宣言部分設定
            // xmlDocument.AppendChild(head);

            string xmlDir = null;
            if ("L".Equals(leftRight))
            {
                xmlDir = this.fromArtifactDir;
            }
            else
            {
                xmlDir = this.toArtifactDir;
            }
            ArtifactXmlReader atfReader = new ArtifactXmlReader(xmlDir);

            // 現在処理中の成果物のGUID、およびメソッドのGUIDから（左もしくは右の）XMLノードを取得
            XmlNode methodNode = atfReader.readMethodNode(procArtifact.guid, mth.guid);

            // 成果物のGUID, メソッドのGUIDから読み取ったメソッドNodeがNULLだったらスキップ
            if (methodNode != null)
            {
                // 新しいrootノードを、成果物XMLからインポートする形で作成
                XmlNode root = xmlDocument.ImportNode(methodNode, true);

                // ルート配下に引数のドキュメントを追加
                xmlDocument.AppendChild(root);

                // この内容で操作の詳細に記録する
                xmlDocument.Save(this.outputDir + "\\detail\\#method_" + mth.guid.Substring(1, 36) + "_" + leftRight + ".xml");
            }
        }


        private static void makeDetailDirIfNotExist(string detailDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(detailDir))
            {
                Directory.CreateDirectory(detailDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + detailDir);
            }
        }

    }
}
