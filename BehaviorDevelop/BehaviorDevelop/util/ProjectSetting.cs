/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/12
 * Time: 14:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;
using BehaviorDevelop.vo;

namespace BehaviorDevelop.vo
{
	/// <summary>
	/// Description of ProjectSetting.
	/// </summary>
	public class ProjectSetting
	{
		static ProjectSettingVO vo = null;	

		public ProjectSetting()
		{
		}
		
		/// <summary>
        /// All_Artifacts.xml を読み、リスト化する
        /// 
        /// XML例：
        /// <artifacts  targetProject='Logical'  lastUpdated='2017/10/13 10:27:32'  targetModel='Logical'  >
        ///   <artifact  guid='{11EF4332-5CB7-4ecd-8E78-0E50A6E7D3E7}'  name='共通設計モデル'  path='/論理モデル/レイヤ別ビュー/フレームワーク_STEP3移管対象/'  stereotype='fw.adesk_cmn' />
        /// </artifacts>
        /// 
        /// </summary>
        /// <returns>ArtifactVOのリスト</returns>
		public static Boolean load(string project_file) {
            // XMLテキストをロードする
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( project_file );

            // projectノードに移動する
            XmlNode projectNode = xmlDoc.SelectSingleNode("/project");

            ProjectSettingVO settingvo = new ProjectSettingVO();
            if( projectNode != null ) {
	            foreach (XmlNode settingNode in projectNode.ChildNodes) {
	            	switch( settingNode.Name ) {
	            		case "projectName":
            				settingvo.projectName = settingNode.InnerText;
	            			break;
	            			
	            		case "dbName":
            				settingvo.dbName = settingNode.InnerText;
            				break;

	            		case "artifactsFile":
            				settingvo.artifactsFile = settingNode.InnerText;
            				break;
            				
	            		default:
	            			// do nothing
            				break;
	            	}
	            }
            } 
            
            // dbファイル名が取得できたら、プロファイルディレクトリを追加
            if (settingvo.dbName != null) {
            	settingvo.dbName = GetAppProfileDir() + settingvo.dbName ;
            }
            
            vo = settingvo;
            return true;
		}
		
    	public static ProjectSettingVO getVO( ) {
       		return vo;
        }
    	
        
        public static string GetAppProfileDir() {
        	return System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\.bd\\";
        }
        
	}
}
