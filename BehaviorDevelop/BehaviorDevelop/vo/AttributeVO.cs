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
	public class AttributeVO
	{
		public AttributeVO()
		{
		}

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
 
	}
}
