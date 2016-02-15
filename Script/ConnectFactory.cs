using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Net;
using System.Net.Sockets;
 

public class ConnectFactory 
{
    [CompilerGenerated]
    private static CallBackMsg f__am_cache6;
    private long collateDelay = 0x7530L;
    private Timer collateTimer;
    protected List<Connect> connectArray = new List<Connect>();
    private long pingDelay = 0x4e20L;
    private Timer pingTimer;
    private bool startPing;

    public Connect beginConnect(string localAddress, int localPort, CallBackHandle callBack, ConnectInvalidHandle invalidBack)
    {
        Connect item = this.checkInstance(localAddress, localPort);
        if (item == null)
        {
            item = this.openConnect(localAddress, localPort);
            this.connectArray.Add(item);
            item.CallBack = callBack;
            item.ConnectInvalidBack = invalidBack;
            return item;
        }
        callBack();
        return item;
    }
	public Connect beginConnect(Socket _socket)
	{
		String localAddress = (_socket.RemoteEndPoint as IPEndPoint).Address.ToString ();
		int localPort = (_socket.RemoteEndPoint as IPEndPoint).Port;
		Connect item = this.checkInstance(localAddress, localPort);
		if (item == null)
		{
			//item = this.openConnect(_socket);
			item = new ErlConnect();
			item.socket = _socket;
			this.connectArray.Add(item);
			//item.CallBack = callBack;
			//item.ConnectInvalidBack = invalidBack;
			return item;
		}
		//callBack();
		return item;
	}

    public Connect checkInstance(string localAddress, int localPort)
    {
        Connect[] connectArray = this.connectArray.ToArray();
        for (int i = connectArray.Length - 1; i >= 0; i--)
        {
            Connect item = connectArray[i];
            if (!item.Active)
            {
                this.connectArray.Remove(item);
            }
            else if (item.isSameConnect(localAddress, localPort))
            {
                return item;
            }
        }
        return null;
    }

    public void closeAllConnects()
    {
        Connect[] connectArray = this.connectArray.ToArray();
        for (int i = connectArray.Length - 1; i >= 0; i--)
        {
            Connect connect = connectArray[i];
            if (connect.Active)
            {
                connect.Dispose();
            }
        }
        this.connectArray.Clear();
    }

    public void closeConnect(string localAddress, int localPort)
    {
        Connect[] connectArray = this.connectArray.ToArray();
        for (int i = connectArray.Length - 1; i >= 0; i--)
        {
            Connect item = connectArray[i];
            if (!item.Active)
            {
                this.connectArray.Remove(item);
            }
            else if (item.isSameConnect(localAddress, localPort))
            {
                item.Dispose();
                this.connectArray.Remove(item);
            }
        }
    }

    protected void collate()
    {
        long num = TimeKit.getMillisTime();
        Connect[] connectArray = this.connectArray.ToArray();
        for (int i = connectArray.Length - 1; i >= 0; i--)
        {
            Connect item = connectArray[i];
            if (item.Active && (num >= (item.TimeOut + item.ActiveTime)))
            {
                item.Dispose();
                this.connectArray.Remove(item);
            }
        }
    }

    private void FixedUpdate()
    {
        this.run();
    }

    public Connect getConnect(string localAddress, int localPort)
    {
        Connect item = this.checkInstance(localAddress, localPort);
        if (item == null)
        {
            item = this.openConnect(localAddress, localPort);
            this.connectArray.Add(item);
        }
        return item;
    }

    public virtual Connect openConnect(string localAddress, int localPort)
    {
        Connect connect = new Connect();
        connect.open(localAddress, localPort);
        return connect;
    }

	 

    public virtual void ping()
    {
        long num = TimeKit.getMillisTime();
        Connect[] connectArray = this.connectArray.ToArray();
        for (int i = connectArray.Length - 1; i >= 0; i--)
        {
            Connect connect = connectArray[i];
            if (connect.Active)
            {
                if (connect.PingTime == 0)
                {
                    connect.PingTime = num;
                    ByteBuffer data = new ByteBuffer();
                    data.writeShort(1);
                    data.writeByte(1);
                    connect.send(data);
                }
                else
                {
                    connect.ping = num - connect.PingTime;
                }
            }
        }
    }

    protected virtual void pingHandle(ErlConnect connect, ErlKVMessage erlMessage)
    {
    }

    private void receive()
    {
        Connect[] connectArray = this.connectArray.ToArray();
        for (int i = connectArray.Length - 1; i >= 0; i--)
        {
            connectArray[i].receive();
        }
    }

    public void removeConnect(Connect connect)
    {
        this.connectArray.Remove(connect);
    }

    public void run()
    {
        try
        {
            this.receive();
        }
        catch (Exception exception)
        {
            //Debug.LogWarning(exception);
			//Log.debug(exception);
            if (f__am_cache6 == null)
            {
                //f__am_cache6 = msg => GameManager.Instance.logOut();
            }
           // SystemMessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0105"), f__am_cache6);
        }
    }

    public void startTime()
    {
        this.pingTimer = TimerManager.Instance.getTimer(this.pingDelay);
        this.pingTimer.addOnTimer(new TimerHandle(this.ping));
        this.pingTimer.start();
        this.collateTimer = TimerManager.Instance.getTimer(this.collateDelay);
        this.collateTimer.addOnTimer(new TimerHandle(this.collate));
        this.collateTimer.start();
    }
}

