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
		
		private static void checkAndMakeElementDir(string edir) {
			if (!Directory.Exists(edir)) {
				Directory.CreateDirectory(edir);
			}
		}
		
		public static void writeElementXml(ElementVO elemvo, int depth, StreamWriter sw) {
			sw.Write(indent(depth) + "<element ");
			if( elemvo.changed != ' ' ) {
				sw.Write("changed=\"" + elemvo.changed + "\" ");
			}
			sw.Write("guid=\"" + escapeXML(elemvo.guid) + "\" ");
			sw.Write("tpos=\"" + elemvo.treePos + "\" ");
			sw.Write("type=\"" + elemvo.eaType + "\" ");
			sw.Write("name=\"" + escapeXML(elemvo.name) + "\" ");
			sw.Write("alias=\"" + escapeXML(elemvo.alias) + "\" ");
            sw.Write("elementId=\"" + elemvo.elementId + "\" ");
            sw.Write("parentId=\"" + elemvo.parentID + "\"");

            if ( elemvo.stereoType != null ) {
				sw.Write( " stereotype=\"" + elemvo.stereoType + "\"" );
			}
			sw.WriteLine(">");

			ElementReferenceVO elemref = elemvo.elementReferenceVO;
			if( elemref != null ) {
				sw.Write( indent(depth+1) +"<ref " );
				sw.Write( "gentype=\"" + escapeXML(elemref.gentype) + "\" " );
				sw.Write( "fqcn=\"" + escapeXML(elemref.fqcn) + "\" " );
				sw.Write( "genfile=\"" + escapeXML(elemref.genfile) + "\"" );
				sw.WriteLine( "/>" );
			}
	
			if (elemvo.notes != null) {
				sw.WriteLine(indent(depth+1) +"<notes>" + escapeXML(elemvo.notes) + "</notes>" );
			}

			// 1.2.1 クラスのタグ付き値出力
			if (elemvo.taggedValues.Count > 0) {
				outputClassTags(elemvo, depth+1, sw);
			}
	
			// 1.2.2 クラスの属性出力
			if (elemvo.attributes.Count > 0) {
				outputClassAttributes(elemvo, depth+1, sw);
			}
	
			// 1.2.3 クラスの操作（メソッド）出力
			if (elemvo.methods.Count > 0) {
				outputClassMethods(elemvo, depth+1, sw);
			}
	
			// elementの閉じタグ
			if( depth == 0 ) {
				sw.Write("</element>");
			} else {
				sw.WriteLine(indent(depth) + "</element>");
			}
		}
	

		// 1.2.1 クラスのタグ付き値を出力
		private static void outputClassTags( ElementVO currentElement, int depth, StreamWriter sw ) {
	
			if ( currentElement.taggedValues.Count <= 0 ) {
				return ;
			}
	
			sw.WriteLine( indent(depth) + "<taggedValues>" );
	
			// 　取得できたタグ付き値の情報をファイルに展開する
			foreach( TaggedValueVO tagv in currentElement.taggedValues ) {
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


		// 1.2.2 クラスの属性を出力
		private static void outputClassAttributes( ElementVO currentElement, int depth, StreamWriter sw ) {
	
			if ( currentElement.attributes.Count <= 0 ) {
				return;
			}
	
			//　取得できた属性の情報をファイルに展開する
			foreach(AttributeVO m_Att in currentElement.attributes) {
				outputAttribute(m_Att, depth, sw);
			}
	
		}

		
		
		
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
				sw.Write( "stereotype=\"" + att.stereoType + "\" " );
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
				
			sw.WriteLine( indent(depth) + "</attribute>" );
		}
		
		
		// 1.2.1 属性のタグ付き値を出力
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
		
		

		// 1.2.3 クラスの操作を出力
		private static void outputClassMethods(ElementVO currentElement, int depth, StreamWriter sw) {
	
			if(currentElement.methods.Count <= 0 ) {
				return ;
			}
	
			//　取得できた操作の情報をファイルに展開する
			foreach( MethodVO mth in currentElement.methods ) {
				outputMethod(mth, depth, sw);
			}
		}

		
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
				sw.Write( "stereotype=\"" + mth.stereoType + "\" ");
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

			// メソッドのタグ付き値の出力
			outputMethodTags(mth, depth+1, sw);
				
			// 1.2.3.1.1 メソッドパラメータの出力
			outputMethodParams(mth, depth+1, sw);
	
			if (mth.notes != null) {
				sw.WriteLine(indent(depth) + "  <notes>" + escapeXML( mth.notes ) + "</notes>");
			}
	
			if (mth.behavior != null) {
				sw.WriteLine(indent(depth) + "  <behavior>" + escapeXML( mth.behavior ) + "</behavior>");
			}
	
			// 1.2.3.1.2 メソッドのタグ付き値出力
			outputMethodTags( mth, depth, sw);
	
			sw.WriteLine(indent(depth) + "</method>");
		}


		
		//  1.2.3.1.1 メソッドのパラメータ出力
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
                if (prm.taggedValues != null)
                {
                    outputParameterTags(prm, depth, sw);
                }

				sw.WriteLine(indent(depth+1) + "</parameter>" );
			}

            sw.WriteLine(indent(depth) + "</parameters>");

        }


        // メソッドのタグ付き値を出力
        private static void outputMethodTags( MethodVO mth, int depth, StreamWriter sw ) {
	
			if ( mth.taggedValues == null ||  mth.taggedValues.Count <= 0 ) {
				return ;
			}
	
			sw.WriteLine( indent(depth) + "<taggedValues>" );
	
			// 　取得できたタグ付き値の情報をファイルに展開する
			foreach( TaggedValueVO tagv in mth.taggedValues ) {
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


        // パラメータのタグ付き値を出力
        private static void outputParameterTags(ParameterVO prm, int depth, StreamWriter sw)
        {

            if (prm.taggedValues == null || prm.taggedValues.Count <= 0)
            {
                return;
            }

            sw.WriteLine(indent(depth) + "<taggedValues>");

            // 　取得できたタグ付き値の情報をファイルに展開する
            foreach (TaggedValueVO tagv in prm.taggedValues)
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
