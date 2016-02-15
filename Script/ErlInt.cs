using System;

public class ErlInt : ErlType
{
    private int _value;
    public const int TAG = 0x62;

    public ErlInt(int value)
    {
        this._value = value;
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            this._value = data.readInt();
        }
    }

    public override void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        data.writeShort(5);
        data.writeByte(0x62);
        data.writeInt(this._value);
    }

    public override string getString(object key)
    {
        if (key == null)
        {
            return ("empty:" + this._value);
        }
        return (key.ToString() + ':' + this._value);
    }

    public override string getValueString()
    {
        return (string.Empty + this._value);
    }

    public override bool isTag(int tag)
    {
        return (0x62 == tag);
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }

    public int Value
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

