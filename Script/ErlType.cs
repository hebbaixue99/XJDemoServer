using System;

public class ErlType : ErlBytesReader, ErlBytesWriter
{
    protected int _tag;

    public virtual void bytesRead(ByteBuffer data)
    {
        int position = data.position;
        this._tag = data.readByte();
		if (this._tag == 0) {
			data.position = position + 2;
			this._tag = data.readByte ();
		}
        if (!this.isTag(this._tag))
        {
            data.position = position;
        }
    }

    public virtual void bytesWrite(ByteBuffer data)
    {
    }

    public virtual string getString(object key)
    {
        return string.Empty;
    }

    public virtual string getValueString()
    {
        return string.Empty;
    }

    public virtual bool isTag(int tag)
    {
        return (this._tag == tag);
    }

    public virtual void writeToJson(object key, object jsonObj)
    {
    }
}

