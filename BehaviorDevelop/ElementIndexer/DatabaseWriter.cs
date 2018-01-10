/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/12/27
 * Time: 9:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using BehaviorDevelop.util;
using BehaviorDevelop.vo;
using System.Configuration;

namespace ElementIndexer
{
	/// <summary>
	/// Description of DatabaseWriter.
	/// </summary>
	public class DatabaseWriter
	{
		const string db_file = "element_idx.db";
		SQLiteConnection conn;
		SQLiteTransaction transaction = null;
		Int32 elementRecCount = 0;

	    public DatabaseWriter()
		{
			try { 
				this.conn = new SQLiteConnection("Data Source="+db_file);
			} catch(Exception e) {
				throw e ;
			}

		}

		
		public void writeAllConnector() {
			
			conn.Open();
			recreateConnectorTable();
			
		    Int32 recCount = 0;
		    SQLiteTransaction sqlt = null;
		    
		    ConnectorXmlReader connReader = new ConnectorXmlReader();
		    List<ConnectorVO> conns = connReader.readConnectorAll();
		    
		   　string sql = @"insert into t_connector 
					(connGuid, connName, connType, 
					 srcObjGuid, srcObjName, 
					 destObjGuid, destObjName 
					) values ( 
					 @connGuid, @connName, @connType, 
					 @srcObjGuid, @srcObjName, 
					 @destObjGuid, @destObjName 
					) " ;
		    
		    foreach( ConnectorVO convo in conns ) {
		    	if ( recCount % 2000 == 0 ) {
		    		sqlt = conn.BeginTransaction();
		    	}
		   　	
		    	using (SQLiteCommand command2 = conn.CreateCommand()) {		
                    SQLiteParameter[] parameters = new SQLiteParameter[]{
                    　new SQLiteParameter("@connGuid",convo.guid),
                    　new SQLiteParameter("@connName",convo.name),
                    　new SQLiteParameter("@connType",convo.connectionType),
                    　new SQLiteParameter("@srcObjGuid",convo.srcObjGuid),
                    　new SQLiteParameter("@srcObjName",convo.srcObjName),
                    　new SQLiteParameter("@destObjGuid",convo.destObjGuid),
                    　new SQLiteParameter("@destObjName",convo.destObjName)
                    };
		    					    
		   　		command2.CommandText = sql;
		   　		command2.Parameters.AddRange(parameters);
		        	command2.ExecuteNonQuery();
		    	}
		    	
		   　	if ( (recCount + 1) % 2000 == 0 && recCount > 0) {
		   　		Console.WriteLine("record count reached " + (recCount + 1) + " records. will commit" );
	    			sqlt.Commit();
		    	}
		   　	
	            recCount++;
		    }
		    
		    sqlt.Commit();
		   　Console.WriteLine( recCount + " records commited." );
		    
		    conn.Close();
		}

		
		private void recreateConnectorTable() {
			if (existConnectorTable()) {
				dropConnectorTable();
			} 
			
			createConnectorTable();
		}
		
		private Boolean existConnectorTable() {

			string sql = "select count(*) from sqlite_master where type='table' and name='t_connector'";
			
			using (SQLiteCommand command =conn.CreateCommand())
		    {
	            //クエリの実行
	            command.CommandText = sql;
	            using (SQLiteDataReader sdr = command.ExecuteReader())
	            {
	            	// 
	            	sdr.Read();

	            	//カラムの取り込み
	            	Int64  rowNum = (Int64)sdr[0];
        			
	                // 一件でも取得できたらtrueを返す
	                return (rowNum >= 1);
	            }
			}
		}


