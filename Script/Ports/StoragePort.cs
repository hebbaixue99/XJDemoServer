using System;

public class StoragePort:BaseFPort
{
	public StoragePort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			string stype = (message.getValue ("type") as ErlString).Value;
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (message.getPort ()));
			String strValue = ConfigHelper.GetAppConfig (message.Cmd +"_" + stype);
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ub = new ErlArray (et); 	
			msg.addValue ("msg", ub);
			base.send (this.erlConnect, msg);
			 
		}
	}
}
 

