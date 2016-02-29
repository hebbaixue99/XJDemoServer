using System;

public class ErlString : ErlType
{
    private ByteBuffer _byteArray = new ByteBuffer();
    protected string _value;
    public const int TAG = 0x6b;

    public ErlString(string _value)
    {
        this._value = _value;
    }

    public override void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
            data.position -= 3;
            int len = data.readUnsignedShort() - 1;
            data.readByte();
            uint position = (uint) data.position;
            this._byteArray.clear();
            data.readBytes(this._byteArray, 0, len);
            this._value = this._byteArray.readUTFBytes(len);
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
			ByteBuffer b = new ByteBuffer();
			data.writeBytes(b, 0, b.bytesAvailable);
			b.writeUTFBytes (this._value);
			if (base.isServer) {
				data.writeByte (0x6b);
				data.writeShort(b.top );
			} else {
				data.writeShort(b.top + 1);
				data.writeByte (0x6b);
			}

			data.writeBytes(b, 0, b.bytesAvailable);
		}
	}

    public string getASCII()
    {
        return this.getASCII(false);
    }

    public string getASCII(bool resetPosition)
    {
        this._byteArray.position = 0;
        string str = "[";
        while (this._byteArray.bytesAvailable > 0)
        {
            str = str + this._byteArray.readUnsignedByte();
            if (this._byteArray.bytesAvailable > 0)
            {
                str = str + ",";
            }
        }
        return (str + "]");
    }

    public override string getString(object key)
    {
        this._byteArray.position = 0;
        if ((this._byteArray.bytesAvailable > 0) && (this._byteArray.readUnsignedByte() == 0))
        {
            return (key.ToString() + ':' + this.getASCII());
        }
        if ((this._value == null) || (this._value.Length <= 0))
        {
            return (key.ToString() + ':' + " ");
        }
        object[] objArray1 = new object[] { key.ToString(), ':', this._value, ' ' };
        return string.Concat(objArray1);
    }

    public override string getValueString()
    {
        this._byteArray.position = 0;
        if ((this._byteArray.bytesAvailable > 0) && (this._byteArray.readUnsignedByte() == 0))
        {
            return this.getASCII();
        }
        if ((this._value != null) && (this._value.Length > 0))
        {
            return this._value;
        }
        return string.Empty;
    }

    public override bool isTag(int tag)
    {
        return (0x6b == tag);
    }

    public void sampleBytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (this.isTag(base._tag))
        {
			//data.position = data.position - 3;
            uint num = (uint) data.readUnsignedShort();
            this._value = string.Empty;
            uint position = (uint) data.position;
            this._byteArray.clear();
            data.readBytes(this._byteArray, 0, (int) num);
            data.position = (int) position;
            this._value = this._value + data.readUTFBytes((int) num);
        }
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

