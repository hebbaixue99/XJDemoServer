using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using NLog;

public class StringKit
{
    public const char POUND_SIGN = '#';
    public const char USD_SIGN = '$';
	public static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();
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
	public static void getErlType(string strValue, string strType,ErlArray ea ,Stack skValue ,Stack skType )
	{
		string tmpValue = "";
		string tmpType = "";
		for (int i = 0; i < strValue.Length; i++) {
			if (strValue.Substring (i, 1) == "[") {
				skValue.Push(strValue.Substring(0,i+1));
				tmpValue = strValue.Substring (i+1, strValue.Length - i-1);
				 break;
			}
		}
		for (int i = 0; i < strType.Length; i++) {
			if (strType.Substring (i, 1) == "[") {
				skType.Push(strType.Substring(0,i+1));
				tmpType = strType.Substring (i+1, strType.Length - i-1);
				 break;
			}
			 
		} 
	}
	 
	public static ErlType[]  toErlTypeArray(string strValue , string strType, char c)
	{
		if (string.IsNullOrEmpty(strValue))
		{
			return null;
		}
		char[] separator = new char[] { c };
		string[] strArray = strValue.Split(separator);
		string[] strTypeArray = strType.Split(separator);
		ErlType[] erlTypeArray = new ErlType[strArray.Length];
		for (int i = 0; i < strArray.Length; i++)
		{
			erlTypeArray[i] = toErlType(strArray[i],strTypeArray[i]);
		}
		return erlTypeArray;	
	}
	public static ErlType  toErlType(string strValue , string strType)
	{
		if (string.IsNullOrEmpty(strValue))
		{
			return new ErlNullList();
		}
		switch (strType) {
		case "atom":
			return new ErlAtom (strValue);
		case "string":
			return new ErlString (strValue);
		case "byte":
			return new ErlByte (toInt (strValue));
		case "int":
			return new ErlInt (toInt (strValue));
		case "double":
			return new ErlDouble (toLong(strValue));
		default :
			if (strType.Length>5&&strType.Substring (0, 4) == "list") {
				string tmpValue = strValue.Substring (1, strValue.Length - 2);
				string tmpType = strType.Substring (5, strType.Length - 6);
				ErlType[] list = toErlTypeArray (tmpValue,tmpType , ':');
				ErlList elist = new ErlList (list);
				return elist;
			}
			if (strType.Length>6&&strType.Substring (0, 5) == "array") {
				string tmpValue = strValue.Substring (1, strValue.Length - 2);
				string tmpType = strType.Substring (6, strType.Length - 7);
				ErlType[] list = toErlTypeArray (tmpValue,tmpType , ':');
				ErlArray elist = new ErlArray (list);
				return elist;
			}
			return new ErlNullList (); 
		}
		 
	}

	public static ErlType[] strToErlTypeArray(string strValue)
	{
		strValue =strValue.Replace("'","\"");
		List<Object> list = MiniJSON.Json.Deserialize (strValue) as List<Object> ; 
		ErlType[] et = listToErlTypeArray (list);
		return et;
	}
	public static ErlType[] listToErlTypeArray(List<Object> list )
	{
		ErlType[] et = new ErlType[list.Count];
		//string[] strTypeArray = strType.Split(',');
		int k = 0;
		for (int i = 0; i < list.Count; i++) {
			if (list [i].GetType ().Name.Contains ("List")) {
				List<Object> tmpList = list [i] as List<Object>;
				if (tmpList.Count > 0) {	
					et [i] = new ErlArray (
						listToErlTypeArray (tmpList));
				} else {
					et [i] = new ErlArray (new ErlType[0]);
				}
			} else {
				string tmpstr = list [i].ToString ();
				string tmpType = "string";
				if (list [i].GetType ().Name.ToLower ().Contains ("int")) {
					int count = StringKit.toInt (list [i].ToString ());

					if (count < 255) {
						tmpType = "byte";
					} else {
						tmpType = "int";
					}
				} else if(tmpstr.ToLower ().Contains("(erlatom)")) {
					tmpstr = tmpstr.Substring (0, tmpstr.Length - 9);
					tmpType = "atom";
				}
				et[i] = toErlType(tmpstr,tmpType);
				k ++;
			}

		}
		return et;
	}

	public static void Main(string[] args)
	{
		Log.Info ("---------------");
		String strValue = "[\"281629595547903\",\"蒲冰\",1,1,211060878,15215,\"281629599247928\",10533472,199,0,60,60,0,3,3,0,5,5,0,1,50,\"281629595533335\",\"名剑山庄\",0,0,0,0,0,0,0,\"1455784932990\",1,47190,41,[0],129276,126868,152,1429891200,18320,1396530,0,0,20,11776]]";
		String strType = "string,string,byte,byte,int,int,string,int,byte,byte,byte,byte,int,byte,byte,byte,byte,byte,byte,byte,byte,string,string,";
		strType = strType+"byte,byte,byte,byte,byte,byte,byte,string,byte,int,byte,int,int,int,byte,int,int,int,byte,byte,byte,int";

		ErlType[] et = strToErlTypeArray (strValue);
		ErlArray ea = new ErlArray (et);
		Log.Info (ea.getValueString());


		//Log.Info (ov);
	    
		/*
		ErlType[] et = toErlTypeArray (strValue, strType, ',');
		ErlArray ea = new ErlArray (et);
		ErlString e = new ErlString ("dsf");
		*/
		//e.bytesWrite

		//Log.Info (ea.getValueString ());
	}
}

