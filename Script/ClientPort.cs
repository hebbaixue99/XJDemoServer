using System;
using System.Runtime.CompilerServices;
using NLog;
using System.Net.Sockets;

public class ClientPort : BaseFPort
{
	//public readonly Logger Log = LogManager.GetLogger("ClientPort");
	private CallBack _callBack;

	public ClientPort(ErlConnect _erlConnect)
	{
		this.erlConnect = _erlConnect;
		this.erlConnect.portHandler = this;
	}
	public void closeContect()
	{
		ErlKVMessage message = new ErlKVMessage("/yxzh/close");
		base.send(message);
	}

	public void login(string platform, string server, string name, CallBack callback)
	{
		this._callBack = callback;
		ErlKVMessage message = new ErlKVMessage("/yxzh/user_login");
		message.addValue("platform", new ErlInt(StringKit.toInt(platform)));
		message.addValue("server", new ErlInt(StringKit.toInt(server)));
		message.addValue("userid", new ErlString(name));
		base.access(message);
	}

	public void login(string url, int sid, string deviceId, string dns, string appVersion, CallBack callback)
	{
		this._callBack = callback;
		ErlKVMessage message = new ErlKVMessage("/yxzh/user_login");
		message.addValue("url", new ErlString(url));
		message.addValue("server", new ErlInt(sid));
		message.addValue("deviceId", new ErlString(deviceId));
		message.addValue("dns", new ErlString(dns));
		message.addValue("version", new ErlString(appVersion));
		base.access(message);
	}

	public void login(string uid, string vip, string platform, string serverid, string time, string inviteuser, string sig, CallBack callback)
	{
		this._callBack = callback;
		ErlKVMessage message = new ErlKVMessage("/yxzh/user_login");
		message.addValue("userid", new ErlString(uid));
		message.addValue("vip", new ErlString(vip));
		message.addValue("time", new ErlString(time));
		message.addValue("platform", new ErlString(platform));
		message.addValue("server", new ErlString(serverid));
		message.addValue("inviteuser", new ErlString(inviteuser));
		base.access(message);
	}

	public void loginGM(string id, int sid, CallBack callback)
	{
		this._callBack = callback;
		ErlKVMessage message = new ErlKVMessage("/yxzh/trust");
		message.addValue("userid", new ErlString(id));
		message.addValue("server", new ErlInt(sid));
		base.access(message);
	}

	public void receive()
	{
		base.erlConnect.receive ();
	}
	public override void erlReceive(Connect connect, ErlKVMessage message)
	{
		this.readMessage (message);
	}

	public void readMessage(ErlKVMessage message)
	{
		string str = message.Cmd;
		switch (str) {
		case "/yxzh/user_login":
			ErlKVMessage _message = new ErlKVMessage ("r_ok");
			_message.addValue (null, 1);
			_message.addValue ("msg", new ErlString ("login_ok"));
			//base.send (message); 
			//ConnectManager.manager ().sendMessage ();
			DataAccess.getInstance().access(this.erlConnect, _message, null, null, 0x4e20L);
			break;
		case "echo":
			ErlKVMessage _messag = new ErlKVMessage ("r_ok");
			_messag.addValue (null, 2);
			 
			//base.send (message); 
			//ConnectManager.manager ().sendMessage ();
			DataAccess.getInstance().access(this.erlConnect, _messag, null, null, 0x4e20L);
			break;
		default:
			break;

		}
	}

	public override void read(ErlKVMessage message)
	{
		string str = (message.getValue("msg") as ErlType).getValueString();
		switch (str)
		{
		case "login_ok":
			 
			Log.Warn ("LOGIN_LOGIN_OK");
			this._callBack();
			break;

		case "info_error":
			Log.Warn ("LOGIN_INFO_ERROR");
			break;

		case "create_user_ok":
			Log.Warn ("LOGIN_CREATE_USER_OK");
			break; 

		case "no_user":
			Log.Warn ("LOGIN_NO_USER");
			break;

		case "password_error":
			Log.Warn ("LOGIN_PASSWORD_ERROR");
			break;

		case "no_role":
			{
				Log.Warn ("LOGIN_NO_ROLE");
				ErlArray names = message.getValue("names") as ErlArray;
				 
				break;
			}
		default:
			if (str == "relogin_ok")
			{
				Log.Warn ("LOGIN_RELOGIN_OK");
				this._callBack();
			}
			else if (str == "limit_count")
			{
				Log.Warn ("LOGIN_COUNT");
				//MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("serverState01"));
				//ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
			}
			 
			break;
		}
	}
}

