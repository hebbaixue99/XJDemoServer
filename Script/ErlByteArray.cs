using System;

public class ErlByteArray : ErlType
{
    private ErlType _erlValue;
    private ByteBuffer _value;
    public const int TAG = 0x6d;

    public ErlByteArray(ByteBuffer buffer)
    {
        if (buffer != null)
        {
            this._value = buffer;
        }
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            int num = data.readInt();
            this._value = new ByteBuffer();
            this._value.writeBytes(data, 0, (uint) num);
            this._value.position = 0;
            data.position += num;
        }
        ByteBuffer buffer2 = new ByteBuffer(ZIPUtil.Decompress(this._value.toArray()));
        buffer2.readByte();
        this._erlValue = ByteKit.complexAnalyse(buffer2);
    }

    public override void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        if ((this._value == null) || (this._value.length() < 1))
        {
            new ErlNullList().bytesWrite(data);
        }
        else
        {
            data.writeShort(this._value.length() + 1);
            data.writeByte(0x6d);
            data.writeBytes(this._value, 0, (uint) this._value.length());
        }
    }

    public override string getString(object key)
    {
        if (this._erlValue is ErlArray)
        {
            return (this._erlValue as ErlArray).getString(key);
        }
        if (this._erlValue is ErlList)
        {
            return (this._erlValue as ErlList).getString(key);
        }
        object[] objArray1 = new object[] { '"', key.ToString(), '"', ":" };
        return (string.Concat(objArray1) + "[]");
    }

    public override string getValueString()
    {
        string str = "[";
        for (int i = 0; i < this._value.length(); i++)
        {
            int num2 = this._value.readByte();
            str = str + (((num2 >> 4) & 15)).ToString() + ((num2 & 15)).ToString();
            if (i < (this._value.length() - 1))
            {
                str = str + ",";
            }
        }
        return (str + ']');
    }

    public override bool isTag(int tag)
    {
        return (0x6d == tag);
    }

    public void simpleBytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            data.position -= 3;
            int num = data.readUnsignedShort();
            data.readByte();
            this._value = new ByteBuffer();
            this._value.writeBytes(data, 0, (uint) (num - 1));
            this._value.position = 0;
            data.position += num - 1;
        }
        ByteBuffer buffer2 = new ByteBuffer(ZIPUtil.Decompress(this._value.toArray()));
        buffer2.readByte();
        this._erlValue = ByteKit.complexAnalyse(buffer2);
    }

    public byte[] toArray()
    {
        if (this._value == null)
        {
            return null;
        }
        this._value.setOffset(0);
        int num = this._value.readLength();
        byte[] buffer = new byte[num];
        for (int i = 0; i < num; i++)
        {
            buffer[i] = this._value.read(i);
        }
        return buffer;
    }

    public string toUTFString()
    {
        this._value.setOffset(0);
        return this._value.readUTF();
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }

    public ErlType ErlValue
    {
        get
        {
            return this._erlValue;
        }
        set
        {
            this._erlValue = value;
        }
    }

    public ByteBuffer Value
    {
        get
        {
            this._value.setOffset(0);
            return this._value;
        }
        set
        {
            this._value = value;
        }
    }
}

