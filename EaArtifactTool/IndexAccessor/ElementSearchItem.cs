using System;

namespace IndexAccessor
{
    /// <summary>
    /// Description of ElementSearchItem.
    /// </summary>
    public class ElementSearchItem : IComparable<ElementSearchItem>
	{
        /// <summary>名前</summary>
        public int elementId { get; set; }

        /// <summary>名前</summary>
        public string elemName { get; set; }

		/// <summary>別名</summary>
    	public string elemAlias { get; set; }

		/// <summary>要素のタイプ</summary>
    	public string elemType { get; set; }

		/// <summary>ステレオタイプ</summary>
    	public string elemStereotype { get; set; }

		/// <summary>要素GUID</summary>
    	public string elemGuid { get; set; }

        /// <summary>要素の配置パス</summary>
        public string elemPath { get; set; }

        /// <summary>成果物GUID </summary>
        public string artifactGuid { get; set; }

        /// <summary>成果物名(パッケージ名) </summary>
        public string artifactName { get; set; }

        public ElementSearchItem()
		{
		}

        /// <summary>
        /// ソートのための要素比較メソッド
        /// </summary>
        /// <param name="other">比較対象インスタンス</param>
        /// <returns>this.elementId と other.elementId の差</returns>
        public int CompareTo(ElementSearchItem other)
        {
            return this.elementId - other.elementId;
        }
    }
}
