using System;

public class ErlKit
{
    private ErlKit()
    {
    }

    public static int[] ErlArray2Int(ErlArray array)
    {
        int[] numArray = new int[array.Value.Length];
        for (int i = 0; i < numArray.Length; i++)
        {
            numArray[i] = StringKit.toInt(array.Value[i].getValueString());
        }
        return numArray;
    }

    public static string[] ErlArray2String(ErlArray array)
    {
        string[] strArray = new string[array.Value.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            strArray[i] = array.Value[i].getValueString();
        }
        return strArray;
    }
}

