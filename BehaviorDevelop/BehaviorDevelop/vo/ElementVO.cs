/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/05
 * Time: 13:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BehaviorDevelop.vo
{
	/// <summary>
	/// Description of ElementVO.
	/// </summary>
	public class ElementVO
	{
		public ElementVO()
		{
		}

		// 	Public Name ' As String
		/// <summary>名前</summary>
    	public string name { get; set; }
    
		//	Public Alias ' As String
		/// <summary>別名</summary>
    	public string alias { get; set; }

		//	Public Attributes ' As Collection
		/// <summary>属性コレクション</summary>
    	public IList<AttributeVO> attributes { get; set; }

		//	Public Author ' As String
		/// <summary>作成者名</summary>
    	public string author { get; set; }
    	
		//	Public StereoTypeEx ' As String
		/// <summary>ステレオタイプのコレクション</summary>
    	public string stereoTypeEx { get; set; }

		// <summary>接続コレクション</summary>
    	public IList<ConnectorVO> connectors { get; set; }

    	//	Public Created ' As Date
		/// <summary>作成日時</summary>
    	public DateTime created { get; set; }
    	
		//	Public Diagrams ' As Collection
		// <summary>ダイアグラムのコレクション</summary>
    	// public IList<ElementVO> diagrams { get; set; }
    	
		//	Public ElementGUID ' As String
		/// <summary>guid</summary>
    	public string guid { get; set; }
		
		//	Public ElementID ' As Long
		/// <summary>要素のid</summary>
    	public int elementId { get; set; }

    	//	Public Elements ' As Collection
    	/// <summary>子要素のコレクション</summary>
    	public IList<ElementVO> elements { get; set; }

		//	Public Methods ' As Collection
    	/// <summary>操作コレクション</summary>
    	public IList<MethodVO> methods { get; set; }
    	
		//	Public Modified ' As Date
		/// <summary>最終更新日時</summary>
    	public DateTime modified { get; set; }

		//	Public Notes ' As String
		/// <summary>ノート</summary>
    	public string notes { get; set; }
    	
		//	Public ObjectType ' As String
		/// <summary>要素の種類</summary>
    	public string objectType { get; set; }
    	
		//	Public PackageID ' As Long
		/// <summary>所属パッケージID</summary>
    	public int packageID { get; set; }

		//	Public GenType ' As String
		/// <summary>生成言語タイプ</summary>
    	public string genType { get; set; }
    	
		//	Public GenFile ' As String
		/// <summary>生成ソースファイル</summary>
    	public string genFile { get; set; }
    	
		//	Public Tag ' As String
		/// <summary>タグ項目(タグ付き値ではない)</summary>
    	public string tag { get; set; }
    	
		//	Public TaggedValuesEx ' As Collection
		/// <summary>タグ項目(タグ付き値ではない)</summary>
    	public IList<ElementVO> taggedValues { get; set; }
		
		//	Public TreePos ' As Long
		/// <summary>表示順</summary>
    	public int treePos { get; set; }
					
		//	Public EA_Type ' As String
		/// <summary>表示順</summary>
    	public string eaType { get; set; }
    	
		//	Public Version ' As String
		/// <summary>バージョン</summary>
    	public string version { get; set; }
		
		//	Public Visibility ' As String
		/// <summary>可視性</summary>
    	public string visibility { get; set; }

		//	Public TransformToClass ' As String
		/// <summary>変換先クラス名（FQCN）</summary>
    	public string transformToClass { get; set; }
    	
		//	Public Properties ' As String
		// <summary>要素のプロパティ</summary>
    	// public IList<PropertyVO> properties { get; set; }
		
		//	Public Priority ' As Long
		// <summary>優先度（要求要素などだけがもつ特殊プロパティ）</summary>
    	// public string Priority { get; set; }

		//	Public ParentID ' As Long
		// <summary>親要素ID（親の場合は0）</summary>
    	// public int parentID { get; set; }
        	
	}
}
