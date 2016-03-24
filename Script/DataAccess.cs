using System;
using System.Collections.Generic;
using NLog;
 

public class DataAccess : PortHandler
{
    private static DataAccess _dataAccess;
    private List<ErlEntry> _list = new List<ErlEntry>();
    public ReceiveFun defaultHandle;
    public const int DELAY = 0x1388;
    public Timer timeout;
    public const int TIMEOUT = 0x4e20;
	public readonly Logger Log = LogManager.GetLogger("DemoMain");

    public void access(ErlConnect connect, ErlKVMessage message, ReceiveFun receiveFun, List<object> argus, long timeOut)
    {
        ByteBuffer data = new ByteBuffer();
        message.bytesWrite(data);
        //this._list.Add(new ErlEntry(connect, message.getPort(), receiveFun, argus, timeOut + TimeKit.getMillisTime()));
        connect.sendErl(data, 1, 1, 1, 1);
         
    }
	public void send(ErlConnect connect, ErlKVMessage message, ReceiveFun receiveFun, List<object> argus, long timeOut)
	{
		ByteBuffer data = new ByteBuffer();
		message.bytesWrite(data);
		//Log.Info ("+++"+string.Concat (data.getArray ()));
		//this._list.Add(new ErlEntry(connect, message.getPort(), receiveFun, argus, timeOut + TimeKit.getMillisTime()));
		connect.sendErl(data, 0, 0, 0, 0);
		 
	}

	public void sendServer(ErlConnect connect, ErlKVMessage message, ReceiveFun receiveFun, List<object> argus, long timeOut)
	{
		ByteBuffer data = new ByteBuffer();
		message.bytesWrite(data);
		//Log.Info ("+++"+string.Concat (data.getArray ()));
		//this._list.Add(new ErlEntry(connect, message.getPort(), receiveFun, argus, timeOut + TimeKit.getMillisTime()));
		connect.sendErl(data, 1, 1, 1, 1);

	}

    public void clearDataAccess()
    {
        List<ErlEntry> list = this._list;
        lock (list)
        {
            this._list.Clear();
        }
    }

    public override void erlReceive(Connect connect, ErlKVMessage message)
    {
        if (!MiniConnectManager.IsRobot )
        {
            try
            {

               // Log.debug("===============this is socketReceive! cmd=" + message.Cmd + " jsonString " + message.toJsonString(),true);
            }
            catch (Exception exception)
            {
                //Log.debug(" =================== : " + exception.ToString(),true);
            }
        }
        string cmd = message.Cmd;
        switch (cmd)
        {
            case "r_ok":
            case "r_err":
            {
                int port = message.getPort();
                ErlEntry entry = this.removeReciveFun(port);
                if ((entry == null) || (entry.receiveFun == null))
                {
                    return;
                }
                entry.receiveFun(connect, message);
                break;
            }
            default:
                message.addValue("cmd", new ErlString(cmd));
                this.defaultHandle(connect, message);
                break;
        }
    }

    public static DataAccess getInstance()
    {
        if (_dataAccess == null)
        {
            _dataAccess = new DataAccess();
        }
        return _dataAccess;
    }

    public void onTimer()
    {
        try
        {
            List<ErlEntry> list = this._list;
            lock (list)
            {
                foreach (ErlEntry entry in this._list.ToArray())
                {
                    if (entry.timeOut <= TimeKit.getMillisTime())
                    {
                        this._list.Remove(entry);
                        if (entry.receiveFun != null)
                        {
                            ErlKVMessage message = new ErlKVMessage("r_timeOut");
                            List<object> argus = entry.argus;
                            this.defaultHandle(entry.connect, message);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    private ErlEntry removeReciveFun(int port)
    {
        for (int i = 0; i < this._list.Count; i++)
        {
            ErlEntry item = this._list[i];
            if (item.number == port)
            {
                this._list.Remove(item);
                return item;
            }
        }
        return null;
    }
}

