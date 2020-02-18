using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogTransfer
{
    public class AuditLogItem
    {
        //  <SnapshotID>L20</SnapshotID>
        public string snapshotId { get; set; }

        //  <SeriesID>LOG</SeriesID>
        public string seriesId { get; set; }

        //  <Position>43801483</Position>
        public int position { get; set; }

        //  <SnapshotName>t_attribute</SnapshotName>
        public string snapshotName { get; set; }

        //  <Notes>{871F1CE7-6B10-4a77-97E6-C27AE7801532},属性の追加テスト２,属性,
        //    {50410763-C9B4-4695-BBAF-9D1EE2834E4E},会員情報サービス,Class,</Notes>
        public string notes { get; set; }

        //  <Style>INSERT</Style>
        public string style { get; set; }

        //  <ElementID>1</ElementID>
        public long elementId { get; set; }

        //  <ElementType>&lt;EMPTY&gt;</ElementType>
        public string elementType { get; set; }

        //  <StrContent/>
        public string strContent { get; set; }

        //  <BinContent1 xmlns:dt="urn:schemas-microsoft-com:datatypes" dt:dt="bin.base64">
        //    UEsDBBQAAAAIAIRcgk+OjJJs6QEAANQIAAAHABEAc3RyLmRhdFVUDQAHIvfkXSL35F0i9+Rd
        //    xVbdSgJREP6ug256gsWb6kLcNX9LgzKDICzMuopM3S0MWyV/yqJn6BV6jV4geqmob+ZoYkTg
        //    ....略....
        //  </BinContent1>
        public Byte[] binContent1 { get; set; }

        //  <BinContent2 xmlns:dt="urn:schemas-microsoft-com:datatypes" dt:dt="bin.base64">
        //    PABtAGUAdABhAGQAYQB0AGEAPgA8AFIAbwB3ACAATgB1AG0AYgBlAHIAPQAiADAAIgAgAFMA
        //    dABhAG4AZABhAHIAZAA9ACIAMQAiAD4APABMAGUAdgBlAGwAIABMAGUAdgBlAGwATgBhAG0A
        //    ....略....
        //  </BinContent2>
        public Byte[] binContent2 { get; set; }

    }
}
