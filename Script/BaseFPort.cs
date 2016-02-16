using System;
using NLog;
using System.Net.Sockets;
 

public class BaseFPort:PortHandler
{
	private string _host="127.0.0.1";
	private int _port=7612;
	private ErlConnect _erlConnect;
	//private CallBack callback;
	public static bool flag;
	protected bool lockUI = true;
	public readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();
	public BaseFPort()
	{
	}
	public BaseFPort(ErlConnect _socket)
	{
		this._erlConnect = _socket;
	}
	public void access(ErlKVMessage message)
	{ 
		ConnectManager.manager().sendMessage(this._host, this._port, message, new ReceiveFun(this.receive), null); 
	}

	public int Int(ErlType data)
	{
		return StringKit.toInt(data.getValueString());
	}

	public virtual void read(ErlKVMessage message)
	{
	}

	public void receive(Connect c, object obj)
	{
		 
		try
		{
			this.read(obj as ErlKVMessage);
		}
		catch (Exception exception)
		{
			
			//Debug.LogError(string.Concat(new object[] { exception.Message, "\n", exception.Data, "\n", exception.StackTrace }));
			Log.Error(string.Concat(new object[] { exception.Message, "\n", exception.Data, "\n", exception.StackTrace }));
			throw exception;
		}
	}

	public void send(ErlKVMessage message )
	{
		ConnectManager.manager().sendMessage(this._host, this._port, message, null, null);
	}
	public ErlConnect erlConnect
	{
		get
		{
			return this._erlConnect;
		}
		set
		{
			this._erlConnect = value;
		}
	}
	public string host
	{
		get
		{
			return this._host;
		}
		set
		{
			this._host = value;
		}
	}
	public int port
	{
		get
		{
			return this._port;
		}
		set
		{
			this._port = value;
		}
	}
}

