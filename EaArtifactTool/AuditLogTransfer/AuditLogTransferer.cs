using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using ArtifactFileAccessor.vo;


namespace AuditLogTransfer
{
    class AuditLogTransferer
    {
        private string fromConnStr;
        private string targetIndexDbFile;
        private Encoding enc = Encoding.GetEncoding("utf-16");

        public AuditLogTransferer(string fromConnStr, string targetIndexDbFile)
        {
            this.fromConnStr = fromConnStr;
            this.targetIndexDbFile = targetIndexDbFile;
        }


        public void transAuditLogToChangeLog()
        {

            List<ChangeLogItem> changeLogs = new List<ChangeLogItem>();

            //  t_snap
            SnapShotDataReader reader = new SnapShotDataReader(this.fromConnStr);

            List<AuditLogItem> logItems = reader.readSnapshotData();

            if (logItems == null)
            {
                Console.WriteLine("対象のSnapShotデータがありませんでした");
                return;
            }

            // reader.openConnection();

            ChangeLogIndexWriter writer =
                new ChangeLogIndexWriter(targetIndexDbFile);

            foreach (AuditLogItem aItem in logItems)
            {
                // このレコードが転送対象かを判断
                if ( isTransferable(aItem) )
                {
                    Console.WriteLine("row: [ SnapshotID={0}, SeriesID={1}, SnapshotName={2}, Position={3} ]", aItem.snapshotId, aItem.seriesId, aItem.snapshotName, aItem.position);

                    ChangeLogItem chItem = transToChangeLog(aItem);

                    // IndexDBの t_change_log テーブルに記録する(毎行commit)
                    writer.writeOneChangeLog(chItem);

                    // 成功したら該当行を t_change_log テーブルに記録する(毎行commit)
                    reader.deleteSnapShotTableBySnapShotId(aItem.snapshotId);

                    // 作成したChangeLogの情報をリストに保管
                    changeLogs.Add(chItem);
                }
                else
                {
                    Console.WriteLine("  ");
                }

            }

            reader.closeConnection();

            // ChangeLogに記録
            // writer.writeChangeLogs(changeLogs);

        }


        // 
        private bool isTransferable(AuditLogItem aItem)
        {
            // t_snapshotの snapshotName は更新されたテーブルを示すため、 この値が "t_snapshot" なら転送不要
            if (aItem.snapshotName == "t_snapshot")
            {
                return false;
            }
            
            return true;
        }


        private ChangeLogItem transToChangeLog(AuditLogItem aItem)
        {

            ChangeLogItem chItem = new ChangeLogItem();
            chItem.snapshotId = aItem.snapshotId;
            chItem.seriesId = aItem.seriesId;
            chItem.notes = aItem.notes;

            // 変更タイプ(INSERT/UPDATE/DELETE) = t_snapshot.style
            chItem.changeType = aItem.style;
            // 変更テーブル名 = t_auditlog.SnapshotName;
            chItem.changeItemName = aItem.snapshotName;
            // notes項目から ElementGuid項目の取得を試みる
            chItem.elementGuid = getElementGuidFromNotes(aItem);

            // メタデータ(更新されたテーブルのキー項目などが設定) = t_auditlog.BinContent2
            chItem.metadata = enc.GetString(aItem.binContent2);

            // ログ項目(更新内容の実際値(From-To)) = t_auditlog.BinContent1 を unzip したもの
            chItem.logItem = getUnzippedString(aItem.binContent1);

            // metadataタグの中身からトラッカー情報（更新者、更新日時）を取得
            Tracker trc = MetadataXmlReader.readXmlMetaData(enc.GetString(aItem.binContent2));
            chItem.changeUser = trc.user;
            chItem.changeDateTime = trc.dateTime;

            return chItem;
        }



        /// <summary>
        /// t_snapshotのnotes項目がカンマ区切り形式で {GUID},{name},{type}と記録されるのを利用し、
        /// GUIDの値を取得する。
        /// 対象テーブルが t_object の場合は １番目（添字０）、t_attribute や t_method の場合は
        /// 4番目（添え字３）となるようだが、他のテーブルでのnotes項目の出力内容の詳細は不明。
        /// </summary>
        /// <param name="aItem">t_snapshot テーブルから読み込まれたAuditLogItem項目</param>
        /// <returns></returns>
        private string getElementGuidFromNotes( AuditLogItem aItem )
        {

            // elementGuid項目のセット
            if (aItem.snapshotName == "t_object")
            {
                // t_objectテーブルの場合、
                return getColumnDataFromCsv(aItem.notes, 0);
            }
            else if (aItem.snapshotName == "t_attribute" || aItem.snapshotName == "t_operation")
            {
                return getColumnDataFromCsv(aItem.notes, 3);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 引数の文字列origをCSVデータと見なし、カンマ(,)でsplitされた列のidx番目を返却する
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="idx"></param>
        /// <returns>idx番目の列情報（idxが実際の列インデックスを超えていた場合は空文字列）</returns>
        private string getColumnDataFromCsv(string orig, int idx)
        {
            // 引数の文字列を","でSplitする
            string[] ary = orig.Split(',');

            // idx値が配列の要素数の範囲内と判断されたら
            if(idx >= 0 && idx < ary.Length)
            {
                // 配列のidx列目の値を返却
                return ary[idx];
            }

            return "";
        }


        /// <summary>
        /// 引数のバイト配列をZipのストリームと見なし展開された str.dat の中身を読み込み返却する
        /// </summary>
        /// <param name="buffer">バイト配列のバッファ</param>
        /// <returns>Zip</returns>
        private static string getUnzippedString(byte[] buffer)
        {
            // バイト配列をストリームとして読み込む
            MemoryStream ms = new MemoryStream(buffer);
            ZipArchive archive = new ZipArchive(ms);

            //「str.dat」のZipArchiveEntryを取得する
            ZipArchiveEntry e = archive.GetEntry(@"str.dat");
            if (e == null)
            {
                //見つからなかった時
                Console.WriteLine("str.dat が見つかりませんでした。");
                return "file str.dat not found";
            }
            else
            {
                //見つかった時は開く
                using (StreamReader sr = new StreamReader(e.Open(), Encoding.GetEncoding("UTF-16")))
                {
                    //すべて読み込み結果を返す
                    return sr.ReadToEnd();
                }
            }
        }


    }
}
