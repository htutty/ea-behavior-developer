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
        static void Main(string[] args)
        {
            string repositoriesFile;
            string targetRepositoryName;

            if ( args.Length >= 2 )
            {
                repositoriesFile = args[0];
                targetRepositoryName = args[1];

                // .bdprjファイルの読み込み
                RepositorySettingVO repo =
                    RepositorySetting.readRepositoryAndSelect(repositoriesFile, targetRepositoryName);

                // 転送元DB(MDB, SQLServer, FireBird etc)情報（接続文字列）はrepositories.xmlから取得
                string fromConnStr = repo.connectionString;
                // 転送先DB情報はプロジェクト設定から取得
                string toIndexDbFile = repo.projectSettingVO.projectPath + "\\" + repo.projectSettingVO.dbName;

                // indexDBファイルのパスを取得
                //string indexDbFilePath = ProjectSetting.getVO().projectPath + "\\" + ProjectSetting.getVO().dbName;

                // t_snapshotの監査ログ情報をindexDbのchangelogデータに変換する
                AuditLogTransferer trans = new AuditLogTransferer(fromConnStr, toIndexDbFile);
                trans.transAuditLogToChangeLog();
            }
            else
            {
                Console.WriteLine("引数が足りません：");
                Console.WriteLine("使い方： AuditLogTransfer.exe <repositoriesFile(.xml)> <targetRepoName(ex:cuvic_aswea_master)> ");
            }

        }

    }
}
