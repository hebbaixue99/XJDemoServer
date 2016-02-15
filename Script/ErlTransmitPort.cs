using System;

public class ErlTransmitPort : TransmitPort
{
    private ReceiveFun receiveFunction;

    public override void erlReceive(Connect connect, ErlKVMessage message)
    {
        if (this.receiveFunction != null)
        {
            this.receiveFunction(connect, message);
        }
    }

    public ReceiveFun ReceiveFunction
    {
        get
        {
            return this.receiveFunction;
        }
        set
        {
            this.receiveFunction = value;
        }
    }
}

