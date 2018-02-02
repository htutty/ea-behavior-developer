/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/10/27
 * Time: 10:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BehaviorDevelop.vo
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
        public string packageId { get; set; }

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
		
	}
}
