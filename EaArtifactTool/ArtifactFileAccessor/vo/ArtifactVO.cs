using System;
using System.Collections.Generic;

namespace ArtifactFileAccessor.vo
{
	/// <summary>
	/// Description of ArtifactVO.
	/// </summary>
	public class ArtifactVO : IComparable<ArtifactVO>
	{
        /// <summary>
        /// 名前
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// ステレオタイプ
        /// </summary>
        public string stereoType { get; set; }

        /// <summary>
        /// 成果物ID
        /// </summary>
        public string artifactId { get; set; }

        /// <summary>
        /// パッケージID
        /// </summary>
        public int packageId { get; set; }

        /// <summary>
        /// パス名(パス区切りに"/" を使用し /aaa/bbb 形式で保持)
        /// </summary>
        public string pathName { get; set; }

        /// <summary>
        /// プロジェクト名(ASW_COMMONなど)
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// ノート
        /// </summary>
        public string notes { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public string createDate { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public string updateDate { get; set; }

        /// <summary>
        /// 成果物パッケージ
        /// </summary>
        public PackageVO package { get; set; }

        /// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

		public ArtifactVO()
		{
			changed = ' ';
		}

		public int CompareTo(ArtifactVO o) {
			return (this.guid.CompareTo(o.guid));
		}

        public List<ElementVO> getOwnElements()
        {
            List<ElementVO> retElements = new List<ElementVO>();

            if( this.package != null) {
                retrievePackage(this.package, retElements);
            }

            return retElements;
        }

        public void retrievePackage(PackageVO package, List<ElementVO> elements)
        {
            if( package.elements != null && package.elements.Count > 0)
            {
                elements.AddRange(package.elements);
            }

            foreach ( PackageVO pack in package.childPackageList )
            {
                retrievePackage(pack, elements);
            }
        }

	}
}
