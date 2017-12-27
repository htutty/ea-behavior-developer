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

namespace ElementIndexer
{
	/// <summary>
	/// Description of DatabaseWriter.
	/// </summary>
	public class DatabaseWriter
	{
		const string db_file = "element_idx.db";
		SQLiteConnection conn ;
		
		public DatabaseWriter()
		{
			try { 
				this.conn = new SQLiteConnection("Data Source="+db_file);
			} catch(Exception e) {
				throw e ;
			}

		}

		public void write() {
			
			conn.Open();
		    using (SQLiteCommand command =conn.CreateCommand())
		    {
			    command.CommandText =
					@"create table t_connector
					(id INTEGER PRIMARY KEY AUTOINCREMENT,
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
		    
		    conn.Close();
		}
		    
	}
}
