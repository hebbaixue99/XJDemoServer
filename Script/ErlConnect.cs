using System;
using System.Net.Sockets;


public class ErlConnect : Connect
{
	private int _compress = 1;
	private int _crc = 1;
	private int _encryption = 1;
	private bool _isConnectReady;
	private int _kv = 1;
	private int[] _receiveChallengeCode;
	private int[] _sendChallengeCode;
	public const int COMPRESS = 1;
	public const int CRC = 1;
	public const int ENCRYPTION = 1;
	public const int KV = 1;
	private int length;
	private const int RAND_A = 0x41a7;
	private const int RAND_MASK = 0x75bd924;
	private const int RAND_Q = 0x1f31d;
	public const int VERSION = 0;
	public bool isServer = false;
	public CallBack transCallBack = null ;
	public int myPos = 0;
	public int myLen = 0;
	public ByteBuffer tmpBuffer;

	private ByteBuffer _dataBuffer ;
	public ByteBuffer dataBuffer{
		get {
			return _dataBuffer;
		}
		set {
			int len = 0;
			if (_dataBuffer != null) {
				len = _dataBuffer.top;
				byte[] tmp = new byte[len + value.top];
				_dataBuffer.readBytes (tmp, 0, len);
				value.readBytes (tmp, len, value.top);
				_dataBuffer= new ByteBuffer(tmp);
			} else {
				byte[] tmp = new byte[value.top]; 
				value.readBytes (tmp, len, value.top);
				_dataBuffer= new ByteBuffer(tmp);
			}

		}
	}

	public void close ()
	{
		this.Dispose ();
	}

	protected override ByteBuffer createDataByHead (ByteBuffer head)
	{
		ByteBuffer buffer = new ByteBuffer ();
		int b = (((0 | (this._encryption << 3)) | (this._crc << 2)) | (this._compress << 1)) | this._kv;
		buffer.writeShort (head.length () + 1);
		buffer.writeByte (b);
		buffer.writeBytes (head.toArray ());
		return buffer;
	}

	private ByteBuffer encryptionCode (ByteBuffer data, int[] code)
	{
		byte[] buffer = CodecKit.encodeXor (data.toArray (), this.nextPK (code));
		data = new ByteBuffer ();
		for (int i = 0; i < buffer.Length; i++) {
			data.writeByte (buffer [i]);
		}
		data.position = 0;
		return data;
	}

	protected int[] getPK (int seed)
	{
		int num = this.getRandome (seed + 11);
		int num2 = this.getRandome (num + 13);
		int num3 = this.getRandome (num2 + 0x11);
		int num4 = this.getRandome (num3 + 0x13);
		int num5 = this.getRandome (num4 + 0x17);
		int num6 = this.getRandome (num5 + 0x1d);
		int num7 = this.getRandome (num6 + 0x1f);
		int num8 = this.getRandome (num7 + 0x25);
		return new int[] { num, num2, num3, num4, num5, num6, num7, num8 };
	}

	private int getRandome (int seed)
	{
        
		int num = seed ^ 0x75bd924;
		int num2 = (0x41a7 * num) - (((int)Math.Round ((float)(num / 0x1f31d))) * 0x7fffffff);
		if (num2 < 0) {
			return (num2 + 0x7fffffff);
		}
		return num2;
	}

	protected byte[] nextPK (int[] pk)
	{
		if (pk == null) {
			return null;
		}
		int index = 0;
		int length = pk.Length;
		while (index < length) {
			pk [index] = this.getRandome (pk [index]);
			index++;
		}
		return this.toPK (pk);
	}

