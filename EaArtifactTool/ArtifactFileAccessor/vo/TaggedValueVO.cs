using System;
using System.Collections.Generic;
using System.IO;

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


        /// <summary>
        /// JavaのtoString()と同様、自身の項目値を全てつなげた文字列を生成して返却する
        /// </summary>
        /// <returns>自身の項目値を全てつなげた文字列</returns>
        public string getComparableString()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("name = " + name);
            sw.WriteLine("guid = " + guid);
            sw.WriteLine("tagValue = " + tagValue);
            sw.WriteLine("notes = " + notes);
            return sw.ToString();
        }

        /// <summary>
        /// 引数のオブジェクトと自身の値を比較し、差異のあった項目のみで"name = value"の文字列を返却する
        /// </summary>
        /// <param name="o">比較先のタグ付き値オブジェクト</param>
        /// <returns></returns>
        public string getComparedString(TaggedValueVO o)
        {
            StringWriter sw = new StringWriter();

            if (name != o.name)
            {
                sw.WriteLine("name = " + name);
            }

            if (guid != o.guid)
            {
                sw.WriteLine("guid = " + guid);
            }

            if (tagValue != o.tagValue)
            {
                sw.WriteLine("tagValue = " + tagValue);
            }

            if (notes != o.notes)
            {
                sw.WriteLine("[notes]\r\n" + notes);
            }

            // sw.WriteLine("notes = " + notes);
            return sw.ToString();
        }

    }


    /// <summary>
    /// Description of TaggedValueComparer.
    /// </summary>
    public class TaggedValueGuidComparer : IComparer<TaggedValueVO>
	{
		public TaggedValueGuidComparer()
		{
		}

	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(TaggedValueVO x, TaggedValueVO y)
	    {
	    	return x.guid.CompareTo(y.guid);
	    }
	}

}

