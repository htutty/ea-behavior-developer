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

        Dictionary<int, ElementVO> outputElementMap;
        Dictionary<int, AttributeVO> outputAttributeMap;
        Dictionary<int, MethodVO> outputMethodMap;

        BehaviorParser bhvParser;

        SQLiteConnection conn;
		SQLiteTransaction transaction = null;
		Int32 elementRecCount = 0;
        Int32 behaviorRecCount = 0;

        /// <summary>
        ///　コンストラクタ
        /// </summary>
        /// <param name="projectDir">.bdprjファイルの配置ディレクトリ</param>
        /// <param name="dbfile">.dbファイル名</param>
	    public IndexDbWriter(string projectDir, string dbfile)
		{
            try {
                this.db_file = dbfile;
                this.projectDir = projectDir;
                this.artifactDir = projectDir + "\\" + ProjectSetting.getVO().artifactsPath;

                outputElementMap = new Dictionary<int, ElementVO>();
                outputAttributeMap = new Dictionary<int, AttributeVO>();
                outputMethodMap = new Dictionary<int, MethodVO>();

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

        #region キャッシュDBの書き込み：t_connector
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
        #endregion

        #region キャッシュDBの書き込み：t_element, t_attr_meth

        /// <summary>
        /// 成果物リスト内に存在する全ての要素をIndexDBに登録する
        /// </summary>
        /// <param name="artifacts">成果物VOのリスト</param>
        public void writeAllElements(List<ArtifactVO> artifacts)
        {
            ArtifactsVO allArtifacts = new ArtifactsVO();
            allArtifacts.artifactList = artifacts;
            writeAllElements(allArtifacts);
        }

        /// <summary>
        /// 成果物リスト内に存在する全ての要素をIndexDBに登録する
        /// </summary>
        /// <param name="allArtifacts">全成果物VO</param>
        public void writeAllElements(ArtifactsVO allArtifacts)
        {
            Console.Write("inserting tables for element, attribute, method ");

            try
            {
                // DB接続をオープンする
                conn.Open();
                recreateElementTable();
                recreateAttrMthTable();

                // トランザクションの開始
                // （トランザクションのcommitは指定行数がINSERTされる度にループ内で実施）
                transaction = conn.BeginTransaction();

                // 実装モデルを除いてインデックスに登録
                foreach (ArtifactVO atf in allArtifacts.getArtifactsExcludeImplModel())
                {
                    // 要素テーブルの書き込み
                    insertElementsInArtifact(atf);
                    // 属性・操作テーブルの書き込み
                    insertAttrMthInArtifact(atf);
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

        /// <summary>
        /// 引数の成果物フォルダ内の要素を全て要素テーブルに書き込み
        /// </summary>
        /// <param name="atf">成果物フォルダ</param>
		private void insertElementsInArtifact(ArtifactVO atf) {

            foreach ( ElementVO elem in atf.getOwnElements() ) {
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

            // ElementId が既に出力済みMapに存在しているかをチェックし、存在していたらリターン
            if( outputElementMap.ContainsKey(elem.elementId) )
            {
                return;
            }

			string sql = @"insert into t_element
					(objectId, elemGuid, elemName, elemAlias, elemType, elemStereotype,
					 artifactGuid, artifactName, packageId, elementPath
					) values (
					 @objectId, @elemGuid, @elemName, @elemAlias, @elemType, @elemStereotype,
					 @artifactGuid, @artifactName, @packageId, @elementPath
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
                  , new SQLiteParameter("@packageId",elem.packageId)
                  , new SQLiteParameter("@elementPath",elem.elementPath)
				};

				command2.CommandText = sql;
				command2.Parameters.AddRange(parameters);
				command2.ExecuteNonQuery();
	    	}

            outputElementMap.Add(elem.elementId, elem);

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="atf"></param>
        private void insertAttrMthInArtifact(ArtifactVO atf)
        {
            foreach (ElementVO elem in atf.getOwnElements())
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
        /// t_attr_meth テーブルに属性行の追加
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        private void insertAttrTable(ArtifactVO atf, ElementVO elem, AttributeVO attr)
        {
            // AttrbuteId が既に出力済みMapに存在しているかをチェックし、存在していたらINSERTせずリターン
            if (outputAttributeMap.ContainsKey(attr.attributeId))
            {
                return;
            }

            string sql = @"insert into t_attr_mth
					(
                      attrMthId, elemId, elemGuid, attrMthFlg, attrMthType,
                      attrMthGuid, attrMthName, attrMthAlias, attrMthNotes
					) values (
					  @attrMthId, @elemId, @elemGuid, @attrMthFlg, @attrMthType,
                      @attrMthGuid, @attrMthName, @attrMthAlias, @attrMthNotes
					) ";

            using (SQLiteCommand command2 = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@attrMthId",(attr.attributeId)*-1)
                  , new SQLiteParameter("@elemId",elem.elementId)
                  , new SQLiteParameter("@elemGuid",elem.guid)
                  , new SQLiteParameter("@attrMthFlg", "a")
                  , new SQLiteParameter("@attrMthType", attr.eaType)
                  , new SQLiteParameter("@attrMthGuid", attr.guid)
                  , new SQLiteParameter("@attrMthName", attr.name)
                  , new SQLiteParameter("@attrMthAlias",attr.alias)
                  , new SQLiteParameter("@attrMthNotes",attr.notes)
                };

                command2.CommandText = sql;
                command2.Parameters.AddRange(parameters);
                command2.ExecuteNonQuery();
            }

            outputAttributeMap.Add(attr.attributeId, attr);
        }

        /// <summary>
        /// 要素情報のインデックステーブルの行追加
        /// </summary>
        /// <param name="atf"></param>
        /// <param name="elem"></param>
        private void insertMthTable(ArtifactVO atf, ElementVO elem, MethodVO mth)
        {
            // MethodId が既に出力済みMapに存在しているかをチェックし、存在していたらINSERTせずリターン
            if (outputMethodMap.ContainsKey(mth.methodId))
            {
                return;
            }

            string sql = @"insert into t_attr_mth
					(
                      attrMthId, elemId, elemGuid, attrMthFlg, attrMthType,
                      attrMthGuid, attrMthName, attrMthAlias, attrMthNotes, mthParamDesc
					) values (
					  @attrMthId, @elemId, @elemGuid, @attrMthFlg, @attrMthType,
                      @attrMthGuid, @attrMthName, @attrMthAlias, @attrMthNotes, @mthParamDesc
					) ";

            using (SQLiteCommand command2 = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@attrMthId",(mth.methodId))
                  , new SQLiteParameter("@elemId",elem.elementId)
                  , new SQLiteParameter("@elemGuid",elem.guid)
                  , new SQLiteParameter("@attrMthFlg", "m")
                  , new SQLiteParameter("@attrMthType", mth.returnType)
                  , new SQLiteParameter("@attrMthGuid", mth.guid)
                  , new SQLiteParameter("@attrMthName", mth.name)
                  , new SQLiteParameter("@attrMthAlias",mth.alias)
                  , new SQLiteParameter("@attrMthNotes",mth.notes)
                  , new SQLiteParameter("@mthParamDesc",mth.getParamDesc())
                };

                command2.CommandText = sql;
                command2.Parameters.AddRange(parameters);
                command2.ExecuteNonQuery();
            }

            outputMethodMap.Add(mth.methodId, mth);
        }
        #endregion

        #region キャッシュDBの書き込み：t_parsed_behavior
        public void writeAllBehaviors(ArtifactsVO allArtifacts)
        {
            Console.Write("inserting table for behaviors ");

            try
            {
                conn.Open();
                recreateBehaviorTable();

                behaviorRecCount = 0;
                bhvParser = new BehaviorParser();

                // トランザクションの開始
                // （トランザクションのcommitは指定行数がINSERTされる度にループ内で実施）
                transaction = conn.BeginTransaction();

                // 実装モデルを除いてインデックスに登録
                foreach (ArtifactVO atf in allArtifacts.getArtifactsExcludeImplModel())
                {
                    // ふるまいテーブルの書き込み（成果物フォルダ配下のクラス要素）
                    insertBehaviorsInArtifact(atf);
                }

                transaction.Commit();
                Console.WriteLine(".  done(" + behaviorRecCount + "records)");

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        /// <summary>
        /// 引数の成果物フォルダ内の要素のふるまいをインサートする
        /// </summary>
        /// <param name="atf"></param>
        private void insertBehaviorsInArtifact(ArtifactVO atf)
        {
            foreach (ElementVO elem in atf.getOwnElements())
            {
                if (elem.eaType == "Class")
                {
                    foreach (MethodVO mth in elem.methods)
                    {
                        // メソッド単位でのふるまいのインサート
                        insertBehaviorsInMethod(elem, mth);

                    }
                }

            }
        }

        private void insertBehaviorsInMethod(ElementVO elem, MethodVO mth)
        {
            // ふるまいを行ごとに切り分け、チャンク単位で登録
            List<BehaviorChunk> chunks = bhvParser.parseBehavior(mth);

            foreach (BehaviorChunk chk in chunks)
            {
                insertBehaviorTable(mth, chk);
                behaviorRecCount++;

                if (behaviorRecCount > 0 && (behaviorRecCount + 1) % 1000 == 0)
                {
                    Console.Write(".");
                    transaction.Commit();
                    transaction = conn.BeginTransaction();
                }
            }
        }


        /// <summary>
        /// ふるまい情報のインデックステーブルの行追加
        /// </summary>
        private void insertBehaviorTable(MethodVO mth, BehaviorChunk chk)
        {
            string sql = @"insert into t_parsed_behavior
			               (
                             chunkId, methodId, pos, parentId, previousId,
                             indLv, dottedNum, indent, behavior
				           ) values (
                             @chunkId, @methodId, @pos, @parentId, @previousId,
                             @indLv, @dottedNum, @indent, @behavior
			               ) ";

            using (SQLiteCommand command2 = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@chunkId",(chk.chunkId))
                  , new SQLiteParameter("@methodId",chk.methodId)
                  , new SQLiteParameter("@pos",chk.pos)
                  , new SQLiteParameter("@parentId", chk.parentChunkId)
                  , new SQLiteParameter("@previousId", chk.previousChunkId)
                  , new SQLiteParameter("@indLv", chk.indLv)
                  , new SQLiteParameter("@dottedNum", chk.dottedNum)
                  , new SQLiteParameter("@indent",chk.indent)
                  , new SQLiteParameter("@behavior",chk.behavior)
                };

                command2.CommandText = sql;
                command2.Parameters.AddRange(parameters);
                command2.ExecuteNonQuery();
            }

        }
        #endregion

        #region t_elementテーブルの存在チェック、再作成の処理

        /// <summary>
        /// t_elementテーブルの再作成(存在チェック＋DROP、CREATE)
        /// </summary>
        private void recreateElementTable()
        {
            string tableName = "t_element";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createElementTable();
        }

        /// <summary>
        /// t_elementテーブルのCREATE
        /// </summary>
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
                     packageId INTEGER,
					 elementPath TEXT
					)";
                command.ExecuteNonQuery();
            }

        }

        #endregion

        #region t_connectorテーブルの存在チェック、再作成の処理

        /// <summary>
        /// t_connectorテーブルの再作成(存在チェック＋DROP、CREATE)
        /// </summary>
        private void recreateConnectorTable()
        {
            string tableName = "t_connector";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createConnectorTable();
        }

        /// <summary>
        /// t_connectorテーブルのCREATE
        /// </summary>
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

        #region t_attr_mthテーブルの存在チェック、再作成の処理
        /// <summary>
        /// t_attr_mthテーブルの存在チェック、再作成の処理
        /// </summary>
        private void recreateAttrMthTable()
        {
            string tableName = "t_attr_mth";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createAttrMthTable();
        }

        /// <summary>
        /// t_attr_mthテーブルのCREATE
        /// </summary>
        private void createAttrMthTable()
        {

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText =
                    @"create table t_attr_mth (
                      attrMthId INTEGER PRIMARY KEY,
                      elemId INTEGER ,
                      elemGuid TEXT ,
                      attrMthFlg TEXT ,
                      attrMthType TEXT ,
                      attrMthGuid TEXT ,
                      attrMthName TEXT ,
                      attrMthAlias TEXT ,
                      attrMthNotes TEXT ,
                      mthParamDesc TEXT
					)";
                command.ExecuteNonQuery();
            }

        }
        #endregion

        #region t_parsed_behavior テーブルの存在チェック、再作成の処理
        /// <summary>
        /// t_parsed_behavior テーブルの存在チェック、再作成の処理
        /// </summary>
        private void recreateBehaviorTable()
        {
            string tableName = "t_parsed_behavior";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createBehaviorTable();
        }

        /// <summary>
        /// t_parsed_behavior テーブルのCREATE
        /// </summary>
        private void createBehaviorTable()
        {

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText =
                    @"create table t_parsed_behavior (
                      chunkId INTEGER PRIMARY KEY,
                      methodId INTEGER,
                      pos INTEGER,
                      parentId INTEGER,
                      previousId INTEGER,
                      indLv INTEGER,
                      hasFollower BOOL,
                      followeeIdx INTEGER,
                      dottedNum TEXT,
                      indent TEXT,
                      behavior TEXT
				    )";
                command.ExecuteNonQuery();
            }

        }
        #endregion

        #region t_change_log テーブルの存在チェック、再作成の処理
        /// <summary>
        /// t_parsed_behavior テーブルの存在チェック、再作成の処理
        /// </summary>
        private void recreateChangeLogTable()
        {
            string tableName = "t_change_log";

            if (existTargetTable(tableName))
            {
                dropTargetTable(tableName);
            }

            createChangeLogTable();
        }


        /// <summary>
        /// t_change_log テーブルのCREATE
        /// </summary>
        private void createChangeLogTable()
        {

            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText =
                    @"create table t_change_log (
                      ChangeLogId INTEGER PRIMARY KEY AUTOINCREMENT,
                      SnapshotID TEXT,
                      SeriesID TEXT,
                      Notes TEXT,
                      ElementGuid TEXT,
                      ChangeUser TEXT,
                      ChangeDateTime TEXT,
                      ChangeItemType TEXT,
                      Metadata TEXT,
                      LogItem TEXT
				    )";
                command.ExecuteNonQuery();
            }

        }
        #endregion

        #region 各テーブルの存在チェックとDROP共通処理

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

	}
}
