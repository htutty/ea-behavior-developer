/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/09
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BDFileReader.vo
{
	/// <summary>
	/// Description of ElementSearchVO.
	/// </summary>
	public class ElementSearchVO
	{
		/// <summary>名前</summary>
    	public string elemName { get; set; }
    
		/// <summary>別名</summary>
    	public string elemAlias { get; set; }

		/// <summary>要素のタイプ</summary>
    	public string elemType { get; set; }
    	
		/// <summary>ステレオタイプ</summary>
    	public string elemStereotype { get; set; }

		/// <summary>要素GUID</summary>
    	public string elemGuid { get; set; }

    	/// <summary>成果物GUID </summary>
    	public string artifactGuid { get; set; }

    	/// <summary>成果物名(パッケージ名) </summary>
    	public string artifactName { get; set; }

    	/// <summary>成果物パッケージのパス（"/"区切り） </summary>
    	public string artifactPath { get; set; }

    	public ElementSearchVO()
		{
		}

	}
}
