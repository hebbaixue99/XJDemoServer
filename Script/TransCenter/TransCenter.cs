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
	public static string targetIP = "123.59.34.161";
	public static int targetPort = 7612;
	public static Socket transCenterServerSocket;

	public TransCenter ()
	{
	}
	private void initConfig ()
	{
		transCenterPort = StringKit.toInt (ConfigHelper.GetAppConfig ("transCenterPort"));
		targetIP = ConfigHelper.GetAppConfig ("targetIP");
		targetPort = StringKit.toInt (ConfigHelper.GetAppConfig ("targetPort"));
	}
	public static void Main (string[] args)
	{
		Log.IsEnabled (LogLevel.Debug);

		//服务器IP地址  
		IPAddress ip = IPAddress.Parse ("0.0.0.0");  
		transCenterServerSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
		transCenterServerSocket.Bind (new IPEndPoint (ip, transCenterPort));  //绑定IP地址：端口  
		transCenterServerSocket.Listen (10);    //设定最多10个排队连接请求  
		Log.Info ("启动监听{0}成功", transCenterServerSocket.LocalEndPoint.ToString ());  
		//通过Clientsoket发送数据  
		Thread ListenClientThread = new Thread (ListenClientConnect);  
		ListenClientThread.Start(transCenterServerSocket);  

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
		TransCenterSockets tcs = (TransCenterSockets)transCenterSockets ;
		//接收到新的客户端连接后，建立到远程服务器的连接
		Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint _point = new IPEndPoint(Dns.GetHostAddresses(targetIP)[0], targetPort);
		WaitCallback callback = delegate { toTargetSuccess ((TransCenterSockets)tcs); };
		tcs.socketServer = _socket;
		tcs.socketServer.BeginConnect (_point, new AsyncCallback (callback), tcs.socketClient);

 

		while (true) {  
			try {  
				//通过clientSocket接收数据  

				if ( tcs.socketClient.Available > 0) {
					//clientPort.receive ();
					transToTargetData(tcs);
				} 
				Thread.Sleep(1);
			} catch (Exception ex) {  
				Console.WriteLine (ex.Message);  
				tcs.socketClient.Shutdown (SocketShutdown.Both);  
				tcs.socketClient.Close ();  
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
		TransCenterSockets tcs = (TransCenterSockets)transCenterSockets ;
		while (true) {  
			try {  
				//通过clientSocket接收数据  
				if ( tcs.socketServer.Available > 0) {
					//clientPort.receive ();
					transToClientData(tcs);
				} 
				Thread.Sleep(1);
			} catch (Exception ex) {  
				Console.WriteLine (ex.Message);  
				tcs.socketServer.Shutdown (SocketShutdown.Both);  
				tcs.socketServer.Close ();  
				break;  
			}  
		} 

	}

	private static bool transToTargetData(TransCenterSockets tcs)
	{
		Log.Info (tcs.socketClient.Available);
		ByteBuffer data = new ByteBuffer(tcs.socketClient.Available);
		data.setTop(tcs.socketClient.Available);
		tcs.socketClient.Receive(data.getArray(), SocketFlags.None);
		tcs.socketServer.Send (data.getArray ());
		data.position = 0;
		try{
			Socket c = tcs.socketClient;
			tcs.transPortClient.receive (data,true);
		}catch(Exception e) {
			Log.Error (e.Message);
		}
		return false;
	}
	private static bool transToClientData(TransCenterSockets tcs)
	{
		Log.Info (tcs.socketServer.Available);
		ByteBuffer data = new ByteBuffer(tcs.socketServer.Available);
		data.setTop(tcs.socketServer.Available);
		tcs.socketServer.Receive(data.getArray(), SocketFlags.None);
		try{
		tcs.transPortClient.receive (data,true);
		}catch(Exception e) {
			Log.Error (e.Message);
		}
		data.position = 0;
		tcs.socketClient.Send (data.getArray ());
		return false;
	}
	private static void toTargetSuccess(TransCenterSockets tcs)
	{
		//TransCenterSockets tcs = new TransCenterSockets ();
		//tcs.socketClient = clientSocket;
		Thread receiveServerThread = new Thread (ReceiveServerMessage);  
		receiveServerThread.Start (tcs);  
		Log.Info ("建立到远程服务器的连接[{0}-->{1}]",tcs.socketServer.LocalEndPoint.ToString(),tcs.socketServer.RemoteEndPoint.ToString());
	}



	public class TransCenterSockets
	{
		private Socket _socketServer;
		private Socket _socketClient;
		private TransPort _transPortServer;
		private TransPort _transPortClient;

		public TransCenterSockets ()
		{
		}

		public Socket socketServer {  
			get { return _socketServer; }  
			set {
				_socketServer = value;
				ErlConnect erlConnect = ConnectManager.manager ().transBeginConnect (_socketServer) as ErlConnect;
				TransPort  transPort = new TransPort (erlConnect);
				transPortServer = transPort;
			}  
		}

		public Socket socketClient {  
			get { return _socketClient; }  
			set { 
				_socketClient = value;
				ErlConnect erlConnect = ConnectManager.manager ().transBeginConnect (_socketClient) as ErlConnect;
				TransPort  transPort = new TransPort (erlConnect);
				transPortClient = transPort;
			}  
		}
		public TransPort transPortServer {  
			get { return _transPortServer; }  
			set { _transPortServer = value; }  
		}

		public TransPort transPortClient {  
			get { return _transPortClient; }  
			set { _transPortClient = value; }  
		}

	}
}
 

