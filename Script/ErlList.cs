using System;

public class ErlList : ErlType
{
    private ErlType[] _value;
    private bool isString = true;
    public const int TAG = 0x6c;

    public ErlList(ErlType[] array)
    {
        if (array != null)
        {
            this._value = array;
        }
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            int num = data.readInt();
            this._value = new ErlType[num];
            for (int i = 0; i < num; i++)
            {
                ErlType type = ByteKit.natureAnalyse(data);
                if (!(type is ErlByte) && !(type is ErlInt))
                {
                    this.isString = false;
                }
                this._value[i] = type;
            }
            data.readUnsignedByte();
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
            data.writeByte(0x6c);
            data.writeInt(this._value.Length);
            for (int i = 0; i < this._value.Length; i++)
            {
                ErlBytesWriter writer = this._value[i];
                if (writer == null)
                {
                    writer = new ErlNullList();
                }
                writer.bytesWrite(data);
            }
            new ErlNullList().bytesWrite(data);
        }
    }

    public string checkTrans(int num)
    {
        string str = null;
        if (num == 0x22)
        {
            return (str + '"');
        }
        if (num == 0x5c)
        {
            return (str + '\\');
        }
        if (num == 8)
        {
            return (str + '\b');
        }
        if (num == 9)
        {
            return (str + '\t');
        }
        if (num == 10)
        {
            return (str + '\n');
        }
        if (num == 12)
        {
            return (str + '\f');
        }
        if (num == 13)
        {
            str = str + '\r';
        }
        return str;
    }

    public string getListString()
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

    public override string getString(object key)
    {
        object[] objArray1 = new object[] { '"', key.ToString(), '"', ':' };
        string str = string.Concat(objArray1);
        if (this._value.Length == 0)
        {
            return "[]";
        }
        string str2 = this.transNumber();
        if (str2 != null)
        {
            string str3 = str;
            object[] objArray2 = new object[] { str3, '"', str2, '"' };
            return string.Concat(objArray2);
        }
        str = str + "[";
        for (int i = 0; i < this._value.Length; i++)
        {
            ErlType type = this._value[i];
            if (type != null)
            {
                if (type is ErlArray)
                {
                    str = str + (type as ErlArray).getListArray();
                }
                else
                {
                    str = str + type.getValueString();
                }
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
        string str = this.transNumber();
        string str2 = string.Empty;
        if (str != null)
        {
            return (str2 + str);
        }
        return this.getListString();
    }

    public override bool isTag(int tag)
    {
        return (0x6c == tag);
    }

    public string transNumber()
    {
        if (!this.isString)
        {
            return null;
        }
        ByteBuffer buffer = new ByteBuffer();
        ErlByte num = null;
        ErlInt num2 = null;
        string str = null;
        string str2 = null;
        for (int i = 0; i < this._value.Length; i++)
        {
            num = this._value[i] as ErlByte;
            if ((num != null) && (num.Value > 0))
            {
                str2 = this.checkTrans(num.Value);
                if (str == null)
                {
                    str = string.Empty;
                }
                if (str2 != null)
                {
                    str = str + str2;
                }
                else
                {
                    char[] chArray1 = new char[] { (char) num.Value };
                    str = str + new string(chArray1);
                }
            }
            else
            {
                num2 = this._value[i] as ErlInt;
                if (((num2 != null) && (num2.Value > 0)) && (num2.Value < 0x7fffffff))
                {
                    str2 = this.checkTrans(num2.Value);
                    if (str == null)
                    {
                        str = string.Empty;
                    }
                    if (str2 != null)
                    {
                        str = str + str2;
                    }
                    else
                    {
                        str = str + char.ConvertFromUtf32(num2.Value);
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        return str;
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

