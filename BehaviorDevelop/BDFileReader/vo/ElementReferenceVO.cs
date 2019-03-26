/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/05/16
 * Time: 11:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BDFileReader.vo
{
	/// <summary>
	/// Description of ElementReferenceVO.
	/// </summary>
	public class ElementReferenceVO
	{
		/// <summary>実装ソースファイル（フルパス）</summary>
    	public string genfile { get; set; }
    
		/// <summary>パッケージ込みのクラス名</summary>
    	public string fqcn { get; set; }

    	/// <summary>生成言語</summary>
    	public string gentype { get; set; }

    	
		public ElementReferenceVO()
		{
		}
	}
}
