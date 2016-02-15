using System;

public class ErlDouble : ErlType
{
    private double _value;
    public const int TAG = 0x62;

    public ErlDouble(double _value)
    {
        this._value = _value;
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            this._value = data.readDouble();
        }
    }

    public override void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        data.writeByte(0x62);
        data.writeDouble(this._value);
    }

    public override bool isTag(int tag)
    {
        return (0x62 == tag);
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }

    public double value
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

