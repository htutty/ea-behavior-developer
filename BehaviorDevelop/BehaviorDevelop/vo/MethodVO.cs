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
	/// Description of MethodVO.
	/// </summary>
	public class MethodVO : IComparable<MethodVO>
	{
		
		// 	Public Name ' As String
		/// <summary>名前</summary>
    	public string name { get; set; }
    
		//	Public Alias ' As String
		/// <summary>別名</summary>
    	public string alias { get; set; }
    	
    	//	Public MethodGUID ' As String
		/// <summary>操作のguid</summary>
    	public string guid { get; set; }
		
		//	Public MethodID ' As Long
		/// <summary>操作のid</summary>
    	public int methodId { get; set; }

    	//	Public Notes ' As String
		/// <summary>ノート</summary>
    	public string notes { get; set; }
    	
    	//	Public Behavior ' As String
		/// <summary>振る舞い</summary>
    	public string behavior { get; set; }
    	
		/// <summary>戻り型</summary>
    	public string returnType { get; set; }
    	
		/// <summary>可視性</summary>
    	public string visibility { get; set; }
    	
		/// <summary>並び順</summary>
    	public Int32 pos { get; set; }

		/// <summary>ステレオタイプ</summary>
    	public string stereoType { get; set; }
    	
    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }
    	
		public MethodVO()
		{
			changed = ' ';
		}
		
		public int CompareTo( MethodVO o ) {
			return ((this.pos - o.pos) == 0 ? this.name.CompareTo(o.name):(this.pos - o.pos));
		}
    	
	}
}
