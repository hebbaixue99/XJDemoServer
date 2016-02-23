using System;

public class PyxPort:BaseFPort
{
	public PyxPort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			//user 返回值 "msg":"get","value":[0,[]]
			String strValue = ConfigHelper.GetAppConfig (message.Cmd+"_msg");
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue ( null , new ErlInt( message.getPort()));

			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 
			msg.addValue ("msg", ea);
		
			String strValue1 = ConfigHelper.GetAppConfig (message.Cmd+"_times");
			ErlType[] et1 = StringKit.strToErlTypeArray (strValue1);
			ErlArray ea1 = new ErlArray (et1); 
			msg.addValue ("times", ea1);

			base.send (this.erlConnect, msg);

		}
	}
}
 

