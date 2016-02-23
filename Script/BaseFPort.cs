using System;
using NLog;
using System.Net.Sockets;
using System.Collections.Generic;
 

public class BaseFPort:PortHandler
{
	 
	private string _host="127.0.0.1";
	private int _port=7612;
	private ErlConnect _erlConnect;
	//private CallBack callback;
	public static bool flag;
	protected bool lockUI = true;
	public static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();
	public static Dictionary<string, BaseFPort> portDict = new Dictionary<string, BaseFPort> () {
		{ "/yxzh/user_login", new LoginPort { } },
		{ "/yxzh/role/get_user,/yxzh/get_online_time,/yxzh/get_stone_lv,/yxzh/get_user_switch,/yxzh/role/get_user_new", new UserPort { } },
		{ "/yxzh/storage/get_storage", new StoragePort { } },
		{ "/yxzh/fb/get_fbinfo", new FbPort { } },
		{ "/yxzh/guide/get", new GuidePort { } }, 
		{ "/yxzh/labyrinth/get_info", new LabyrinthPort { } } ,
		{ "/yxzh/cash/get_cash_list_fore", new CashPort { } },
		{ "/yxzh/arena/get_final,/yxzh/supreme_port/get_base_info,/yxzh/mystic_shop/get_count,/yxzh/relations/get_relations,/yxzh/share/get_share", new ArenaPort { } },
		{ "/yxzh/mail/get_mails", new MailPort { } } ,
		{ "/yxzh/guild/builds_level,/yxzh/guild/get_guild_skill", new GuildPort { } } ,
		{ "/yxzh/fashion_port/get_fashion,/yxzh/furnace_port/get_info", new FashionPort { } } ,
		{ "/yxzh/pyx_info", new PyxPort { } },
		{ "active_port" , new ActivePort{} } ,
		{ "/yxzh/title/get_title" , new TitlePort{} }
	};
	 

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
	public void send (ErlConnect connect , ErlKVMessage message)
	{
		DataAccess.getInstance().send(connect, message, null, null, 0x4e20L);
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

