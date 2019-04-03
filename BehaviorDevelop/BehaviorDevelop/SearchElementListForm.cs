/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/09
 * Time: 20:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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


            List<ElementSearchItem> results = new List<ElementSearchItem>();
            if ( searchWord.Length > 1 && "{".Equals(searchWord.Substring(0,1)) ) {
				ElementSearchItem element = srch.findByGuid( searchWord );
                results.Add(element);
            } else {
				results = srch.findByKeyword( searchWord );
			}
			
			listView1.Items.Clear();
			
			foreach(ElementSearchItem elem in results ) {
				ListViewItem item = new ListViewItem(getStringArrayFromVO(elem));
	            item.Tag = elem;
	            listView1.Items.Add(item);
			}
			
		}
		
		private string[] getStringArrayFromVO(ElementSearchItem vo) {
            string projectName= "";
            if (vo.elemPath != null && vo.elemPath.Contains("/"))
            {
                string[] ary = vo.elemPath.Split('/');
                projectName = ary[1];
            }

			string[] retAry = new String[] { vo.elemName, vo.elemAlias, vo.elemType, vo.elemStereotype,
				vo.elemPath, projectName };
			return retAry;
		}
		
		
		void ListView1MouseDoubleClick(object sender, MouseEventArgs e)
		{
            ElementSearchItem vo = (ElementSearchItem)(listView1.SelectedItems[0].Tag);
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
