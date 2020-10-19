using System;
using System.Text;
using System.Collections.Generic;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of PackageVO.
	/// </summary>
	public class PackageVO : IComparable<PackageVO>
	{
        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// パッケージID
        /// </summary>
        public int packageId { get; set; }

        /// <summary>
        /// 親パッケージID
        /// </summary>
        public int parentPackageId { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 別名
        /// </summary>
        public string alias { get; set; }

        /// <summary>
        /// ステレオタイプ
        /// </summary>
        public string stereoType { get; set; }

        /// <summary>
        /// パス名(パス区切りに"/" を使用し /aaa/bbb 形式で保持)
        /// </summary>
        public string pathName { get; set; }

        /// <summary>
        /// ノート
        /// </summary>
        public string notes { get; set; }

        /// <summary>
        /// コントロールパッケージフラグ
        /// </summary>
        public bool isControlled { get; set; }

        /// <summary>
        /// ツリーノード上の並び順
        /// </summary>
        public int treePos { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public string updateDate { get; set; }

        /// <summary>
        /// 子パッケージリスト
        /// </summary>
        public List<PackageVO> childPackageList { get; set; }

        /// <summary>
        /// 要素リスト
        /// </summary>
        public List<ElementVO> elements { get; set; }

        /// <summary>
        /// ダイアグラムリスト
        /// </summary>
        public List<DiagramVO> diagrams { get; set; }

        /// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

        /// <summary>
        /// このパッケージ配下で保持している要素数
        /// （子や孫以降の末端までをすべて含む）
        /// </summary>
        public int elementsCount { get; set; }

        /// <summary>
        /// このパッケージ配下で保持している要素のうち変更のあったものの数
        /// （子や孫以降の末端までをすべて含む）
        /// </summary>
        public int changedElementsCount { get; set; }


        /// <summary>
        /// このパッケージ配下で保持しているダイアグラム数
        /// </summary>
        public int diagramsCount { get; set; }


        public PackageVO()
		{
        	changed = ' ';

            this.childPackageList = new List<PackageVO>();
            this.elements = new List<ElementVO>();
            this.diagrams = new List<DiagramVO>();
        }

        public PackageVO(PackageVO orig)
        {
            this.alias = orig.alias;
            this.changed = orig.changed;
            this.guid = orig.guid;
            this.isControlled = orig.isControlled;
            this.name = orig.name;
            this.notes = orig.notes;
            this.packageId = orig.packageId;
            this.parentPackageId = orig.parentPackageId;
            this.pathName = orig.pathName;
            this.stereoType = orig.stereoType;
            this.treePos = orig.treePos;
            this.updateDate = orig.updateDate;

            this.childPackageList = new List<PackageVO>();
            this.elements = new List<ElementVO>();
            this.diagrams = new List<DiagramVO>();
        }


        public int CompareTo( PackageVO o ) {
			return ((this.treePos - o.treePos) == 0 ? this.name.CompareTo(o.name):(this.treePos - o.treePos));
		}

        public void sortChildNodes()
        {

            for (int i = 0; i < elements.Count; i++)
            {
                ElementVO elm = elements[i];
                elm.sortChildNodes();
            }
            elements.Sort();

            for (int i = 0; i < childPackageList.Count; i++)
            {
                PackageVO pkg = childPackageList[i];
                pkg.sortChildNodes();
            }
            childPackageList.Sort();
        }


        public void sortChildNodesGuid()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ElementVO elm = elements[i];
                elm.sortChildNodesGuid();
            }
            ElementGuidComparer ecmp = new ElementGuidComparer();
            elements.Sort(ecmp);

            foreach (PackageVO pkg in childPackageList)
            {
                pkg.sortChildNodesGuid();
            }
            PackageGuidComparer pcmp = new PackageGuidComparer();
            childPackageList.Sort(pcmp);

        }

        public string toDescriptorString() {
			StringBuilder sb = new StringBuilder();

			sb.Append("Package " + this.name + " {" + "\r\n");

			foreach(PackageVO packvo in this.childPackageList) {
				sb.Append(packvo.toDescriptorString());
			}

			foreach(ElementVO elemvo in this.elements) {
				sb.Append(elemvo.toDescriptorString());
			}

			sb.Append("}" + "\r\n");

			return sb.ToString();
   		}
	}


	/// <summary>
	/// Description of PackageComparer.
	/// </summary>
	public class PackageGuidComparer : IComparer<PackageVO>
	{
		public PackageGuidComparer()
		{
		}

	    // xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
	    public int Compare(PackageVO x, PackageVO y)
	    {
	    	return x.guid.CompareTo(y.guid);
	    }
	}

}
