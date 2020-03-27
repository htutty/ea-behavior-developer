using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.writer;
using IndexAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciidocGenerator
{
    class IndexMaker
    {
        private string outputDir;

        private ArtifactsVO allArtifacts;
        private List<ConnectorVO> allConnectors;

        public IndexMaker(ArtifactsVO artifacts, string projectDir)
        {
            ConnectorXmlReader connReader = new ConnectorXmlReader(projectDir);

            this.allArtifacts = artifacts;
            this.allConnectors = connReader.readConnectorAll();
        }

        /// <summary>
        /// Index用データベース(SQLite)に、接続と要素（属性・操作）の情報を登録する
        /// </summary>
        public void doMakeIndex()
        {

            try
            {
                IndexDbWriter dbWriter = new IndexDbWriter(ProjectSetting.getVO().projectPath, ProjectSetting.getVO().dbName);
                dbWriter.writeAllConnector(this.allConnectors);
                dbWriter.writeAllElements(this.allArtifacts);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 要素毎のXMLを elements 配下に出力する
        /// </summary>
        public void doMakeElementFiles()
        {
            // 
            foreach (ArtifactVO atf in this.allArtifacts.getArtifactsExcludeImplModel())
            {
                foreach (ElementVO elem in atf.getOwnElements())
                {
                    ElementXmlWriter.outputElementXmlFile(elem);
                    // outputCSharpCode(elem);
                    // ElementAsciidocWriter.doWrite(outputDir, elem);
                }

            }
        }


    }
}
