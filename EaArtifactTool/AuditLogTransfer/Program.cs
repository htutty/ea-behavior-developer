using System;
using System.Collections.Generic;
using System.Linq;

using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.reader;

namespace AuditLogTransfer
{
    class Program
    {
        static int Main(string[] args)
        {
            string repositoriesFile;
            string targetRepositoryName;

            if ( args.Length < 2 )
            {
                Console.WriteLine("引数が足りません：");
                Console.WriteLine("使い方： AuditLogTransfer.exe <repositoriesFile(.xml)> <targetRepoName(ex:cuvic_aswea_master)> ");
                return 1;
            }

            repositoriesFile = args[0];
            targetRepositoryName = args[1];

            try
            {
                // repositories.xml, bdprjファイルの読み込み
                RepositorySettingVO repo =
                    RepositorySetting.readRepositoryAndSelect(repositoriesFile, targetRepositoryName);

                // 転送元DB(MDB, SQLServer, FireBird etc)情報（接続文字列）はrepositories.xmlから取得
                string fromConnStr = repo.connectionString;
                // 転送先DB情報はリポジトリ設定から取得
                //string toIndexDbFile = repo.projectSettingVO.projectPath + "\\" + repo.projectSettingVO.dbName;
                string toIndexDbFile = repo.changeLogDbPath;

                // t_snapshotの監査ログ情報をchangelogデータに変換する
                AuditLogTransferer trans = new AuditLogTransferer(fromConnStr, toIndexDbFile);
                List<ChangeLogItem> changeLogs = trans.transAuditLogToChangeLog(repo.projectSettingVO.projectPath);

                // history用のchangeLogDbにChangeLogデータを記録する
                trans.writeChangeLogDb(changeLogs);

                // 転送が成功したら t_snapshot テーブルから削除する
                trans.deleteSnapshotTable(changeLogs);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(ex.Message);
                return 2;
            }

        }

    }
}
