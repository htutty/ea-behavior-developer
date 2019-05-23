using System;
using System.Data.OleDb;
using System.Collections.Generic;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.writer;
using ArtifactFileAccessor.util;
using IndexAccessor;
using System.IO;
using System.Text;

namespace ArtifactFileExporter
{
    class ArtifactExporter
    {
        string outputDir;
        string artifactDir;
        ArtifactsVO allArtifacts;
        List<ConnectorVO> allconnectors;

        private BehaviorParser behaviorParser = new BehaviorParser();

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

                // パッケージツリーXMLファイルを出力する
                AllPackagesExporter packagesExporter = new AllPackagesExporter(outputDir);
                packagesExporter.outputPackageXml(atfReader.rootPackages);

                // 全接続情報リストXMLファイルを出力する
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
                // 成果物の保有クラスごとにふるまい→コードへの変換を実施
                setParsedCodeInArtifact(atf);
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
                    outputCSharpCode(elem);
                    // ElementAsciidocWriter.doWrite(outputDir, elem);
                }

            }
        }

        /// <summary>
        /// クラスの内容をソースファイルとして出力する
        /// </summary>
        /// <param name="artifacts">全成果物リスト</param>
        public void outputCSharpCode(ElementVO elem)
        {
            StreamWriter srcsw = null;

            if (elem.eaType != "Class")
            {
                return;
            }

            try
            {
                string outputDir = @"C:\ea-artifacts\in-progress\cuvic_aswea_20190401\logicsrc\" + elem.elementPath.Replace('/', '\\');
                makeSrcDirIfNotExist(outputDir);
                Console.WriteLine("elementPath = " + elem.elementPath);

                srcsw = new StreamWriter(outputDir + @"\" + elem.name + ".cs", false, System.Text.Encoding.GetEncoding("utf-8"));

                // 1行目に列タイトルをつける
                srcsw.WriteLine("namespace " + elem.elementPath.Replace('/', '.'));
                srcsw.WriteLine("{");
                srcsw.WriteLine(elem.generateDeclareString(1));
                srcsw.WriteLine("}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (srcsw != null) srcsw.Close();
            }

        }


        private static void makeSrcDirIfNotExist(string logicSrcDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(logicSrcDir))
            {
                Directory.CreateDirectory(logicSrcDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + logicSrcDir);
            }
        }

        /// <summary>
        /// 要素リストを受け、要素のノートと操作のふるまいをテキスト出力する。
        /// テキスト解析に使用する。
        /// </summary>
        /// <param name="atf">対象成果物VO</param>
        /// <param name="elements">要素の</param>
        /// <param name="sw"></param>
        private void setParsedCodeInArtifact(ArtifactVO atf)
        {
            foreach (ElementVO elem in atf.getOwnElements())
            {
                if (elem.methods != null)
                {
                    foreach (MethodVO mth in elem.methods)
                    {
                        List<BehaviorChunk> chunks = behaviorParser.parseBehavior(mth);
                        StringWriter outsw = new StringWriter();

                        foreach (BehaviorChunk cnk in chunks)
                        {

                            if(cnk.behaviorToken == null || cnk.behaviorToken.token == "[comment]" )
                            {
                                outsw.WriteLine(generateIndentStr(cnk.indLv) + "// " + cnk.dottedNum + "　" + cnk.behavior);
                            }
                            else
                            {
                                outsw.WriteLine(generateIndentStr(cnk.indLv) + cnk.getTokenRepresented());
                            }
                        }

                        mth.code = outsw.ToString();
                        outsw.Close();
                    }
                }
            }
        }



        private static string generateIndentStr(int indentLevel)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append("\t");
            }

            return sb.ToString();
        }

    }
}
