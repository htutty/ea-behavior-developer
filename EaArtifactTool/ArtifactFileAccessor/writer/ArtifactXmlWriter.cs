using System;
using System.IO;

using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.writer
{
	/// <summary>
	/// Description of ArtifactXmlWriter.
	/// </summary>
	public class ArtifactXmlWriter
	{
		public ArtifactXmlWriter()
		{
		}
		
		public static void outputArtifactXml(string outputDir, ArtifactVO atf) {
			StreamWriter atfsw = null;
			
			try {
				
				//BOM無しのUTF8でテキストファイルを作成する
				atfsw = new StreamWriter(outputDir + "\\atf_" + atf.guid.Substring(1,36) + ".xml");
				atfsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?> " );
				atfsw.WriteLine( "" );
	
				if( atf.package != null ) {
					atfsw.Write( "<artifact " );
					atfsw.Write( " changed=\"" + atf.changed + "\" " );
					atfsw.Write( " guid=\"" + atf.guid + "\" " );
					atfsw.Write( " name=\"" + escapeXML(atf.name) + "\" " );
					atfsw.Write( " path=\"" + escapeXML(atf.pathName) + "\" " );
					atfsw.Write( " project=\"asw\" " );
					atfsw.Write( " stereotype=\"" + atf.stereoType + "\" " );
					atfsw.WriteLine( ">" );
					
					//sw.WriteLine("■成果物(" + atf.changed + "): GUID="+ atf.guid + ", Name=" + atf.name );
					outputPackage(atf.package, 1, atfsw);
					
					atfsw.WriteLine( "</artifact>" );
				}

			} catch( Exception e ) {
				Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            } finally {
				if( atfsw != null ) atfsw.Close();
			}

		}

		
		private static void outputPackage(PackageVO pkg, int depth, StreamWriter sw) {

			sw.Write(indent(depth) + "<package ");
			sw.Write("guid=\"" + pkg.guid + "\" ");
			sw.Write("name=\"" + escapeXML(pkg.name) + "\" ");
			sw.Write("isControlled=\"" + pkg.isControlled + "\" ");
			if (pkg.stereoType != null) {
				sw.Write("stereoType=\"" + pkg.stereoType + "\" ");
			}
			sw.Write("changed=\"" + pkg.changed + "\" ");
			sw.WriteLine(" >");

            if (pkg.elements != null && pkg.elements.Count > 0)
            {
                foreach (ElementVO elem in pkg.elements)
                {
                    ElementXmlWriter.writeElementXml(elem, depth+1, sw);
                }
            }

            if (pkg.diagrams != null && pkg.diagrams.Count > 0)
            {
                foreach (DiagramVO dia in pkg.diagrams)
                {
                    DiagramXmlWriter.outputDiagramXml(dia, depth+1, sw);
                }
            }

            if (pkg.childPackageList.Count > 0) {
				foreach(PackageVO p in pkg.childPackageList) {
					outputPackage(p, depth+1, sw);
				}
			}

			sw.WriteLine(indent(depth) + "</package>");
		}
		
		
		public void rewriteElementXmlFiles(ArtifactVO atf) {
			rewriteElementXmlPackage(atf.package);
		}
		
		private void rewriteElementXmlPackage(PackageVO pkg ) {
			if (pkg.childPackageList.Count > 0) {
				foreach(PackageVO p in pkg.childPackageList) {
					rewriteElementXmlPackage(p);
				}
			}

			if (pkg.elements.Count > 0 ) {
				foreach( ElementVO elem in pkg.elements ) {
					ElementXmlWriter.outputElementXmlFile(elem);
				}
			}
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
