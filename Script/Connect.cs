using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
 

public class Connect : IDisposable
{
    private volatile bool _active=true;
    private long _activeTime;
    private CallBackHandle _callback;
    private string _localAddress;
    private int _localPort;
    private long _ping = -1L;
    private long _pingTime;
    protected PortHandler _portHandler;
    private Socket _socket;
    private long _startTime;
    private int _timeout = 0x2bf20;
    private ConnectInvalidHandle connectInvalidBack;
    private int len;
    public const int MAX_DATA_LENGTH = 0x1f800;
    public Socket socket;
    public const int TIMEOUT = 0x2bf20;
    private Timer timer;

    private void connectFail()
    {
        this.socket.Close();
        this.timer.stop();
    }

    protected virtual ByteBuffer createDataByHead(ByteBuffer body)
    {
        int len = body.length();
        ByteBuffer data = new ByteBuffer();
        ByteKit.writeLength(data, len);
        data.writeBytes(body.toArray());
        return data;
    }

    public virtual void Dispose()
    {
        Connect connect = this;
        lock (connect)
        {
            if (this.Active)
            {
                this._active = false;
                if ((this.socket != null) && this.socket.Connected)
                {
                    this.socket.Disconnect(true);
                }
            }
        }
    }

    protected void init(string address, int port)
    {
        this._localAddress = address;
        this._localPort = port;
    }

    public bool isSameConnect(string localAddress, int localPort)
    {
        return ((this._localAddress == localAddress) && (this._localPort == localPort));
    }
	public void open(Socket _socket)
	{
		if (_socket != null) {
			this._localAddress = (_socket.RemoteEndPoint as IPEndPoint).Address.ToString ();
			this._localPort = (_socket.RemoteEndPoint as IPEndPoint).Port;
			this.socket = _socket;
		}
	}
    public void open(string address, int port)
    {
		 
        this.init(address, port);
        if (this.Active)
        {
            throw new Exception(base.GetType() + ", open, connect is active");
        }
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint point = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
		this.socket.BeginConnect(point, new AsyncCallback(this.suceess), this.socket);
       /* if (ServerManagerment.Instance.lastServer != null)
        {
            if (ServerManagerment.Instance.lastServer.ipEndPoint == null)
            {
                IPEndPoint point = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
                ServerManagerment.Instance.lastServer.ipEndPoint = point;
            }
            this.socket.BeginConnect(ServerManagerment.Instance.lastServer.ipEndPoint, new AsyncCallback(this.suceess), this.socket);
            this.timer = TimerManager.Instance.getTimer(0x2710L, 1);
            this.timer.addOnTimer(new TimerHandle(this.connectFail));
            this.timer.start();
        }
        else
        {
           // Debug.LogWarning("no server ~");
			//IPEndPoint point = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
			//this.socket.BeginConnect(point, new AsyncCallback(this.suceess), this.socket);
			//this.timer = TimerManager.Instance.getTimer(0x2710L, 1);
			//this.timer.addOnTimer(new TimerHandle(this.connectFail));
			//this.timer.start();
        }*/
    }

    public int readLength()
    {
        byte[] buffer = new byte[1];
        this.socket.Receive(buffer, SocketFlags.None);
        int num = buffer[0];
        if (num >= 0x80)
        {
            return (num - 0x80);
        }
        if (num >= 0x40)
        {
            buffer = new byte[1];
            this.socket.Receive(buffer, SocketFlags.None);
            return (((num << 8) + ByteKit.readUnsignedByte(buffer, 0)) - 0x4000);
        }
        if (num >= 0x20)
        {
            buffer = new byte[3];
            return ((((num << 0x18) + (ByteKit.readUnsignedByte(buffer, 0) << 0x10)) + ByteKit.readUnsignedByte(buffer, 1)) - 0x20000000);
        }
        object[] objArray1 = new object[] { base.GetType(), ", readLength, invalid number:", num, ", ", this };
        throw new Exception(string.Concat(objArray1));
    }

    public virtual void receive()
    {
        if ((this.Active && this.socket.Connected) && (this.socket.Available > 0))
        {
            if (this.len <= 0)
            {
                this.len = this.readLength();
            }
            if (this.len <= this.socket.Available)
            {
                ByteBuffer data = new ByteBuffer(this.len);
                data.setTop(this.len);
                this.socket.Receive(data.getArray(), SocketFlags.None);
                this.len = 0;
                this.receive(data);
            }
        }
    }

    public virtual void receive(ByteBuffer data)
    {
        if (this._portHandler != null)
        {
            this._activeTime = TimeKit.getMillisTime();
            try
            {
                this._portHandler.receive(this, data);
            }
            catch (Exception exception)
            {
                //Log.error(base.GetType() + ", receive error, " + this, exception);
            }
        }
    }

    public void send(ByteBuffer data)
    {
        if (this._active)
        {
            data = this.createDataByHead(data);
			//Log.info(data.toArray().Length);
            this.send(data.toArray(), 0, data.length());
        }
    }

    public void send(byte[] data, int offset, int len)
    {
        if ((this._active && (this.socket != null)) && ((this.socket.Connected && (data != null)) && (data.Length != 0)))
        {
            if (len > 0x1f800)
            {
               // Debug.LogWarning(string.Concat(new object[] { base.GetType(), ", send, data overflow:", len, ", ", this }));
            }
            try
            {
                this.socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch
            {
               /* if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnLostConnect(true);
                }*/
            }
        }
    }

    private void suceess(IAsyncResult asyncresult)
    {
        if (this.socket.Connected)
        {
			//Log.info("success----");
            this._active = true;
            this.socket.ReceiveBufferSize = 0x5000;
            this._activeTime = this._startTime = DateTime.Now.ToFileTime();
            this.timer.stop();
        }
    }

    public bool Active
    {
        get
        {
            return ((this._active && (this.socket != null)) && this.socket.Connected);
        }
    }

    public long ActiveTime
    {
        get
        {
            return this._activeTime;
        }
        set
        {
            this._activeTime = value;
        }
    }

    public CallBackHandle CallBack
    {
        get
        {
            return this._callback;
        }
        set
        {
            this._callback = value;
        }
    }

    public ConnectInvalidHandle ConnectInvalidBack
    {
        get
        {
            return this.connectInvalidBack;
        }
        set
        {
            this.connectInvalidBack = value;
        }
    }

    public string LocalAddress
    {
        get
        {
            return this._localAddress;
        }
    }

    public int LocalPort
    {
        get
        {
            return this._localPort;
        }
    }

    public long ping
    {
        get
        {
            return this._ping;
        }
        set
        {
            this._ping = value;
        }
    }

    public long PingTime
    {
        get
        {
            return this._pingTime;
        }
        set
        {
            this._pingTime = value;
        }
    }

    public PortHandler portHandler
    {
        get
        {
            return this._portHandler;
        }
        set
        {
            this._portHandler = value;
        }
    }

    public long StartTime
    {
        get
        {
            return this._startTime;
        }
    }

    public int TimeOut
    {
        get
        {
            return this._timeout;
        }
        set
        {
            this._timeout = value;
        }
    }
}