		private void dropConnectorTable() {
			
		    using (SQLiteCommand command =conn.CreateCommand())
		    {
			    command.CommandText = "drop table t_connector";
		        command.ExecuteNonQuery();
		    }
			
		}		

		
		private void createConnectorTable() {
			
		    using (SQLiteCommand command =conn.CreateCommand())
		    {
			    command.CommandText =
					@"create table t_connector
					(connid INTEGER PRIMARY KEY AUTOINCREMENT,
					 connGuid TEXT, 
					 connName TEXT,
					 connType TEXT,
					 srcObjGuid TEXT, 
					 srcObjName TEXT, 
					 destObjGuid TEXT, 
					 destObjName TEXT 
					)";
		        command.ExecuteNonQuery();
		    }
			
		}

		
		public void writeElements() {
			
			conn.Open();
			recreateElementTable();
		    
			ArtifactXmlReader atfReader = new ArtifactXmlReader();
			IList<ArtifactVO> artifacts = ArtifactsXmlReader.readArtifactList(null);

			string target_dir = null;
			target_dir = ConfigurationManager.AppSettings["artifact_dir"];

            elementRecCount = 0;
            
			foreach( ArtifactVO atf in artifacts ) {
				// 成果物パッケージ別のXMLファイル読み込み
				// atfReader.readArtifactDesc(atf, target_dir);
				List<ElementVO> elements = atfReader.readAllElements(atf, null);
				
				// 読み込んだ成果物情報を元に読み込み
				readElementsInArtifact(atf, elements);
			}
		    
            transaction.Commit();
		   　Console.WriteLine( elementRecCount + " records commited." );
		   　
            conn.Close();
		}

	    
		private void readElementsInArtifact(ArtifactVO atf, List<ElementVO> elements) {
			
		    foreach( ElementVO elem in elements ) {
		    	if ( elementRecCount % 2000 == 0 ) {
		    		transaction = conn.BeginTransaction();
		    	}
				
				writeElement(atf, elem);
				// Console.WriteLine("insert element : " + elementRecCount + " records." );

		   　	if ( (elementRecCount + 1) % 2000 == 0 && elementRecCount > 0) {
		   　		Console.WriteLine("record count reached " + (elementRecCount + 1) + " records. will commit" );
	    			transaction.Commit();
		    	}
		   　	
	            elementRecCount++;
			}
		}

		
		private void writeElement(ArtifactVO atf, ElementVO elem) {

		   　string sql = @"insert into t_element
					(elemGuid, elemName, elemAlias, elemType, elemStereotype, 
					 artifactGuid, artifactName, artifactPath
					) values ( 
					 @elemGuid, @elemName, @elemAlias, @elemType, @elemStereotype, 
					 @artifactGuid, @artifactName, @artifactPath
					) " ;
		   　
	    	using (SQLiteCommand command2 = conn.CreateCommand()) {		
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                　new SQLiteParameter("@elemGuid",elem.guid)
                　, new SQLiteParameter("@elemName",elem.name)
                　, new SQLiteParameter("@elemAlias",elem.alias)
                　, new SQLiteParameter("@elemType",elem.eaType)
                　, new SQLiteParameter("@elemStereotype",elem.stereoTypeEx)
                　, new SQLiteParameter("@artifactGuid",atf.guid)
                　, new SQLiteParameter("@artifactName",atf.name)
                 , new SQLiteParameter("@artifactPath",atf.pathName)
		   　	};
	    					    
	   　		command2.CommandText = sql;
	   　		command2.Parameters.AddRange(parameters);
	        	command2.ExecuteNonQuery();
	    	}
	    	
		}
		

		private void recreateElementTable() {
			if (existElementTable()) {
				dropElementTable();
			} 
			
			createElementTable();
		}
		
	    
		private Boolean existElementTable() {

			string sql = "select count(*) from sqlite_master where type='table' and name='t_element'";
			
			using (SQLiteCommand command =conn.CreateCommand())
		    {
	            //クエリの実行
	            command.CommandText = sql;
	            using (SQLiteDataReader sdr = command.ExecuteReader())
	            {
	            	// 
	            	sdr.Read();

	            	//カラムの取り込み
	            	Int64  rowNum = (Int64)sdr[0];
        			
	                // 一件でも取得できたらtrueを返す
	                return (rowNum >= 1);
	            }
			}
		}


		private void dropElementTable() {
			
		    using (SQLiteCommand command =conn.CreateCommand())
		    {
			    command.CommandText = "drop table t_element";
		        command.ExecuteNonQuery();
		    }
			
		}		

		
		private void createElementTable() {
			
		    using (SQLiteCommand command =conn.CreateCommand())
		    {
			    command.CommandText =
					@"create table t_element
					(elemid INTEGER PRIMARY KEY AUTOINCREMENT,
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
	    
		
	}
}
