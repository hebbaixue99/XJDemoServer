using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class ConnectManager
{
    private static ConnectManager _manager;
    private ErlConnectFactory factory;
    private static string maxCmd;
    private static long maxTime;
    public ReceiveFun messageHandle;
    private static long startTime;

    public Connect beginConnect(string address, int port, CallBackHandle handlel, ConnectInvalidHandle invalidHandle)
    {
        return this.factory.beginConnect(address, port, handlel, invalidHandle);
    }
	public Connect beginConnect(Socket _socket)
	{
		ErlConnect connect = (this.factory.beginConnect(_socket)) as ErlConnect ;
		//connect.socket = _socket;
		byte[] b = connect.getCode ();
		connect.socket.Send (b);
		return connect;
	}
	public Connect transBeginConnect(Socket _socket)
	{
		ErlConnect connect = new ErlConnect();
		connect.socket = _socket;
		//byte[] b = connect.getCode ();
		//connect.socket.Send (b);
		return connect;
	}

    public void closeAllConnects()
    {
        this.factory.closeAllConnects();
    }

    public void closeConnect(string address, int port)
    {
        this.factory.closeConnect(address, port);
    }

    public int getConnectStatus(string address, int port)
    {
        ErlConnect connect = this.factory.checkInstance(address, port) as ErlConnect;
        if (connect == null)
        {
            return 1;
        }
        if (!connect.isActive)
        {
            return 2;
        }
        return 0;
    }

    public Connect getInstance(string address, int port)
    {
        return this.factory.getConnect(address, port);
    }

    public int getPing(string address, int port)
    {
        ErlConnect connect = this.factory.checkInstance(address, port) as ErlConnect;
        if (connect == null)
        {
            return 0;
        }
        return (int) connect.ping;
    }

    public void init(Object gameobject)
    {
		this.factory = new ErlConnectFactory ();  //gameobject.AddComponent(typeof(ErlConnectFactory)) as ErlConnectFactory;
         //this.factory.startTime();
    }

    public bool isActive(string address, int port)
    {
        ErlConnect connect = this.factory.checkInstance(address, port) as ErlConnect;
        if (connect == null)
        {
            return false;
        }
        return connect.isActive;
    }

    public static ConnectManager manager()
    {
        if (_manager == null)
        {
            _manager = new ConnectManager();
			_manager.init (null);
        }
        return _manager;
    }

    public void ping()
    {
        this.factory.ping();
    }

    public void sendMessage(string address, int port, ErlKVMessage message, ReceiveFun handle, List<object> argus)
    {
		ErlConnect connect = this.factory.getConnect(address, port) as ErlConnect;
        if (connect != null)
        {
            if (connect.isActive)
            {
                if (handle != null)
                {
                    long now = MiniConnectManager.now;
                    DataAccess.getInstance().access(connect, message, handle, argus, 0x4e20L);
                }
                else
                {
                    DataAccess.getInstance().access(connect, message, this.messageHandle, argus, 0x4e20L);
                }
            }
            else
            {
                //Log.error("connect error!" + connect.isActive);
                /*MonoBehaviour.print("connect error!" + connect.isActive);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnLostConnect(true);
                }*/
            }
        }
    }
}

