using System;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of AttributeTagVO.
	/// </summary>
	public class AttributeTagVO
	{

		/// <summary>タグ付き値の名前</summary>
    	public string name { get; set; }

		/// <summary>guid</summary>
    	public string guid { get; set; }

		/// <summary> タグ付き値の内容 </summary>
    	public string tagValue { get; set; }

		/// <summary> ノートの内容(tagValue="memo"の時はこちらに入る) </summary>
    	public string notes { get; set; }

    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

		public AttributeTagVO()
		{
			changed = ' ';
		}

		public int CompareTo( AttributeTagVO o ) {
			return this.name.CompareTo(o.name);
		}

		// コピーを作成するメソッド
		public AttributeTagVO Clone() {
			return (AttributeTagVO)MemberwiseClone();
		}

	}
}
