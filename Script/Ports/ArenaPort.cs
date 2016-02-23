using System;

public class ArenaPort:BaseFPort
{
	public ArenaPort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			//user 返回值
			String strValue = ConfigHelper.GetAppConfig (message.Cmd);
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (message.getPort ()));
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 	
			msg.addValue ("msg", ea);
			base.send (this.erlConnect, msg);

		}
	}
}
 

