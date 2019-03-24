using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

namespace ArtifactFileAccessor.reader
{
	/// <summary>
	/// Description of ElementsXmlReader.
	/// </summary>
	public class ElementsXmlReader
	{
		private XmlDocument xmlDoc = null;
        private string target_dir ;

        public ElementsXmlReader(string project_dir)
		{
			if( project_dir == null ) {
				throw new ArgumentException("project_dirを指定してください");
			}

            target_dir = project_dir;

            string target_file = "ElemList_all.xml";
			string fileName = target_dir + "/" + target_file;

			// XMLテキストをロードする
            this.xmlDoc = new XmlDocument();
            this.xmlDoc.Load( fileName );
		}

        /// <summary>
        /// ElementList_all.xml を読み、リスト化する
        ///
        /// </summary>
        /// <returns>ElementVOのリスト</returns>
        public ElementVO readElementByGUID(string guid)
        {
            // 対象のelementノードに移動する
            XmlNode elemNode = this.xmlDoc.SelectSingleNode("//element[@guid='" + guid + "']");

            // GUIDをキーとしたXMLツリーの検索でマッチしたら
            if ( elemNode != null ) {
            	// readElementメソッドの結果をそのまま返却（sort条件はPOS・名称によるソート）
				return readElement(elemNode, true);
            } else {
            	ElementVO elemvo = new ElementVO();
            	elemvo.name = "(Unknown Object)";
            	elemvo.guid = "";

            	return elemvo;
            }

        }


		/// <summary>
		/// XMLノードからの要素（クラス）の情報読み込み
		/// </summary>
		/// <param name="elementNode"></param>
		/// <returns></returns>
		public static ElementVO readElement(XmlNode elementNode, bool sortByPosFlg)
		{
			ElementVO elem = new ElementVO();
			foreach (XmlAttribute attr in elementNode.Attributes) {
				switch (attr.Name) {
					case "name":
						elem.name = attr.Value;
						break;
					case "guid":
						elem.guid = attr.Value;
						break;
					case "type":
						elem.eaType = attr.Value;
						break;
					case "alias":
						elem.alias = attr.Value;
						break;
					case "stereotype":
						elem.stereoType = attr.Value;
						break;
					case "tpos":
						Int32 p;
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						elem.treePos = p;
						break;
					case "changed":
						elem.changed = attr.Value[0];
						break;
				}
			}

			readElementContents(elementNode, sortByPosFlg, ref elem);

			return elem;
		}

		/// <summary>
		/// 要素ごとの変更済(changed)ファイルが存在するかを確認する
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static bool existChangedElementFile(string guid) {
			return File.Exists(getChangedElementFilePath(guid));
		}

		/// <summary>
		/// 要素ごとのXMLファイルが存在するかを確認する
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static bool existElementFile(string guid) {
            //string elementsDir = ProjectSetting.getVO().projectPath + @"\elements";

            //if ( !Directory.Exists(elementsDir) )
            //{
            //return false;
            //} else {
            //return File.Exists(getElementFilePath(guid));
            //}

            return false;
        }

        /// <summary>
        /// 変更済み要素ファイルを読み込み、結果を要素VOとして返す
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ElementVO readChangedElementFile(string guid) {
			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( getChangedElementFilePath(guid) );

			// elementノードに移動する
			XmlNode elementNode = xmlDoc.SelectSingleNode("element");

			return readElement(elementNode, true);
		}

		public static ElementVO readElementFile(string guid) {
			// XMLテキストをロードする
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( getElementFilePath(guid) );

			// elementノードに移動する
			XmlNode elementNode = xmlDoc.SelectSingleNode("element");

			return readElement(elementNode, true);
        }


