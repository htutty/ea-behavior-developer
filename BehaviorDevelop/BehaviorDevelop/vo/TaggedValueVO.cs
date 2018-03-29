/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/02/23
 * Time: 9:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BehaviorDevelop.vo
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
    	
    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }
    	
		public TaggedValueVO()
		{
			changed = ' ';
		}

		public int CompareTo( TaggedValueVO o ) {
			return this.name.CompareTo(o.name);
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

