using System;
using System.Xml;

namespace ArtifactFileAccessor.writer
{
    public class ProjectFileWriter
    {

        public static void outputProjectFile(string outputDir, string projectName)
        {
            // DOMオブジェクト生成
            XmlDocument document = new XmlDocument();

            // XML宣言部分生成、追加
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            document.AppendChild(declaration);

            // ルート生成(projectタグ)
            XmlElement projectRoot = document.CreateElement("project");
            XmlElement subNode;

            subNode = document.CreateElement("projectName");
            subNode.InnerText = projectName;
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("exportDate");
            DateTime dt = DateTime.Now;
            subNode.InnerText = dt.ToString("yyyy/MM/dd HH:mm:ss");
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("artifactsPath");
            subNode.InnerText = "artifacts";
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("artifactsFile");
            subNode.InnerText = "AllArtifacts.xml";
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("artifactFile");
            subNode.SetAttribute("prefix", "atf_");
            subNode.SetAttribute("suffix", ".xml");
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("notes");
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("allConnector");
            subNode.InnerText = "AllConnectors.xml";
            projectRoot.AppendChild(subNode);

            subNode = document.CreateElement("dbName");
            subNode.InnerText = projectName + ".db";
            projectRoot.AppendChild(subNode);

            // DOMにrootを追加
            document.AppendChild(projectRoot);

            document.Save(outputDir + @"\project.bdprj");
        }
    }
}
