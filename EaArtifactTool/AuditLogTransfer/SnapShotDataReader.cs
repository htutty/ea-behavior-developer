using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogTransfer
{
    class SnapShotDataReader
    {
        // 接続文字列(OLEDB用)
        private string connStr ;

        // 接続オブジェクト
        private OleDbConnection objConn;

        private Encoding enc;

        public SnapShotDataReader(string targetConnStr)
        {
            objConn = new OleDbConnection();
            this.connStr = targetConnStr;
            enc = Encoding.GetEncoding("utf-8");
        }

        /// <summary>
        /// t_snapshotテーブルを読み込む
        /// </summary>
        /// <returns></returns>
        public List<AuditLogItem> readSnapshotData()
        {
            List<AuditLogItem> retList = new List<AuditLogItem>();

            try
            {
//                string connStr = "Provider=SQLOLEDB.1;Password=P[ssw0rd!;Persist Security Info=True;User ID=sa;Initial Catalog=uiux-modeling-sample;Data Source=192.168.79.129\\SQLEXPRESS,1433;LazyLoad=1;";

                // MDB名など
                //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + eapfile; (Access Connector Environment 2016)
                //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.15.0;Data Source=" + eapfile; (Access Connector Environment 2013)
                //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + eapfile; (Access Connector Environment 2010)
                // this.objConn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + eapfile; // Jet OleDB 4.0(32bit)
                this.objConn.ConnectionString = this.connStr;

                // 接続する
                this.objConn.Open();

                // t_snapshot テーブルを読み込み、データを返却リストにセットする
                retList = readSnapShotTable();

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
                this.objConn.Close();

            }

            return retList;
        }


        /// <summary>
        /// パラメータのタグ付き値をDBから読み込み、Mapに格納する
        /// </summary>
        private List<AuditLogItem> readSnapShotTable()
        {
            Console.WriteLine("readSnapShotTable()");

            List<AuditLogItem> retList = new List<AuditLogItem>();
            string strSQL, strFields;

            // 読み込む項目リスト
            strFields =
                " SnapshotID, SeriesID, [Position], SnapshotName, Notes, " +
                " [Style], ElementID, ElementType, StrContent, BinContent1, BinContent2 ";

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_snapshot " +
                " order by SnapshotID ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            // DB読み込み
            OleDbDataReader reader = dbCom.ExecuteReader();
            while (reader.Read())
            {
                AuditLogItem logItem = new AuditLogItem();

                logItem.snapshotId = DbUtil.readStringField(reader, 0);
                logItem.seriesId = DbUtil.readStringField(reader, 1);
                logItem.position = DbUtil.readIntField(reader, 2);
                logItem.snapshotName = DbUtil.readStringField(reader, 3);
                logItem.notes = DbUtil.readStringField(reader, 4);

                logItem.style = DbUtil.readStringField(reader, 5);
                logItem.elementId = Int32.Parse(DbUtil.readStringField(reader, 6)); 
                logItem.elementType = DbUtil.readStringField(reader, 7);
                logItem.strContent = DbUtil.readStringField(reader, 8);

                logItem.binContent1 = readBinaryField(reader, 9);
                logItem.binContent2 = readBinaryField(reader, 10);

                retList.Add(logItem);
            }

            reader.Close();

            Console.WriteLine("readSnapShotTable() 終了");


            return retList;

        }


        private Byte[] readBinaryField(OleDbDataReader reader, int fieldNo )
        {
            const int bufSize = 1000;
            int startIdx = 0;
            Byte[] buf = new Byte[bufSize];

            MemoryStream ms = new MemoryStream();

            try
            {
                long getLength = reader.GetBytes(fieldNo, startIdx, buf, 0, bufSize);
                //Console.WriteLine("buf=" + getDumpStr(buf));
                ms.Write(buf, 0, (int)getLength);

                // バイナリフィールドから読み込めたバイト数がバッファサイズと同じであれば、まだ続きがあるということ
                while (getLength >= bufSize)
                {
                    startIdx = startIdx + bufSize;
                    getLength = reader.GetBytes(fieldNo, startIdx, buf, 0, bufSize);
                    // Console.WriteLine("buf=" + getDumpStr(buf));
                    ms.Write(buf, 0, (int)getLength);
                }

                return ms.GetBuffer();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("バイナリフィールドの読み込みで例外が発生しました:");
                Console.WriteLine(ex.Message);
            }

            return new byte[0];

            // BinContent2 の内容を Base64 デコードしたものを出力
            //MyBase64str myBase64 = new MyBase64str("UTF-16");
            //Console.Write(myBase64.Decode(item.binContent2));
        }

        /// <summary>
        /// バイナリデータを受けて16進でダンプする
        /// </summary>
        /// <param name="args">バイナリデータ</param>
        /// <returns>各バイトをの16進2桁(00～FF)で表した文字列</returns>
        private string getDumpStr(byte[] args)
        {
            string str = "";
            for (int i = 0; i < args.Length; i++)
            {
                str += string.Format("{0:X2} ", args[i]);
            }

            return str;
        }

        public void openConnection()
        {
            this.objConn.Open();
        }

        public void closeConnection()
        {
            this.objConn.Close();
        }

        /// <summary>
        /// 転送が完了したSnapshotId
        /// </summary>
        /// <param name="snapshotId"></param>
        public void deleteSnapShotTableBySnapShotId(string snapshotId)
        {
            Console.WriteLine("deleteSnapShotTableBySnapShotId()");

            List<AuditLogItem> retList = new List<AuditLogItem>();


            // 接続する
            this.objConn.Open();
            OleDbTransaction transaction = objConn.BeginTransaction();

            using (OleDbCommand command = this.objConn.CreateCommand())
            {

                // SQL文 を作成
                string sql = "delete from t_snapshot " +
                    " where SnapshotID = '" + snapshotId + "' ";

                command.Transaction = transaction;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
            this.objConn.Close();

            Console.WriteLine("deleteSnapShotTableBySnapShotId(SnapshotID = {0}) done.", snapshotId);
        }

    }


}
