using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public class CrossRefVO
    {
        /*
         * CREATE TABLE t_xref (
         * XrefID		CHAR(255),
         * Name    	CHAR(255),
         * Type		CHAR(255),
         * Visibility	CHAR(255),
         * Namespace	CHAR(255),
         * Requirement	CHAR(255),
         * Constraint	CHAR(255),
         * Behavior	CHAR(255),
         * Partition	CHAR(255),
         * Description	MEMO,
         * Client		CHAR(255),
         * Supplier	CHAR(255),
         * Link		CHAR(255)
         * ) ;
        */

        /// <summary> 相互参照のユニークID(形式はGUID) </summary>
        public string xrefId { get; set; }

        /// <summary>名前</summary>
        public string name { get; set; }

        /// <summary>型</summary>
        public string eaType { get; set; }

        /// <summary>可視性</summary>
        public string visibility { get; set; }

        /// <summary>名前空間</summary>
        public string eaNameSpace { get; set; }

        /// <summary>要求</summary>
        public string requirement { get; set; }

        /// <summary>制約</summary>
        public string constraint { get; set; }

        /// <summary>ふるまい</summary>
        public string behavior { get; set; }

        /// <summary>パーティション</summary>
        public string partition { get; set; }

        /// <summary>説明</summary>
        public string description { get; set; }

        /// <summary>クライアント</summary>
        public string client { get; set; }

        /// <summary>サプライヤー</summary>
        public string supplier { get; set; }

        /// <summary>リンク</summary>
        public string link { get; set; }

        /// <summary>
        ///  自然順序付け用比較メソッド
        /// </summary>
        /// <param name="o">比較相手のインスタンス</param>
        /// <returns>xrefIdで比較し、this&gt;o → 1 / this==o → 0 / this&lt;o → -1</returns>
        public int CompareTo(CrossRefVO o)
        {
            return this.xrefId.CompareTo(o.xrefId);
        }

    }
}
