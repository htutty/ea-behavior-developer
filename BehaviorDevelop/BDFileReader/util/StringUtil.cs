using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDFileReader.util
{
    public class StringUtil
    {

        public static string excludeSpecialChar(string tbl, string fld, string guid, string str)
        {
            string result = "";

            if (str == null)
            {
                return "";
            }

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                //	WScript.Echo "i=" & i & "b=" & b & "c=" & c
                if (c < 0x20)
                {
                    Console.WriteLine(i + "文字目, c=" + c);
                }
                else
                {
                    result = result + c;
                }
            }

            if (result != str)
            {
                // Dim objFS, objOutFile
                // Set objFS = CreateObject("Scripting.FileSystemObject")
                // ' strTemp = "c:\test\temp.txt"
                // Set objOutFile = objFS.OpenTextFile("illegalchars.txt", 8, True, -2)    

                // objOutFile.WriteLine  "<updateField table='" & tbl & "' field='" & fld & "' guid='" & guid & "' value='" & result & "' />"
                // objOutFile.Close
                // Set objFS = Nothing
                // Set objOutFile = Nothing
                Console.WriteLine("差異あり in=" + str + ", out=" + result);
            }

            return result;
        }

        public static string indent(int depth)
        {
            string retStr = "";
            Int32 i;

            for (i = 0; i < depth; i++)
            {
                retStr = retStr + "  ";
            }
            return retStr;
        }

        public static string escapeXML(string orig)
        {
            if (orig == null)
            {
                return "";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&apos;");
            return sb.ToString();
        }


    }
}
