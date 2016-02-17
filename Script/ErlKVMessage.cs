using System;
using System.Collections.Generic;
using NLog;

public class ErlKVMessage
{
    private string _cmd;
    private List<string> _key;
    private List<object> _value;
    private string jsonString = string.Empty;
    public const int VER = 0x83;
	public readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public ErlKVMessage(string cmd)
    {
        this._cmd = cmd;
        this._key = new List<string>();
        this._value = new List<object>();
    }

    public void addValue(string key, object Value)
    {
        int index = this._key.IndexOf(key);
        if (index >= 0)
        {
            this._value[index] = Value;
        }
        else
        {
            this._key.Add(key);
            this._value.Add(Value);
        }
    }

    public void bytesRead(ByteBuffer data)
    {
        this._cmd = this.bytesReadKey(data);
        this.bytesReadInfo(data);
    }

    public void bytesReadInfo(ByteBuffer data)
    {
        try
        {
            while (data.position < data.top)
            {
                this.addValue(this.bytesReadKey(data), this.bytesReadValue(data));
            }
        }
        catch
        {
        }
    }

    public string bytesReadKey(ByteBuffer data)
    {
        int num = data.readUnsignedByte();
        string str = null;
        for (int i = 0; i < num; i++)
        {
            if (str == null)
            {
                str = string.Empty;
            }
            str = str + ((char) data.readUnsignedByte());
        }
        return str;
    }

    public object bytesReadValue(ByteBuffer data)
    {
        uint num = (uint) data.readUnsignedShort();
        uint position = (uint) data.position;
		uint tag = (uint)data.readUnsignedByte ();
		if (tag == 0x83||tag==0x68||tag==0x69)
         {
			//data.position = (int) position;
			return ByteKit.complexAnalyse(data);
        }
        data.position = (int) position;
        return ByteKit.simpleAnalyse(data);
		//return ByteKit.complexAnalyse(data);
    }

    public void bytesWrite(ByteBuffer data)
    {
        this.addValue(null, new ErlInt(ConnectCount.getInstance().number));
        this.bytesWriteKey(data, this._cmd);
        this.bytesWriteInfo(data);
    }

    public void bytesWriteInfo(ByteBuffer data)
    {
        for (int i = 0; i < this._key.Count; i++)
        {
            this.bytesWriteKey(data, this._key[i]);
            this.bytesWriteValue(data, this._value[i]);
        }
    }

    public void bytesWriteKey(ByteBuffer data, string key)
    {
        if (key != null)
        {
            byte[] b = StringKit.DefaultToUTF8Byte(key);
            data.writeByte(b.Length);
            data.writeBytes(b);
        }
        else
        {
            data.writeByte(0);
        }
    }

    public void bytesWriteValue(ByteBuffer sc_data, object value)
    {
        ErlType type = value as ErlType;
        if (type == null)
        {
            type = new ErlNullList();
        }
        ByteBuffer data = new ByteBuffer();

		if (type.GetType().ToString() == "ErlArray") {
			data.writeByte((byte)0x83);
			Log.Info (type.GetType ().ToString());
		}
        type.bytesWrite(data);
        sc_data.writeBytes(data, 0, data.bytesAvailable);
    }

    public int getPort()
    {
        ErlInt num = this.getValue(null) as ErlInt;
        if (num != null)
        {
            return num.Value;
        }
        ErlByte num2 = this.getValue(null) as ErlByte;
        if (num2 != null)
        {
            return num2.Value;
        }
        return 0;
    }

    public object getValue(string key)
    {
        int index = this._key.IndexOf(key);
        if (index >= 0)
        {
            return this._value[index];
        }
        return null;
    }

    public string toJsonString()
    {
        if (this.jsonString == string.Empty)
        {
            this.jsonString = "{";
            for (int i = 0; i < this.Key.Count; i++)
            {
                ErlType type = this.Value[i] as ErlType;
                if (type != null)
                {
                    type.getString(this._key[i]);
                    this.jsonString = this.jsonString + type.getString(this.Key[i]);
                    if (i < (this.Key.Count - 1))
                    {
                        this.jsonString = this.jsonString + ",";
                    }
                }
            }
            this.jsonString = this.jsonString + "}";
        }
        return this.jsonString;
    }

    public string toString()
    {
        object[] objArray1 = new object[] { "[_cmd=", this._cmd, ",_key=", this._key, ",_value=", this._value, "]" };
        return string.Concat(objArray1);
    }

    public string Cmd
    {
        get
        {
            return this._cmd;
        }
        set
        {
            this._cmd = value;
        }
    }

    public List<string> Key
    {
        get
        {
            return this._key;
        }
    }

    public List<object> Value
    {
        get
        {
            return this._value;
        }
    }
}

