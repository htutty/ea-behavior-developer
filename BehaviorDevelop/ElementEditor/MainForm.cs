/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/08/11
 * Time: 14:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using EA;
using BDFileReader.util;
using BDFileReader.vo;

namespace ElementEditor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private ElementVO targetElement = null;
		
		List<string> openedElementGuids = new List<string>();
		List<ElementForm> openedForms = new List<ElementForm>();

        public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}


        private void MainForm_MouseEnter(object sender, EventArgs e)
        {
            getElementFromEA();
        }

        void TextBoxElementNameMouseEnter(object sender, EventArgs e)
		{
			getElementFromEA();
		}
		
		void TextBoxElementNameClick(object sender, EventArgs e)
		{
			getElementFromEA();
		}
		
		
		void TextBoxElementNameDoubleClick(object sender, EventArgs e)
		{
			openNewElementWindow();
		}

        private void ButtonOpenElement_Click(object sender, EventArgs e)
        {
            openNewElementWindow();
        }


        void getElementFromEA( ) {
			EA.Repository repo = ProjectSetting.getEARepo();
            EA.ObjectType otype = repo.GetContextItemType();

            if ( otype == EA.ObjectType.otElement )
            {

                // Code for when an element is selected
                EA.Element eaElemObj = (EA.Element)repo.GetContextObject();

                if (eaElemObj != null && (eaElemObj.Type == "Class" || eaElemObj.Type == "Enumeration")) 
                {
                    ElementVO eaElement = ObjectEAConverter.getElementFromEAObject(eaElemObj);
                    textBoxElementName.Text = eaElement.name;
                    targetElement = eaElement;
                } else {
                    textBoxElementName.Text = "(EA上でクラスを選択してください)";
                    targetElement = null;
                }

            } else {
                textBoxElementName.Text = "(EA上でクラスを選択してください)";
                targetElement = null;
            }

        }
		
	
		void openNewElementWindow()
		{
			if( !existOnOpenElements(targetElement) ) {
				addOpenedElements(targetElement);
				
				ElementForm elemForm = new ElementForm(ref targetElement);
				elemForm.Show(this);
				openedForms.Add(elemForm);
			}

		}


        public void deleteOpenedElement(ElementVO targetElement)
        {
            string guid = targetElement.guid;

            if (openedElementGuids.Contains(guid)) {
                int idx = openedElementGuids.IndexOf(guid);
                openedElementGuids.RemoveAt(idx);
                listBoxElements.Items.RemoveAt(idx);
            }

        }
		
		void addOpenedElements(ElementVO targetElement) {
			openedElementGuids.Add(targetElement.guid);
			listBoxElements.Items.Add(targetElement.name);
		}
	
		
		protected void closeElement(ElementVO targetElement) {
			return;
		}
		
		bool existOnOpenElements(ElementVO targetElement) {
			foreach( string elemGuid in openedElementGuids ) {
				if (targetElement.guid == elemGuid) {
					return true;
				}
			}
			return false;
		}

    }
}
