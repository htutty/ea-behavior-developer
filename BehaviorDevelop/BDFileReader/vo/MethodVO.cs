/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/11/13
 * Time: 19:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace BDFileReader.vo
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

        //	Public ElementID ' As Long
        /// <summary>操作のid</summary>
        public int elementId { get; set; }

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
    	
		// Public Abstract 'As Boolean
		/// <summary>ステレオタイプ</summary>
    	public Boolean isAbstract { get; set; }

		// Public ClassifierID '    #NEW# As Integer
		/// <summary>分類子のID(戻り値型)</summary>
    	public string classifierID { get; set; }

		// Public Code 'As String
		/// <summary>コード</summary>
    	public string code { get; set; }

		// Public Concurrency 'As String
		/// <summary>並列性</summary>
    	public string concurrency { get; set; }

		// Public IsConst '         #NEW# As Boolean
		/// <summary>定数</summary>
    	public Boolean isConst { get; set; }

		// Public IsLeaf '          #NEW# As Boolean
		/// <summary>リーフ</summary>
    	public Boolean isLeaf { get; set; }

		// Public IsPure          #NEW# As Boolean
		/// <summary>純粋仮想関数（C++）</summary>
    	public Boolean isPure { get; set; }
		// Public IsQuery '         #NEW# As Boolean
		/// <summary>クエリー</summary>
    	public Boolean isQuery { get; set; }

		// Public IsRoot '          #NEW# As Boolean
		/// <summary>操作がrootであることを示すブール値</summary>
    	public Boolean isRoot { get; set; }

		// Public IsStatic 'As Boolean
		/// <summary>Staticか否か</summary>
    	public Boolean isStatic { get; set; }

		// Public ObjectType 'As String
		/// 操作を表すEA上の型ID
    	public string objectType { get; set; }

		// Public ParentID 'As Long
		/// <summary>親の要素ID</summary>
    	public string parentID { get; set; }

		// Public PostConditions '  #NEW# As Collection
		/// <summary>事後条件</summary>
//    	public string postConditions { get; set; }

		// Public PreConditions '   #NEW# As Collection
		/// <summary>事前条件</summary>
