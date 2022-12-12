using System;
using System.Collections.Generic;
using System.IO;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// メソッドパラメータ用のタグ付き値.
	/// </summary>
	public class ParamTagVO : IComparable<ParamTagVO>
	{
		/// <summary>タグ付き値の名前</summary>
    	public string name { get; set; }

		/// <summary>guid</summary>
    	public string guid { get; set; }

        /// <summary>親となるパラメータのGUID</summary>
        public string paramGuid { get; set; }

        /// <summary> ノート(他のタグ付き値と違い、値は常にnotesに入る) </summary>
        public string notes { get; set; }

    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

        /// <summary>
        /// 新旧比較時に差分があった時に使用
        /// </summary>
        public ParamTagVO srcParamTag { get; set; }

        public ParamTagVO destParamTag { get; set; }


        public ParamTagVO()
		{
			changed = ' ';
		}

		public int CompareTo(ParamTagVO o) {
			return this.name.CompareTo(o.name);
		}

		// コピーを作成するメソッド
		public ParamTagVO Clone() {
			return (ParamTagVO)MemberwiseClone();
		}


        /// <summary>
        /// JavaのtoString()と同様、自身の項目値を全てつなげた文字列を生成して返却する
        /// </summary>
        /// <returns>自身の項目値を全てつなげた文字列</returns>
        public string getComparableString()
        {
            StringWriter sw = new StringWriter();

            sw.WriteLine("paramGuid = " + paramGuid);
            sw.WriteLine("guid = " + guid);
            sw.WriteLine("name = " + name);
            sw.WriteLine("notes = " + notes);
            return sw.ToString();
        }

        /// <summary>
        /// JavaのtoString()と同様、自身の項目値を全てつなげた文字列を生成して返却する
        /// </summary>
        /// <returns>自身の項目値を全てつなげた文字列</returns>
        public string toString()
        {
            StringWriter sw = new StringWriter();

            sw.Write("paramGuid = " + paramGuid);
            sw.Write(", guid = " + guid);
            sw.Write(", name = " + name);
            sw.Write(", notes = " + notes);
            sw.WriteLine("");

            return sw.ToString();
        }
    }


    /// <summary>
    /// Description of ParamTagGuidComparer .
    /// </summary>
    public class ParamTagGuidComparer : IComparer<ParamTagVO>
	{
		public ParamTagGuidComparer()
		{
		}

	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(ParamTagVO x, ParamTagVO y)
	    {
	    	return x.guid.CompareTo(y.guid);
	    }
	}

}

