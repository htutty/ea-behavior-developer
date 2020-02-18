using System;
using System.Text;

public class MyBase64str
{
    private Encoding enc;

    public MyBase64str(string encStr)
    {
        enc = Encoding.GetEncoding(encStr);
    }

    public string Encode(string str)
    {
        return Convert.ToBase64String(enc.GetBytes(str));
    }

    public string Decode(string str)
    {
        return enc.GetString(Convert.FromBase64String(str));
    }

    public byte[] DecodeBytes(string str)
    {
        return Convert.FromBase64String(str);
    }

}