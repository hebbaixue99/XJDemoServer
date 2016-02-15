using System;

public class ErlConnectFactory : ConnectFactory
{
    private bool pingBack;
    private static int pingCount;
    private static long pingTime;
    private static long st;

    public override Connect openConnect(string localAddress, int localPort)
    {
        ErlConnect connect = new ErlConnect();
        connect.open(localAddress, localPort);
        connect.portHandler = DataAccess.getInstance();
        return connect;
    }

    public override void ping()
    {
        long num = TimeKit.getMillisTime();
       // if ((GameManager.Instance.lastUpdateTime != 0) && ((num - GameManager.Instance.lastUpdateTime) >= 0x3e8L))
       // {
       //     MaskWindow.NetLockMaskHide();
       // }
       // else
        {
            Connect[] connectArray = base.connectArray.ToArray();
            for (int i = connectArray.Length - 1; i >= 0; i--)
            {
                ErlConnect connect = connectArray[i] as ErlConnect;
                if (connect.Active && ((num - connect.PingTime) >= 0x3e8L))
                {
                    connect.PingTime = num;
                    ErlKVMessage message = new ErlKVMessage("echo");
                    DataAccess.getInstance().access(connect, message, new ReceiveFun(this.pingHandle), null, 0x4e20L);
                }
            }
        }
    }

    protected void pingHandle(Connect erlConnect, object erlMessage)
    {
       // MaskWindow.NetLockMaskHide();
        long num = TimeKit.getMillisTime();
        erlConnect.ping = num - erlConnect.PingTime;
    }

    private void Update2()
    {
        if (!this.pingBack)
        {
            for (int i = 0; i < base.connectArray.Count; i++)
            {
                ErlConnect connect = base.connectArray[i] as ErlConnect;
                if (connect.Active)
                {
                    this.ping();
                    return;
                }
            }
        }
    }
}

