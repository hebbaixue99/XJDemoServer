using System;
using System.Text;
using System.Text.RegularExpressions;

public class StringKit
{
    public const char POUND_SIGN = '#';
    public const char USD_SIGN = '$';

    public static bool containsEmoji(string source)
    {
        if (!string.IsNullOrEmpty(source))
        {
            foreach (char ch in source.ToCharArray())
            {
                if (!isEmojiCharacter(ch))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static byte[] DefaultToUTF8Byte(string p_str)
    {
        try
        {
            byte[] bytes = Encoding.Default.GetBytes(p_str);
            return Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
        }
        catch
        {
            return null;
        }
    }

    public static string frontIdToServerId(string fId)
    {
        if (toInt(fId) < 0)
        {
            return "error";
        }
        long num = 1;//toLong(UserManager.Instance.self.uid) >> 0x30;
        long num2 = 1 ;//(toLong(UserManager.Instance.self.uid) << 0x10) >> 0x30;
        if (fId.Length <= num2.ToString().Length)
        {
            return "error";
        }
        long num3 = toLong(fId.Substring(num2.ToString().Length));
        long num4 = ((num << 0x30) | (num2 << 0x20)) | num3;
        return num4.ToString();
    }

    public static string intToFixString(int data)
    {
        return data.ToString("D3");
    }

    public static string intToThousand(int number)
    {
        if (number >= 0x3b9aca00)
        {
            return (Math.Round((double) (((float) number) / 1E+09f), 2).ToString("f1") + "b");
        }
        if ((number >= 1000000f) && (number < 0x3b9aca00))
        {
            return (Math.Round((double) (((float) number) / 1000000f), 2).ToString("f1") + "m");
        }
        if ((number >= 0x3e8) && (number < 0xf4240))
        {
            return (Math.Round((double) (((float) number) / 1000f), 2).ToString("f1") + "k");
        }
        return number.ToString();
    }

    private static bool isEmojiCharacter(char codePoint)
    {
        return (((((codePoint == '\0') || (codePoint == '\t')) || ((codePoint == '\n') || (codePoint == '\r'))) || (((codePoint >= ' ') && (codePoint <= 0xd7ff)) || ((codePoint >= 0xe000) && (codePoint <= 0xfffd)))) || ((codePoint >= 0x10000) && (codePoint <= 0x10ffff)));
    }

    public static bool isNum(string s)
    {
        string pattern = "^[0-9]*$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(s);
    }

    public static bool IsNumeric(string str)
    {
        if ((str == null) || (str.Length == 0))
        {
            return false;
        }
        foreach (char ch in str)
        {
            if (!char.IsNumber(ch))
            {
                return false;
            }
        }
        return true;
    }

    public static string NumToChinese(long n, bool fang)
    {
        string str = n.ToString();
        string str2 = string.Empty;
        string str3 = "0";//LanguageConfigManager.Instance.getLanguage("intNN");
        string str4 = "0";//LanguageConfigManager.Instance.getLanguage("intLN");
        string str5 = "0";//LanguageConfigManager.Instance.getLanguage("intMM");
        string str6 = "0";//LanguageConfigManager.Instance.getLanguage("intLM");
        for (int i = 0; i < str.Length; i++)
        {
            int startIndex = int.Parse(str.Substring(i, 1));
            if (fang)
            {
                str2 = str2 + str3.Substring(startIndex, 1);
                if (str6.Substring(str.Length - i, 1) != " ")
                {
                    str2 = str2 + str5.Substring(str.Length - i, 1);
                }
            }
            else
            {
                str2 = str2 + str4.Substring(startIndex, 1);
                if (str6.Substring(str.Length - i, 1) != " ")
                {
                    str2 = str2 + str6.Substring(str.Length - i, 1);
                }
            }
        }
       // if (str2.Substring(str2.Length - 1) == LanguageConfigManager.Instance.getLanguage("int0"))
        if (str2.Substring(str2.Length - 1) == "零")
        {
            str2 = str2.Substring(0, str2.Length - 1);
        }
        //if ((str2.Length > 1) && (str2.Substring(0, 2) == LanguageConfigManager.Instance.getLanguage("int10L")))
        if ((str2.Length > 1) && (str2.Substring(0, 2) == "一十"))
        {
            str2 = str2.Substring(1);
        }
        //if ((str2.Length > 1) && (str2.Substring(0, 2) == LanguageConfigManager.Instance.getLanguage("int10N")))壹拾
        if ((str2.Length > 1) && (str2.Substring(0, 2) == "壹拾"))
        {
            str2 = str2.Substring(1);
        }
        return str2;
    }

    public static string replaceEmoji(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            char[] chArray = str.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                if (!isEmojiCharacter(chArray[i]))
                {
                    chArray[i] = '*';
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in chArray)
            {
                builder.Append(ch);
            }
            str = builder.ToString();
        }
        return str;
    }

    public static string serverIdToFrontId(string str)
    {
        long num = (0 >> 0x30);//(toLong(UserManager.Instance.self.uid) >> 0x30;
        long num2 = (0 << 0x10) >> 0x30;//(toLong(UserManager.Instance.self.uid) << 0x10) >> 0x30;
        long num4 = (toLong(str) << 0x20) >> 0x20;
        return (num2.ToString() + num4.ToString());
    }

    public static string stringListTostring(string[] changeText, char tmpchar)
    {
        if (changeText == null)
        {
            return null;
        }
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < changeText.Length; i++)
        {
            builder.Append(changeText[i] + tmpchar);
        }
        string str = builder.ToString();
        if (builder.Length > 0)
        {
            str = str.Substring(0, builder.Length - 1);
        }
        return str;
    }

    public static string StringReverse(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            char[] array = str.ToCharArray();
            Array.Reverse(array);
            str = new string(array);
        }
        return str;
    }

