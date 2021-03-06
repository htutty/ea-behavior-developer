﻿using System;
using System.IO;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public abstract class AbstractValueObject
    {
        /// <summary>
        /// モデルからフォワードした時の宣言文を出力
        /// </summary>
        /// <param name="indentLv"></param>
        /// <returns></returns>
        public abstract string generateDeclareString(int indentLv);

        public abstract void sortChildNodes();

        public abstract void sortChildNodesGuid();


        /// <summary>
        /// 複数行の文字列の行頭にコメント文字を置き、指定分インデントした文字列を作成して返却する
        /// </summary>
        /// <param name="origStr"></param>
        /// <param name="commentStr"></param>
        /// <returns></returns>
        protected static string genCommentized(string origStr, string commentStr, int indentLv)
        {
            StringWriter sw = new StringWriter();

            // 改行文字で文字列を分割
            string[] delimiter = { "\r\n" };
            string[] lins = origStr.Split(delimiter, StringSplitOptions.None);

            foreach (string l in lins)
            {
                sw.WriteLine(getIndentStr(indentLv) + commentStr + " " + l);
            }

            return sw.ToString();
        }

        /// <summary>
        /// 指定されたレベルでインデントするための文字列を返却する
        /// </summary>
        /// <param name="indentLv">インデントレベル</param>
        /// <returns></returns>
        protected static string getIndentStr(int indentLv)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indentLv; i++)
            {
                sb.Append("  ");
            }
            return sb.ToString();
        }


        /// <summary>
        /// 複数行の文字列を受け取り、各行ごとに指定のインデントを施した結果を返却する
        /// </summary>
        /// <param name="origStr">元の文字列</param>
        /// <param name="indentLv">インデントレベル</param>
        /// <returns></returns>
        protected static string indentMultiLine(string origStr, int indentLv)
        {
            StringWriter sw = new StringWriter();

            string[] delimiter = { "\r\n" };
            string[] lins = origStr.Split(delimiter, StringSplitOptions.None);
            foreach (string l in lins)
            {
                sw.WriteLine(getIndentStr(indentLv) + l);
            }

            return sw.ToString();
        }

    }
}
