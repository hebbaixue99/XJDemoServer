using System;

public class CashPort:BaseFPort
{
	public CashPort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (message.getPort ()));
			msg.addValue ("msg", new ErlAtom ("error"));

			base.send (this.erlConnect, msg);
			 
		}
	}
}
 

