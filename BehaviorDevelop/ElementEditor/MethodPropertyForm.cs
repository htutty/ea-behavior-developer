/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/06/28
 * Time: 21:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

using BDFileReader.vo;

namespace ElementEditor
{
	/// <summary>
	/// Description of MethodPropertyForm.
	/// </summary>
	public partial class MethodPropertyForm : Form
	{
		MethodVO targetMethod;
		
		public MethodPropertyForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			targetMethod = null;
		}

		public MethodPropertyForm(MethodVO mth)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			targetMethod = mth;			
		}

		void MethodPropertyFormLoad(object sender, EventArgs e)
		{
			this.textMethodName.Text = targetMethod.name;
			this.textMethodAlias.Text = targetMethod.alias;
			this.textMethodGUID.Text = targetMethod.guid;
			this.textMethodNotes.Text = targetMethod.notes;
			this.textMethodStereotype.Text = targetMethod.stereoType;
			this.textMethodVisibility.Text = targetMethod.visibility;
			this.textMethodReturnType.Text = targetMethod.returnType;
			this.textMethodClassifierID.Text = targetMethod.classifierID;
			this.textMethodPos.Text = targetMethod.pos.ToString();
		}		
		
	}
}
