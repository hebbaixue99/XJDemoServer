using System;

public class TextKit
{
    public static readonly char FIRST_ASCII = ' ';

    public static bool parseBoolean(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }
        char[] chArray = str.ToCharArray();
        if (str.Length == 1)
        {
            return (chArray[0] == '1');
        }
        return str.Equals("true", StringComparison.CurrentCultureIgnoreCase);
    }

    public static int parseInt(string str)
    {
        return (int) parseLong(str);
    }

    public static int[] parseIntArray(string[] strs)
    {
        if (strs == null)
        {
            return null;
        }
        int[] numArray = new int[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            numArray[i] = parseInt(strs[i]);
        }
        return numArray;
    }

    public static long parseLong(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return 0L;
        }
        char[] chArray = str.ToCharArray();
        if (chArray[0] == '#')
        {
            return Convert.ToInt64(str.Substring(1), 0x10);
        }
        if (((str.Length > 1) && (chArray[0] == '0')) && (chArray[1] == 'x'))
        {
            return Convert.ToInt64(str.Substring(2), 0x10);
        }
        return Convert.ToInt64(str);
    }

    public static string replace(string str, string target, string swap)
    {
        return replace(str, target, swap, false, null);
    }

    public static string replace(string str, string swap, int start, int count)
    {
        int capacity = (str.Length + swap.Length) - count;
        CharBuffer buffer = new CharBuffer(capacity);
        buffer.append(str.Substring(0, start)).append(swap);
        buffer.append(str.Substring(start + count));
        return buffer.getString();
    }

    public static string replace(string str, string swap, int start, int count, CharBuffer cb)
    {
        int len = (str.Length + swap.Length) - count;
        cb.clear();
        cb.setCapacity(len);
        cb.append(str.Substring(0, start)).append(swap);
        cb.append(str.Substring(start + count));
        return cb.getString();
    }

    public static string replace(string str, string target, string swap, bool caseless, CharBuffer cb)
    {
        string str2 = str;
        string str3 = target;
        if (caseless)
        {
            str2 = str.ToLower();
            str3 = target.ToLower();
        }
        int index = str2.IndexOf(str3);
        if (index < 0)
        {
            return str;
        }
        if (cb != null)
        {
            cb.clear();
            cb.setCapacity((str.Length + swap.Length) - target.Length);
        }
        else
        {
            cb = new CharBuffer((str.Length + swap.Length) - target.Length);
        }
        return replace(str, swap, index, target.Length, cb);
    }

    public static string replaceAll(string str, string target, string swap)
    {
        return replaceAll(str, target, swap, false, null);
    }

    public static string replaceAll(string str, string target, string swap, bool caseless)
    {
        return replaceAll(str, target, swap, caseless, null);
    }

    public static string replaceAll(string str, string target, string swap, bool caseless, CharBuffer cb)
    {
        int length = target.Length;
        if (length == 0)
        {
            return str;
        }
        string str2 = str;
        string str3 = target;
        if (caseless)
        {
            str2 = str.ToLower();
            str3 = target.ToLower();
        }
        int startIndex = 0;
        int index = str2.IndexOf(str3, startIndex);
        if (index < 0)
        {
            return str;
        }
        if (cb != null)
        {
            cb.clear();
            cb.setCapacity(str.Length);
        }
        else
        {
            cb = new CharBuffer(str.Length);
        }
        while (index >= 0)
        {
            cb.append(str.Substring(startIndex, index)).append(swap);
            startIndex = index + length;
            index = str2.IndexOf(str3, startIndex);
        }
        cb.append(str.Substring(startIndex));
        return cb.getString();
    }

    public static string toString(bool[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 2) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(byte[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 5) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(char[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 2) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(double[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 0x10) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(short[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 6) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(int[] array)
    {
        if (array == null)
        {
            return null;
        }
        CharBuffer cb = new CharBuffer((array.Length * 9) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(long[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 0x10) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(object[] array)
    {
        if (array == null)
        {
            return null;
        }
        CharBuffer cb = new CharBuffer((array.Length * 0x19) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(float[] array)
    {
        CharBuffer cb = new CharBuffer((array.Length * 10) + 2);
        cb.append('{');
        toString(array, ",", cb);
        cb.append('}');
        return cb.getString();
    }

    public static string toString(bool[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 1));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(byte[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 4));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(char[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 1));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(double[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 15));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(short[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 5));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(int[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 8));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(long[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 15));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(object[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 0x18));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static string toString(float[] array, string separator)
    {
        CharBuffer cb = new CharBuffer(array.Length * (separator.Length + 9));
        toString(array, separator, cb);
        return cb.getString();
    }

    public static void toString(bool[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(!array[i] ? '0' : '1').append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static void toString(byte[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append((int) array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append((int) array[index]);
        }
    }

    public static void toString(char[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static void toString(double[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static void toString(short[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append((int) array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append((int) array[index]);
        }
    }

    public static void toString(int[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static void toString(long[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static void toString(object[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static void toString(float[] array, string separator, CharBuffer cb)
    {
        int index = array.Length - 1;
        for (int i = 0; i < index; i++)
        {
            cb.append(array[i]).append(separator);
        }
        if (index >= 0)
        {
            cb.append(array[index]);
        }
    }

    public static bool valid(char c, char[] charRangeSet)
    {
        if (c >= FIRST_ASCII)
        {
            if (charRangeSet == null)
            {
                return true;
            }
            int index = 0;
            int num2 = charRangeSet.Length - 1;
            while (index < num2)
            {
                if ((c >= charRangeSet[index]) && (c <= charRangeSet[index + 1]))
                {
                    return true;
                }
                index += 2;
            }
        }
        return false;
    }

    public static char valid(string str, char[] charRangeSet)
    {
        int length = str.Length;
        char[] chArray = str.ToCharArray();
        for (int i = 0; i < length; i++)
        {
            char c = chArray[i];
            if (c < FIRST_ASCII)
            {
                return c;
            }
            if (!valid(c, charRangeSet))
            {
                return c;
            }
        }
        return Convert.ToChar(0);
    }
}

