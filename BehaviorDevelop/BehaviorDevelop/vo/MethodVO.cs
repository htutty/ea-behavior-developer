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
	public class MethodVO
	{
		public MethodVO()
		{	
		}
		
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
	}
}