		/// <summary>
		/// 変更済み要素ファイルのフルパスを取得する
		/// </summary>
		/// <param name="guid">対象要素のGUID</param>
		/// <returns>変更済み要素ファイルのフルパス</returns>
		private static string getChangedElementFilePath(string guid) {
			string outputDir = ProjectSetting.getVO().projectPath + @"\elements" ;
            //	string changedFile = outputDir + @"\" + guid.Substring(1,1) + @"\" + guid.Substring(2,1) + @"\" + guid.Substring(1,36) + "_changed.xml";
            string changedFile = outputDir + @"\" + guid.Substring(1,36) + "_changed.xml";
            return changedFile;
		}


		/// <summary>
		/// 要素ファイルのフルパスを取得する
		/// </summary>
		/// <param name="guid">対象要素のGUID</param>
		/// <returns>要素ファイルのフルパス</returns>
		private static string getElementFilePath(string guid) {
			string outputDir = ProjectSetting.getVO().projectPath + @"\elements" ;
			string elementFile = outputDir + @"\" + guid.Substring(1,1) + @"\" + guid.Substring(2,1) + @"\" + guid.Substring(1,36) + ".xml";
			return elementFile;
		}


		/// <summary>
		///
		/// </summary>
		/// <param name="elemNode"></param>
		/// <param name="sortByPosFlg"></param>
		/// <param name="elemvo"></param>
		private static void readElementContents(XmlNode elemNode, bool sortByPosFlg, ref ElementVO elemvo) {
			List<AttributeVO> retAttrList = new List<AttributeVO>();
			List<MethodVO> retMethList = new List<MethodVO>();
			List<TaggedValueVO> retTagValList = new List<TaggedValueVO>();
			List<ConnectorVO> retConnList = new List<ConnectorVO>();

			foreach (XmlNode eNode in elemNode.ChildNodes)
			{
				// ElementReferenceの読み込み
				if ( "ref".Equals(eNode.Name) ) {
					elemvo.elementReferenceVO = readElementReference(eNode);
				}

				// attributesの読み込み
				if ( "attribute".Equals(eNode.Name) ) {
					retAttrList.Add(readAttribute(eNode));
				}

				// methodsの読み込み
				if ( "method".Equals(eNode.Name) ) {
					retMethList.Add(readMethod(eNode));
				}

				// タグ付き値の読み込み
				if ( "taggedValues".Equals(eNode.Name) ) {
					retTagValList = readTaggedValues(eNode);
				}

				// クラス間接続情報の読み込み
				//    this.connReader.readConnectorByGUID(convo, elemvo);
				//    retConnList.Add(convo);

			}

			elemvo.attributes = retAttrList;
			elemvo.methods = retMethList;
			elemvo.taggedValues = retTagValList;

			// ConnectionSearcher
			elemvo.connectors = new List<ConnectorVO>();

			// 要素配下のソート
			if (sortByPosFlg) {
				elemvo.sortAttributes();
				elemvo.sortMethods();
				elemvo.sortTaggedValues();
			} else {
				elemvo.sortAttributesGUID();
				elemvo.sortMethodsGUID();
				elemvo.sortTaggedValuesGUID();
			}

		}


        /// <summary>
        /// XMLのelementRefノードを読み取り、ElementReferenceVOを返却する
        /// </summary>
        /// <param name="refNode">elementRefferenceノード</param>
        /// <returns></returns>
        private static ElementReferenceVO readElementReference(XmlNode refNode) {
			ElementReferenceVO ervo = new ElementReferenceVO();

			foreach (XmlAttribute attr in refNode.Attributes) {
				switch (attr.Name) {
					case "gentype":
						ervo.gentype = attr.Value;
						break;
					case "fqcn":
						ervo.fqcn = attr.Value;
						break;
					case "genfile":
						ervo.genfile = attr.Value;
						break;
				}

			}

			return ervo;
		}


