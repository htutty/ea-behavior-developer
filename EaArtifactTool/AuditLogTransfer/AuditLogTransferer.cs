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


        public List<ChangeLogItem> transAuditLogToChangeLog(string outputDir)
        {
            List<ChangeLogItem> changeLogs = new List<ChangeLogItem>();

            Console.WriteLine("{0} start read SnapShot data", DateTime.Now.ToString());

            // t_snapshot テーブルを読んでその中身をAuditLogItemリストに格納
            SnapShotDataReader reader = new SnapShotDataReader(this.fromConnStr);
            List<AuditLogItem> logItems = reader.readSnapshotData();

            // logItemsが空ならコンソールに出力して終了
            if (logItems == null)
            {
                Console.WriteLine("対象のSnapShotデータがありませんでした");
                return changeLogs;
            }

            Console.WriteLine("{0} SnapShot data record count: {1}", DateTime.Now.ToString(), logItems.Count);

            // reader.openConnection();

            StreamWriter sw = null;

            try
            {
                //BOM無しのUTF8でテキストファイルを作成する
                sw = new StreamWriter(outputDir + "\\changeLogs.txt");
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                sw.WriteLine("");

                for (int i = 0; i < logItems.Count; i++)
                {
                    AuditLogItem aItem = logItems[i];

                    // このレコードが転送対象かを判断
                    if (isTransferable(aItem))
                    {
                        Console.WriteLine("row: [ SnapshotID={0}, SeriesID={1}, SnapshotName={2}, Position={3} ]", aItem.snapshotId, aItem.seriesId, aItem.snapshotName, aItem.position);

                        ChangeLogItem chItem = transToChangeLog(aItem);

                        sw.WriteLine("--------------------------------------------------------------------------------");
                        sw.WriteLine("SnapshotId:" + chItem.snapshotId);
                        sw.WriteLine("seriesId:" + chItem.seriesId);
                        sw.WriteLine("notes:" + chItem.notes);
                        sw.WriteLine("changeType:" + chItem.changeType);
                        sw.WriteLine("changeItemName:" + chItem.changeItemName);
                        sw.WriteLine("elementGuid:" + chItem.elementGuid);
                        sw.WriteLine("metadata:" + chItem.metadata);
                        sw.WriteLine("logItem:" + chItem.logItem);
                        sw.WriteLine("changeUser:" + chItem.changeUser);
                        sw.WriteLine("changeDateTime:" + chItem.changeDateTime);

                        // 作成したChangeLogの情報をリストに保管
                        changeLogs.Add(chItem);
                    }
                    else
                    {
                        Console.WriteLine("row: [SnapshotID={0}] can not acceptable for ChangeLog", aItem.snapshotId);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sw != null) sw.Close();
                reader.closeConnection();
            }

            return changeLogs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="changeLogs"></param>
        internal void writeChangeLogDb(List<ChangeLogItem> changeLogs)
        {
            const int commitPeriod = 1000;
            int recCount = 0;

            ChangeLogIndexWriter writer = new ChangeLogIndexWriter(this.targetIndexDbFile);

            List<ChangeLogItem> commitList = new List<ChangeLogItem>();

            Console.Write("inserting table for changelogs");
            foreach (var cl in changeLogs)
            {
                if (recCount >= commitPeriod)
                {
                    // IndexDBの t_change_log テーブルに記録する(commit)
                    writer.writeChangeLogs(commitList);

                    commitList.Clear();
                    recCount = 0;
                }

                commitList.Add(cl);
                recCount++;
            }

            if (recCount > 0)
            {
                writer.writeChangeLogs(commitList);
                //writer.commitTransaction();
            }

            Console.WriteLine();
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="changeLogs"></param>
        /// <param name="writer"></param>
        internal void deleteSnapshotTable(List<ChangeLogItem> changeLogs)
        {
            const int commitPeriod = 100;
            int recCount = 0;

            List<string> deletableSnapshotIDs = new List<string>();

            SnapShotDataReader reader = new SnapShotDataReader(this.fromConnStr);
            reader.openConnection();

            Console.Write("delete table for t_snapshot");
            foreach (var cl in changeLogs)
            {
                if (recCount >= commitPeriod)
                {
                    // 読み込み元のDBから、リスト内で指定したIDのレコードを物理削除する
                    reader.deleteSnapShotTableBySnapShotIds(deletableSnapshotIDs);

                    deletableSnapshotIDs.Clear();
                    recCount = 0;
                }

                deletableSnapshotIDs.Add(cl.snapshotId);
                recCount++;
            }

            if (recCount > 0)
            {
                // 読み込み元のDBから、リスト内で指定したIDのレコードを物理削除する
                reader.deleteSnapShotTableBySnapShotIds(deletableSnapshotIDs);
                //writer.commitTransaction();
            }

            Console.WriteLine();
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

        /// <summary>
        /// t_snapshotから取得されたAuditLogデータをChangeLogデータに変換する
        /// </summary>
        /// <param name="aItem"></param>
        /// <returns></returns>
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
            chItem.metadata = rtrim(enc.GetString(aItem.binContent2));

            // ログ項目(更新内容の実際値(From-To)) = t_auditlog.BinContent1 を unzip したもの
            chItem.logItem = getUnzippedString(aItem.binContent1);

            // metadataタグの中身からトラッカー情報（更新者、更新日時）を取得
            Tracker trc = MetadataXmlReader.readXmlMetaData(chItem.metadata);
            chItem.changeUser = trc.user;
            chItem.changeDateTime = trc.dateTime;

            return chItem;
        }

        /// <summary>
        /// 文字列の後ろにNull文字が充填されている場合、そのNull文字を除いた文字列を返却する
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        private string rtrim(string orig)
        {
            char[] origAry = orig.ToCharArray();
            int charsEnd = -1;

            for (int i = 0; i < origAry.Length; i++)
            {
                if( origAry[i] == 0x00 && i > 0)
                {
                        charsEnd = i;
                        break;
                }
            }

            // 文字列の最後（Nullが現れる以前の最終地点）が初期値のままであれば
            // 特にtrim対象が存在しなかったので、origをそのまま返却する
            if (charsEnd == -1)
            {
                return orig;
            }

            //Console.WriteLine("rtrim: origLen=" + orig.Length + ", trimmed=" + charsEnd);

            // Nullが現れるまでの最終位置の長さのchar配列を用意し、
            // そちらに正常な文字列をコピーして返却する
            Char[] ret = new Char[charsEnd];

            for (int j = 0; j < charsEnd; j++)
            {
                ret[j] = origAry[j];
            }

            return new String(ret);

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
            if (idx >= 0 && idx < ary.Length)
            {
                return selectGuidString(ary, idx);
            }

            return "";
        }

        /// <summary>
        /// 指定された配列のidx番目から文字列の内容をチェックし、GUIDとしての特徴に合致したフィールドを選択する。
        /// ※t_operationテーブルの変更履歴の場合、要素のGUIDは４列目にセット
        /// </summary>
        /// <param name="ary">文字列の配列(CSVを","でsplitしたもの)</param>
        /// <param name="idx">開始インデックス</param>
        /// <returns></returns>
        private string selectGuidString(string[] ary, int idx)
        {
            for(int i=idx; i < ary.Length; i++)
            {
                if (checkGuidField(ary[i]))
                {
                    return ary[i];
                }
            }

            return "";
        }

        /// <summary>
        /// 指定された文字列がGUIDであるかをチェックする
        /// </summary>
        /// <param name="cand"></param>
        /// <returns></returns>
        private bool checkGuidField(string cand)
        {
            // "{" で始まり, "}" で終わり、文字数が38 ならGUID項目 ex: "{11111111-2222-3333-4444-555555555555}"
            if (cand.StartsWith("{") && cand.Length == 38 && cand.EndsWith("}"))
            {
                return true;
            }
            return false;
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
