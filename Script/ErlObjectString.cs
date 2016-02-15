using System;

public class ErlObjectString : ErlType
{
    public const int TAG = 110;

    public ErlObjectString(string _value)
    {
    }

    public override bool isTag(int tag)
    {
        return (110 == tag);
    }

    public override void writeToJson(object key, object jsonObj)
    {
    }
}

