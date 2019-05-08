using ArtifactFileAccessor.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.util
{
    public class BehaviorTranslator
    {

        public static string translateBehavior(string behavior)
        {
            BehaviorParser parser = new BehaviorParser();
            List<BehaviorChunk> chunkList = parser.parseBehavior(behavior);

            StringWriter outsw = new StringWriter();

            foreach (var bc in chunkList)
            {
                // 
                if( bc.dottedNum != null && bc.dottedNum != "" ) {
                    outsw.WriteLine(generateIndentStr(bc.indLv) + "// " + bc.dottedNum + "　" + bc.behavior);
                }
                else
                {
                    outsw.WriteLine(generateIndentStr(bc.indLv) + "// " + bc.behavior);
                }

                outsw.WriteLine(generateIndentStr(bc.indLv) + bc.getTokenRepresented());
            }

            return outsw.ToString();
        }


        private static string generateIndentStr(int indentLevel)
        {
            StringBuilder sb = new StringBuilder();

            for(int i=0; i < indentLevel; i++)
            {
                sb.Append("\t");
            }

            return sb.ToString();

            //switch (indentLevel)
            //{
            //    case 0:
            //        return "";
            //    case 1:
            //        return "＃ ";
            //    case 2:
            //        return "＃＃ ";
            //    case 3:
            //        return "＃＃＃ ";
            //    case 4:
            //        return "＃＃＃＃ ";
            //    case 5:
            //        return "＃＃＃＃＃ ";
            //    case 6:
            //        return "＃＃＃＃＃＃ ";
            //    case 7:
            //        return "＃＃＃＃＃＃＃ ";
            //    case 8:
            //        return "＃＃＃＃＃＃＃＃ ";
            //    case 9:
            //        return "＃＃＃＃＃＃＃＃＃ ";
            //    case 10:
            //        return "＃＃＃＃＃＃＃＃＃＃ ";
            //    default:
            //        return "＃＃＃＃＃＃＃＃＃＃" + indentLevel + " ";
            //}

        }


    }
}
