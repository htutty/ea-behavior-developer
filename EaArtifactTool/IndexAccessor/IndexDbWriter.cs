using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml;

using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

namespace IndexAccessor
{
	/// <summary>
	/// Description of DatabaseWriter.
	/// </summary>
	public class IndexDbWriter
	{
		string db_file ;
		string projectDir = null;
        string artifactDir = null;

		SQLiteConnection conn;
		SQLiteTransaction transaction = null;
		Int32 elementRecCount = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectDir"></param>
        /// <param name="dbfile"></param>
	    public IndexDbWriter(string projectDir, string dbfile)
		{
            try {
                this.db_file = dbfile;
                this.projectDir = projectDir;
                this.artifactDir = projectDir + "\\" + ProjectSetting.getVO().artifactsPath;

                Console.WriteLine("projectdir = " + this.projectDir);
                Console.WriteLine("artifactdir = " + this.artifactDir);
                Console.WriteLine("dbfile = " + this.db_file);

                string datasourceStr = projectDir + "\\" + this.db_file;
                this.conn = new SQLiteConnection("Data Source=" + datasourceStr);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
				throw ex ;
			}

		}

        /// <summary>
        /// 全接続リスト(All_Connectors.xml)からキャッシュDBの接続(t_connector)テーブルに登録する
        /// </summary>
        public void writeAllConnector(List<ConnectorVO> conns) {

			Console.Write("inserting connector table ");

			conn.Open();
			recreateConnectorTable();

			Int32 recCount = 0;
			SQLiteTransaction sqlt = null;

			// ConnectorXmlReader connReader = new ConnectorXmlReader(projectDir);
			// List<ConnectorVO> conns = connReader.readConnectorAll();

			string sql = @"insert into t_connector
					(connId, connGuid, connName, connType,
					 srcObjId, srcObjGuid, srcObjName,
					 destObjId, destObjGuid, destObjName
					) values (
					 @connId, @connGuid, @connName, @connType,
					 @srcObjId, @srcObjGuid, @srcObjName,
					 @destObjId, @destObjGuid, @destObjName
					) ";

		    foreach( ConnectorVO convo in conns ) {
				if ( recCount % 2000 == 0 ) {
					sqlt = conn.BeginTransaction();
				}

				using (SQLiteCommand command2 = conn.CreateCommand()) {
                    SQLiteParameter[] parameters = new SQLiteParameter[]{
                      new SQLiteParameter("@connId",convo.connectorId),
                      new SQLiteParameter("@connGuid",convo.guid),
                      new SQLiteParameter("@connName",convo.name),
                      new SQLiteParameter("@connType",convo.connectorType),
                      new SQLiteParameter("@srcObjId",convo.srcObjId),
                      new SQLiteParameter("@srcObjGuid",convo.srcObjGuid),
                      new SQLiteParameter("@srcObjName",convo.srcObjName),
                      new SQLiteParameter("@destObjId",convo.destObjId),
                      new SQLiteParameter("@destObjGuid",convo.destObjGuid),
                      new SQLiteParameter("@destObjName",convo.destObjName)
                    };

		     		command2.CommandText = sql;
		     		command2.Parameters.AddRange(parameters);
		        	command2.ExecuteNonQuery();
		    	}

		     	if ( (recCount + 1) % 2000 == 0 && recCount > 0) {
		     		Console.Write("." );
	    			sqlt.Commit();
		    	}

	            recCount++;
		    }

		    sqlt.Commit();
		    Console.WriteLine(".  done(" + recCount + "records)" );

		    conn.Close();
		}



