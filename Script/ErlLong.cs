using System;

public class ErlLong : ErlType
{
    private long _value;
    public const int TAG = 110;

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            int len = data.readUnsignedByte();
            int num2 = data.readUnsignedByte();
            byte[] bytes = new byte[8];
            data.readBytes(bytes, 0, len);
            this._value = BitConverter.ToInt64(bytes, 0);
            if (num2 == 1)
            {
                this._value = -this._value;
            }
        }
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
        return (110 == tag);
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }

    public long Value
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

