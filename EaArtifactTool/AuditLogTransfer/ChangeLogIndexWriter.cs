using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SQLite;
using ArtifactFileAccessor.vo;

namespace AuditLogTransfer
{
    class ChangeLogIndexWriter
    {
        string db_file;
        string projectDir = null;

        SQLiteConnection conn;
        SQLiteTransaction sqlt;

        // SQLiteTransaction transaction = null;
        Int32 changeLogRecCount = 0;

        public ChangeLogIndexWriter(string targetIndexDbFile)
        {
            this.db_file = targetIndexDbFile;

            try
            {
                string datasourceStr = this.db_file;
                this.conn = new SQLiteConnection("Data Source=" + datasourceStr);
                conn.Open();

                // t_change_log テーブルがIndexDbに存在しない場合は再生成
                createIfNotExistChangeLogTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

        public void beginTransaction()
        {
            try
            {
                sqlt = conn.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chItem"></param>
        public void writeOneChangeLog(ChangeLogItem chItem)
        {

            try
            {
                //conn.Open();
                //sqlt = conn.BeginTransaction();

                insertChangeLogTable(chItem);

                //sqlt.Commit();
                //conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void commitTransaction()
        {
            try
            {
                sqlt.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void writeChangeLogs(List<ChangeLogItem> changelogs)
        {
            //Console.Write("inserting table for changelogs ");

            try
            {
                beginTransaction();

                changeLogRecCount = 0;

                foreach (ChangeLogItem log in changelogs)
                {
                    insertChangeLogTable(log);
                }

                sqlt.Commit();
                Console.Write(".");


                //Console.WriteLine(".  done(" + changeLogRecCount + " records)");

                //conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        /// <summary>
        /// ふるまい情報のインデックステーブルの行追加
        /// </summary>
        private void insertChangeLogTable(ChangeLogItem changelog)
        {
            string sql = @"insert into t_change_log (
                           SnapshotID, SeriesID, Notes,
                           ElementGuid, ChangeUser, ChangeDateTime,
                           ChangeType, ChangeItemName, Metadata, LogItem
                          ) values (
                           @snapshotId, @seriesId, @notes,
                           @elementGuid, @changeUser, @changeDateTime,
                           @changeType, @changeItemName, @metadata, @logItem
                          ) ";

            using (SQLiteCommand command2 = conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[]{
                  new SQLiteParameter("@snapshotId",changelog.snapshotId)
                  , new SQLiteParameter("@seriesId",changelog.seriesId)
                  , new SQLiteParameter("@notes",changelog.notes)
                  , new SQLiteParameter("@elementGuid", changelog.elementGuid)
                  , new SQLiteParameter("@changeUser", changelog.changeUser)
                  , new SQLiteParameter("@changeDateTime", changelog.changeDateTime)
                  , new SQLiteParameter("@changeType", changelog.changeType)
                  , new SQLiteParameter("@changeItemName", changelog.changeItemName)
                  , new SQLiteParameter("@metadata",changelog.metadata)
                  , new SQLiteParameter("@logItem",changelog.logItem)
                };

                command2.CommandText = sql;
                command2.Parameters.AddRange(parameters);
                command2.ExecuteNonQuery();

                // Console.WriteLine("insert t_change_log ( SnapShotID = {0} )", changelog.snapshotId);
            }


        }


        /// <summary>
        /// t_parsed_behavior テーブルの存在チェック、再作成の処理
        /// </summary>
        private void createIfNotExistChangeLogTable()
        {
            string tableName = "t_change_log";

            //this.conn.Open();

            if (!existTargetTable(tableName))
            {
                createChangeLogTable();
            }

            //this.conn.Close();
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
                      ChangeType TEXT,
                      ChangeItemName TEXT,
                      Metadata TEXT,
                      LogItem TEXT
				    )";
                command.ExecuteNonQuery();
            }

        }


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


    }

}