		public void writeAllElements(ArtifactsVO allArtifacts)
        {
            Console.Write("inserting tables for element, attribute, method ");

            try
            {
                conn.Open();
                recreateElementTable();
                recreateAttrMthTable();


                transaction = conn.BeginTransaction();

                foreach (ArtifactVO atf in allArtifacts.artifactList)
                {
                    insertElementsInArtifact(atf, atf.getOwnElements());

                    insertAttrMthInArtifact(atf, atf.getOwnElements());
                }

                transaction.Commit();
                Console.WriteLine(".  done(" + elementRecCount + "records)");

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

		private void insertElementsInArtifact(ArtifactVO atf, List<ElementVO> elements) {

            foreach ( ElementVO elem in elements ) {
				insertElementTable(atf, elem);
				// Console.WriteLine("insert element : " + elementRecCount + " records." );

				if ( (elementRecCount + 1) % 1000 == 0 && elementRecCount > 0) {
					Console.Write("." );
					transaction.Commit();
                    transaction = conn.BeginTransaction();
                }

				elementRecCount++;
			}

        }

		/// <summary>
        /// 要素情報のインデックステーブルの行追加
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
		private void insertElementTable(ArtifactVO atf, ElementVO elem) {

			string sql = @"insert into t_element
					(objectId, elemGuid, elemName, elemAlias, elemType, elemStereotype,
					 artifactGuid, artifactName, artifactPath
					) values (
					 @objectId, @elemGuid, @elemName, @elemAlias, @elemType, @elemStereotype,
					 @artifactGuid, @artifactName, @artifactPath
					) ";

	    	using (SQLiteCommand command2 = conn.CreateCommand()) {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@objectId",elem.elementId)
                  , new SQLiteParameter("@elemGuid",elem.guid)
                  , new SQLiteParameter("@elemName",elem.name)
                  , new SQLiteParameter("@elemAlias",elem.alias)
                  , new SQLiteParameter("@elemType",elem.eaType)
                  , new SQLiteParameter("@elemStereotype",elem.stereoType)
                  , new SQLiteParameter("@artifactGuid",atf.guid)
                  , new SQLiteParameter("@artifactName",atf.name)
                 , new SQLiteParameter("@artifactPath",atf.pathName)
				};

				command2.CommandText = sql;
				command2.Parameters.AddRange(parameters);
				command2.ExecuteNonQuery();
	    	}

		}

        /// <summary>
        ///
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elements"></param>
        private void insertAttrMthInArtifact(ArtifactVO atf, List<ElementVO> elements)
        {
            foreach (ElementVO elem in elements)
            {

                foreach(AttributeVO attr in elem.attributes )
                {
                    insertAttrTable(atf, elem, attr);
                    elementRecCount++;

                    if (elementRecCount > 0 && (elementRecCount + 1) % 1000 == 0)
                    {
                        Console.Write(".");
                        transaction.Commit();
                        transaction = conn.BeginTransaction();
                    }
                }


                foreach (MethodVO mth in elem.methods)
                {
                    insertMthTable(atf, elem, mth);
                    elementRecCount++;

                    if (elementRecCount > 0 && (elementRecCount + 1) % 1000 == 0)
                    {
                        Console.Write(".");
                        transaction.Commit();
                        transaction = conn.BeginTransaction();
                    }
                }
            }

        }


        /// <summary>
        /// 要素情報のインデックステーブルの行追加
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        private void insertAttrTable(ArtifactVO atf, ElementVO elem, AttributeVO attr)
        {

            string sql = @"insert into t_attr_mth
					(
                      attrMthId,  artifactGuid,  elemId,  elemGuid, elemName, elemAlias,
                      attrMthType,  attrMthGuid,  attrMthName,  attrMthAlias,  attrMthNotes
					) values (
					  @attrMthId, @artifactGuid, @elemId, @elemGuid, @elemName, @elemAlias,
                      @attrMthType, @attrMthGuid, @attrMthName, @attrMthAlias, @attrMthNotes
					) ";

            using (SQLiteCommand command2 = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@attrMthId",(attr.attributeId)*-1)
                  , new SQLiteParameter("@artifactGuid",atf.guid)
                  , new SQLiteParameter("@elemId",elem.elementId)
                  , new SQLiteParameter("@elemGuid",elem.guid)
                  , new SQLiteParameter("@elemName",elem.name)
                  , new SQLiteParameter("@elemAlias",elem.alias)
                  , new SQLiteParameter("@attrMthType", "a")
                  , new SQLiteParameter("@attrMthGuid", attr.guid)
                  , new SQLiteParameter("@attrMthName", attr.name)
                  , new SQLiteParameter("@attrMthAlias",attr.alias)
                  , new SQLiteParameter("@attrMthNotes",attr.notes)
                };

                command2.CommandText = sql;
                command2.Parameters.AddRange(parameters);
                command2.ExecuteNonQuery();
            }

        }



        /// <summary>
        /// 要素情報のインデックステーブルの行追加
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        private void insertMthTable(ArtifactVO atf, ElementVO elem, MethodVO mth)
        {

            string sql = @"insert into t_attr_mth
					(
                      attrMthId,  artifactGuid,  elemId,  elemGuid, elemName, elemAlias,
                      attrMthType,  attrMthGuid,  attrMthName,  attrMthAlias,  attrMthNotes
					) values (
					  @attrMthId, @artifactGuid, @elemId, @elemGuid, @elemName, @elemAlias,
                      @attrMthType, @attrMthGuid, @attrMthName, @attrMthAlias, @attrMthNotes
					) ";

            using (SQLiteCommand command2 = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@attrMthId",mth.methodId)
                  , new SQLiteParameter("@artifactGuid",atf.guid)
                  , new SQLiteParameter("@elemId",elem.elementId)
                  , new SQLiteParameter("@elemGuid",elem.guid)
                  , new SQLiteParameter("@elemName",elem.name)
                  , new SQLiteParameter("@elemAlias",elem.alias)
                  , new SQLiteParameter("@attrMthType", "m")
                  , new SQLiteParameter("@attrMthGuid", mth.guid)
                  , new SQLiteParameter("@attrMthName", mth.name)
                  , new SQLiteParameter("@attrMthAlias",mth.alias)
                  , new SQLiteParameter("@attrMthNotes",mth.notes)
                };

                command2.CommandText = sql;
                command2.Parameters.AddRange(parameters);
                command2.ExecuteNonQuery();
            }

        }



