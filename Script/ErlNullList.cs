using System;

public class ErlNullList : ErlType
{
    public const int TAG = 0x6a;

    public override void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        data.writeByte(0x6a);
    }

    public override string getString(object key)
    {
        return (key.ToString() + ":[]");
    }

    public override string getValueString()
    {
        return "[]";
    }

    public override bool isTag(int tag)
    {
        return (0x6a == tag);
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }
}

