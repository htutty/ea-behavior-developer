/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/13
 * Time: 19:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BehaviorDevelop.vo
{
	/// <summary>
	/// Description of AttributeVO.
	/// </summary>
	public class AttributeVO : IComparable<AttributeVO>
	{
		// 	Public Name ' As String
		/// <summary>名前</summary>
    	public string name { get; set; }
    
		//	Public Alias ' As String
		/// <summary>別名</summary>
    	public string alias { get; set; }
    	
    	//	Public AttributeGUID ' As String
		/// <summary>属性のguid</summary>
    	public string guid { get; set; }
		
		//	Public AttributeID ' As Long
		/// <summary>属性のid</summary>
    	public int attributeId { get; set; }

    	//	Public Notes ' As String
		/// <summary>ノート</summary>
    	public string notes { get; set; }
 
    	/// <summary>並び順</summary>
    	public Int32 pos { get; set; }

		/// <summary>ステレオタイプ</summary>
    	public string stereoType { get; set; }
    	
    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

        public AttributeVO()
		{
        	changed = ' ';
		}

		public int CompareTo( AttributeVO o ) {
			return ((this.pos - o.pos) == 0 ? this.name.CompareTo(o.name):(this.pos - o.pos));
		}
    		
    	
	}
}