	public void parseMessage (ByteBuffer socketbuffer)
	{
		int num = socketbuffer.readByte ();
		bool flag = (num & 8) != 0;
		bool flag2 = (num & 4) != 0;
		bool flag3 = (num & 2) != 0;
		ByteBuffer data = new ByteBuffer (this.length - 1);
		data.write (socketbuffer.toArray (), 0, this.length - 1);
		if (base.socket.Available >= 2) {
			byte[] buffer = new byte[2];
			base.socket.Receive (buffer, SocketFlags.None);
			this.length = ByteKit.readUnsignedShort (buffer, 0);
		} else {
			this.length = 0;
		}
		if (flag) {
			data = this.encryptionCode (data, this._receiveChallengeCode);
		}
		if (flag3) {
			data = new ByteBuffer (ZIPUtil.Decompress (data.toArray ()));
		}
		if (flag2) {
			int num2 = data.readInt ();
			ByteBuffer buffer4 = new ByteBuffer ();
			buffer4.writeBytes (data.toArray (), 0, data.top - data.position);
			int num3 = (int)ChecksumUtil.Adler32 (buffer4);
			if (num2 != num3) {
				//MonoBehaviour.print(string.Concat(new object[] { "crc is err,crcValue", num2, ",nowCrc=", num3 }));
				// Log.info
				return;
			}
		}
		ErlKVMessage message = new ErlKVMessage (null);

		message.bytesRead (data);

		if (base._portHandler != null) {
			base._portHandler.erlReceive (this, message);
		}
	}

	public void TransParseMessage (ByteBuffer socketbuffer , bool isServer , ByteBuffer src)
	{
		try{
		int num = socketbuffer.readByte ();
		bool flag = (num & 8) != 0;
		bool flag2 = (num & 4) != 0;
		bool flag3 = (num & 2) != 0;
		ByteBuffer data = new ByteBuffer (this.length - 1);
		data.write (socketbuffer.toArray (), 0, this.length - 1);
		if ((this.dataBuffer.bytesAvailable) >= 2) {
			byte[] buffer = new byte[2];
			//base.socket.Receive (buffer, SocketFlags.None);
			this.dataBuffer.readBytes(buffer,0,2);
			this.length = ByteKit.readUnsignedShort (buffer, 0);
				 
		} else {
			this.length = 0;
		}
		if (flag) {
			if (this.isServer) {
				data = this.encryptionCode (data, this._sendChallengeCode);
			} else {
				data = this.encryptionCode (data, this._receiveChallengeCode);
			}
		}
		if (flag3) {
			data = new ByteBuffer (ZIPUtil.Decompress (data.toArray ()));
		}
		if (flag2) {
			int num2 = data.readInt ();
			ByteBuffer buffer4 = new ByteBuffer ();
			buffer4.writeBytes (data.toArray (), 0, (int)data.bytesAvailable);
			int num3 = (int)ChecksumUtil.Adler32 (buffer4);
			if (num2 != num3) {
				Log.Error(string.Concat(new object[] { "crc is err,crcValue", num2, ",nowCrc=", num3 }));
				// Log.info
				//if (this.transCallBack != null) {
				//	this.transCallBack.Invoke ();
			//	}
					int len = (int)this.dataBuffer.bytesAvailable;
					int pos = this.dataBuffer.position;

					byte[] tmp = new byte[len];
					byte[] bak = new byte[pos];

					this.dataBuffer.position = 0;
					this.dataBuffer.readBytes (bak, 0, pos);
					this.dataBuffer.readBytes (tmp, 0, len);
					this.dataBuffer.clear ();
					this.dataBuffer = new ByteBuffer (tmp);
					if(!this.isServer){
						base.socket.Send (bak);}

					if (this.dataBuffer.bytesAvailable > 0) {
						this.TransReceive (null, this.isServer);
					}

				return;
			}
		}
	 
			ErlKVMessage message = new ErlKVMessage (null);
			this.tmpBuffer = data;
			message.bytesRead (data);
			Log.Debug(string.Concat( data.getArray())+"++++++");
			this.myLen = this.dataBuffer.top;
 
			if (base._portHandler != null) {
				base._portHandler.erlReceive (this, message);
		 
		}
		}
		catch(Exception e) {
			Log.Error (e.Message);
			if (this.dataBuffer.bytesAvailable > 0) {
				this.TransReceive (null, this.isServer);
			} else {
				int len = (int)this.dataBuffer.bytesAvailable;
				int pos = this.dataBuffer.position;

				byte[] tmp = new byte[len];
				byte[] bak = new byte[pos];

				this.dataBuffer.position = 0;
				this.dataBuffer.readBytes (bak, 0, pos);
				this.dataBuffer.readBytes (tmp, 0, len);
				this.dataBuffer.clear ();
				this.dataBuffer = new ByteBuffer (tmp);
				if (!this.isServer) {
					base.socket.Send (bak);
				}

				if (this.dataBuffer.bytesAvailable > 0) {
					this.TransReceive (null, this.isServer);
				}

			}
		}
	}

