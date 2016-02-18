using MiniJSON;
using System;
using System.Collections.Generic;

public class ErlArray : ErlType
{
    private ErlType[] _value;
    public static int[] TAG = new int[] { 0x68, 0x69 };

    public ErlArray(ErlType[] array)
    {
        if (array != null)
        {
            this._value = array;
        }
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (base._tag == TAG[0])
        {
            int num = data.readUnsignedByte();
            this._value = new ErlType[num];
            for (int i = 0; i < num; i++)
            {
                this._value[i] = ByteKit.natureAnalyse(data);
            }
        }
        else if (base._tag == TAG[1])
        {
            int num3 = data.readInt();
            this._value = new ErlType[num3];
            for (int j = 0; j < num3; j++)
            {
                this._value[j] = ByteKit.natureAnalyse(data);
            }
        }
    }

    public override void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        if ((this._value == null) || (this._value.Length < 1))
        {
            new ErlNullList().bytesWrite(data);
        }
        else
        {
            if (this._value.Length > 0xff)
            {
                data.writeByte(TAG[1]);
                data.writeInt(this._value.Length);
            }
            else
            {
                data.writeByte(TAG[0]);
                data.writeByte(this._value.Length);
            }
            for (int i = 0; i < this._value.Length; i++)
            {
                ErlBytesWriter writer = this._value[i];
                if (writer == null)
                {
                    writer = new ErlNullList();
                }
                writer.bytesWriteServer(data);
            }
        }
    }

    public string checkSample(ErlType[] array)
    {
        string str = null;
        if ((((this._value.Length != 3) || !(this._value[0] is ErlAtom)) || !((this._value[0] as ErlAtom).Value == "sid")) || (!(this._value[1] is ErlByte) && !(this._value[1] is ErlInt)))
        {
            return str;
        }
        if ((!(this._value[2] is ErlNullList) && !(this._value[2] is ErlList)) && !(this._value[2] is ErlString))
        {
            return str;
        }
        int num = 0;
        if (this._value[1] is ErlByte)
        {
            num = (this._value[1] as ErlByte).Value;
        }
        else
        {
            num = (this._value[1] as ErlInt).Value;
        }
        if (this._value[2] is ErlNullList)
        {
            object[] objArray1 = new object[] { "[sid,", num, ",", (this._value[2] as ErlNullList).getValueString(), "]" };
            return string.Concat(objArray1);
        }
        if (this._value[2] is ErlList)
        {
            object[] objArray2 = new object[] { "[sid,", num, ",", (this._value[2] as ErlList).getListString(), "]" };
            return string.Concat(objArray2);
        }
        object[] objArray3 = new object[] { "[sid,", num, ",", (this._value[2] as ErlString).getASCII(true), "]" };
        return string.Concat(objArray3);
    }

    public static string erlArrToStr(ErlArray arr)
    {
        if (arr == null)
        {
            return string.Empty;
        }
        string str = string.Empty + '[';
        for (int i = 0; i < arr.Value.Length; i++)
        {
            ErlType type = arr.Value[i];
            if (type != null)
            {
                string str2;
                if (type is ErlArray)
                {
                    str2 = str;
                    string[] textArray1 = new string[] { str2, "[\"", type.GetType().Name, "\",", erlArrToStr(type as ErlArray), "]" };
                    str = string.Concat(textArray1);
                }
                else
                {
                    str2 = str;
                    string[] textArray2 = new string[] { str2, "[\"", type.GetType().Name, "\",", type.getValueString(), "]" };
                    str = string.Concat(textArray2);
                }
                if (i < (arr.Value.Length - 1))
                {
                    str = str + ",";
                }
            }
        }
        return (str + "]");
    }

    private static ErlType[] getErlArr(List<object> jsonList)
    {
        ErlType[] typeArray = new ErlType[jsonList.Count];
        for (int i = 0; i < jsonList.Count; i++)
        {
            List<object> list = jsonList[i] as List<object>;
            ErlType type = null;
            if ((list[0] as string) == typeof(ErlInt).Name)
            {
                type = new ErlInt(StringKit.toInt(list[1].ToString()));
            }
            else if ((list[0] as string) == typeof(ErlString).Name)
            {
                type = new ErlString(list[1].ToString());
            }
            else if ((list[0] as string) == typeof(ErlByte).Name)
            {
                type = new ErlByte(byte.Parse(list[1].ToString()));
            }
            else if ((list[0] as string) == typeof(ErlAtom).Name)
            {
                type = new ErlAtom(list[1].ToString());
            }
            else if ((list[0] as string) == typeof(ErlNullList).Name)
            {
                type = new ErlNullList();
            }
            else if ((list[0] as string) == typeof(ErlArray).Name)
            {
                type = new ErlArray(getErlArr(list[1] as List<object>));
            }
            typeArray[i] = type;
        }
        return typeArray;
    }

    public string getListArray()
    {
        string str = string.Empty;
        uint length = (uint) this._value.Length;
        str = this.checkSample(this._value);
        if (str != null)
        {
            return str;
        }
        if ((((length == 2) && (this._value[0] is ErlByte)) && (((this._value[0] as ErlByte).Value == 1) && (this._value[1] is ErlString))) && ((this._value[1] as ErlString).Value == "nil"))
        {
            return "[]";
        }
        if ((length <= 0) || ((length % 2) != 0))
        {
            throw new Exception("ErlArray function:getListArray len=" + length);
        }
        str = "{";
        for (int i = 0; i < (length - 1); i += 2)
        {
            if ((((this._value[i] is ErlAtom) || (this._value[i] is ErlList)) || ((this._value[i] is ErlByteArray) || (this._value[i] is ErlString))) || (((this._value[i] is ErlInt) || (this._value[i] is ErlByte)) || (this._value[i] is ErlLong)))
            {
                if (this._value[i + 1] != null)
                {
                    string str2 = str;
                    object[] objArray1 = new object[] { str2, this._value[i].getValueString(), ':', this._value[i + 1].getValueString() };
                    str = string.Concat(objArray1);
                }
                else
                {
                    str = str + this._value[i].getValueString() + ":null";
                }
                if (i < (length - 2))
                {
                    str = str + ",";
                }
            }
            else
            {
                object[] objArray2 = new object[] { "ErlArray function:getListArray  i=", i, " len=", length, " _value[i]=", this._value[i] };
                throw new Exception(string.Concat(objArray2));
            }
        }
        return (str + '}');
    }

    public override string getString(object key)
    {
        object[] objArray1 = new object[] { '"', key.ToString(), '"', ":" };
        string str = string.Concat(objArray1);
        string str2 = this.checkSample(this._value);
        if (str2 != null)
        {
            return (str + str2);
        }
        str = str + '[';
        for (int i = 0; i < this._value.Length; i++)
        {
            ErlType type = this._value[i];
            if (type != null)
            {
                str = str + type.getValueString();
                if (i < (this._value.Length - 1))
                {
                    str = str + ",";
                }
            }
        }
        return (str + "]");
    }

    public override string getValueString()
    {
        string str = "[";
        for (int i = 0; i < this._value.Length; i++)
        {
            ErlType type = this._value[i];
            if (type != null)
            {
                str = str + type.getValueString();
                if (i < (this._value.Length - 1))
                {
                    str = str + ",";
                }
            }
        }
        return (str + "]");
    }

    public override bool isTag(int tag)
    {
        return ((TAG[0] == tag) || (TAG[1] == tag));
    }

    public static ErlArray strToErlArr(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        List<object> jsonList = Json.Deserialize(str) as List<object>;
        return new ErlArray(getErlArr(jsonList));
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }

    public ErlType[] Value
    {
        get
        {
            return this._value;
        }
        set
        {
            this._value = value;
        }
    }
}

