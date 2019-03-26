/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/12/09
 * Time: 12:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BDFileReader.vo
{
	/// <summary>
	/// Description of ConnectorVO.
	/// </summary>
	public class ConnectorVO
	{
		// 	Public Name ' As String
		/// <summary>名前</summary>
    	public string name { get; set; }
    	
    	//	Public AttributeGUID ' As String
		/// <summary>接続のguid</summary>
    	public string guid { get; set; }

        /// <summary> 接続のユニークID </summary>
        public int connectorId { get; set; }
        
        /// <summary>接続の種類 (Dependency, Association, etc)</summary>
        public string connectorType { get; set; }	
    	
		/// <summary>接続相手の要素guid</summary>
    	public string targetObjGuid { get; set; }	

		/// <summary>接続相手の要素名</summary>
    	public string targetObjName { get; set; }

        /// <summary>接続元要素のObjectId</summary>
        public int srcObjId { get; set; }

        /// <summary>接続元要素のguid</summary>
        public string srcObjGuid { get; set; }	

		/// <summary>接続元要素の名前</summary>
    	public string srcObjName { get; set; }

        /// <summary>接続先要素のObjectId</summary>
        public int destObjId { get; set; }

        /// <summary>接続先要素のguid</summary>
        public string destObjGuid { get; set; }	

		/// <summary>接続先要素の名前</summary>
    	public string destObjName { get; set; }	
    	
     	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }
        
    	public ConnectorVO()
		{
        	changed = ' ';
		}

    	// コピーを作成するメソッド
		public ConnectorVO Clone() {
			return (ConnectorVO)MemberwiseClone();
		}
	}
}
