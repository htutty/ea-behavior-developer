/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/09
 * Time: 20:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using IndexAccessor;
using ArtifactFileAccessor.vo;

namespace BehaviorDevelop
{
	/// <summary>
	/// Description of SearchElementListForm.
	/// </summary>
	public partial class SearchElementListForm : Form
	{
		public SearchElementListForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void SearchTermTextBoxTextChanged(object sender, EventArgs e)
		{
			ElementSearcher srch = new ElementSearcher();
			string searchWord = SearchTermTextBox.Text;

			if( searchWord.Length <= 0 ) {
				listView1.Items.Clear();
				return ;
			}


            List<ElementVO> results = new List<ElementVO>();
            if ( searchWord.Length > 1 && "{".Equals(searchWord.Substring(0,1)) ) {
				ElementVO element = srch.findByGuid( searchWord );
                results.Add(element);
            } else {
				results = srch.findByKeyword( searchWord );
			}
			
			listView1.Items.Clear();
			
			foreach( ElementVO elem in results ) {
				ListViewItem item = new ListViewItem(getStringArrayFromVO(elem));
	            item.Tag = elem;
	            listView1.Items.Add(item);
			}
			
		}
		
		private string[] getStringArrayFromVO(ElementVO vo) {
            string projectName= "";
            if (vo.elementPath != null && vo.elementPath.Contains("/"))
            {
                string[] ary = vo.elementPath.Split('/');
                projectName = ary[1];
            }

			string[] retAry = new String[] { vo.name, vo.alias, vo.eaType, vo.stereoType,
				vo.elementPath, projectName };
			return retAry;
		}
		
		
		void ListView1MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ElementSearchVO vo = (ElementSearchVO)(listView1.SelectedItems[0].Tag);
			MainForm parentForm = (MainForm)(this.Owner);
			ArtifactVO atfvo = parentForm.getArtifactByGuid(vo.artifactGuid);
			
			ElementVO targetElem = searchElementVoFromArtifact(atfvo, vo.elemGuid);
			parentForm.openNewElementForm(targetElem);
		}
		
		private ElementVO searchElementVoFromArtifact(ArtifactVO atf, string elemGuid ) {
			return retrievePackage( atf.package, elemGuid );
		}
		
		private ElementVO retrievePackage(PackageVO pacvo, string elemGuid) {
			foreach( ElementVO elem in pacvo.elements ) {
				if (elemGuid.Equals(elem.guid)) {
					return elem;
				}
			}
			
			foreach( PackageVO subpac in pacvo.childPackageList ) {
				ElementVO retElem;
				retElem = retrievePackage(subpac, elemGuid);
				if ( retElem != null ) {
					return retElem;
				}
			}
			
			return null;
		}
		
	}
}
