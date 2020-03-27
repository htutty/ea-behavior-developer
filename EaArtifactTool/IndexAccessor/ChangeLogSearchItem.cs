using System;

namespace IndexAccessor
{
    /// <summary>
    /// Description of ElementSearchItem.
    /// </summary>
    public class ChangeLogSearchItem : IComparable<ChangeLogSearchItem>
	{
        /// <summary>変更履歴ID</summary>
        public int changeLogId { get; set; }

        /// <summary>要素GUID</summary>
        public string elemGuid { get; set; }

        /// <summary>ノート</summary>
        public string notes { get; set; }

		/// <summary>変更ユーザー</summary>
    	public string changeUser { get; set; }

        /// <summary>変更日時</summary>
        public DateTime changeDateTime { get; set; }

		/// <summary>変更の種類(INSERT/UPDATE/DELETE)</summary>
    	public string changeType { get; set; }

        /// <summary>変更対象（テーブル名）</summary>
        public string changeItem { get; set; }

        /// <summary>メタデータ </summary>
        public string metadata { get; set; }

        /// <summary>LogItem</summary>
        public string logItem { get; set; }

        /// <summary>要素名</summary>
        public string elemName { get; set; }

        /// <summary>要素パス</summary>
        public string elemPath { get; set; }


        public ChangeLogSearchItem()
		{
		}

        /// <summary>
        /// ソートのための要素比較メソッド
        /// </summary>
        /// <param name="other">比較対象インスタンス</param>
        /// <returns>this.elementId と other.elementId の差</returns>
        public int CompareTo(ChangeLogSearchItem other)
        {
            return this.changeLogId - other.changeLogId;
        }
    }
}
