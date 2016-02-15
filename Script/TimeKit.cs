using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TimeKit
{
    [CompilerGenerated]
    private static Dictionary<string, int> f__switch_map5A;
    public const int DAY_SECONDS = 0x15180;
    private static readonly DateTime initTime2 = new DateTime(0x7b2, 1, 1, 8, 0, 0);
    private static long timeFix;

    private TimeKit()
    {
    }

    private static long currentTimeMillis()
    {
        return Convert.ToInt64(DateTime.UtcNow.Subtract(initTime2).TotalMilliseconds);
    }

    public static string dateToFormat(int time, string format)
    {
        return getDateTime(time).ToString(format);
    }

    public static int firstDayOfMonth(DateTime date)
    {
        return date.AddDays((double) (1 - date.Day)).Day;
    }

    public static DateTime getDateTime(int time)
    {
        return initTime2.AddSeconds((double) time);
    }

    public static DateTime getDateTimeMillis(long time)
    {
        return initTime2.AddMilliseconds((double) time);
    }

    public static DateTime getDateTimeMin(int time)
    {
        return initTime2.AddMinutes((double) (time / 60));
    }

    public static int getDayInYear(int time)
    {
        return getDateTime(time).DayOfYear;
    }

    public static long getMillisTime()
    {
        return (currentTimeMillis() - timeFix);
    }

    public static int getSecondTime()
    {
        return (int) ((currentTimeMillis() - timeFix) / 0x3e8L);
    }

    public static long getTimeMillis(DateTime date)
    {
        return Convert.ToInt64(date.Subtract(initTime2).TotalMilliseconds);
    }

    public static int getWeekCHA(DayOfWeek dw)
    {
        string key = dw.ToString();
        if (key != null)
        {
            int num2;
            if (f__switch_map5A == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
                dictionary.Add("Monday", 0);
                dictionary.Add("Tuesday", 1);
                dictionary.Add("Wednesday", 2);
                dictionary.Add("Thursday", 3);
                dictionary.Add("Friday", 4);
                dictionary.Add("Saturday", 5);
                dictionary.Add("Sunday", 6);
                f__switch_map5A = dictionary;
            }
            if (f__switch_map5A.TryGetValue(key, out num2))
            {
                switch (num2)
                {
                    case 0:
                        return 1;

                    case 1:
                        return 2;

                    case 2:
                        return 3;

                    case 3:
                        return 4;

                    case 4:
                        return 5;

                    case 5:
                        return 6;

                    case 6:
                        return 7;
                }
            }
        }
        return 1;
    }

    public static void resetTime(long time)
    {
        timeFix = currentTimeMillis() - time;
    }

    public static long timeMillis(long timeSecond)
    {
        return (timeSecond * 0x3e8L);
    }

    public static int timeSecond(long timeMillis)
    {
        return (int) (timeMillis / 0x3e8L);
    }

    public static string timeTransform(double time)
    {
        time /= 1000.0;
        string str = ((int) (time / 3600.0)).ToString();
        if (str.Length == 1)
        {
            str = "0" + str;
        }
        string str2 = ((int) ((time % 3600.0) / 60.0)).ToString();
        if (str2.Length == 1)
        {
            str2 = "0" + str2;
        }
        string str3 = ((int) ((time % 3600.0) % 60.0)).ToString();
        if (str3.Length == 1)
        {
            str3 = "0" + str3;
        }
        string[] textArray1 = new string[] { str, ":", str2, ":", str3 };
        return string.Concat(textArray1);
    }

    public static string timeTransformDHMS(double time)
    {
        int num = (int) (time / 86400.0);
        string str = string.Empty;
        if (num != 0)
        {
            str = num + "天";//LanguageConfigManager.Instance.getLanguage("s0018");
        }
        int num2 = (int) ((time % 86400.0) / 3600.0);
        string str2 = string.Empty;
        if ((num > 0) || (num2 != 0))
        {
            str2 = num2 + "小时" ;//LanguageConfigManager.Instance.getLanguage("s0019");
        }
        int num3 = (int) (((time % 86400.0) % 3600.0) / 60.0);
        string str3 = string.Empty;
        if ((num2 > 0) || (num3 != 0))
        {
            str3 = num3.ToString().PadLeft(2, '0') + "分";//LanguageConfigManager.Instance.getLanguage("s0020");
        }
        int num4 = (int) (((time % 86400.0) % 3600.0) % 60.0);
        string str4 = string.Empty;
        if ((num3 > 0) || (num4 != 0))
        {
            str4 = num4.ToString().PadLeft(2, '0') + "秒";//LanguageConfigManager.Instance.getLanguage("s0021");
        }
        return (str + str2 + str3 + str4);
    }
}

