using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Text;
using NLog;

 
public class TransCenter
{
	public static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger ();
	public static int transCenterPort = 7612;
	public static string transCenterIP="0.0.0.0";
	public static string targetIP = "123.59.34.161";
	public static int targetPort = 7612;
	public static Socket transCenterServerSocket;
	public static int ports = 0 ;
	public static bool proceMsg = true ;
	public delegate int AddHandler();

	public TransCenter ()
	{
	}

	private static void initConfig ()
	{
		Log.Debug("initConfig：" + DateTime.Now.ToString("HH:mm:ss"));
		transCenterIP = (ConfigHelper.GetAppConfig ("transCenterIP","0.0.0.0"));
		transCenterPort = StringKit.toInt (ConfigHelper.GetAppConfig ("transCenterPort","7612"));
		targetIP = ConfigHelper.GetAppConfig ("targetIP","123.59.34.161");
		targetPort = StringKit.toInt (ConfigHelper.GetAppConfig ("targetPort","7612"));
		proceMsg = (ConfigHelper.GetAppConfig ("proceMsg","false")=="true");
		Log.Info("initConfig[transCenterIP:{0},transCenterPort:{1},targetIP:{2},targetPort:{3},procMsg:{4}]",transCenterIP,transCenterPort,targetIP,targetPort,proceMsg);
	}

	public static void Main (string[] args)
	{
		Log.IsEnabled (LogLevel.Debug);
		initConfig ();
		//服务器IP地址  
		IPAddress ip = IPAddress.Parse (transCenterIP);  
		transCenterServerSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
		transCenterServerSocket.Bind (new IPEndPoint (ip, transCenterPort));  //绑定IP地址：端口  
		transCenterServerSocket.ReceiveTimeout = 5000 ;
		transCenterServerSocket.Listen (10);    //设定最多10个排队连接请求  
		Log.Info ("启动监听{0}成功", transCenterServerSocket.LocalEndPoint.ToString ());  
		//通过Clientsoket发送数据  
		Thread ListenClientThread = new Thread (ListenClientConnect);  
		ListenClientThread.Start (transCenterServerSocket);  

	}

	/// <summary>  
	/// 监听客户端连接  
	/// </summary>  
	private static void ListenClientConnect (Object _socket)
	{  
		while (true) {  
			Socket clientSocket = ((Socket)_socket).Accept ();  
			Log.Info ("监听到[{0}-->{1}]的连接。", clientSocket.RemoteEndPoint.ToString (), clientSocket.LocalEndPoint.ToString ());  
			TransCenterSockets tcs = new TransCenterSockets ();
			tcs.socketClient = clientSocket;
			Thread receiveClientThread = new Thread (ReceiveMessage);  
			receiveClientThread.Start (tcs);  
		}  
	}

	/// <summary>  
	/// 接收消息  
	/// </summary>  
	/// <param name="clientSocket"></param>  
	private static void ReceiveMessage (object transCenterSockets)
	{  
		TransCenterSockets tcs = (TransCenterSockets)transCenterSockets;
		//接收到新的客户端连接后，建立到远程服务器的连接
		Socket _socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint _point = new IPEndPoint (Dns.GetHostAddresses (targetIP) [0], targetPort);
		WaitCallback callback = delegate {
			toTargetSuccess ((TransCenterSockets)tcs);
		};
		tcs.socketServer = _socket;
		tcs.socketServer.BeginConnect (_point, new AsyncCallback (callback), tcs.socketClient);
		//启动处理线程
		Thread proceMsgThread = new Thread (ProceMessage);  
		proceMsgThread.Start (tcs);  
 

		while (true) {  
			try {  
				//通过clientSocket接收数据  

				if (tcs.socketClient.Available > 0) {
					//clientPort.receive ();
					transToTargetData (tcs);
				} 
				Thread.Sleep (2);
			} catch (Exception ex) {  
				Console.WriteLine (ex.Message);  
				tcs.socketClient.Shutdown (SocketShutdown.Both);  
				tcs.socketClient.Close ();  
				break;  
			}  
		} 
		 
	}
	public static bool SocketTest(object transCenterSockets)
	{
		TransCenterSockets tcs = (TransCenterSockets)transCenterSockets;
		try{
			 
			tcs.transPortClient.erlConnect.socket.Send(new byte[0]);

			Socket s = tcs.transPortClient.erlConnect.socket; 
			if (s.Poll(-1, SelectMode.SelectRead))
			{
				 ;
				int nRead = s.Available ;
				if (nRead == 0)
				{
					return true ;
				}
			}      
			return false  ;
		}catch(Exception e) {
			Log.Error (e.Message);
			return false;
		}
	}
	/// <summary>  
	/// 接收消息  
	/// </summary>  
	/// <param name="clientSocket"></param>  
	private static void ProceMessage (object transCenterSockets)
	{  
		TransCenterSockets tcs = (TransCenterSockets)transCenterSockets;
		while (true) {  
			try {  
				//通过clientSocket接收数据  

				if (proceMsg) {
					//clientPort.receive ();
					//Log.Debug("proceMsg：" + DateTime.Now.ToString("hh24:mm:ss"));
					 
					if (!SocketTest(tcs)) { 
						tcs.transPortServer.isServer = true;
					    tcs.transPortServer.procServerCmd();
					}
					 
					 
				} 
				Thread.Sleep (10000);
			} catch (Exception ex) {  
				Log.Error (ex.Message);
				break;  
			}  
		} 

	}

