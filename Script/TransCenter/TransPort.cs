﻿using System;
using System.Runtime.CompilerServices;
using NLog;
using System.Net.Sockets;

public class TransPort : BaseFPort
{
	//public readonly Logger Log = LogManager.GetLogger("ClientPort");
	//private CallBack _callBack;
	 
	public bool isSend = false ;
	public bool isServer = false ;
	public bool sendFinish = false ;
	public static int messagePort = 0;
	public ByteBuffer dataBuffer{
		get{ return this.erlConnect.dataBuffer;}
		set{ this.erlConnect.dataBuffer = value;}
	}

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
	public void sendUser ()
	{
		String strValue = ConfigHelper.GetAppConfig ("/yxzh/role/get_user");
		ErlKVMessage msg = new ErlKVMessage ("r_ok");
		msg.addValue (null, new ErlInt (messagePort));
		ErlType[] et = StringKit.strToErlTypeArray (strValue);
		ErlArray ea = new ErlArray (et); 	
		msg.addValue ("msg", ea);
		base.send (this.erlConnect, msg);
	}
	public override void erlReceive (Connect connect, ErlKVMessage message)
	{
		//this.readMessage (message);
		//Log.Info ("+++++111111111---222222222" + this.isSend + "+++++++++++");
		/*if (messagePort!=0&& messagePort == message.getPort()) {
			String strValue = ConfigHelper.GetAppConfig ("/yxzh/role/get_user");
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (messagePort));
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 	
			msg.addValue ("msg", ea);
			base.send (this.erlConnect, msg);
			if (this.erlConnect.dataBuffer.bytesAvailable > 0) {
				byte[] bts = new byte[this.erlConnect.dataBuffer.bytesAvailable];
				this.dataBuffer.readBytes (bts, 0, (int)this.erlConnect.dataBuffer.bytesAvailable);
				this.erlConnect.socket.Send (bts);
				this.dataBuffer = new ByteBuffer (bts);
			}
			isSend = true;
		}*/
		if (message.Cmd == "/yxzh/role/get_user") {
			messagePort = message.getPort();
		}

		Log.Info(message.Cmd+"|"+ message.toJsonString());
		//if (!isSend && this.erlConnect.transCallBack != null) {
			//this.erlConnect.transCallBack.Invoke ();
		if (!this.isServer) {
			ByteBuffer bf = (ByteBuffer)this.erlConnect.dataBuffer.Clone ();
			int pos = bf.position;
			bf.position = 0; 
			this.erlConnect.socket.Send (bf.getArray ());
		}
		//this.erlConnect.dataBuffer.position = pos;


		//}
		isSend = false;

	}
	public override void erlReceive (Connect connect, ErlKVMessageClient message)
	{
		//this.readMessage (message);

		Log.Info(message.Cmd+"|"+ message.toJsonString());
		if (messagePort == message.getPort()) {
			String strValue = ConfigHelper.GetAppConfig ("/yxzh/role/get_user");
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (message.getPort ()));
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 	
			msg.addValue ("msg", ea);
			base.send (this.erlConnect, msg);
		}
		if (message.Cmd == "/yxzh/role/get_user") {
			messagePort = message.getPort();
		}




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

