using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace AuditLogTransfer
{
    public class DbUtil
    {

        #region ユーティリティメソッド
        /// <summary>
        /// DbDataReaderからbool項目の取得
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        public static bool readBoolField(OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetBoolean(colIndex);
            return false;
        }

        /// <summary>
        /// DbDataReaderから文字列項目の取得
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        public static string readStringField(OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetString(colIndex);
            return string.Empty;
        }

        /// <summary>
        /// DbDataReaderからint項目の取得
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        public static int readIntField(OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetInt32(colIndex);
            return 0;
        }

        /// <summary>
        /// DbDataReaderから項目を取得する際にDB側の型定義が数値なものをboolに変換して取得する
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        public static bool readBooleanIntField(OleDbDataReader reader, int colIndex)
        {
            if (reader.IsDBNull(colIndex)) return false;
            int b = reader.GetInt32(colIndex);
            if (b == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// DbDataReaderから項目を取得する際にDB側の型定義が文字列なものをboolに変換して取得する
        /// </summary>
        /// <param name="reader">DbDataReader</param>
        /// <param name="colIndex">カラムIndex</param>
        /// <returns>取得した値</returns>
        public static bool readBooleanStringField(OleDbDataReader reader, int colIndex)
        {
            if (reader.IsDBNull(colIndex)) return false;
            string b = reader.GetString(colIndex);
            if (b == "1")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static DateTime readDateTimeField(OleDbDataReader reader, int colIndex)
        {
            if (reader.IsDBNull(colIndex))
            {
                return new DateTime(0L);
            }
            else
            {
                return (DateTime)reader.GetValue(colIndex);
            }

        }

        #endregion

    }
}
