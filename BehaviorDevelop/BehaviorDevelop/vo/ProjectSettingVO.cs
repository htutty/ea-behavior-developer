/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/12
 * Time: 15:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BehaviorDevelop.vo
{
	/// <summary>
	/// Description of ProjectSettingVO.
	/// </summary>
	public class ProjectSettingVO
	{
		
		/// <summary>
        /// 名前
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// dbName
        /// </summary>
        public string dbName { get; set; }
		
        /// <summary>
        /// artifactsFile
        /// </summary>
        public string artifactsFile { get; set; }

        public ProjectSettingVO()
		{
		}
	}
}