        /// <summary>
        /// XMLの属性ノードを読み取り、AttributeVOを返却する
        /// </summary>
        /// <param name="aNode">attributeノード</param>
        /// <returns></returns>
        internal static AttributeVO readAttribute(XmlNode aNode)
		{
			AttributeVO attvo = new AttributeVO();
			Int32 p=0;

			foreach (XmlAttribute attr in aNode.Attributes) {
				switch (attr.Name) {
					case "name":
						attvo.name = attr.Value;
						break;
					case "alias":
						attvo.alias = attr.Value;
						break;
					case "stereotype":
						attvo.stereoType = attr.Value;
						break;
                    case "type":
                        attvo.eaType = attr.Value;
                        break;
                    case "guid":
						attvo.guid = attr.Value;
						break;
					case "pos":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						attvo.pos = p;
						break;
					case "allowDuplicates":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}

						if ( p != 0 ) {
							attvo.allowDuplicates = true;
						} else {
							attvo.allowDuplicates = false;
						}
						break;

					case "length":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						attvo.length = p;
						break;

					case "classifierID":
						attvo.classifierID = attr.Value;
						break;

					case "container":
						attvo.container = attr.Value;
						break;
					case "containment":
						attvo.containment = attr.Value;
						break;
					case "isDerived":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}

						if ( p != 0 ) {
							attvo.isDerived = true;
						} else {
							attvo.isDerived = false;
						}
						break;
					case "isID":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}

						if ( p != 0 ) {
							attvo.isID = true;
						} else {
							attvo.isID = false;
						}
						break;
					case "lowerBound":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						attvo.lowerBound = p;
						break;

					case "upperBound":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						attvo.upperBound = p;
						break;
					case "precision":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						attvo.precision = p;
						break;
					case "scale":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						attvo.scale = p;
						break;

