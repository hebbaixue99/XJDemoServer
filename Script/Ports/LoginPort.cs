using System;

public class LoginPort:BaseFPort
{
	public LoginPort ()
	{
	}

	public override void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) { 
			//登陆成功返回值
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue ("msg", new ErlAtom ("login_ok"));
			msg.addValue (null, new ErlInt (message.getPort ()));
			base.send (this.erlConnect, msg);
			//guild 返回值
			String strValue = ConfigHelper.GetAppConfig ("guild");
			msg = new ErlKVMessage ("guild");
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 	
			msg.addValue ("guild_skill", ea);
			base.send (this.erlConnect, msg);
			//chat 返回值
		    strValue = ConfigHelper.GetAppConfig ("chat");
			msg = new ErlKVMessage ("chat");
			et = StringKit.strToErlTypeArray (strValue);
			ea = new ErlArray (et); 	
			msg.addValue ("msg2", ea);
			base.send (this.erlConnect, msg);
		}
	}
}
 

