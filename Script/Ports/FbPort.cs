using System;

public class FbPort:BaseFPort
{
	public FbPort ()
	{
	}

	public override  void read (ErlKVMessage message)
	{
		if (this.erlConnect != null) {
			ErlKVMessage msg = new ErlKVMessage ("r_ok");
			msg.addValue (null, new ErlInt (message.getPort ()));
			//"sids":[],"number":0,"buy":0,"act_free":0,type:502,"pstep":41
			msg.addValue ("sids", new ErlArray (new ErlType[0]));
			msg.addValue ("number", new ErlInt (0));
			msg.addValue ("buy", new ErlInt (0));
			msg.addValue ("act_free", new ErlInt (0));
			msg.addValue ("type", new ErlInt (502));
			msg.addValue ("pstep", new ErlInt (41));
		 
			base.send (this.erlConnect, msg);
			 
		}
	}
}
 

