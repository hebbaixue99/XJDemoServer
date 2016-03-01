using System;
using System.Net.Sockets;
using System.Threading;

public class ClientSocketPort : BaseFPort
{
	private CallBack callback;
	public void open(string IP, int port)
	{
		this.erlConnect = (ErlConnect)ConnectManager.manager().beginConnect(IP, port, new CallBackHandle(this.openSuccess),null);
		if (this.erlConnect != null) {
			//this.erlConnect.CallBack ();
			Thread receiveClientThread = new Thread (openSuccess);  
			receiveClientThread.Start ();  
			this.login("http://123.59.34.161:7612?game_platform=1&game_server=36&userid=360IOS_28832604&nickname=&face=&time=1453099295&callback=&sig=478934c26f3adef5074fbe2b34f462b&isfangchenmi=0&platform=360IOS&bi_platform_id=222",1036,"ios","360IOS_28832604","2.0.0",new CallBack(this.loginOk));
		}

	}
	public void login(string url, int sid, string deviceId, string dns, string appVersion, CallBack callback)
	{
		//this.initUser = callback;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage("/yxzh/user_login");
		message.addValue("url", new ErlString(url));
		message.addValue("server", new ErlInt(sid));
		message.addValue("deviceId", new ErlString(deviceId));
		message.addValue("dns", new ErlString(dns));
		message.addValue("version", new ErlString(appVersion));
		base.access(message);
	}
	public void loginOk()
	{
		Log.Info ("loginOk");
	}
	public static string getSocketStr( Socket sk)
	{
		if (sk == null) {
			return "[]";
		}
		return "["+sk.LocalEndPoint.ToString () + "->" + sk.RemoteEndPoint.ToString ()+"]";
	}

	private void openSuccess()
	{
		Log.Info (getSocketStr(this.erlConnect.socket) + this.erlConnect.socket.Available);
		while (true) {  
			try {  
				//通过clientSocket接收数据  
				 
				if (this.erlConnect.socket.Available > 0) {
					this.erlConnect.receive ();
					//transToTargetData (tcs);
				} 
				Thread.Sleep (2);
			} catch (Exception ex) {  
				Log.Error (ex.Message);  
				this.erlConnect.socket.Shutdown (SocketShutdown.Both);  
				this.erlConnect.socket.Close ();  
				break;  
			}  
		} 
	}
	public void access(CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage("/yxzh/role/get_user");
		base.access(message);
	}
	public void access(ErlKVMessage message, CallBack callback)
	{
		this.callback = callback;
		//ErlKVMessage message = new ErlKVMessage("/yxzh/role/get_user");
		base.access(message);
	}

	public void parseKVMsg(ErlKVMessage message)
	{
		Log.Info(message.Cmd+"|"+ message.toJsonString());
	}

	private int[] parseVipAwardSids(ErlArray sids)
	{
		int[] numArray = new int[sids.Value.Length];
		for (int i = 0; i < sids.Value.Length; i++)
		{
			numArray[i] = StringKit.toInt(sids.Value[i].getValueString());
		}
		return numArray;
	}

	public override void read(ErlKVMessage message)
	{
		this.parseKVMsg(message);
		this.callback();
	}
}

