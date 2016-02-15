using System;

public class ConnectCount
{
    private static ConnectCount _count;
    private int _number;

    public static ConnectCount getInstance()
    {
        if (_count == null)
        {
            _count = new ConnectCount();
        }
        return _count;
    }

    public int number
    {
        get
        {
            if (this._number >= 0x7fffffff)
            {
                this._number = 0;
            }
            else
            {
                this._number++;
            }
            return this._number;
        }
    }
}

