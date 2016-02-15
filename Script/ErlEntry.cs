using System;
using System.Collections.Generic;

public class ErlEntry
{
    public List<object> argus;
    public Connect connect;
    public int number;
    public ReceiveFun receiveFun;
    public long timeOut;

    public ErlEntry(Connect connect, int number, ReceiveFun receiveFun, List<object> argus, long timeOut)
    {
        this.connect = connect;
        this.number = number;
        this.receiveFun = receiveFun;
        this.argus = argus;
        this.timeOut = timeOut;
    }
}

