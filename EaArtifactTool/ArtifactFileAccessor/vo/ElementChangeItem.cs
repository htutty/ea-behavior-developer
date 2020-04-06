using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ArtifactFileAccessor.vo
{
    public class ElementChangeItem
    {
        public int elementId { get; set; }
        public string elementGuid { get; set; }
        public string elementName { get; set; }
        public string changeTableName { get; set; }

        public ChangeItemType targetItemType { get; set; }

        /// <summary>変更ユーザー</summary>
        public string changeUser { get; set; }

        /// <summary>変更日時</summary>
        public DateTime changeDateTime { get; set; }

        /// <summary>変更の種類(INSERT/UPDATE/DELETE)</summary>
        public string changeType { get; set; }

        public string targetGuid { get; set; }
        public string targetName { get; set; }

        // logItemをparseして得られた変更された列のList
        public List<ChangeItemColumn> changeItemColumns;

    }


    public enum ChangeItemType
    {
        TYPE_ELEMENT,
        TYPE_ATTRIBUTE,
        TYPE_METHOD,
        TYPE_METHOD_PARAM
    }

}