    public static string[] stringToStringList(string changeText, char[] tmpchar)
    {
        char[] chArray;
        if (changeText == null)
        {
            return null;
        }
        if (tmpchar == null)
        {
            chArray = new char[] { ',', '|' };
        }
        else
        {
            chArray = tmpchar;
        }
        return changeText.Split(chArray);
    }

    public static int[] toArrayInt(string strValue, char sepChar)
    {
        char[] separator = new char[] { sepChar };
        string[] strArray = strValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        int length = strArray.Length;
        int[] numArray = new int[length];
        for (int i = 0; i < length; i++)
        {
            numArray[i] = toInt(strArray[i]);
        }
        return numArray;
    }

    public static float toFloat(string strValue)
    {
        float num;
        if (strValue == null)
        {
            return 0f;
        }
        try
        {
            float.TryParse(strValue, out num);
        }
        catch
        {
            return 0f;
        }
        return num;
    }

    public static int[] toInArray(string strValue, char c)
    {
        if (string.IsNullOrEmpty(strValue))
        {
            return null;
        }
        char[] separator = new char[] { c };
        string[] strArray = strValue.Split(separator);
        int[] numArray = new int[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            numArray[i] = toInt(strArray[i]);
        }
        return numArray;
    }

    public static int toInt(string strValue)
    {
        int num;
        if (strValue == null)
        {
            return 0;
        }
        try
        {
            int.TryParse(strValue, out num);
        }
        catch
        {
            return 0;
        }
        return num;
    }

    public static long toLong(string strValue)
    {
        long num;
        if (strValue == null)
        {
            return 0L;
        }
        try
        {
            long.TryParse(strValue, out num);
        }
        catch
        {
            return 0L;
        }
        return num;
    }

    public static long[] toLongArray(string strValue, char c)
    {
        if (string.IsNullOrEmpty(strValue))
        {
            return null;
        }
        char[] separator = new char[] { c };
        string[] strArray = strValue.Split(separator);
        long[] numArray = new long[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            numArray[i] = toLong(strArray[i]);
        }
        return numArray;
    }

    public static string UTF8ToDefaultString(byte[] p_bt)
    {
        if ((p_bt == null) || (p_bt.Length == 0))
        {
            return string.Empty;
        }
        try
        {
            byte[] bytes = Encoding.Convert(Encoding.UTF8, Encoding.Default, p_bt);
            return Encoding.Default.GetString(bytes);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}