        #region "elementテーブルの存在チェック、再作成の処理"

        private void recreateElementTable()
        {
            string tableName = "t_element";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createElementTable();
        }

        private void createElementTable()
        {

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText =
                    @"create table t_element
					(objectId INTEGER PRIMARY KEY,
					 elemGuid TEXT,
					 elemName TEXT,
					 elemAlias TEXT,
					 elemType TEXT,
					 elemStereotype TEXT,
					 artifactGuid TEXT,
					 artifactName TEXT,
					 artifactPath TEXT
					)";
                command.ExecuteNonQuery();
            }

        }

        #endregion

        #region "connectorテーブルの存在チェック、再作成の処理"

        private void recreateConnectorTable()
        {
            string tableName = "t_connector";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createConnectorTable();
        }


        private void createConnectorTable()
        {

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText =
                    @"create table t_connector
					(connId INTEGER PRIMARY KEY,
					 connGuid TEXT,
					 connName TEXT,
					 connType TEXT,
					 srcObjId INTEGER,
					 srcObjGuid TEXT,
					 srcObjName TEXT,
					 destObjId INTEGER,
					 destObjGuid TEXT,
					 destObjName TEXT
					)";
                command.ExecuteNonQuery();
            }

        }
        #endregion


        #region "attrMthテーブルの存在チェック、再作成の処理"

        private void recreateAttrMthTable()
        {
            string tableName = "t_attr_mth";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createAttrMthTable();
        }


        private void createAttrMthTable()
        {

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText =
                    @"create table t_attr_mth (
                      attrMthId INTEGER PRIMARY KEY,
                      artifactGuid TEXT ,
                      elemId INTEGER ,
                      elemGuid TEXT ,
                      elemName TEXT ,
                      elemAlias TEXT ,
                      attrMthType TEXT ,
                      attrMthGuid TEXT ,
                      attrMthName TEXT ,
                      attrMthAlias TEXT ,
                      attrMthNotes TEXT
					)";
                command.ExecuteNonQuery();
            }

        }
        #endregion


        #region "各テーブルの存在チェックとDROP共通処理"

        private Boolean existTargetTable(string tableName)
        {
            string sql = "select count(*) from sqlite_master where type='table' and name='" + tableName + "'";

            using (SQLiteCommand command = conn.CreateCommand())
            {
                //クエリの実行
                command.CommandText = sql;
                using (SQLiteDataReader sdr = command.ExecuteReader())
                {
                    //
                    sdr.Read();

                    //カラムの取り込み
                    Int64 rowNum = (Int64)sdr[0];

                    // 一件でも取得できたらtrueを返す
                    return (rowNum >= 1);
                }
            }
        }


        private void dropTargetTable(string tableName)
        {
            string sql = "drop table " + tableName + " ";

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }


        #endregion


        private void writeElementFiles(XmlNodeList elementNodes) {
	    	foreach( XmlNode elem in elementNodes ) {
	    		if( elem.SelectNodes("@guid") !=null ) {
	    			string elemguid = elem.SelectSingleNode("@guid").Value;
	    			writeElementXml(elemguid, elem);
	    		}
	    	}
	    }

	    private void writeElementXml(string guid, XmlNode elemNode) {

	    	string outputDir = ProjectSetting.getVO().projectPath;
    		XmlDocument xmlDocument = new XmlDocument();

    		// XML宣言を設定する
			XmlDeclaration xmlDecl =
				xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

			//作成したXML宣言をDOMドキュメントに追加
			xmlDocument.AppendChild(xmlDecl);

    		// XML宣言部分生成
//			XmlProcessingInstruction head = xmlDocument.CreateProcessingInstruction("xml", "version='1.0'");

    		// 新しいrootノードを、成果物XMLからインポートする形で作成
			XmlNode root = xmlDocument.ImportNode( elemNode, true );

	    	// ルート配下に引数のドキュメントを追加
			xmlDocument.AppendChild(root);

			// 要素XMLを出力するフォルダ (projectDir + /elements/ 配下) が存在するかを調べ、なければ作る
			string edir = outputDir + @"\elements\" + guid.Substring(1,1) + @"\" + guid.Substring(2,1) ;
			checkAndMakeElementDir(edir);

			Console.WriteLine("output element xml : " + edir + @"\" + guid.Substring(1,36) + ".xml" );
			// この内容で要素XMLに記録する
			xmlDocument.Save(edir + @"\" + guid.Substring(1,36) + ".xml");
	    }


		private static void checkAndMakeElementDir(string edir) {
			if (!Directory.Exists(edir)) {
				Directory.CreateDirectory(edir);
			}
		}

	}
}
