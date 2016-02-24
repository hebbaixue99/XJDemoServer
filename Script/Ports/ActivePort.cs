using System;

public class ActivePort:BaseFPort
{
	public ActivePort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		string str = message.Cmd;
		switch (str) {
		case "/yxzh/active_port/get_active_info":
			//ErlArray eaa = (message.getValue ("sid") as ErlType).getValueString ();
			String strValus = (message.getValue ("sid") as ErlType).getValueString ();
			if (strValus != null && strValus.Split(',').Length > 1) {
				strValus = ConfigHelper.GetAppConfig (message.Cmd);
			} else {
				strValus = (message.getValue ("sid") as ErlType).getValueString ();
				strValus = ConfigHelper.GetAppConfig(message.Cmd + "_" + strValus);
			}
			if (this.erlConnect != null) {
				// /yxzh/title/get_title 返回值

				ErlKVMessage msg = new ErlKVMessage ("r_ok");
				msg.addValue (null, new ErlInt (message.getPort ()));
				//String strValue = ConfigHelper.GetAppConfig (message.Cmd);
				ErlType[] et = StringKit.strToErlTypeArray (strValus);
				ErlArray ea = new ErlArray (et); 
				msg.addValue ("msg", ea);
				base.send (this.erlConnect, msg);

			}
			break;
		case "/yxzh/active_port/get_text_notice":
		default:
			if (this.erlConnect != null) {
				// /yxzh/title/get_title 返回值

				ErlKVMessage msg = new ErlKVMessage ("r_ok");
				msg.addValue (null, new ErlInt (message.getPort ()));
				String strValue = ConfigHelper.GetAppConfig (message.Cmd);
				ErlType[] et = StringKit.strToErlTypeArray (strValue);
				ErlArray ea = new ErlArray (et); 
				msg.addValue ("msg", ea);
				base.send (this.erlConnect, msg);
				Log.Debug (msg.toJsonString ());

			}
			break;
		}



	}
}
 

