using System;

public class ErlAtom : ErlType
{
    private string _value;
    public const int TAG = 100;

    public ErlAtom(string _value)
    {
        this._value = this.Value;
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            this._value = string.Empty;
            int num = data.readShort();
            for (int i = 0; i < num; i++)
            {
                this._value = this._value + ((char) data.readUnsignedByte());
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
            data.writeByte(100);
            data.writeShort(this._value.Length);
            for (int i = 0; i < this._value.Length; i++)
            {
            }
        }
    }

    public override string getString(object key)
    {
        object[] objArray1 = new object[] { '"', key.ToString(), '"', ":", '"' };
        return (string.Concat(objArray1) + this._value + '"');
    }

    public override string getValueString()
    {
        return this._value;
    }

    public override bool isTag(int tag)
    {
        return (100 == tag);
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }

    public string Value
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

