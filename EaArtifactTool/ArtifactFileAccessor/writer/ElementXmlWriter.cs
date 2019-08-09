using System;
using System.IO;

using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.writer
{
	/// <summary>
	/// Description of ElementXmlWriter.
	/// </summary>
	public class ElementXmlWriter
	{
		public ElementXmlWriter()
		{
		}

        /// <summary>
        /// パラメータの要素を、要素単体のXMLファイルに出力する。
        /// 要素のGUID から "{}" を除いた文字列をファイル名とし、
        /// プロジェクトパスの要素ファイル配置ディレクトリ:elements配下に出力する。
        /// 
        /// ただし要素数単位となるとファイル数が大量となり、全てをelements直下に出すと
        /// ファイルアクセス時の速度低下が予測されるので GUIDの１文字目、２文字目で
        /// それぞれディレクトリを作成し、出力先を分散させる。
        /// </summary>
        /// <param name="elem">ファイル出力したい要素</param>
        /// <returns>ファイル出力が成功したらtrue</returns>
        public static bool outputElementXmlFile(ElementVO elem) {
			string outputDir = ProjectSetting.getVO().projectPath;
			bool retFlg = false;
			StreamWriter swe = null;
			try { 
				// 要素XMLを出力するフォルダ (projectDir + /elements/ 配下) が存在するかを調べ、なければ作る
				string edir = outputDir + @"\elements\" + elem.guid.Substring(1,1) + @"\" + elem.guid.Substring(2,1) ;
				checkAndMakeElementDir(edir);

                //BOM無しのUTF8でテキストファイルを作成する
				swe = new StreamWriter(edir + @"\" + elem.guid.Substring(1,36) + ".xml");
				swe.WriteLine( @"<?xml version=""1.0"" encoding=""UTF-8""?>" );

                // このStreamに対象の要素をXML出力する
				writeElementXml(elem, 0, swe);
				retFlg = true;
			} catch(Exception exp) {
				Console.WriteLine(exp.Message);
			} finally {
				if (swe != null) swe.Close();
			}
			
			return retFlg;
		}
		

        /// <summary>
        /// 要素ディレクトリの存在チェックとディレクトリ作成
        /// </summary>
        /// <param name="edir">目的の要素ディレクトリ</param>
		private static void checkAndMakeElementDir(string edir) {
			if (!Directory.Exists(edir)) {
				Directory.CreateDirectory(edir);
			}
		}


        #region 要素の出力処理
        /// <summary>
        /// パラメータのStreamに向けて要素配下のXML文書を出力。
        /// 
        /// ##タグの構成
        /// Element
        ///   +- TaggedValue
        ///   +- Attribute
        ///      +- AttributeTag
        ///   +- Method
        ///      +- MethodTag
        ///      +- Parameter
        ///         +- ParameterTag
        /// </summary>
        /// <param name="elemvo">要素VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        public static void writeElementXml(ElementVO elemvo, int depth, StreamWriter sw) {

            //　elementの保有プロパティ出力
            outputElementPropertyXml(elemvo, depth, sw);

            // クラスのタグ付き値出力
            if (elemvo.taggedValues.Count > 0) {
				outputClassTags(elemvo, depth+1, sw);
			}
	
			// クラスの属性出力
			if (elemvo.attributes.Count > 0) {
				outputClassAttributes(elemvo, depth+1, sw);
			}
	
			// クラスの操作（メソッド）出力
			if (elemvo.methods.Count > 0) {
				outputClassMethods(elemvo, depth+1, sw);
			}
	
			// elementの閉じタグ
			sw.WriteLine(indent(depth) + "</element>");

		}

        /// <summary>
        /// elementタグの出力処理
        /// </summary>
        /// <param name="elemvo">要素VO</param>
        /// <param name="depth">深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputElementPropertyXml(ElementVO elemvo, int depth, StreamWriter sw)
        {
            sw.Write(indent(depth) + "<element ");
            if (elemvo.changed != ' ')
            {
                sw.Write("changed=\"" + elemvo.changed + "\" ");
                if(elemvo.propertyChanged != ' ')
                {
                    sw.Write("propChanged=\"" + elemvo.propertyChanged + "\" "); 
                }
            }
            sw.Write("guid=\"" + escapeXML(elemvo.guid) + "\" ");
            sw.Write("tpos=\"" + elemvo.treePos + "\" ");
            sw.Write("type=\"" + elemvo.eaType + "\" ");
            sw.Write("name=\"" + escapeXML(elemvo.name) + "\" ");
            sw.Write("alias=\"" + escapeXML(elemvo.alias) + "\" ");
            sw.Write("elementId=\"" + elemvo.elementId + "\" ");
            sw.Write("parentId=\"" + elemvo.parentID + "\"");

            if (elemvo.stereoType != null)
            {
                sw.Write(" stereotype=\"" + escapeXML(elemvo.stereoType) + "\"");
            }
            sw.WriteLine(">");

            ElementReferenceVO elemref = elemvo.elementReferenceVO;
            if (elemref != null)
            {
                sw.Write(indent(depth + 1) + "<ref ");
                sw.Write("gentype=\"" + escapeXML(elemref.gentype) + "\" ");
                sw.Write("fqcn=\"" + escapeXML(elemref.fqcn) + "\" ");
                sw.Write("genfile=\"" + escapeXML(elemref.genfile) + "\"");
                sw.WriteLine("/>");
            }

            // ノートが入っていたら notes タグを出力
            if (elemvo.notes != null)
            {
                //if( elemvo.notes.IndexOf("&lt;") >= 0)
                //{
                //    Console.WriteLine("XMLの多重エスケープが疑われるノート発見:");
                //    Console.WriteLine("GUID:" + elemvo.guid);
                //    Console.WriteLine("Notes:" + elemvo.notes);
                //}

                sw.WriteLine(indent(depth + 1) + "<notes>" + escapeXML(elemvo.notes) + "</notes>");
            }

            if( elemvo.propertyChanged == 'U' && elemvo.srcElementProperty != null && elemvo.destElementProperty != null)
            {
                depth++;

                // srcElementProperty を出力
                sw.WriteLine(indent(depth) + "<srcElementProperty>");
                outputElementDiffedProperty(elemvo.srcElementProperty, depth+1, sw);
                sw.WriteLine(indent(depth) + "</srcElementProperty>");

                // destElementProperty を出力
                sw.WriteLine(indent(depth) + "<destElementProperty>");
                outputElementDiffedProperty(elemvo.destElementProperty, depth + 1, sw);
                sw.WriteLine(indent(depth) + "</destElementProperty>");
            }


        }



        private static void outputElementDiffedProperty(ElementPropertyVO elemPropvo, int depth, StreamWriter sw)
        {
            sw.Write(indent(depth) + "<elementProperty ");
            sw.Write("tpos=\"" + elemPropvo.treePos + "\" ");
            sw.Write("type=\"" + elemPropvo.eaType + "\" ");
            sw.Write("name=\"" + escapeXML(elemPropvo.name) + "\" ");
            sw.Write("alias=\"" + escapeXML(elemPropvo.alias) + "\" ");

            if (elemPropvo.stereoType != null)
            {
                sw.Write(" stereotype=\"" + escapeXML(elemPropvo.stereoType) + "\"");
            }
            sw.WriteLine(">");

            // ノートが入っていたら notes タグを出力
            if (elemPropvo.notes != null)
            {
                sw.WriteLine(indent(depth + 1) + "<notes>" + escapeXML(elemPropvo.notes) + "</notes>");
            }

			sw.WriteLine( indent(depth) + "</elementProperty>");
		}


        // 1.2.1 クラスのタグ付き値を出力
        private static void outputClassTags(ElementVO currentElement, int depth, StreamWriter sw)
        {

            if (currentElement.taggedValues.Count <= 0)
            {
                return;
            }

            sw.WriteLine(indent(depth) + "<taggedValues>");

            // 　取得できたタグ付き値の情報をファイルに展開する
            foreach (TaggedValueVO tagv in currentElement.taggedValues)
            {
                if ("<memo>".Equals(tagv.tagValue))
                {
                    if (tagv.notes != null)
                    {
                        sw.WriteLine(indent(depth) + "  <tv guid=\"" + escapeXML(tagv.guid) + "\" name=\"" + escapeXML(tagv.name) + "\" value=\"" + escapeXML(tagv.tagValue) + "\">" + escapeXML(tagv.notes) + "</tv>");
                    }
                }
                else
                {
                    sw.WriteLine(indent(depth) + "  <tv guid=\"" + escapeXML(tagv.guid) + "\" name=\"" + escapeXML(tagv.name) + "\" value=\"" + escapeXML(tagv.tagValue) + "\"/>");
                }
            }

            sw.WriteLine(indent(depth) + "</taggedValues>");
        }

        #endregion


        #region 属性の出力処理
        /// <summary>
        /// クラスの属性を出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="currentElement">要素VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputClassAttributes( ElementVO currentElement, int depth, StreamWriter sw ) {
	
			if ( currentElement.attributes.Count <= 0 ) {
				return;
			}
	
			//　取得できた属性の情報をファイルに展開する
			foreach(AttributeVO m_Att in currentElement.attributes) {
				outputAttribute(m_Att, depth, sw);
			}
	
		}


        /// <summary>
        /// 属性を１件出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="att">属性VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputAttribute(AttributeVO att, int depth, StreamWriter sw )
        {
			sw.Write( indent(depth) + "<attribute " );
			if( att.changed != ' ' ) {
				sw.Write( "changed=\"" + att.changed + "\" " );
			}
			sw.Write( "guid=\"" + escapeXML(att.guid) + "\" " );
			sw.Write( "pos=\"" + att.pos + "\" " ); 
			sw.Write( "type=\"" + escapeXML(att.eaType) + "\" " );
			sw.Write( "name=\"" + escapeXML(att.name) + "\" " );
			sw.Write( "alias=\"" + escapeXML(att.alias) + "\" " );
            sw.Write( "attributeId=\"" + att.attributeId + "\" " );
            // sw.Write( "position=\"" + att.pos + "\" " );

            if (att.stereoType != null) {
				sw.Write( "stereotype=\"" + escapeXML(att.stereoType) + "\" " );
			}
			sw.Write( "length=\"" + att.length + "\" " );
			sw.Write( "allowDuplicates=\"" + att.allowDuplicates + "\" " );
			sw.Write( "classifierID=\"" + escapeXML(att.classifierID) + "\" " );
			sw.Write( "container=\"" + att.container + "\" " );
			sw.Write( "containment=\"" + att.containment + "\" " );
			sw.Write( "isDerived=\"" + att.isDerived + "\" " );
			sw.Write( "isID=\"" + att.isID + "\" " );
			sw.Write( "lowerBound=\"" + att.lowerBound + "\" " );
			sw.Write( "upperBound=\"" + att.upperBound + "\" " );
			sw.Write( "precision=\"" + att.precision + "\" " );
			sw.Write( "scale=\"" + att.scale + "\"" );
			sw.WriteLine( ">" );

            sw.WriteLine(indent(depth) + "  <visibility>" + att.visibility + "</visibility>");

            if (att.notes != null)
            {
                sw.WriteLine(indent(depth) + "  <notes>" + escapeXML(att.notes) + "</notes>");
            }
            if (att.defaultValue != null)
            {
                sw.WriteLine(indent(depth) + "  <default>" + escapeXML(att.defaultValue) + "</default>");
            }

            // 属性のタグ付き値の出力
            outputAttributeTags( att, depth+1, sw );

            // 属性の変更フラグが 'U' かつ 変更元・変更先属性がそれぞれセットされている場合
            if (att.changed == 'U' && att.srcAttribute != null && att.destAttribute != null)
            {
                // srcAttributeタグを出力
                sw.WriteLine(indent(depth + 1) + "<srcAttribute>");
                outputAttribute(att.srcAttribute, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</srcAttribute>");

                // destAttributeタグを出力
                sw.WriteLine(indent(depth + 1) + "<destAttribute>");
                outputAttribute(att.destAttribute, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</destAttribute>");
            }

            sw.WriteLine( indent(depth) + "</attribute>" );
		}


        /// <summary>
        /// 属性のタグ付き値を１件出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="att">属性VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputAttributeTags( AttributeVO attr, int depth, StreamWriter sw ) {
	
			if (attr.taggedValues == null || attr.taggedValues.Count <= 0 ) {
				return ;
			}
	
			sw.WriteLine( indent(depth) + "<taggedValues>" );
	
			// 　取得できたタグ付き値の情報をファイルに展開する
			foreach( TaggedValueVO tagv in attr.taggedValues ) {
				if ("<memo>".Equals(tagv.tagValue)) {
					if (tagv.notes != null) {
						sw.WriteLine( indent(depth) + "  <tv guid=\"" + escapeXML(tagv.guid) + "\" name=\"" + escapeXML(tagv.name) + "\" value=\"" + escapeXML(tagv.tagValue) + "\">" + escapeXML(tagv.notes) + "</tv>" );
					}
				} else {
					sw.WriteLine( indent(depth) + "  <tv guid=\"" + escapeXML(tagv.guid) + "\" name=\"" + escapeXML(tagv.name) + "\" value=\"" + escapeXML(tagv.tagValue) + "\"/>" );
				}
			}
	
			sw.WriteLine( indent(depth) + "</taggedValues>" );
		}
#endregion

        #region メソッドの出力処理
        /// <summary>
        /// クラスの操作を出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="currentElement">要素VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputClassMethods(ElementVO currentElement, int depth, StreamWriter sw) {
	
			if(currentElement.methods.Count <= 0 ) {
				return ;
			}
	
			//　取得できた操作の情報をファイルに展開する
			foreach( MethodVO mth in currentElement.methods ) {
				outputMethod(mth, depth, sw);
			}
		}


        /// <summary>
        /// 操作を１件出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="mth">操作VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputMethod(MethodVO mth, int depth, StreamWriter sw)
        {
			sw.Write( indent(depth) + "<method ");

			if(mth.changed != ' ') {
				sw.Write("changed=\"" + mth.changed + "\" ");
			}
			sw.Write("guid=\"" + escapeXML(mth.guid) + "\" ");
			sw.Write("pos=\"" + mth.pos + "\" ");
			sw.Write("name=\"" + escapeXML(mth.name) + "\" ");
			sw.Write("alias=\"" + escapeXML(mth.alias) + "\" ");
            sw.Write("methodId=\"" + mth.methodId + "\" ");
            if (mth.stereoType != null) {
				sw.Write( "stereotype=\"" + escapeXML(mth.stereoType) + "\" ");
			}
			if (mth.classifierID != null) {
				sw.Write( "classifierID=\"" + mth.classifierID + "\" ");
			}
			sw.Write("isConst=\"" + mth.isConst + "\" ");
			sw.Write("isLeaf=\"" + mth.isLeaf + "\" ");
			sw.Write("isPure=\"" + mth.isPure + "\" ");
			sw.Write("isQuery=\"" + mth.isQuery + "\" ");
			sw.Write("isRoot=\"" + mth.isRoot + "\" ");
			sw.Write("returnIsArray=\"" + mth.returnIsArray + "\" ");
			sw.Write("stateFlags=\"" + mth.stateFlags + "\"");
	
			if (mth.throws != null) {
				sw.Write(" throws=\"" + mth.throws + "\"");
			}
			sw.WriteLine( ">");
	
			sw.WriteLine( indent(depth) + "  <visibility>" + mth.visibility + "</visibility>");
			sw.WriteLine( indent(depth) + "  <returnType>" + escapeXML(mth.returnType) + "</returnType>");

            // 1.2.3.1.1 メソッドのタグ付き値出力
            outputMethodTags(mth, depth, sw);

            // 1.2.3.1.2 メソッドパラメータの出力
            outputMethodParams(mth, depth, sw);
	
			if (mth.notes != null) {
				sw.WriteLine(indent(depth) + "  <notes>" + escapeXML( mth.notes ) + "</notes>");
			}
	
			if (mth.behavior != null) {
				sw.WriteLine(indent(depth) + "  <behavior>" + escapeXML( mth.behavior ) + "</behavior>");
			}

            if (mth.code != null)
            {
                sw.WriteLine(indent(depth) + "  <code>" + escapeXML(mth.code) + "</code>");
            }

            // 操作の変更フラグが 'U' かつ 変更元・変更先操作がそれぞれセットされている場合
            if (mth.changed == 'U' && mth.srcMethod != null && mth.destMethod != null)
            {
                // srcMethodタグを出力
                sw.WriteLine(indent(depth + 1) + "<srcMethod>");
                outputMethod(mth.srcMethod, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</srcMethod>");

                // destMethodタグを出力
                sw.WriteLine(indent(depth + 1) + "<destMethod>");
                outputMethod(mth.destMethod, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</destMethod>");
            }

            sw.WriteLine(indent(depth) + "</method>");
		}


        /// <summary>
        /// 操作のタグ付き値を出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="mth">操作VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputMethodTags(MethodVO mth, int depth, StreamWriter sw)
        {

            if (mth.taggedValues == null || mth.taggedValues.Count <= 0)
            {
                return;
            }

            sw.WriteLine(indent(depth) + "<taggedValues>");

            // 　取得できたタグ付き値の情報をファイルに展開する
            foreach (TaggedValueVO tagv in mth.taggedValues)
            {
                if ("<memo>".Equals(tagv.tagValue))
                {
                    if (tagv.notes != null)
                    {
                        sw.WriteLine(indent(depth) + "  <tv guid=\"" + escapeXML(tagv.guid) + "\" name=\"" + escapeXML(tagv.name) + "\" value=\"" + escapeXML(tagv.tagValue) + "\">" + escapeXML(tagv.notes) + "</tv>");
                    }
                }
                else
                {
                    sw.WriteLine(indent(depth) + "  <tv guid=\"" + escapeXML(tagv.guid) + "\" name=\"" + escapeXML(tagv.name) + "\" value=\"" + escapeXML(tagv.tagValue) + "\"/>");
                }
            }

            sw.WriteLine(indent(depth) + "</taggedValues>");
        }


        /// <summary>
        /// 操作のパラメータを出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="mth">操作VO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputMethodParams(MethodVO mth, int depth, StreamWriter sw) {
			if (mth.parameters == null || mth.parameters.Count <= 0) {
				return ;
			}

            sw.WriteLine(indent(depth) + "<parameters>");

            // メソッドのパラメータ
            foreach ( ParameterVO prm in mth.parameters ) {
				sw.Write(indent(depth+1) + "<parameter ");
				sw.Write("guid=\"" + escapeXML(prm.guid) + "\" ");
				sw.Write("name=\"" + escapeXML(prm.name) + "\" ");
				sw.Write("type=\"" + escapeXML(prm.eaType) + "\" ");
				sw.Write("alias=\"" + escapeXML(prm.alias) + "\" ");
				sw.Write("stereotype=\"" + escapeXML(prm.stereoType) + "\" ");
				sw.Write("pos=\"" + prm.pos + "\" ");
				sw.Write("classifier=\"" + prm.classifierID + "\" ");
				sw.Write("default=\"" + escapeXML(prm.defaultValue) + "\" ");
				sw.Write("const=\"" + prm.isConst + "\" ");
				sw.Write("kind=\"" + prm.kind + "\" ");
				sw.Write("objectType=\"" + prm.objectType + "\"");
				sw.WriteLine( ">" );

				if (prm.notes != null) {
					sw.WriteLine(indent(depth+2) + "<notes>" + escapeXML( prm.notes ) + "</notes>");
				}

                // パラメータのタグ付き値出力
                if (prm.paramTags != null)
                {
                    outputParameterTags(prm, depth + 2, sw);
                }

				sw.WriteLine(indent(depth+1) + "</parameter>" );
			}

            sw.WriteLine(indent(depth) + "</parameters>");

        }


        /// <summary>
        /// パラメータのタグ付き値を出力。
        /// パラメータのStreamに向けてXML文書を出力
        /// </summary>
        /// <param name="prm">パラメータVO</param>
        /// <param name="depth">現在のインデントの深さ</param>
        /// <param name="sw">出力用に開かれたStreamWriter</param>
        private static void outputParameterTags(ParameterVO prm, int depth, StreamWriter sw)
        {

            if (prm.paramTags == null || prm.paramTags.Count <= 0)
            {
                return;
            }

            sw.WriteLine(indent(depth) + "<paramTags>");

            // 　取得できたタグ付き値の情報をファイルに展開する
            foreach (ParamTagVO ptagv in prm.paramTags)
            {
                if (ptagv.notes != null)
                {
                    sw.WriteLine(indent(depth) + "  <ptg guid=\"" + escapeXML(ptagv.guid) + "\" name=\"" + escapeXML(ptagv.name) + "\" >" + escapeXML(ptagv.notes) + "</ptg>");
                }
            }

            sw.WriteLine(indent(depth) + "</paramTags>");
        }
        #endregion


        private static string indent(int depth) {
			string retStr = "";
			Int32  i;
		
			for(i=0; i < depth; i++) {
				retStr = retStr + "  ";
			}
			return retStr;
		}
		

		private static string escapeXML( string orig ) {
			if (orig == null) {
				return "";
			}

            // Notes項目など、EAに格納された文字列が既にXMLエスケープされている場合があるため、
            // 既に "<", ">" が "&lt;" "&gt;" になっていればそれ以上の変換はしない。
            if( orig.IndexOf("&lt;") >= 0 && orig.IndexOf("<") < 0)
            {
                return orig;
            }

            if (orig.IndexOf("&gt;") >= 0 && orig.IndexOf(">") < 0)
            {
                return orig;
            }


            System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
			sb.Replace("&", "&amp;");
			sb.Replace("<", "&lt;");
			sb.Replace(">", "&gt;");
			sb.Replace("\"", "&quot;");
			sb.Replace("'", "&apos;");
			return sb.ToString();
		}
		
	}
}
