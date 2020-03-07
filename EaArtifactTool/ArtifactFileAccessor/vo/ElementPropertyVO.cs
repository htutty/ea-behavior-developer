using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of ElementVO.
	/// </summary>
	public class ElementPropertyVO 
	{
		/// <summary>名前</summary>
    	public string name { get; set; }

		/// <summary>別名</summary>
    	public string alias { get; set; }

		/// <summary>作成者名</summary>
    	public string author { get; set; }

		/// <summary>ステレオタイプ</summary>
    	public string stereoType { get; set; }

		/// <summary>作成日時</summary>
    	public DateTime created { get; set; }

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

		/// <summary>表示順</summary>
    	public int treePos { get; set; }

		/// <summary>要素のタイプ（Class, Interface など）</summary>
    	public string eaType { get; set; }

		/// <summary>バージョン</summary>
    	public string version { get; set; }

		/// <summary>可視性</summary>
    	public string visibility { get; set; }

    	/// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

   		public ElementPropertyVO()
		{
   			changed = ' ';
		}


        public ElementPropertyVO(ElementVO elem)
        {
            this.name = elem.name;
            this.alias = elem.alias;
            this.guid = elem.guid;
            this.elementId = elem.elementId;
            this.notes = elem.notes;
            this.stereoType = elem.stereoType;
            this.objectType = elem.objectType;
            this.author = elem.author;
            this.created = elem.created;
            this.modified = elem.modified;
            this.objectType = elem.objectType;
            this.packageID = elem.packageId;
            this.tag = elem.tag;
            this.treePos = elem.treePos;
            this.eaType = elem.eaType;
            this.version = elem.version;
            this.visibility = elem.visibility;

            changed = ' ';
        }


        public string getComparableString()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("name = " + name);
            sw.WriteLine("alias = " + alias);
            sw.WriteLine("notes = " + notes);
            sw.WriteLine("stereoType = " + stereoType);
            sw.WriteLine("objectType = " + objectType);
            sw.WriteLine("author = " + author);
            sw.WriteLine("created = " + created);
            sw.WriteLine("modified = " + modified);
            sw.WriteLine("objectType = " + objectType);
            sw.WriteLine("packageID = " + packageID);
            sw.WriteLine("tag = " + tag);
            sw.WriteLine("treePos = " + treePos);
            sw.WriteLine("eaType = " + eaType);
            sw.WriteLine("version = " + version);
            sw.WriteLine("visibility = " + visibility);
            return sw.ToString();
        }



        /// <summary>
        /// 引数のオブジェクトと自身の値を比較し、差異のあった項目のみで"name = value"の文字列を返却する
        /// </summary>
        /// <param name="o">比較先の属性オブジェクト</param>
        /// <returns></returns>
        public string getComparedString(ElementPropertyVO o)
        {
            StringWriter sw = new StringWriter();

            if (compareNullable(name,o.name))
            {
                sw.WriteLine("name = " + name);
            }

            if (compareNullable(alias, o.alias))
            {
                sw.WriteLine("alias = " + alias);
            }

            if (compareNullable(notes, o.notes))
            {
                sw.WriteLine("notes = " + notes);
            }

            if (compareNullable(stereoType, o.stereoType))
            {
                sw.WriteLine("stereoType = " + stereoType);
            }

            if (compareNullable(objectType, o.objectType))
            {
                sw.WriteLine("objectType = " + objectType);
            }

            if (compareNullable(author, o.author))
            {
                sw.WriteLine("author = " + author);
            }

            if (created != o.created)
            {
                sw.WriteLine("created = " + created);
            }

            if (modified != o.modified)
            {
                sw.WriteLine("modified = " + modified);
            }

            if (compareNullable(objectType, o.objectType))
            {
                sw.WriteLine("objectType = " + objectType);
            }

            if (compareNullable(tag, o.tag))
            {
                sw.WriteLine("tag = " + tag);
            }

            if (treePos != o.treePos)
            {
                sw.WriteLine("treePos = " + treePos);
            }

            if (compareNullable(eaType, o.eaType))
            {
                sw.WriteLine("eaType = " + eaType);
            }

            if (compareNullable(version, o.version))
            {
                sw.WriteLine("version = " + version);
            }

            if (compareNullable(visibility, o.visibility))
            {
                sw.WriteLine("visibility = " + visibility);
            }

            return sw.ToString();
        }


        private static bool compareNullable(string src, string dest)
        {
            if( src == null )
            {
                if(dest == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (dest == null)
                {
                    return true;
                }
                else
                {
                    return (src!=dest);
                }
            }
        }


    }

}
