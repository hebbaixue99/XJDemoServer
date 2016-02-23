using System;

public class TitlePort:BaseFPort
{
	public TitlePort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			//user 返回值
			String strValue = ConfigHelper.GetAppConfig (message.Cmd+"_title");
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (message.getPort ()));
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 	
			msg.addValue ("title", ea);

			String strValue1 = ConfigHelper.GetAppConfig (message.Cmd+"_effect");
			ErlType[] et1 = StringKit.strToErlTypeArray (strValue1);
			ErlArray ea1 = new ErlArray (et1); 	
			msg.addValue ("effect", ea1); 

			base.send (this.erlConnect, msg);
			 
		}
	}
}
 

