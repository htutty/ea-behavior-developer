using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of AttributeVO.
	/// </summary>
	public class AttributeVO : AbstractValueObject, IComparable<AttributeVO>
	{
		/// <summary>名前</summary>
    	public string name { get; set; }

		/// <summary>別名</summary>
    	public string alias { get; set; }

		/// <summary>属性のguid</summary>
    	public string guid { get; set; }

		/// <summary>属性のid</summary>
    	public int attributeId { get; set; }

        /// <summary>親要素のid</summary>
        public int elementId { get; set; }

        /// <summary>ノート</summary>
        public string notes { get; set; }

    	/// <summary>並び順</summary>
    	public Int32 pos { get; set; }

		/// <summary>ステレオタイプ</summary>
    	public string stereoType { get; set; }

		/// <summary>
		/// コレクション内で重複を許すかどうかを表す。
		/// </summary>
    	public Boolean allowDuplicates { get; set; }

		/// <summary>
		/// 分類子のID
		/// </summary>
    	public string classifierID { get; set; }

		/// <summary>
		/// コンテナの型
		/// </summary>
    	public string container { get; set; }

		/// <summary>
		/// 属性の保持方法を指定する。
		/// </summary>
    	public string containment { get; set; }

    	//	Public Constraints '  #NEW# As Collection
		//    	public List<Constraint> Constraints { get; set; }

		/// <summary>
		/// 属性のデフォルト値
		/// </summary>
    	public string defaultValue { get; set; }

		/// <summary>
		/// この属性がコレクションであるかを表すブール値
		/// </summary>
    	public Boolean isCollection { get; set; }

		/// <summary>
		/// この属性が定数であるかを表すブール値
		/// </summary>
    	public Boolean isConst { get; set; }

		/// <summary>
		/// この属性が派生値(他の属性から導出できる値)であるかどうかを表すブール値
		/// </summary>
    	public Boolean isDerived { get; set; }

		/// <summary>
		/// この属性がIDであるかを表すブール値
		/// </summary>
    	public Boolean isID { get; set; }

		/// <summary>
		/// この属性がコレクションの場合、コレクションに順序を持つかを表すブール値
		/// </summary>
    	public Boolean isOrdered { get; set; }

		/// <summary>
		/// この属性がStaticかを表すブール値
		/// </summary>
    	public Boolean isStatic { get; set; }

		/// <summary>
		/// 数値、文字列の長さ（RDBとの接続機能におけるTABLE型でのみ使用する）
		/// </summary>
    	public Int32 length { get; set; }

		/// <summary>
		/// 添え字の下限
		/// </summary>
    	public Int32 lowerBound { get; set; }

		/// <summary>
		/// 属性を表すEA上の型ID
		/// </summary>
    	public string objectType { get; set; }

		/// <summary>
		/// 親となる要素のID
		/// </summary>
    	public string parentID { get; set; }

		/// <summary>
		/// 精度（小数点以下桁数）（RDBとの接続機能におけるTABLE型でのみ使用する）
		/// </summary>
    	public Int32 precision { get; set; }

		/// <summary>
		/// 「再定義されたプロパティ」の内容
		/// </summary>
    	public string redefinedProperty { get; set; }

		/// <summary>
		/// 小数点より上の桁数（RDBとの接続機能におけるTABLE型でのみ使用する）
		/// </summary>
    	public Int32 scale { get; set; }

		/// <summary>
		/// DB上のStyle値（あまり使う機会は無いはずだが、、）
		/// </summary>
    	public string styleEx { get; set; }

		/// <summary>
		/// 属性のタグ付き値
		/// </summary>
		public List<TaggedValueVO> taggedValues { get; set; }

		/// <summary>
		/// 型名称
		/// </summary>
    	public string eaType { get; set; }

		/// <summary>
		/// 添え字の上限
		/// </summary>
    	public int upperBound { get; set; }

		/// <summary>
		/// 可視性
		/// </summary>
    	public string visibility { get; set; }

    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

    	public AttributeVO srcAttribute { get; set; }

    	public AttributeVO destAttribute { get; set; }

        public AttributeVO()
		{
        	changed = ' ';
		}

		public int CompareTo( AttributeVO o ) {
			return ((this.pos - o.pos) == 0 ? this.name.CompareTo(o.name):(this.pos - o.pos));
		}

		// コピーを作成するメソッド
		public AttributeVO Clone() {
			return (AttributeVO)MemberwiseClone();
		}

        /// <summary>
        /// JavaのtoString()と同様、自身の項目値を全てつなげた文字列を生成して返却する
        /// </summary>
        /// <returns>自身の項目値を全てつなげた文字列</returns>
        public string getComparableString()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("name = " + name);
            sw.WriteLine("alias = " + alias);
            sw.WriteLine("guid = " + guid);
            sw.WriteLine("attributeId = " + attributeId );
            sw.WriteLine("elementId = " + elementId );
            sw.WriteLine("notes = " + notes );
            sw.WriteLine("pos = " + pos );
            sw.WriteLine("stereoType = " + stereoType );
            sw.WriteLine("allowDuplicates = " + allowDuplicates);
            sw.WriteLine("classifierID = " + classifierID );
            sw.WriteLine("container = " + container );
            sw.WriteLine("containment = " + containment );
            sw.WriteLine("defaultValue = " + defaultValue );
            sw.WriteLine("isCollection = " + isCollection );
            sw.WriteLine("isConst = " + isConst );
            sw.WriteLine("isDerived = " + isDerived );
            sw.WriteLine("isID = " + isID );
            sw.WriteLine("isOrdered = " + isOrdered );
            sw.WriteLine("isStatic = " + isStatic );
            sw.WriteLine("length = " + length );
            sw.WriteLine("lowerBound = " + lowerBound );
            sw.WriteLine("objectType = " + objectType );
            sw.WriteLine("parentID = " + parentID );
            sw.WriteLine("precision = " + precision );
            sw.WriteLine("redefinedProperty = " + redefinedProperty );
            sw.WriteLine("scale = " + scale );
            sw.WriteLine("styleEx = " + styleEx );
            sw.WriteLine("eaType = " + eaType );
            sw.WriteLine("upperBound = " + upperBound );
            sw.WriteLine("visibility = " + visibility );

            if (taggedValues != null && taggedValues.Count > 0)
            {
                sw.WriteLine("taggedValues=[");
                foreach (var tv in taggedValues)
                {
                    sw.WriteLine("tv=" + tv.getComparableString() + ",");
                }
                sw.WriteLine("]");
            }

            return sw.ToString();
        }

        /// <summary>
        /// 引数のオブジェクトと自身の値を比較し、差異のあった項目のみで"name = value"の文字列を返却する
        /// </summary>
        /// <param name="o">比較先の属性オブジェクト</param>
        /// <returns></returns>
        public string getComparedString(AttributeVO o)
        {
            StringWriter sw = new StringWriter();

            if (name != o.name)
            {
                sw.WriteLine("name = " + name);
            }

            if (alias != o.alias)
            {
                sw.WriteLine("alias = " + alias);
            }

            if (guid != o.guid)
            {
                sw.WriteLine("guid = " + guid);
            }

            if (attributeId != o.attributeId)
            {
                sw.WriteLine("attributeId = " + attributeId);
            }

            if (elementId != o.elementId)
            {
               sw.WriteLine("elementId = " + elementId);
            }

            if (notes != o.notes)
            {
                sw.WriteLine("notes = " + notes);
            }

            if (pos != o.pos)
            {
                sw.WriteLine("pos = " + pos);
            }

            if (stereoType != o.stereoType)
            {
                sw.WriteLine("stereoType = " + stereoType);
            }

            if (allowDuplicates != o.allowDuplicates)
            {
                sw.WriteLine("allowDuplicates = " + allowDuplicates);
            }

            if (classifierID != o.classifierID)
            {
                sw.WriteLine("classifierID = " + classifierID);
            }

            if (container != o.container)
            {
                sw.WriteLine("container = " + container);
            }

            if (containment != o.containment)
            {
                sw.WriteLine("containment = " + containment);
            }

            if (defaultValue != o.defaultValue)
            {
                sw.WriteLine("defaultValue = " + defaultValue);
            }

            if (isCollection != o.isCollection)
            {
                sw.WriteLine("isCollection = " + isCollection);
            }

            if (isConst != o.isConst)
            {
                sw.WriteLine("isConst = " + isConst);
            }

            if (isDerived != o.isDerived)
            {
                sw.WriteLine("isDerived = " + isDerived);
            }

            if (isID != o.isID)
            {
                sw.WriteLine("isID = " + isID);
            }

            if (isOrdered != o.isOrdered)
            {
                sw.WriteLine("isOrdered = " + isOrdered);
            }

            if (isStatic != o.isStatic)
            {
                sw.WriteLine("isStatic = " + isStatic);
            }

            if (length != o.length)
            {
                sw.WriteLine("length = " + length);
            }

            if (lowerBound != o.lowerBound)
            {
                sw.WriteLine("lowerBound = " + lowerBound);
            }

            if (objectType != o.objectType)
            {
                sw.WriteLine("objectType = " + objectType);
            }

            if (parentID != o.parentID)
            {
                sw.WriteLine("parentID = " + parentID);
            }

            if (precision != o.precision)
            {
                sw.WriteLine("precision = " + precision);
            }

            if (redefinedProperty != o.redefinedProperty)
            {
                sw.WriteLine("redefinedProperty = " + redefinedProperty);
            }

            if (scale != o.scale)
            {
                sw.WriteLine("scale = " + scale);
            }

            if (styleEx != o.styleEx)
            {
                sw.WriteLine("styleEx = " + styleEx);
            }

            if (eaType != o.eaType)
            {
                sw.WriteLine("eaType = " + eaType);
            }

            if (upperBound != o.upperBound)
            {
                sw.WriteLine("upperBound = " + upperBound);
            }

            if (visibility != o.visibility)
            {
                sw.WriteLine("visibility = " + visibility);
            }

            sw.Write("taggedValues=[");
            foreach (var tv in taggedValues)
            {
                //                tv.getComparedString();
            }
            sw.WriteLine("]");

            return sw.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override public string generateDeclareString(int indentLv)
        {
            StringWriter sw = new StringWriter();
            string staticFinalStr;
            string defaultStr;
            string stereotypeStr = "";
            if (this.stereoType != null && this.stereoType != "")
            {
                stereotypeStr = this.stereoType;
            }

            staticFinalStr = (isStatic ? "static" : "");
            if (this.isConst)
            {
                staticFinalStr = staticFinalStr + (staticFinalStr != "" ? " " : "");
                staticFinalStr = staticFinalStr + "final";
            }
            staticFinalStr = staticFinalStr + (staticFinalStr != "" ? " " : "");

            if (this.defaultValue != null && this.defaultValue != "")
            {
                defaultStr = " = " + this.defaultValue;
            }
            else
            {
                defaultStr = "";
            }


            sw.WriteLine(getIndentStr(indentLv) + "///<summary>");
            genCommentized(notes, "///", indentLv);
            sw.WriteLine(getIndentStr(indentLv) + "///</summary>");

            if( stereotypeStr != "" )
            {
                sw.WriteLine("//@stereotype:" + stereotypeStr);
            }
            sw.WriteLine(getIndentStr(indentLv) + this.visibility + " " + staticFinalStr + this.eaType +
                          " " + this.name + defaultStr + ";");

            return sw.ToString();
        }


        override public void sortChildNodes()
        {
            taggedValues.Sort();
        }

        override public void sortChildNodesGuid()
        {
            TaggedValueGuidComparer cmp = new TaggedValueGuidComparer();
            taggedValues.Sort(cmp);
        }

    }


    /// <summary>
    /// Description of AttributeComparer.
    /// </summary>
    public class AttributeGuidComparer : IComparer<AttributeVO>
	{
		public AttributeGuidComparer()
		{
		}

	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(AttributeVO x, AttributeVO y)
	    {
	    	return x.guid.CompareTo(y.guid);
	    }
	}

    /// <summary>
    /// Description of MethodComparer.
    /// </summary>
    public class AttributeIdComparer : IComparer<AttributeVO>
    {
        public AttributeIdComparer()
        {
        }

        // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
        public int Compare(AttributeVO x, AttributeVO y)
        {
            return x.attributeId.CompareTo(x.attributeId);

            // return x.guid.CompareTo(y.guid);
        }
    }



}
