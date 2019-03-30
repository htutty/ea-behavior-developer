using System;
using System.Data.OleDb;
using System.Collections.Generic;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.util;
using IndexAccessor;

namespace ArtifactFileExporter
{
    class ArtifactExporter
    {
        string outputDir;
        string artifactDir;
        ArtifactsVO allArtifacts;
        List<ConnectorVO> allconnectors;

        public ArtifactExporter(string outputDir, string artifactDir)
        {
            this.outputDir = outputDir;
            this.artifactDir = artifactDir;
        }


        public void readDataBase(string eapfile, string projectName)
        {

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand comm = new OleDbCommand();

            try
            {

                // MDB名など
                //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + eapfile; (Access Connector Environment 2016)
                //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.15.0;Data Source=" + eapfile; (Access Connector Environment 2013)
                //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + eapfile; (Access Connector Environment 2010)
                conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + eapfile; // Jet OleDB 4.0(32bit)

                // 接続する
                conn.Open();

                // DBからの成果物情報読み込みクラスを起動
                ArtifactDbReader atfReader = new ArtifactDbReader(conn);
                this.allArtifacts =  atfReader.getAllArtifacts(projectName);


                // 全接続情報リストXMLファイルを出力
                AllConnectorsExporter expConn = new AllConnectorsExporter(conn, atfReader.AllPackageMap);
                expConn.outputAllConnectors(outputDir);
                this.allconnectors = expConn.allConnectorList;

                // 全相互参照(xref)リストのCSVファイルを出力する
                AllXrefsCsvExporter xrefsExp = new AllXrefsCsvExporter(conn);
                xrefsExp.outputAllCrossRefs(outputDir);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("stack trace: ");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                // 接続を解除します。
                conn.Close();
            }

        }

        /// <summary>
        /// 各種ファイル出力処理
        /// </summary>
        public void doExport()
        {
            // 成果物ファイル達を出力
            writeArtifactFiles();

            // Projectファイルを出力
            ProjectFileWriter.outputProjectFile(outputDir, allArtifacts.targetProject);

            // 要素一覧CSVファイル、属性・操作一覧CSVファイルの出力
            AllElementsCsvWriter csvWriter = new AllElementsCsvWriter(outputDir);
            csvWriter.outputElementsCsv(this.allArtifacts);
            csvWriter.outputMthAttrCsv(this.allArtifacts);

            // ふるまいファイルの出力
            AllBehaviorsWriter bhvWriter = new AllBehaviorsWriter(outputDir);
            bhvWriter.outputBehaviorsText(this.allArtifacts);
        }

        /// <summary>
        /// 成果物ファイルの出力
        /// </summary>
        private void writeArtifactFiles()
        {
            // 成果物リストの中身を artifact.xml ファイルに出力
            foreach (var atf in this.allArtifacts.artifactList)
            {
                ArtifactXmlWriter.outputArtifactXml(artifactDir, atf);
            }

            // AllArtifacts.xml ファイルを出力
            AllArtifactsFileWriter.outputAllArtifactsFile(artifactDir, this.allArtifacts);

        }


        /// <summary>
        /// Index用データベース(SQLite)に、接続と要素（属性・操作）の情報を登録する
        /// </summary>
        public void doMakeIndex()
        {

            try
            {
                ProjectSetting.load(this.outputDir + "\\project.bdprj");
                IndexDbWriter dbWriter = new IndexDbWriter(this.outputDir, ProjectSetting.getVO().dbName);
                dbWriter.writeAllConnector(this.allconnectors);
                dbWriter.writeAllElements(this.allArtifacts);
                dbWriter.writeAllBehaviors(this.allArtifacts);
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

                    // ElementAsciidocWriter.doWrite(outputDir, elem);
                }
            }
        }



    }
}
