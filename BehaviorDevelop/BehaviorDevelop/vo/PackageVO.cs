/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/10/27
 * Time: 10:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BehaviorDevelop.vo
{
	/// <summary>
	/// Description of PackageVO.
	/// </summary>
	public class PackageVO
	{
		public PackageVO()
		{
		}

        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// パッケージID
        /// </summary>
        public string packageId { get; set; }
        
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
        /// 更新日時
        /// </summary>
        public string updateDate { get; set; }
        
        /// <summary>
        /// 子パッケージリスト
        /// </summary>
        public IList<PackageVO> childPackageList { get; set; }
        
        /// <summary>
        /// 要素リスト
        /// </summary>
        public IList<ElementVO> elements { get; set; }
	}
}
