using System;
using System.Collections.Generic;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of TaggedValueVO.
	/// </summary>
	public class TaggedValueVO : IComparable<TaggedValueVO>
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

        /// <summary>
        /// 新旧比較時に差分があった時に使用
        /// </summary>
        public TaggedValueVO srcTaggedValue { get; set; }

        public TaggedValueVO destTaggedValue { get; set; }


        public TaggedValueVO()
		{
			changed = ' ';
		}

		public int CompareTo( TaggedValueVO o ) {
			return this.name.CompareTo(o.name);
		}

		// コピーを作成するメソッド
		public TaggedValueVO Clone() {
			return (TaggedValueVO)MemberwiseClone();
		}
	}


	/// <summary>
	/// Description of TaggedValueComparer.
	/// </summary>
	public class TaggedValueComparer : IComparer<TaggedValueVO>
	{
		public TaggedValueComparer()
		{
		}

	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(TaggedValueVO x, TaggedValueVO y)
	    {
	    	return x.guid.CompareTo(y.guid);
	    }
	}

}

