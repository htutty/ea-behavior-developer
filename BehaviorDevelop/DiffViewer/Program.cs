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
            DetailType detailType;

            if (args == null || args.Length < 5)
            {
                dispErrorMessage("引数が足りません");
                Application.Exit();
            }

            string prjfile = "project.bdprj";
			if( ProjectSetting.load(args[0] + "\\" + prjfile) == false )
            {
                dispErrorMessage("第１引数のプロジェクトパスが正しいかを確認ください");
                Application.Exit();
                return;
            }

            ElementVO elem = new ElementVO();
			elem.guid = args[3];

			elem.attributes = new List<AttributeVO>();
			elem.methods = new List<MethodVO>();
            elem.taggedValues = new List<TaggedValueVO>();

            switch(args[1])
            {
                case "a":
                case "A":
                    detailType = DetailType.Attribute;
                    break;

                case "m":
                case "M":
                    detailType = DetailType.Method;
                    break;

                case "t":
                case "T":
                    detailType = DetailType.TaggedValue;
                    break;

                default:
                    dispErrorMessage("第２引数には a,m,t のいずれかを指定してください");
                    Application.Exit();
                    return;
            }

			char changed ;
			if( args[2] == "C" || args[2] == "U" || args[2] == "D" ) {
				changed = args[2][0];
			} else {
                dispErrorMessage("第２引数には a,m,t のいずれかを指定してください");
                Application.Exit();
                return;
			}


            switch(detailType)
            {
                case DetailType.Attribute:
                    AttributeVO att = new AttributeVO();
                    att.guid = args[4];
                    att.changed = changed;
                    elem.attributes.Add(att);
                    break;
                case DetailType.Method:
                    MethodVO mth = new MethodVO();
                    mth.guid = args[4];
                    mth.changed = changed;
                    elem.methods.Add(mth);
                    break;
                case DetailType.TaggedValue:
                    TaggedValueVO tgv = new TaggedValueVO();
                    tgv.guid = args[4];
                    tgv.changed = changed;
                    elem.taggedValues.Add(tgv);
                    break;
            }

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(elem));

		}

        private static void dispErrorMessage(string message)
        {
            MessageBox.Show(message + "\r\n" + 
                "Usage: DiffViewer <ArtifactPath> <属性:a/操作:m/タグ:t> <CUD> <ElementGuid> <(Attr/Mth/Tag)Guid>");
        }

    }

    enum DetailType
    {
        Attribute,
        Method,
        TaggedValue
    }

}
