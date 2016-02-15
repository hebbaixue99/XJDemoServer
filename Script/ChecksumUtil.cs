using System;

public class ChecksumUtil
{
    private static uint[] crcTable = makeCRCTable();

    public static uint Adler32(ByteBuffer data)
    {
        return Adler32(data, 0, 0);
    }

    public static uint Adler32(ByteBuffer data, uint start, uint len)
    {
        if (start >= data.length())
        {
            start = (uint) data.length();
        }
        if (len == 0)
        {
            len = ((uint) data.length()) - start;
        }
        if ((len + start) > data.length())
        {
            len = ((uint) data.length()) - start;
        }
        uint index = start;
        uint num2 = 1;
        uint num3 = 0;
        while (index < (start + len))
        {
            num2 = (num2 + data.toArray()[index]) % 0xfff1;
            num3 = (num2 + num3) % 0xfff1;
            index++;
        }
        return ((num3 << 0x10) | num2);
    }

    public static uint CRC32(ByteBuffer data, uint start, uint len)
    {
        if (start >= data.top)
        {
            start = (uint) data.top;
        }
        if (len == 0)
        {
            len = ((uint) data.top) - start;
        }
        if ((len + start) > data.top)
        {
            len = ((uint) data.top) - start;
        }
        uint maxValue = uint.MaxValue;
        for (uint i = start; i < len; i++)
        {
            maxValue = crcTable[(int) ((IntPtr) ((maxValue ^ data.getArray()[i]) & 0xff))] ^ (maxValue >> 8);
        }
        return (maxValue ^ uint.MaxValue);
    }

    private static uint[] makeCRCTable()
    {
        uint[] numArray = new uint[0x100];
        for (uint i = 0; i < 0x100; i++)
        {
            uint num3 = i;
            for (uint j = 0; j < 8; j++)
            {
                if ((num3 & 1) == 1)
                {
                    num3 = 0xedb88320 ^ (num3 >> 1);
                }
                else
                {
                    num3 = num3 >> 1;
                }
            }
            numArray[i] = num3;
        }
        return numArray;
    }
}

