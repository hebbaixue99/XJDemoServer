using ComponentAce.Compression.Libs.zlib;
using System;
using System.IO;

public class ZIPUtil
{
    public static byte[] Compress(byte[] inputBytes)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (ZOutputStream stream2 = new ZOutputStream(stream, 0))
            {
                stream2.Write(inputBytes, 0, inputBytes.Length);
                stream2.finish();
                return stream.ToArray();
            }
        }
    }

    public static byte[] Decompress(byte[] inputBytes)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (ZOutputStream stream2 = new ZOutputStream(stream))
            {
                stream2.Write(inputBytes, 0, inputBytes.Length);
                stream2.finish();
                return stream.ToArray();
            }
        }
    }
}

