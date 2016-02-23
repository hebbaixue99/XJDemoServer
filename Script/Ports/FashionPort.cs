using System;

public class FashionPort:BaseFPort
{
	public FashionPort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			//user 返回值 "msg":"get","value":[0,[]]
			String strValue = ConfigHelper.GetAppConfig (message.Cmd+"_value");
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			ErlType[] et = StringKit.strToErlTypeArray (strValue);
			ErlArray ea = new ErlArray (et); 
			msg.addValue (null, new ErlInt(message.getPort()));
			msg.addValue ("msg", new ErlAtom ("get"));
			msg.addValue ("value", ea);
			base.send (this.erlConnect, msg);

		}
	}
}
 

