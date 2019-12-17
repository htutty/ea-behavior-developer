using System;
using System.Xml;
using System.Collections.Generic;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.reader
{
	/// <summary>
	/// Description of ArtifactXmlReader.
	/// </summary>
	public class ArtifactXmlReader
	{
		private string artifactsPath = null;
        private bool sortByPosFlg = false;
        private bool elementFileReadFlg = false;

        #region "コンストラクタ"

        /// <summary>
        /// 成果物フォルダのみをパラメータとするコンストラクタ
        /// （差分取得などに利用するための、成果物内の各項目の順番をGUID順に並べて返却する）
        /// </summary>
        /// <param name="artifactsDir">プロジェクトフォルダ配下の成果物フォルダ([projectDir]/artifacts)</param>
        public ArtifactXmlReader(string artifactsDir)
		{
			if ( artifactsDir == null ) {
				throw new ArgumentException("project_dirを指定してください");
			} else {
				this.artifactsPath = artifactsDir;
			}
            this.sortByPosFlg = false;
            this.elementFileReadFlg = false;
        }

        /// <summary>
        /// 成果物フォルダ と sortByPosFlg をパラメータとするコンストラクタ
        /// （EA上の画面の表示順に合わせたPos+Nameでの並べ替えと、GUID順の並べ替えとをフラグで選択する）
        /// </summary>
        /// <param name="artifactsDir">プロジェクトフォルダ配下の成果物フォルダ([projectDir]/artifacts)</param>
        /// <param name="sortByPosFlg">取得する成果物内の並び順指定(true:Pos+Name順, false:GUID順)</param>
        public ArtifactXmlReader(string artifactsDir, bool sortByPosFlg)
		{
			if ( artifactsDir == null ) {
				throw new ArgumentException("project_dirを指定してください");
			} else {
				this.artifactsPath = artifactsDir;
			}

			this.sortByPosFlg = sortByPosFlg;
            this.elementFileReadFlg = false;
        }

        /// <summary>
        /// 成果物フォルダ と sortByPosFlg をパラメータとするコンストラクタ
        /// （EA上の画面の表示順に合わせたPos+Nameでの並べ替えと、GUID順の並べ替えとをフラグで選択する）
        /// </summary>
        /// <param name="artifactsDir">プロジェクトフォルダ配下の成果物フォルダ([projectDir]/artifacts)</param>
        /// <param name="sortByPosFlg">取得する成果物内の並び順指定(true:Pos+Name順, false:GUID順)</param>
        /// <param name="elementFileReadFlg">elementsファイル読み込みフラグ(true:読む, false:読まない)</param>
        public ArtifactXmlReader(string artifactsDir, bool sortByPosFlg, bool elementFileReadFlg)
        {
            if (artifactsDir == null) {
                throw new ArgumentException("project_dirを指定してください");
            } else {
                this.artifactsPath = artifactsDir;
            }

            this.sortByPosFlg = sortByPosFlg;
            this.elementFileReadFlg = elementFileReadFlg;
        }
        #endregion


        #region "アーティファクトファイル読み込み"

        /// <summary>
        /// 指定された成果物ファイルを読み込み、読み取り結果のVOを返却する
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ArtifactVO readArtifactFile(string fileName)
        {
            string target_dir = null;

            if (this.artifactsPath != null)
            {
                target_dir = this.artifactsPath;
            }
            else
            {
                throw new ArgumentException();
            }

            string artifactFile = target_dir + @"\" + fileName;

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(artifactFile);

            // artifactノードに移動する
            XmlNode artifactNode = xmlDoc.SelectSingleNode("artifact");

            if (artifactNode != null)
            {
                ArtifactVO retArtifact = readArtifactNode(artifactNode);
                retArtifact.package = readRootPackage(artifactNode);

                return retArtifact;
            } else {
                return null;
            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="atfNode"></param>
        /// <returns></returns>
        public static ArtifactVO readArtifactNode(XmlNode atfNode)
        {

            // 成果物ノードを読み込んで vo を１件作成
            ArtifactVO atf = new ArtifactVO();

            // <artifact> ノードの属性から ArtifactVO のプロパティをセット
            foreach (XmlAttribute attr in atfNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "name":
                        atf.name = attr.Value;
                        break;
                    case "guid":
                        atf.guid = attr.Value;
                        break;
                    case "project":
                        atf.projectName = attr.Value;
                        break;
                    case "stereotype":
                        atf.stereoType = attr.Value;
                        break;
                    case "path":
                        atf.pathName = attr.Value;
                        break;
                    case "changed":
                        atf.changed = attr.Value[0];
                        break;
                }
            }

            // <artifact> の子ノードから ArtifactVO のプロパティをセット
            foreach (XmlNode atfChildNode in atfNode.ChildNodes)
            {
                switch (atfChildNode.Name)
                {
                    case "pathName":
                        atf.pathName = atfChildNode.InnerText;
                        break;
                    case "asciidocFilePath":
                        atf.asciidocFilePath = atfChildNode.InnerText;
                        break;
                }
            }

            return atf;
        }

        /// <summary>
        /// Artifact XML (atf_${guid}.xml) を読み、ArtifactVO のインスタンスに値をセットする。
        ///
        /// XML例：
        /// <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        ///   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        /// </artifacts>
        ///
        /// </summary>
        /// <returns>ArtifactVOのリスト</returns>
        public void readArtifactDesc(ArtifactVO artifact)
		{
			string target_dir = null;
			if ( this.artifactsPath != null ) {
				target_dir = this.artifactsPath;
			} else {
				throw new ArgumentException();
			}

			string fileName = target_dir + "/" + "atf_" + artifact.guid.Substring(1, 36) + ".xml"  ;

			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( fileName );

			// artifactノードに移動する
			XmlNode artifactNode = xmlDoc.SelectSingleNode("artifact");

			if ( artifactNode != null ) {
				artifact.package = readRootPackage(artifactNode) ;
			}

		}

		/// <summary>
		/// 成果物パッケージの読み込み
		/// </summary>
		/// <param name="parentNode">packageを子として持つ親のartifactノード</param>
		/// <returns>読み込まれた成果物のパッケージVO（常に１つ）</returns>
		private PackageVO readRootPackage( XmlNode parentNode ) {
			PackageVO pkgvo = null;

			foreach (XmlNode pkgNode in parentNode.ChildNodes)
			{
				if ( "package".Equals(pkgNode.Name) ) {
					pkgvo = new PackageVO {
						name = pkgNode.SelectSingleNode("@name").Value
					};

					try {
                        // 成果物のルートパッケージから再帰的に子パッケージを読み込み
						readPackages(pkgvo, pkgNode);

                        // ソート順指定フラグにより、ソート処理が分かれる
                        if (this.sortByPosFlg)
                        {
                            pkgvo.sortChildNodes();
                            //pkgvo.sortChildPackages();
                            //pkgvo.sortElements();
                        }
                        else
                        {
                            pkgvo.sortChildNodesGuid();
                            //pkgvo.sortChildPackagesGUID();
                            //pkgvo.sortElementsGUID();
                        }

                    }
                    catch (Exception ex) {
						Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        throw ex;
					}
				}
			}

			return pkgvo;
		}

		/// <summary>
		/// パッケージの読み込み（再帰処理）
		/// 引数のパッケージノード以下を再帰的に読み込み、 Package, Element をVOに読み込む
		/// </summary>
		/// <param name="pkgvo">(out)パッケージのVO</param>
		/// <param name="parentNode">(in)対象のパッケージのXmlNode</param>
		private void readPackages( PackageVO pkgvo, XmlNode parentNode ) {
			List<PackageVO> retList = new List<PackageVO>();
			List<ElementVO> retElementList = new List<ElementVO>();
            List<DiagramVO> retDiagramList = new List<DiagramVO>();

			foreach (XmlNode pkgNode in parentNode.ChildNodes)
			{
				if ( "package".Equals(pkgNode.Name) ) {
					PackageVO pkg = new PackageVO();
					foreach(XmlAttribute attr in pkgNode.Attributes) {
						switch( attr.Name ) {
							case "name" :
								pkg.name = attr.Value ;
								break;
							case "guid" :
								pkg.guid = attr.Value ;
								break;
							case "alias" :
								pkg.alias = attr.Value ;
								break;
							case "stereotype" :
								pkg.stereoType = attr.Value ;
								break;
							case "TPos" :
								pkg.treePos = readAttributeIntValue(attr);
								break;
							case "changed" :
								pkg.changed = readAttributeCharValue(attr);
								break;
						}
					}

					readPackages(pkg, pkgNode) ;
					retList.Add(pkg);
				}

				// package配下で elementノードを見つけたら
				if ( "element".Equals(pkgNode.Name) ) {
                    // elementノード配下の情報から、 ElementVO を返却する
                    ElementVO elem = ElementsXmlReader.readElement(pkgNode, sortByPosFlg);

                    // 強いてelement毎のXMLファイルを読む必要がある場合のみ
                    if ( elementFileReadFlg )
                    {
                        // 強いてelement毎のXMLファイルを読む必要がある場合のみ
                        if (ElementsXmlReader.existElementFile(elem.guid))
                        {
                            elem = ElementsXmlReader.readElementFile(elem.guid);
                        }
                    }

                    retElementList.Add(elem);
                }

                // package配下で diagram ノードを見つけたら
                if ("diagram".Equals(pkgNode.Name))
                {
                    retDiagramList.Add(readDiagramNode(pkgNode));
                }

            }

			pkgvo.childPackageList = retList;
			pkgvo.elements = retElementList;
            pkgvo.diagrams = retDiagramList;

		}



        // <diagram guid="{F491CD2F-BA6F-4570-9AF8-41589EBBBF95}" tpos="0" type="Logical" name="エラー画面" showDetails="0" attPub="True" attPri="True" attPro="True" cx="787" cy="1130" scale="75" orientation="P" createdDate="2018/07/05 13:08:58" swimlanes="locked=false;orientation=0;width=0;inbar=false;names=false;color=-1;bold=false;fcol=0;tcol=-1;ofCol=-1;ufCol=-1;hl=1;ufh=0;hh=0;cls=0;bw=0;hli=0;">
        private DiagramVO readDiagramNode(XmlNode diagNode)
        {
            DiagramVO diag = new DiagramVO();
            foreach (XmlAttribute attr in diagNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "name":
                        diag.name = attr.Value;
                        break;
                    case "guid":
                        diag.guid = attr.Value;
                        break;
                    case "stereotype":
                        diag.stereotype = attr.Value;
                        break;
                    case "tpos":
                        diag.treePos = readAttributeIntValue(attr);
                        break;
                    case "changed":
                        diag.changed = readAttributeCharValue(attr);
                        break;
                    case "type":
                        diag.diagramType = attr.Value;
                        break;
                    case "showDetails":
                        diag.showDetails = readAttributeIntValue(attr);
                        break;
                    case "attPub":
                        diag.attPub = "True".Equals(attr.Value);
                        break;
                    case "attPri":
                        diag.attPri = "True".Equals(attr.Value);
                        break;
                    case "attPro":
                        diag.attPro = "True".Equals(attr.Value);
                        break;
                    case "cx":
                        diag.cx = readAttributeIntValue(attr);
                        break;
                    case "cy":
                        diag.cy = readAttributeIntValue(attr);
                        break;
                    case "scale":
                        diag.scale = readAttributeIntValue(attr);
                        break;
                    case "orientation":
                        diag.orientation = attr.Value;
                        break;
                    case "createdDate":
                        diag.createdDate = readAttributeDateTimeValue(attr);
                        break;
                    case "swimlanes":
                        diag.swimlanes = attr.Value;
                        break;
                }
            }

            List<DiagramObjectVO> retDiagObjList = new List<DiagramObjectVO>();
            List<DiagramLinkVO> retDiagLinkList = new List<DiagramLinkVO>();

            foreach (XmlNode diachNode in diagNode.ChildNodes)
            {
                switch (diachNode.Name)
                {
                    case "notes":
                        diag.notes = diachNode.InnerText;
                        break;
                    case "diagramObject":
                        retDiagObjList.Add(readDiagramObjectNode(diachNode));
                        break;
                    case "diagramLink":
                        retDiagLinkList.Add(readDiagramLinkNode(diachNode));
                        break;
                }
            }

            diag.diagramObjects = retDiagObjList;
            diag.diagramLinks = retDiagLinkList;
            return diag;
        }


        private static DiagramObjectVO readDiagramObjectNode(XmlNode diagObjNode)
        {
            DiagramObjectVO diagObj = new DiagramObjectVO();

            foreach (XmlAttribute attr in diagObjNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "sequence":
                        diagObj.sequence = readAttributeIntValue(attr);
                        break;
                    case "objectId":
                        diagObj.objectId = readAttributeIntValue(attr);
                        break;
                    case "rectTop":
                        diagObj.rectTop = readAttributeIntValue(attr);
                        break;
                    case "rectLeft":
                        diagObj.rectLeft = readAttributeIntValue(attr);
                        break;
                    case "rectRight":
                        diagObj.rectRight = readAttributeIntValue(attr);
                        break;
                    case "rectBottom":
                        diagObj.rectBottom = readAttributeIntValue(attr);
                        break;
                    case "instanceId":
                        diagObj.instanceId = readAttributeIntValue(attr);
                        break;
                    case "objectStyle":
                        diagObj.objectStyle = attr.Value;
                        break;
                }
            }

            return diagObj;
        }


        private static DiagramLinkVO readDiagramLinkNode(XmlNode diagLinkNode)
        {
            DiagramLinkVO diagLinkObj = new DiagramLinkVO();

            foreach (XmlAttribute attr in diagLinkNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "connectorId":
                        diagLinkObj.connectorId = readAttributeIntValue(attr);
                        break;
                    case "hidden":
                        diagLinkObj.hidden = readAttributeBooleanValue(attr);
                        break;
                    case "geometry":
                        diagLinkObj.geometry = attr.Value;
                        break;
                }
            }

            return diagLinkObj;
        }

		#endregion


		/// <summary>
		/// 成果物配下の全要素を読み込んで返却する。
        /// elementノード直下の属性のみ読み込み、attribute/method以下は読み込まない。
		/// </summary>
		/// <param name="artifact"></param>
		/// <param name="outElementNodes"></param>
		/// <returns></returns>
		public List<ElementVO> readAllElements(ArtifactVO artifact, ref XmlNodeList outElemNodes)
		{
			string fileName = this.artifactsPath + "/" + "atf_" + artifact.guid.Substring(1, 36) + ".xml";

			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( fileName );

			List<ElementVO> retList = new List<ElementVO>();

			// elementノードを全て取得する
			outElemNodes = xmlDoc.SelectNodes("//element");

			if ( outElemNodes != null ) {
				foreach( XmlNode elemNode in outElemNodes ) {
					ElementVO elemvo = new ElementVO();
					foreach(XmlAttribute attr in elemNode.Attributes) {
						switch( attr.Name ) {
                            case "elementId": elemvo.elementId = readAttributeIntValue(attr); break;
                            case "name" : elemvo.name = attr.Value; break;
							case "alias" : elemvo.alias = attr.Value; break;
							case "type" : elemvo.eaType = attr.Value; break;
							case "stereotype" : elemvo.stereoType = attr.Value; break;
							case "guid" : elemvo.guid = attr.Value; break;
						}
					}
					retList.Add(elemvo);
				}

			}

			return retList;
		}

		/// <summary>
		/// artifact-xmlファイルから、指定されたGUIDのattributeノードを検索し、XmlNodeで返却する。
		/// （ArtifactDifferで差分詳細（属性、操作）を作成する時に使用）
		/// </summary>
		/// <param name="artifactGuid">成果物のGUID</param>
		/// <param name="attributeGuid">属性のGUID</param>
		/// <returns></returns>
		public XmlNode readAttributeNode(string artifactGuid, string attributeGuid) {
			string target_dir = null;
			if ( this.artifactsPath != null ) {
				target_dir = this.artifactsPath;
			} else {
				throw new ArgumentException();
			}

			string fileName = target_dir + "/" + "atf_" + artifactGuid.Substring(1, 36) + ".xml"  ;

			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( fileName );

			// 対象のattributeNodeを探し、返却する
			XmlNode attributeNode = xmlDoc.SelectSingleNode("//attribute[@guid='" + attributeGuid + "']");
			return attributeNode;
		}

		/// <summary>
		/// artifact-xmlファイルから、指定されたGUIDのmethodノードを検索し、XmlNodeで返却する。
		/// （ArtifactDifferで差分詳細（属性、操作）を作成する時に使用）
		/// </summary>
		/// <param name="artifactGuid"></param>
		/// <param name="methodGuid"></param>
		/// <returns></returns>
		public XmlNode readMethodNode(string artifactGuid, string methodGuid) {
			string target_dir = null;
			if ( this.artifactsPath != null ) {
				target_dir = this.artifactsPath;
			} else {
				throw new ArgumentException("ProjectPathを指定してください");
			}

			string fileName = target_dir + "/" + "atf_" + artifactGuid.Substring(1, 36) + ".xml"  ;

			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( fileName );

			// 対象のattributeNodeを探し、返却する
			XmlNode methodNode = xmlDoc.SelectSingleNode("//method[@guid='" + methodGuid + "']");
			return methodNode;
		}


        /// <summary>
        /// 成果物XMLファイルから指定されたGUIDのタグ付き値のXMLノードを取得する
        /// </summary>
        /// <param name="artifactGuid">成果物GUID(ファイル名として使用)</param>
        /// <param name="tagValGuid">タグ付き値GUID(XMLのキーとして検索)</param>
        /// <returns></returns>
        public XmlNode readTaggedValueNode(string artifactGuid, string tagValGuid)
        {
            string target_dir = null;
            if (this.artifactsPath != null)
            {
                target_dir = this.artifactsPath;
            }
            else
            {
                throw new ArgumentException("ProjectPathを指定してください");
            }

            string fileName = target_dir + "/" + "atf_" + artifactGuid.Substring(1, 36) + ".xml";

            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            // 対象のtaggedValueNode(tvタグ)を探し、返却する
            XmlNode taggedValueNode = xmlDoc.SelectSingleNode("//tv[@guid='" + tagValGuid + "']");
            return taggedValueNode;
        }

        /// <summary>
        /// detail の attribute XML (/detail/#attribute_${guid}_${LR}.xml) を読み、得られたVOを返却する
        ///
        /// </summary>
        /// <returns>AttributeVO</returns>
        public AttributeVO readAttributeDiffDetail(string attributeGuid, string leftRight)
		{
			string target_dir = null;
			if ( this.artifactsPath != null ) {
				target_dir = this.artifactsPath;
			} else {
				throw new ArgumentException();
			}

			string fileName = target_dir + "/detail/" + "#attribute_" + attributeGuid.Substring(1, 36) + "_" + leftRight + ".xml"  ;

			// 指定されたfileNameでファイルが存在しなかったらnullを返す
			if (!System.IO.File.Exists(fileName)) {
				return null;
			}

			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( fileName );

			// attributeノードに移動する
			XmlNode attrNode = xmlDoc.SelectSingleNode("attribute");

			if ( attrNode != null ) {
				return ElementsXmlReader.readAttribute(attrNode) ;
			} else {
				return new AttributeVO();
			}

		}


		/// <summary>
		/// detail の method XML (/detail/#method_${guid}_${LR}.xml) を読み、得られたVOを返却する
		///
		/// </summary>
		/// <returns>MethodVO</returns>
		public MethodVO readMethodDiffDetail(string methodGuid, string leftRight)
		{
			string target_dir = null;
			if ( this.artifactsPath != null ) {
				target_dir = this.artifactsPath;
			} else {
				throw new ArgumentException();
			}

			string fileName = target_dir + "/detail/" + "#method_" + methodGuid.Substring(1, 36) + "_" + leftRight + ".xml"  ;

			// 指定されたfileNameでファイルが存在しなかったらnullを返す
			if (!System.IO.File.Exists(fileName)) {
				return null;
			}

			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( fileName );

			// methodノードに移動する
			XmlNode mthNode = xmlDoc.SelectSingleNode("method");

			if ( mthNode != null ) {
				return ElementsXmlReader.readMethod(mthNode) ;
			} else {
				return new MethodVO();
			}

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
            if (attr.Value != null &&  !"".Equals(attr.Value) )
            {
                return attr.Value[0];
            } else {
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
