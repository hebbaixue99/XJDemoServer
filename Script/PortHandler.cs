using System;

public class PortHandler
{
    public virtual void erlReceive(Connect connect, ErlKVMessage message)
    {
    }
	public virtual void erlReceive(Connect connect, ErlKVMessageClient message)
	{
	}

    public virtual void receive(Connect connect, ByteBuffer data)
    {
    }
}