//    	public string preConditions { get; set; }

		// Public ReturnIsArray '   #NEW# As Boolean
		/// <summary>戻り値が配列かを示すブール値</summary>
    	public Boolean returnIsArray { get; set; }

		// Public StateFlags '      #NEW# As String
		/// <summary>状態要素の操作に適用される追加情報</summary>
    	public string stateFlags { get; set; }

		// Public StyleEx 'As String
		/// <summary>スタイル(aliasが入っている？)</summary>
    	public string styleEx { get; set; }

		// Public TaggedValuesEx 'As Collection
		/// <summary>メソッドタグ付き値</summary>
    	public List<MethodTagVO> taggedValues { get; set; }

		// Public Throws '          #NEW# As String
		/// <summary>Throws句</summary>
    	public string throws { get; set; }
    	
		/// <summary>メソッドパラメータのコレクション</summary>
    	public List<ParameterVO> parameters { get; set; }
    	
    	public MethodVO srcMethod { get; set; }
    	
    	public MethodVO destMethod { get; set; }

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
		
		// コピーを作成するメソッド
		public MethodVO Clone() {
			return (MethodVO)MemberwiseClone();
		}

        public string getComparableString()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("name = " + name);
            sw.WriteLine("alias= " + alias);
            sw.WriteLine("guid= " + guid);
            sw.WriteLine("methodId= " + methodId);
            sw.WriteLine("elementId= " + elementId);
            sw.WriteLine("notes= " + notes);
            sw.WriteLine("behavior= " + behavior);
            sw.WriteLine("returnType= " + returnType);
            sw.WriteLine("visibility= " + visibility);
            sw.WriteLine("pos= " + pos);
            sw.WriteLine("stereoType= " + stereoType);
            sw.WriteLine("isAbstract= " + isAbstract);
            sw.WriteLine("classifierID= " + classifierID);
            sw.WriteLine("code= " + code);
            sw.WriteLine("concurrency= " + concurrency);
            sw.WriteLine("isConst= " + isConst);
            sw.WriteLine("isLeaf= " + isLeaf);
            sw.WriteLine("isPure= " + isPure);
            sw.WriteLine("isQuery= " + isQuery);
            sw.WriteLine("isRoot= " + isRoot);
            sw.WriteLine("isStatic= " + isStatic);
            sw.WriteLine("objectType= " + objectType);
            sw.WriteLine("parentID= " + parentID);
            sw.WriteLine("returnIsArray= " + returnIsArray);
            sw.WriteLine("stateFlags= " + stateFlags);
            sw.WriteLine("styleEx= " + styleEx);
            sw.WriteLine("throws= " + throws);

            //sw.WriteLine("parameters= " +  parameters  );

            //sw.WriteLine("taggedValues= " + taggedValues );
            return sw.ToString();
        }


        public string getComparedString(MethodVO o)
        {
            StringWriter sw = new StringWriter();

            if (name != o.name)
            {
                sw.WriteLine("name = " + name);
            }

            if (alias != o.alias)
            {
                sw.WriteLine("alias= " + alias);
            }

            if (guid != o.guid)
            {
                sw.WriteLine("guid= " + guid);
            }

            if (methodId != o.methodId)
            {
                sw.WriteLine("methodId= " + methodId);
            }

            if (elementId != o.elementId)
            {
                sw.WriteLine("elementId= " + elementId);
            }

            if (notes != o.notes)
            {
                sw.WriteLine("notes= " + notes);
            }

            if (behavior != o.behavior)
            {
                sw.WriteLine("behavior= " + behavior);
            }

            if (returnType != o.returnType)
            {
                sw.WriteLine("returnType= " + returnType);
            }

            if (visibility != o.visibility)
            {
                sw.WriteLine("visibility= " + visibility);
            }

            if (pos != o.pos)
            {
                sw.WriteLine("pos= " + pos);
            }

            if (stereoType != o.stereoType)
            {
                sw.WriteLine("stereoType= " + stereoType);
            }

            if (isAbstract != o.isAbstract)
            {
                sw.WriteLine("isAbstract= " + isAbstract);
            }

            if (classifierID != o.classifierID)
            {
                sw.WriteLine("classifierID= " + classifierID);
            }

            if (code != o.code)
            {
                sw.WriteLine("code= " + code);
            }

            if (concurrency != o.concurrency)
            {
                sw.WriteLine("concurrency= " + concurrency);
            }

            if (isConst != o.isConst)
            {
                sw.WriteLine("isConst= " + isConst);
            }

            if (isLeaf != o.isLeaf)
            {
                sw.WriteLine("isLeaf= " + isLeaf);
            }

            if (isPure != o.isPure)
            {
                sw.WriteLine("isPure= " + isPure);
            }

            if (isQuery != o.isQuery)
            {
                sw.WriteLine("isQuery= " + isQuery);
            }

            if (isRoot != o.isRoot)
            {
                sw.WriteLine("isRoot= " + isRoot);
            }

            if (isStatic != o.isStatic)
            {
                sw.WriteLine("isStatic= " + isStatic);
            }

            if (objectType != o.objectType)
            {
                sw.WriteLine("objectType= " + objectType);
            }

            if (parentID != o.parentID)
            {
                sw.WriteLine("parentID= " + parentID);
            }

            if (returnIsArray != o.returnIsArray)
            {
                sw.WriteLine("returnIsArray= " + returnIsArray);
            }

            if (stateFlags != o.stateFlags)
            {
                sw.WriteLine("stateFlags= " + stateFlags);
            }

            if (styleEx != o.styleEx)
            {
                sw.WriteLine("styleEx= " + styleEx);
            }

            if (throws != o.throws)
            {
                sw.WriteLine("throws= " + throws);
            }
            
            //sw.WriteLine("parameters= " +  parameters  );

            //sw.WriteLine("taggedValues= " + taggedValues );
            return sw.ToString();
        }


        public void sortParametersGUID()
        {
            if (parameters.Count > 0)
            {
                ParameterGuidComparer comp = new ParameterGuidComparer();
                parameters.Sort(comp);
            }
        }

        public string getParamDesc()
        {
            StringWriter strw = new StringWriter();

            if (this.parameters != null)
            {
                for (var i = 0; i < this.parameters.Count; i++)
                {
                    ParameterVO p = this.parameters[i];
                    if (i > 0) strw.Write(", ");
                    strw.Write(p.eaType);
                }
            }

            return strw.ToString();
        }
    }


	/// <summary>
	/// Description of MethodComparer.
	/// </summary>
	public class MethodIdComparer : IComparer<MethodVO>
	{
		public MethodIdComparer()
		{
		}
		
	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(MethodVO x, MethodVO y)
	    {
            return x.methodId.CompareTo(x.methodId);

	    	// return x.guid.CompareTo(y.guid);
	    }
	}

    /// <summary>
    /// Description of MethodComparer.
    /// </summary>
    public class MethodGuidComparer : IComparer<MethodVO>
    {
        public MethodGuidComparer()
        {
        }

        // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
        public int Compare(MethodVO x, MethodVO y)
        {
            return x.guid.CompareTo(x.guid);
        }
    }


    /// <summary>
    /// Description of MethodComparer.
    /// </summary>
    public class MethodDispComparer : IComparer<MethodVO>
    {
        public MethodDispComparer()
        {
        }

        // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
        public int Compare(MethodVO x, MethodVO y)
        {
            return ((x.pos - y.pos) == 0 ? x.name.CompareTo(y.name) : (x.pos - y.pos));
        }

    }


}
