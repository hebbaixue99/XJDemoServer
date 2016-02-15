using System;

public class ErlPingPort : PortHandler
{
    public override void erlReceive(Connect connect, ErlKVMessage message)
    {
        long num = TimeKit.getMillisTime();
        connect.ping = num - connect.PingTime;
        connect.PingTime = 0L;
    }
}

