using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtifactFileAccessor.vo
{
    public class ChangeLogItem
    {
        public int changeLogId { get; set; }

        //  <SnapshotID>L20</SnapshotID>
        public string snapshotId { get; set; }

        //  <SeriesID>LOG</SeriesID>
        public string seriesId { get; set; }

        //  <Notes>{871F1CE7-6B10-4a77-97E6-C27AE7801532},属性の追加テスト２,属性,
        //    {50410763-C9B4-4695-BBAF-9D1EE2834E4E},会員情報サービス,Class,</Notes>
        public string notes { get; set; }

        // 変更タイプ(INSERT/UPDATE/DELETE)
        public string changeType { get; set; }

        // 変更テーブル名
        public string changeItemName { get; set; }

        public string elementGuid { get; set; }

        public string metadata { get; set; }

        public string logItem { get; set; }

        public string changeUser { get; set; }

        public string changeDateTime { get; set; }

    }
}