	/// <summary>  
	/// 接收消息  
	/// </summary>  
	/// <param name="clientSocket"></param>  
	private static void ReceiveServerMessage (object transCenterSockets)
	{  
		TransCenterSockets tcs = (TransCenterSockets)transCenterSockets;
		while (true) {  
			try {  
				//通过clientSocket接收数据  
				if (tcs.socketServer.Available > 0) {
					//clientPort.receive ();

					transToClientData (tcs);
				} 
				Thread.Sleep (2);
			} catch (Exception ex) {  
				Log.Error (ex.Message);  
				tcs.socketServer.Shutdown (SocketShutdown.Both);  
				tcs.socketServer.Close ();  
				break;  
			}  
		} 

	}
	public static string getSocketStr( Socket sk)
	{
		if (sk == null) {
			return "[]";
		}
		return "["+sk.LocalEndPoint.ToString () + "->" + sk.RemoteEndPoint.ToString ()+"]";
	}
	private static bool transToTargetData (TransCenterSockets tcs)
	{
		Log.Info (getSocketStr(tcs.socketClient) + tcs.socketClient.Available);
		ByteBuffer data = new ByteBuffer (tcs.socketClient.Available);
		data.setTop (tcs.socketClient.Available);
		tcs.socketClient.Receive (data.getArray (), SocketFlags.None);
		tcs.transPortServer.isServer = true;

		try {
			tcs.transPortServer.dataBuffer = data.Clone() as ByteBuffer;
			tcs.transPortServer.receive (data, true);
		
		} catch (Exception e) {
			Log.Error (e.Message);
		}
		data.position = 0;
		tcs.socketServer.Send (data.getArray ());
		return true;
	}

	private static bool transToClientData (TransCenterSockets tcs)
	{
		Log.Info ( getSocketStr(tcs.socketServer)+ tcs.socketServer.Available);
		ByteBuffer data = new ByteBuffer (tcs.socketServer.Available);
		data.setTop (tcs.socketServer.Available);
		tcs.socketServer.Receive (data.getArray (), SocketFlags.None);
	   
		 
			tcs.transPortClient.isServer = false;
			try {
			 
				CallBack cb = delegate {
					data.position = 0;
					tcs.socketClient.Send (data.getArray ());
				};
				tcs.transPortClient.erlConnect.transCallBack = null; 
				tcs.transPortClient.dataBuffer = data.Clone () as ByteBuffer;
				tcs.transPortClient.receive (data, false);
			 
				if (data.length () == 10) {
					data.position = 0;
					tcs.transPortServer.isServer = true;
					tcs.transPortServer.dataBuffer = data.Clone () as ByteBuffer;
					tcs.transPortServer.receive (data, true);
				}
				 

			} catch (Exception e) {
				Log.Error (e.Message);
			} finally {
			
				//Log.Info ("+++++" + ports + "+++++++++++");
				//if (ports!=5) {
				
				//}
			}
		 
		return false;
	}

	private static void toTargetSuccess (TransCenterSockets tcs)
	{
		//TransCenterSockets tcs = new TransCenterSockets ();
		//tcs.socketClient = clientSocket;
		Thread receiveServerThread = new Thread (ReceiveServerMessage);  
		receiveServerThread.Start (tcs);  
		Log.Info ("建立到远程服务器的连接[{0}-->{1}]", tcs.socketServer.LocalEndPoint.ToString (), tcs.socketServer.RemoteEndPoint.ToString ());
	}



	public class TransCenterSockets
	{
		
		private Socket _socketServer;
		private Socket _socketClient;
		private TransPortServer _transPortServer;
		private TransPortClient _transPortClient;

		public TransCenterSockets ()
		{
		}

		public Socket socketServer {  
			get { return _socketServer; }  
			set {
				_socketServer = value;
				ErlConnect erlConnect = ConnectManager.manager ().transBeginConnect (_socketServer) as ErlConnect;
				TransPortServer transPort = new TransPortServer (erlConnect);
				transPortServer = transPort;
			}  
		}

		public Socket socketClient {  
			get { return _socketClient; }  
			set { 
				_socketClient = value;
				ErlConnect erlConnect = ConnectManager.manager ().transBeginConnect (_socketClient) as ErlConnect;
				TransPortClient transPort = new TransPortClient (erlConnect);
				transPortClient = transPort;
			}  
		}

		public TransPortServer transPortServer {  
			get { return _transPortServer; }  
			set { _transPortServer = value; }  
		}

		public TransPortClient transPortClient {  
			get { return _transPortClient; }  
			set { _transPortClient = value; }  
		}

	}
}
 

