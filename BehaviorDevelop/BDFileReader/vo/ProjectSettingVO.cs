/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/12
 * Time: 15:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BDFileReader.vo
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

        /// <summary>
        /// projectPath
        /// </summary>
        public string projectPath { get; set; }

        /// <summary>
        /// allConnectorFile
        /// </summary>
        public string allConnectorFile { get; set; }

        /// <summary>
        /// 成果物XMLファイルの接頭辞 .
        ///  "atf_" + artifact-package-guid(36bytes) + ".xml" というファイル名になる。
        /// </summary>
        public string artifactFilePrefix { get; set; }

        /// <summary>
        /// 成果物XMLファイルの接尾辞 .
        ///  "atf_" + artifact-package-guid(36bytes) + ".xml" というファイル名になる。
        /// </summary>
        public string artifactFileSuffix { get; set; }

        /// <summary>
        /// EAリポジトリオブジェクト
        /// </summary>
        public EA.Repository eaRepo { get; set; }

        /// <summary>
        /// 成果物パス
        /// </summary>
        public string artifactsPath { get; set; }


        /// <summary>
        /// コンストラクタ .
        /// 一部のプロパティについては初期値をセットする。
        /// </summary>
        public ProjectSettingVO()
        {
            this.artifactFilePrefix = "atf_";
            this.artifactFileSuffix = ".xml";
            this.allConnectorFile = "AllConnectors.xml";
		}
	}
}
