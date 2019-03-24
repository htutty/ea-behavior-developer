using System;

namespace ArtifactFileAccessor.vo
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
