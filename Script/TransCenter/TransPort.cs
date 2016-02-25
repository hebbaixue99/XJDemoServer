using System;
using System.Runtime.CompilerServices;
using NLog;
using System.Net.Sockets;

public class TransPort : BaseFPort
{
	//public readonly Logger Log = LogManager.GetLogger("ClientPort");
	//private CallBack _callBack;

	public TransPort (ErlConnect _erlConnect)
	{
		this.erlConnect = _erlConnect;
		this.erlConnect.portHandler = this;
	}

	public void closeContect ()
	{
		ErlKVMessage message = new ErlKVMessage ("/yxzh/close");
		base.send (message);
	}

	public void receive (ByteBuffer data, bool isServer)
	{
		base.erlConnect.TransReceive (data, isServer);
		//base.erlConnect.receive();
	}

	public override void erlReceive (Connect connect, ErlKVMessage message)
	{
		//this.readMessage (message);

		Log.Info(message.toJsonString());
	}

	public void readMessage (ErlKVMessage message)
	{
		string str = message.Cmd;
		Log.Info (str);
		Log.Info (message.toJsonString ());
		BaseFPort bf = null;
		foreach (string key in BaseFPort.portDict.Keys) {
			if (key.Contains (str)||str.Contains(key)) {
				bf = BaseFPort.portDict [key];
			}
		}
		if (bf != null) {
			bf.erlConnect = this.erlConnect;
			bf.read (message);
		} else {
			switch (str) { 
			case "echo":
				ErlKVMessage _messag = new ErlKVMessage ("r_ok");
				_messag.addValue (null, new ErlInt (message.getPort ()));
				base.send (this.erlConnect, _messag);
				break;
			case "/yxzh/home_port/get_info":
				ErlKVMessage msg = new ErlKVMessage ("r_ok");
				msg.addValue (null, new ErlInt (message.getPort ()));
				msg.addValue ("msg", new ErlAtom ("home_not_open"));
				base.send (this.erlConnect, msg);
				break;
			case "/yxzh/ww_guild_child_fore_port/get_switch":
				ErlKVMessage msg1 = new ErlKVMessage ("r_ok");
				msg1.addValue (null, new ErlInt (message.getPort ()));
				msg1.addValue ("msg", new ErlByte (1));
				base.send (this.erlConnect, msg1);
				break;
			case "/yxzh/question/get_questions":
			case "/yxzh/week_award/init":
			case "/yxzh/expedition/get_base_info":	
			case "/yxzh/opentask/get_opentasks":
			case "/yxzh/title/get_title":

				String strValue = ConfigHelper.GetAppConfig (message.Cmd);
				ErlKVMessage dmsg = new ErlKVMessage ("r_ok");
				dmsg.addValue (null, new ErlInt (message.getPort ()));
				ErlType[] et = StringKit.strToErlTypeArray (strValue);
				ErlArray ea = new ErlArray (et); 	
				dmsg.addValue ("msg", ea);
				base.send (this.erlConnect, dmsg);
				break;
			default: 
				break;
			}
		}
	}

	public override void read (ErlKVMessage message)
	{
		 
	}
}

