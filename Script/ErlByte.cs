using System;

public class ErlByte : ErlType
{
    private int _value;
    public const int TAG = 0x61;

    public ErlByte(int value)
    {
        this._value = value;
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            this._value = data.readUnsignedByte();
        }
    }

    public override void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        data.writeShort(2);
        data.writeByte(0x61);
        data.writeByte(this._value);
    }

    public override string getString(object key)
    {
        if (key == null)
        {
            object[] objArray1 = new object[] { '"', "empty", '"', ":", this._value };
            return string.Concat(objArray1);
        }
        object[] objArray2 = new object[] { '"', key.ToString(), '"', ":", this._value };
        return string.Concat(objArray2);
    }

    public override string getValueString()
    {
        return (string.Empty + this._value);
    }

    public override bool isTag(int tag)
    {
        return (0x61 == tag);
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

