using System;
using System.Collections.Generic;
using System.IO;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of ParameterVO.
	/// </summary>
	public class ParameterVO : IComparable<ParameterVO>
	{
		// 	Public Name ' As String
		/// <summary>名前</summary>
    	public string name { get; set; }

		//	Public Alias ' As String
		/// <summary>別名</summary>
    	public string alias { get; set; }

    	//	Public ParameterGUID ' As String
		/// <summary>パラメータのguid</summary>
    	public string guid { get; set; }

    	//	Public Notes ' As String
		/// <summary>ノート</summary>
    	public string notes { get; set; }

		/// <summary>ステレオタイプ</summary>
    	public string stereoType { get; set; }

		/// <summary>パラメータの型</summary>
    	public string eaType { get; set; }

		/// <summary>EA上でのパラメータ型を表すID値</summary>
		public string objectType { get; set; }

		/// <summary>並び順</summary>
    	public Int32 pos { get; set; }

		/// <summary>パラメータの分類子ID</summary>
    	public string classifierID { get; set; }

		/// <summary>初期値</summary>
    	public string defaultValue { get; set; }

		/// <summary>定数フラグ</summary>
    	public Boolean isConst { get; set; }

		/// <summary>スタイル</summary>
    	public string styleEx { get; set; }

		/// <summary>パラメータの種類</summary>
    	public string kind { get; set; }

		/// <summary>パラメータの種類</summary>
    	public List<ParamTagVO> paramTags { get; set; }

    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

		public ParameterVO()
		{
			changed = ' ';
            paramTags = new List<ParamTagVO>();
        }

		/// <summary>
		/// ソートで呼び出される比較処理
		/// </summary>
		/// <param name="o">比較対象</param>
		/// <returns></returns>
		public int CompareTo( ParameterVO o ) {
			return ((this.pos - o.pos) == 0 ? this.name.CompareTo(o.name):(this.pos - o.pos));
		}

		// コピーを作成するメソッド
		public ParameterVO Clone() {
			return (ParameterVO)MemberwiseClone();
		}

        /// <summary>
        /// JavaのtoString()と同様、自身の項目値を全てつなげた文字列を生成して返却する
        /// </summary>
        /// <returns>自身の項目値を全てつなげた文字列</returns>
        public string getComparableString()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("name = " + name);
            sw.WriteLine("alias = " + alias);
            sw.WriteLine("guid = " + guid);
            sw.WriteLine("notes = " + notes);
            sw.WriteLine("stereoType = " + stereoType);
            sw.WriteLine("eaType = " + eaType);
            sw.WriteLine("objectType = " + objectType);
            sw.WriteLine("pos = " + pos);
            sw.WriteLine("classifierID = " + classifierID);
            sw.WriteLine("defaultValue = " + defaultValue);
            sw.WriteLine("isConst = " + isConst);
            // sw.WriteLine("styleEx = " + styleEx);
            sw.WriteLine("kind = " + kind);

            if( paramTags != null && paramTags.Count > 0)
            {
                sw.WriteLine("taggedValues=[");
                foreach (var tv in paramTags)
                {
                    sw.WriteLine("tv=" + tv.getComparableString() + ",");
                }
                sw.WriteLine("]");
            }

            return sw.ToString();
        }


        public void sortChildNodes()
        {
            if (paramTags != null && paramTags.Count > 1 )
            {
                paramTags.Sort();
            }
        }


        public void sortChildNodesGuid()
        {
            if (paramTags != null && paramTags.Count > 1)
            {
                ParamTagGuidComparer cmp = new ParamTagGuidComparer();
                paramTags.Sort(cmp);
            }

        }

    }

    /// <summary>
    /// Description of MethodComparer.
    /// </summary>
    public class ParameterGuidComparer : IComparer<ParameterVO>
    {
        public ParameterGuidComparer()
        {
        }

        // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
        public int Compare(ParameterVO x, ParameterVO y)
        {
            return x.guid.CompareTo(x.guid);
        }
    }

}
