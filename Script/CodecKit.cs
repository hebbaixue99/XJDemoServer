using System;
using System.Text;

public class CodecKit
{
    private const string ENCODE_KEY = "xxj";

    public static void coding(Encoding encoding, byte[] bytes, string code)
    {
        coding(bytes, 0, bytes.Length, encoding.GetBytes(code), 0);
    }

    public static void coding(byte[] bytes, int pos, int len, byte[] code, int offset)
    {
        if ((((pos >= 0) && (pos < bytes.Length)) && ((offset >= 0) && (offset < code.Length))) && (len > 0))
        {
            if ((pos + len) > bytes.Length)
            {
                len = bytes.Length - pos;
            }
            int num = code.Length - offset;
            int num2 = (len >= num) ? code.Length : (offset + len);
            for (int i = offset; i < num2; i++)
            {
                bytes[pos++] = (byte) (bytes[pos++] ^ code[i]);
            }
            len -= num;
            if (len > 0)
            {
                int num4 = len / code.Length;
                for (int j = 0; j < num4; j++)
                {
                    for (int m = 0; m < code.Length; m++)
                    {
                        bytes[pos++] = (byte) (bytes[pos++] ^ code[m]);
                    }
                }
                num = len % code.Length;
                for (int k = 0; k < num; k++)
                {
                    bytes[pos++] = (byte) (bytes[pos++] ^ code[k]);
                }
            }
        }
    }

    public static byte[] encodeXor(Encoding encoding, byte[] bytes)
    {
        return encodeXor(bytes, encoding.GetBytes("xxj"));
    }

    public static byte[] encodeXor(byte[] bytes, byte[] keys)
    {
        if (((bytes == null) || (bytes.Length < 1)) || ((keys == null) || (keys.Length < 1)))
        {
            return null;
        }
        int length = bytes.Length;
        int num2 = keys.Length;
        byte[] buffer = new byte[length];
        int index = 0;
        for (int i = 0; i < length; i++)
        {
            if (index == num2)
            {
                index = 0;
            }
            int num5 = bytes[i] ^ keys[index];
            num5 = num5 << 0x18;
            num5 = num5 >> 0x18;
            buffer[i] = (byte) num5;
            index++;
        }
        return buffer;
    }

    public static byte[] encodeXor(Encoding encoding, byte[] bytes, string key)
    {
        return encodeXor(bytes, encoding.GetBytes(key));
    }
}

