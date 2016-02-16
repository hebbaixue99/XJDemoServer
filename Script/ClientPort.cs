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
		Log.Info (str);
		switch (str) {
		case "/yxzh/user_login":
			ErlKVMessage _message = new ErlKVMessage ("r_ok");
			//_message.addValue (null, new ErlByte (1));
			_message.addValue ("msg", new ErlString ("login_ok"));
			//base.send (message); 
			//ConnectManager.manager ().sendMessage ();
			DataAccess.getInstance ().send (this.erlConnect, _message, null, null, 0x4e20L);
			 
			ErlKVMessage m1 = new ErlKVMessage ("guild");
			m1.addValue("guild_skill",new ErlString("sdsf"));
			DataAccess.getInstance ().send (this.erlConnect, m1, null, null, 0x4e20L);  

			ErlKVMessage m2 = new ErlKVMessage ("chat");
			m2.addValue("msg2",new ErlString("sdsf"));
			DataAccess.getInstance ().send (this.erlConnect, m2, null, null, 0x4e20L);
			break;
		case "echo":
			ErlKVMessage _messag = new ErlKVMessage ("r_ok");
			_messag.addValue (null, new ErlByte(1));
			 
			//base.send (message); 
			//ConnectManager.manager ().sendMessage ();
			DataAccess.getInstance().send (this.erlConnect, _messag, null, null, 0x4e20L);
			break;
		case "/yxzh/role/get_user":
			ErlKVMessage get_user = this.getUserInfo ();
			 
			DataAccess.getInstance().send(this.erlConnect, get_user, null, null, 0x4e20L);
			break;
		default:
			
			break;


		}
	}

	private ErlKVMessage getUserInfo()
	{
		ErlKVMessage user = new ErlKVMessage ("r_ok");
		ErlArray ua = new ErlArray (new ErlType[1]);
		ErlArray  a = new ErlArray (new ErlType[3]);
		 
		a.Value [0] = new ErlString("sdfdsssssssss");
		a.Value [1] = new ErlInt (1);
		a.Value [2] = new ErlInt (1);
		ua.Value [0] = a;
		user.addValue("msg",ua);
		//string,list(int,int),byte,byte,int,int,string,int,byte,byte,byte,byte,int,
		//13-20byte,string,list(int,int,int,int),23-29byte,string,byte,int,byte,array(0),int
		//int,byte,int,int,int,byte,byte,byte,int
		//user.addValue ("msg", "");
		return user ;
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

