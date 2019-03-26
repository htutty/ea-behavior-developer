/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/05
 * Time: 13:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace BDFileReader.vo
{
	/// <summary>
	/// Description of ElementVO.
	/// </summary>
	public class ElementVO : IComparable<ElementVO>
	{
		/// <summary>名前</summary>
    	public string name { get; set; }
    
		/// <summary>別名</summary>
    	public string alias { get; set; }

		/// <summary>属性コレクション</summary>
    	public List<AttributeVO> attributes { get; set; }

		/// <summary>作成者名</summary>
    	public string author { get; set; }
    	
		/// <summary>ステレオタイプのコレクション</summary>
    	public string stereoType { get; set; }

		// <summary>接続コレクション</summary>
    	public List<ConnectorVO> connectors { get; set; }

		/// <summary>作成日時</summary>
    	public DateTime created { get; set; }
    	
		// <summary>ダイアグラムのコレクション</summary>
    	// public IList<ElementVO> diagrams { get; set; }
    	
		/// <summary>guid</summary>
    	public string guid { get; set; }
		
		/// <summary>要素のid</summary>
    	public int elementId { get; set; }

    	/// <summary>子要素のコレクション</summary>
    	public IList<ElementVO> elements { get; set; }

    	/// <summary>操作コレクション</summary>
    	public List<MethodVO> methods { get; set; }
    	
		/// <summary>最終更新日時</summary>
    	public DateTime modified { get; set; }

		/// <summary>ノート</summary>
    	public string notes { get; set; }
    	
		/// <summary>要素の種類</summary>
    	public string objectType { get; set; }
    	
		/// <summary>所属パッケージID</summary>
    	public int packageID { get; set; }

		/// <summary>生成言語タイプ</summary>
    	public string genType { get; set; }
    	
		/// <summary>生成ソースファイル</summary>
    	public string genFile { get; set; }
    	
		/// <summary>タグ項目(タグ付き値ではない)</summary>
    	public string tag { get; set; }
    	
		/// <summary>タグ付き値</summary>
    	public List<TaggedValueVO> taggedValues { get; set; }
		
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

    	/// <summary> 要素の参照ファイル（このクラスに対応する実装コード）の情報 </summary>
    	public ElementReferenceVO elementReferenceVO { get; set; }

    	
		//	Public Properties ' As String
		// <summary>要素のプロパティ</summary>
    	// public IList<PropertyVO> properties { get; set; }
		
		//	Public Priority ' As Long
		// <summary>優先度（要求要素などだけがもつ特殊プロパティ）</summary>
    	// public string Priority { get; set; }

		//	Public ParentID ' As Long
		// <summary>親要素ID（親の場合は0）</summary>
    	public int parentID { get; set; }

		/// <summary>差異のあった属性のコレクション</summary>
    	//public List<AttributeVO> margedAttributes { get; set; }

    	/// <summary>差異のあった操作コレクション</summary>
    	//public List<MethodVO> margedMethods { get; set; }

    	
    	
    	
    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }
    	
   		public ElementVO()
		{
   			changed = ' ';
		}
   		
		public int CompareTo( ElementVO o ) {
			return ((this.treePos - o.treePos) == 0 ? this.name.CompareTo(o.name):(this.treePos - o.treePos));
		}
   		
   		public void sortMethods() {
   			methods.Sort();
   		}
   		
        public void sortMethodsGUID() {
        	if (methods.Count > 0 ) {
        		MethodGuidComparer comp = new MethodGuidComparer();
	        	methods.Sort(comp);
        	}
        }
        
   		public void sortAttributes() {
   			attributes.Sort();
   		}

		public void sortAttributesGUID() {
        	if (attributes.Count > 0 ) {
        		AttributeComparer comp = new AttributeComparer();
	        	attributes.Sort(comp);
        	}
        }
        
   		public void sortTaggedValues() {
   			taggedValues.Sort();
   		}

   		public void sortTaggedValuesGUID() {
   			if (taggedValues.Count > 0 ) {
        		TaggedValueComparer comp = new TaggedValueComparer();
	        	taggedValues.Sort(comp);
        	}
   		}

   		
   		public string toDescriptorString() {
			StringBuilder sb = new StringBuilder();
			
			sb.Append("Class " + this.name + " {" + "\r\n");
			
			foreach(TaggedValueVO tagvo in this.taggedValues) {
				sb.Append("  [TaggedValue] " + tagvo.name + " = " + tagvo.tagValue + " ;" + "\r\n"); 
			}
			
			foreach(AttributeVO attrvo in this.attributes) {
				sb.Append("  [Attribute] " + attrvo.name + " ;" + "\r\n"); 
			}

			foreach(ConnectorVO convo in this.connectors) {
				sb.Append("  [Connector " + convo.connectorType + "] " + convo.targetObjName + ": " + convo.targetObjGuid  + ";" + "\r\n"); 
			}

			foreach(MethodVO mthvo in this.methods) {
				sb.Append("\r\n");
				sb.Append("  [Method] " + mthvo.name + " {" + "\r\n");

				if (mthvo.behavior != null) {
					string[] ary = mthvo.behavior.Split('\n');
					for(Int16 i = 0; i < ary.Length; i++ ){
						sb.Append( "    " + ary[i] + "\n");
					}
				}
				sb.Append("  }\r\n");
			}
			sb.Append("}" + "\r\n");
			
			return sb.ToString();
   		}
   		
		// コピーを作成するメソッド
		public ElementVO Clone() {
			ElementVO cloned = (ElementVO)MemberwiseClone();
			
			List<TaggedValueVO> retTaggedValues = new List<TaggedValueVO>();
			List<AttributeVO> retAttributes = new List<AttributeVO>();
			List<MethodVO> retMethods = new List<MethodVO>();
			List<ConnectorVO> retConnectors = new List<ConnectorVO>();
			
			if (this.taggedValues != null) {
				foreach(TaggedValueVO tagvo in this.taggedValues) {
					retTaggedValues.Add(tagvo.Clone());
				}
			}
			
			if (this.attributes != null) {
				foreach(AttributeVO attrvo in this.attributes) {
					retAttributes.Add(attrvo.Clone());
				}
			}
			
			if (this.methods != null) {
				foreach(MethodVO mthvo in this.methods) {
					retMethods.Add(mthvo.Clone());
				}
			}

			if (  this.connectors != null ) {
				foreach(ConnectorVO convo in this.connectors) {
					retConnectors.Add(convo.Clone());
				}
			}

			cloned.taggedValues = retTaggedValues;
			cloned.attributes = retAttributes;
			cloned.methods = retMethods;
			cloned.connectors = retConnectors;
			
			return cloned;
		}
   		
	}

	
	/// <summary>
	/// Description of ElementComparer.
	/// </summary>
	public class ElementComparer : IComparer<ElementVO>
	{
		public ElementComparer()
		{
		}
		
	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(ElementVO x, ElementVO y)
	    {
	    	return x.guid.CompareTo(y.guid);
	    }
	}

}
