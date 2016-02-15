using System;
using NLog;
 

public class BaseFPort
{
	private CallBack callback;
	public static bool flag;
	protected bool lockUI = true;
	public readonly Logger Log = LogManager.GetLogger("BaseFPort");

	public void access(ErlKVMessage message , String host, int port)
	{ 
		ConnectManager.manager().sendMessage(host, port, message, new ReceiveFun(this.receive), null); 
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

	public void send(ErlKVMessage message ,string host,int port)
	{
		ConnectManager.manager().sendMessage(host, port, message, null, null);
	}
}