					case "changed":
						attvo.changed = attr.Value[0];
						break;
				}

			}

			if (aNode.SelectSingleNode("visibility") != null) {
				attvo.visibility = aNode.SelectSingleNode("visibility").InnerText;
			}

			if (aNode.SelectSingleNode("notes") != null) {
				attvo.notes = aNode.SelectSingleNode("notes").InnerText;
			}

            XmlNode srcAttrNode = aNode.SelectSingleNode("srcAttribute");
            XmlNode destAttrNode = aNode.SelectSingleNode("destAttribute");
            if (attvo.changed == 'U' && srcAttrNode != null && destAttrNode != null)
            {
                attvo.srcAttribute = readAttribute(srcAttrNode.SelectSingleNode("attribute"));
                attvo.destAttribute = readAttribute(destAttrNode.SelectSingleNode("attribute"));
            }

			return attvo;
		}


		internal static MethodVO readMethod(XmlNode mNode)
        {
            MethodVO mthvo = new MethodVO();
            Int32 p = 0;

            foreach (XmlAttribute attr in mNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "name":
                        mthvo.name = attr.Value;
                        break;
                    case "alias":
                        mthvo.alias = attr.Value;
                        break;
                    case "stereotype":
                        mthvo.stereoType = attr.Value;
                        break;
                    case "guid":
                        mthvo.guid = attr.Value;
                        break;
                    case "pos":
                        if (!Int32.TryParse(attr.Value, out p))
                        {
                            p = 0;
                        }
                        mthvo.pos = p;
                        break;

                    case "classifierID":
                        mthvo.classifierID = attr.Value;
                        break;

                    case "isConst":
                        if (!Int32.TryParse(attr.Value, out p))
                        {
                            p = 0;
                        }

                        if (p != 0)
                        {
                            mthvo.isConst = true;
                        }
                        else
                        {
                            mthvo.isConst = false;
                        }
                        break;
                    case "isLeaf":
                        mthvo.isLeaf = Boolean.Parse(attr.Value);
                        break;
                    case "isPure":
                        mthvo.isPure = Boolean.Parse(attr.Value);
                        break;
                    case "isQuery":
                        mthvo.isQuery = Boolean.Parse(attr.Value);
                        break;
                    case "isRoot":
                        mthvo.isRoot = Boolean.Parse(attr.Value);
                        break;
                    case "returnIsArray":
                        if (!Int32.TryParse(attr.Value, out p))
                        {
                            p = 0;
                        }

                        if (p != 0)
                        {
                            mthvo.returnIsArray = true;
                        }
                        else
                        {
                            mthvo.returnIsArray = false;
                        }
                        break;
                    case "stateFlags":
                        mthvo.stateFlags = attr.Value;
                        break;

                    case "changed":
                        mthvo.changed = attr.Value[0];
                        break;
                }
            }

            // method タグの子要素をなめる
            mthvo.behavior = "";
            mthvo.notes = "";
            mthvo.returnType = "";
            mthvo.visibility = "";
            mthvo.parameters = new List<ParameterVO>();

            foreach(XmlNode mc in mNode.ChildNodes)
            {
                switch (mc.Name)
                {
                    case "behavior":
                        mthvo.behavior = mc.InnerText;
                        break;
                    case "notes":
                        mthvo.notes = mc.InnerText;
                        break;
                    case "returnType":
                        mthvo.returnType = mc.InnerText;
                        break;
                    case "visibility":
                        mthvo.visibility = mc.InnerText;
                        break;
                    case "parameters":
                        mthvo.parameters = readParameters(mc);
                        break;
                }

            }

            XmlNode srcMthNode = mNode.SelectSingleNode("srcMethod");
            XmlNode destMthNode = mNode.SelectSingleNode("destMethod");
            if (mthvo.changed == 'U' && srcMthNode != null && destMthNode != null)
            {
                mthvo.srcMethod = readMethod(srcMthNode.SelectSingleNode("method"));
                mthvo.destMethod = readMethod(destMthNode.SelectSingleNode("method"));
            }

            return mthvo;
        }

        // parameters タグの子要素（paramaterタグ）を複数読み込む
        private static List<ParameterVO> readParameters(XmlNode mcNode)
        {
            List<ParameterVO> retParams = new List<ParameterVO>();

            // パラメータを読み込み
            XmlNodeList prms = mcNode.SelectNodes("parameter");
            if (prms != null)
            {
                foreach (XmlNode pNode in prms)
                {
                    ParameterVO param = readMethodParam(pNode);
                    retParams.Add(param);
                }
            }

            return retParams;
        }



        private static ParameterVO readMethodParam( XmlNode pNode ) {
			ParameterVO prmvo = new ParameterVO();
			Int32 p=0;

			foreach (XmlAttribute attr in pNode.Attributes) {
				switch (attr.Name) {
					case "name":
						prmvo.name = attr.Value;
						break;
					case "alias":
						prmvo.alias = attr.Value;
						break;
					case "stereotype":
						prmvo.stereoType = attr.Value;
						break;
                    case "type":
                        prmvo.eaType = attr.Value;
                        break;
                    case "guid":
						prmvo.guid = attr.Value;
						break;
					case "pos":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}
						prmvo.pos = p;
						break;

					case "classifier":
						prmvo.classifierID = attr.Value;
						break;
					case "objectType":
						prmvo.objectType = attr.Value;
						break;
					case "default":
						prmvo.defaultValue = attr.Value;
						break;
					case "const":
						if (!Int32.TryParse(attr.Value, out p)) {
							p = 0;
						}

						if ( p != 0 ) {
							prmvo.isConst = true;
						} else {
							prmvo.isConst = false;
						}
						break;
					case "kind":
						prmvo.kind = attr.Value;
						break;
					case "style":
						prmvo.styleEx = attr.Value;
						break;
				}
			}

			if (pNode.SelectSingleNode("notes") != null) {
				prmvo.notes = pNode.SelectSingleNode("notes").InnerText;
			}

			return prmvo;
		}


		private static List<TaggedValueVO> readTaggedValues(XmlNode tvNode) {
			List<TaggedValueVO> retTagVal = new List<TaggedValueVO>();

			foreach (XmlNode tagvalNode in tvNode.ChildNodes) {
                retTagVal.Add(readTaggedValue(tagvalNode));
			}

			return retTagVal ;
		}


        private static TaggedValueVO readTaggedValue(XmlNode tagvalNode)
        {
            TaggedValueVO tvvo = new TaggedValueVO();
            foreach (XmlAttribute attr in tagvalNode.Attributes)
            {
                switch (attr.Name)
                {
                    case "name":
                        tvvo.name = attr.Value;
                        break;

                    case "guid":
                        tvvo.guid = attr.Value;
                        break;

                    case "value":
                        tvvo.tagValue = attr.Value;
                        if ("<memo>".Equals(attr.Value))
                        {
                            tvvo.notes = tagvalNode.InnerText;
                        }
                        break;

                    case "changed":
                        tvvo.changed = attr.Value[0];
                        break;
                }
            }

            // タグ付き値のタグ(tv)の子ノードに <notes> タグがあったら
            XmlNode notesNode = tagvalNode.SelectSingleNode("notes");
            if (notesNode != null)
            {
                // notesタグの中身 を タグ付き値の notes とする
                tvvo.notes = notesNode.InnerText;
            }

            XmlNode srcTagNode = tagvalNode.SelectSingleNode("srcTaggedValue");
            XmlNode destTagNode = tagvalNode.SelectSingleNode("destTaggedValue");
            if (tvvo.changed == 'U' && srcTagNode != null && destTagNode != null)
            {
                tvvo.srcTaggedValue = readTaggedValue(srcTagNode.SelectSingleNode("tv"));
                tvvo.destTaggedValue = readTaggedValue(destTagNode.SelectSingleNode("tv"));
            }

            return tvvo;
        }



        /// <summary>
        /// (deprecated)
        /// </summary>
        /// <param name="elemvo"></param>
        /// <param name="parentNode"></param>
        private void readElementContents__( ElementVO elemvo, XmlNode parentNode ) {
        	List<AttributeVO> retAttrList = new List<AttributeVO>();
        	List<MethodVO> retMethList = new List<MethodVO>();
        	IList<ConnectorVO> retConnList = new List<ConnectorVO>();

    		foreach (XmlNode elemNode in parentNode.ChildNodes)
            {
    			if ( "attribute".Equals(elemNode.Name) ) {
    				AttributeVO attvo = new AttributeVO();
					foreach(XmlAttribute attr in elemNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : attvo.name = attr.Value; break;
    						case "alias" : attvo.alias = attr.Value; break;
//    						case "stereotype" : attvo.stereoType = attr.Value; break;
    						case "guid" : attvo.guid = attr.Value; break;
//    						case "pos" : attvo.pos = attr.Value; break;
    					}
    				}

    				retAttrList.Add(attvo);
    			}

    			if ( "method".Equals(elemNode.Name) ) {

    				MethodVO mthvo = new MethodVO();
					foreach(XmlAttribute attr in elemNode.Attributes) {
    					switch( attr.Name ) {
    						case "name" : mthvo.name = attr.Value; break;
    						case "alias" : mthvo.alias = attr.Value; break;
//    						case "stereotype" : mthvo.stereoType = attr.Value; break;
    						case "guid" : mthvo.guid = attr.Value; break;
//    						case "pos" : mthvo.pos = attr.Value; break;
    					}
    				}

    				if ( elemNode.SelectSingleNode("behavior") != null ) {
    					mthvo.behavior = elemNode.SelectSingleNode("behavior").InnerText;
    				}
    				if ( elemNode.SelectSingleNode("notes") != null ) {
						mthvo.notes = elemNode.SelectSingleNode("notes").InnerText;
    				}
//					mthvo.returnType = elemNode.elemNode.SelectSingleNode("returnType").InnerText;
//					mthvo.visibility = elemNode.elemNode.SelectSingleNode("visibility").InnerText;

    				retMethList.Add(mthvo);
            	}

    		}

			elemvo.attributes = retAttrList;
    		elemvo.sortAttributes();
    		elemvo.methods = retMethList;
    		elemvo.sortMethods();

//    		elemvo.connectors = connSearcher.findByObjectGuid(elemvo.guid);
        }

	}



}