	public byte[] getCode ()
	{
		byte[] seed = new byte[]{ 0x2, 0x3, 0x4, 0x5 };
		byte[] num2 = new byte[]{ 0x6, 0x7, 0x8, 0x9 };
		Array.Reverse (seed);
		Array.Reverse (num2);
		int iseed = BitConverter.ToInt32 (seed, 0);
		int inum2 = BitConverter.ToInt32 (num2, 0);
		this._sendChallengeCode = this.getPK (inum2); //this.getPK(iseed);
		this._receiveChallengeCode = this.getPK (iseed); //this.getPK(inum2);
		this._isConnectReady = true;
		return new byte[]{ 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9 };
	}

	public  void TransReceive (ByteBuffer data , bool isServer  )
	{	 
		base.ActiveTime = TimeKit.getMillisTime ();
		if (this.dataBuffer.top > 0) {
			if (!this._isConnectReady) {
				byte[] buffer = new byte[1];
				//base.socket.Receive (buffer, SocketFlags.None);
				Log.Debug (this.dataBuffer.position);
				this.dataBuffer.readBytes(buffer,0,1);

				byte[] buffer2 = new byte[1];
				Log.Debug (this.dataBuffer.position);
				this.dataBuffer.readBytes (buffer2,0, 1);
				//base.socket.Receive (buffer2, SocketFlags.None);
				byte[] buffer3 = new byte[4];
				Log.Debug (this.dataBuffer.position);
				//data.bytesRead ();
				this.dataBuffer.readBytes (buffer3,0, 4);
				//base.socket.Receive (buffer3, SocketFlags.None);
				Array.Reverse (buffer3);
				int seed = BitConverter.ToInt32 (buffer3, 0);
				byte[] buffer4 = new byte[4];
				Log.Debug (this.dataBuffer.position);
				this.dataBuffer.readBytes (buffer4, 0, 4);
				//base.socket.Receive (buffer4, SocketFlags.None);
				Array.Reverse (buffer4);
				int num2 = BitConverter.ToInt32 (buffer4, 0);
				this._sendChallengeCode = this.getPK (seed);
				this._receiveChallengeCode = this.getPK (num2);
				this._isConnectReady = true;
				/*if (base.CallBack != null) {
					base.CallBack ();
				}*/
				if (this.dataBuffer.bytesAvailable > 0) {
					this.TransReceive  (null, this.isServer);
				} else {
					int len = this.dataBuffer.position;
					byte[] tmp = new byte[len];
					this.dataBuffer.position = 0;
					this.dataBuffer.readBytes (tmp, 0, len);
					if (!this.isServer) {
						this.socket.Send (tmp);
					}
					this.dataBuffer.clear ();
					 
				}
				if (this.transCallBack != null) {
					this.transCallBack.Invoke ();
				}

			} else {
				if (this.length <= 0) {
					if ((this.dataBuffer.bytesAvailable) < 2) {
						if (this.transCallBack != null) {
							this.transCallBack.Invoke ();
						}
						return;
					}
					byte[] buffer5 = new byte[2];
					this.dataBuffer.readBytes(buffer5, 0,2);
					this.length = ByteKit.readUnsignedShort (buffer5, 0);
				}
				if ((this.length > 0) && ((this.dataBuffer.bytesAvailable) >= this.length)) {
					ByteBuffer socketbuffer = new ByteBuffer (this.length);
					socketbuffer.setTop (this.length);
					this.myPos = this.dataBuffer.position;
					this.myLen = this.length ;
					this.dataBuffer.readBytes (socketbuffer.getArray (), 0, this.length);
					//base.socket.Receive (socketbuffer.getArray (), SocketFlags.None);
					this.TransParseMessage (socketbuffer, isServer,data);
				}
			}
		} 
	}

