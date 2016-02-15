using System;
using System.Collections.Generic;

public class TransmitPort : PortHandler
{
    private List<string> _portNameArray = new List<string>();
    private List<ReceiveFun> _receiveFunArray = new List<ReceiveFun>();

    public void addPort(string portName, ReceiveFun receiveFun)
    {
        int index = this._portNameArray.IndexOf(portName);
        if (index >= 0)
        {
            this._receiveFunArray[index] = receiveFun;
        }
        else
        {
            this._portNameArray.Add(portName);
            this._receiveFunArray.Add(receiveFun);
        }
    }

    public ReceiveFun getPort(string portName)
    {
        int index = this._portNameArray.IndexOf(portName);
        if (index >= 0)
        {
            return this._receiveFunArray[index];
        }
        return null;
    }

    public override void receive(Connect connect, ByteBuffer data)
    {
        string item = data.readUTF();
        int index = this._portNameArray.IndexOf(item);
        if (index >= 0)
        {
            ReceiveFun fun = this._receiveFunArray[index];
            fun(connect, data);
        }
    }
}

