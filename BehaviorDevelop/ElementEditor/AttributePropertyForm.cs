using System;
using System.Drawing;
using System.Windows.Forms;

using ArtifactFileAccessor.vo;

namespace ElementEditor
{
	/// <summary>
	/// Description of AttributePropertyForm.
	/// </summary>
	public partial class AttributePropertyForm : Form
	{
		AttributeVO targetAttribute;
		
		public AttributePropertyForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		public AttributePropertyForm(AttributeVO attr)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			targetAttribute = attr;
		}
		
		
		
		void AttributePropertyFormLoad(object sender, EventArgs e)
		{
			this.textAttributeName.Text = targetAttribute.name;
			this.textAttributeAlias.Text = targetAttribute.alias;
			this.textAttributeGUID.Text = targetAttribute.guid;
			this.textAttributeNotes.Text = targetAttribute.notes;
			this.textAttributePos.Text = targetAttribute.pos.ToString();
			this.textAttributeStereotype.Text = targetAttribute.stereoType;
			this.textAttributeClassifierID.Text = targetAttribute.classifierID;
			this.textAttributeDefault.Text = targetAttribute.defaultValue;
			this.textAttributeIsStatic.Text = targetAttribute.isStatic.ToString();
		}
	}
}