	public override void receive ()
	{
		if (!base.socket.Connected) {
			if (base.ConnectInvalidBack != null) {
				base.ConnectInvalidBack ();
			}
		}
		//else if (!GameManager.Instance.disconnetNoRecieve)
		{
			base.ActiveTime = TimeKit.getMillisTime ();
			if (base.socket.Available > 0) {
				if (!this._isConnectReady) {
					byte[] buffer = new byte[1];
					base.socket.Receive (buffer, SocketFlags.None);
					byte[] buffer2 = new byte[1];
					base.socket.Receive (buffer2, SocketFlags.None);
					byte[] buffer3 = new byte[4];
					base.socket.Receive (buffer3, SocketFlags.None);
					Array.Reverse (buffer3);
					int seed = BitConverter.ToInt32 (buffer3, 0);
					byte[] buffer4 = new byte[4];
					base.socket.Receive (buffer4, SocketFlags.None);
					Array.Reverse (buffer4);
					int num2 = BitConverter.ToInt32 (buffer4, 0);
					this._sendChallengeCode = this.getPK (seed);
					this._receiveChallengeCode = this.getPK (num2);
					this._isConnectReady = true;
					if (base.CallBack != null) {
						base.CallBack ();
					}
				} else {
					if (this.length <= 0) {
						if (base.socket.Available < 2) {
							return;
						}
						byte[] buffer5 = new byte[2];
						base.socket.Receive (buffer5, SocketFlags.None);
						this.length = ByteKit.readUnsignedShort (buffer5, 0);
					}
					if ((this.length > 0) && (base.socket.Available >= this.length)) {
						ByteBuffer socketbuffer = new ByteBuffer (this.length);
						socketbuffer.setTop (this.length);
						base.socket.Receive (socketbuffer.getArray (), SocketFlags.None);
						this.parseMessage (socketbuffer);
					}
				}
			}
		}
	}

	public void sendErl (ByteBuffer data, int encryption, int crc, int compress, int kv)
	{
		if ((this._sendChallengeCode != null) && (this._sendChallengeCode.Length >= 0)) {
			this._encryption = encryption;
			this._crc = crc;
			this._compress = compress;
			this._kv = kv;
			int i = 0;
			ByteBuffer buffer = new ByteBuffer ();
			if ((this._compress == 1) && (data.length () >= 0x40)) {
				data = new ByteBuffer (ZIPUtil.Compress (data.toArray ()));
			} else {
				this._compress = 0;
			}
			if ((this._crc == 1) && (this._compress == 0)) {
				i = (int)ChecksumUtil.Adler32 (data);
				buffer.writeInt (i);
			} else {
				this._crc = 0;
			}
			buffer.writeBytes (data.toArray ());
			if (this._encryption == 1) {
				if (!this.isServer) {
					buffer = this.encryptionCode (buffer, this._receiveChallengeCode);
				} else {
					buffer = this.encryptionCode (buffer, this._sendChallengeCode);
				}

				//buffer = this.encryptionCode (buffer, this._receiveChallengeCode);
			}
			base.send (buffer);
			Log.Info(string.Concat(buffer.getArray()));
			this._encryption = 1;
			this._crc = 1;
			this._compress = 1;
			this._kv = 1;
		}
	}

	protected byte[] toPK (int[] pks)
	{
		ByteBuffer buffer = new ByteBuffer ();
		buffer.writeInt (pks [0]);
		buffer.writeInt (pks [1]);
		buffer.writeInt (pks [2]);
		buffer.writeInt (pks [3]);
		buffer.writeInt (pks [4]);
		buffer.writeInt (pks [5]);
		buffer.writeInt (pks [6]);
		buffer.writeInt (pks [7]);
		return buffer.getArray ();
	}

	public string toString ()
	{
		object[] objArray1 = new object[] {
			"[_sendChallengeCode=",
			this._sendChallengeCode,
			",_receiveChallengeCode=",
			this._receiveChallengeCode,
			"]"
		};
		return string.Concat (objArray1);
	}

	public bool isActive {
		get {
			return (base.Active && this._isConnectReady);
		}
	}

	public static void Main (String[] args)
	{
		ErlConnect et = new ErlConnect ();
		System.Console.Write (string.Concat (et.getPK (80543948)));
	}
}

