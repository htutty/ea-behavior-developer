/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/04/16
 * Time: 18:09
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;

using EA;

namespace DiffViewer
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			bool isAttribute;

			if (args != null && args.Length >= 5) {
				string prjfile = "project.bdprj";
				if( ProjectSetting.load(args[0] + "\\" + prjfile) ) {
					ElementVO elem = new ElementVO();
					elem.guid = args[3];

					elem.attributes = new List<AttributeVO>();
					elem.methods = new List<MethodVO>();

					if( args[1] == "a" || args[1] == "A" ) {
						isAttribute = true;
					}
					else {
						isAttribute = false;
					}

					char changed ;
					if( args[2] == "C" || args[2] == "U" || args[2] == "D" ) {
						changed = args[2][0];
					} else {
						MessageBox.Show("コマンド: DiffViewer <ArtifactPath> <属性:a/操作:m> <CUD> <ElementGuid> <MethodGuid>" );
						return;
					}

					if ( isAttribute ) {
						AttributeVO att = new AttributeVO();
						att.guid = args[4];
						att.changed = changed ;
						elem.attributes.Add(att);
					} else {
						MethodVO mth = new MethodVO();
						mth.guid = args[4];
						mth.changed = changed ;
						elem.methods.Add(mth);
					}

					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new MainForm(elem));
				}

			} else {
				MessageBox.Show("コマンド: DiffViewer <ArtifactPath> <属性:a/操作:m> <CUD> <ElementGuid> <MethodGuid>" );
			}

		}

	}
}
