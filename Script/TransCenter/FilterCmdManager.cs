using System;
using System.Collections.Generic;
using NLog;

 
public class FilterCmdManager
{
	public static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();
	private  Dictionary<int, String> needProcCmdDict = new Dictionary<int, String> ();
	private  Dictionary<String, FilterCmd> FilterCmdDict = new Dictionary<String, FilterCmd> ();
	private  static  FilterCmdManager _instance;
	public static FilterCmdManager Instance{
		get{
			if (_instance == null) {
				_instance = new FilterCmdManager ();
			}
			return _instance;
		}
	}
	public FilterCmdManager ()
	{
		String[] filterCmd = ConfigHelper.GetAppConfig ("filter_cmd").Split(',');
		String[] filterBeginCount = ConfigHelper.GetAppConfig ("filter_begin_count").Split(',');
		for (int i = 0; i < filterCmd.Length; i++) {
			FilterCmd fc = new FilterCmd(filterCmd[i],StringKit.toInt(filterBeginCount[i]));
			FilterCmdDict.Add (filterCmd [i], fc);
		}
	}
	public Boolean procCmd(int _port , BaseFPort fport){
		if (needProcCmdDict.ContainsKey(_port)) {
			string cCmd = needProcCmdDict [_port];
			if (FilterCmdDict [cCmd]._count >= FilterCmdDict [cCmd]._beginCount) {
				String strValue = ConfigHelper.GetAppConfig (cCmd);
				ErlKVMessage msg = new ErlKVMessage ("r_ok");
				msg.addValue (null, new ErlInt (_port));
				ErlType[] et = StringKit.strToErlTypeArray (strValue);
				ErlArray ea = new ErlArray (et); 	
				msg.addValue ("msg", ea);
				Log.Info(msg.Cmd+"|"+ msg.toJsonString());
				ByteBuffer data = new ByteBuffer();
				//data.writeBytes (mybak);
				msg.bytesWrite(data);
				data.top = (int)data.bytesAvailable;
				byte[] tmpdata= new byte[(int)data.bytesAvailable];
				data.readBytes (tmpdata, 0, tmpdata.Length);

				fport.erlConnect.tmpBuffer.position = 0;
				Log.Info (fport.erlConnect.tmpBuffer) ;

				ByteBuffer tmp1 = new ByteBuffer();
				tmp1.writeBytes (tmpdata);
				//this.erlConnect.send (this.erlConnect.tmpBuffer);
				if (fport.erlConnect.socket.Connected) {
					fport.send (fport.erlConnect, msg);
				} else {
					Log.Info ("客户端已断开不再回传");
				}
				//base.erlConnect.socket.Send (bak);
				FilterCmdDict [cCmd]._count--;
				Log.Info ("处理完成的CMD[" + cCmd + "]");
				return true ;
			}

		}
		return false;		
	}
	public void procPort(string _cmd, int _port)
	{
		if (FilterCmdDict.ContainsKey (_cmd)) {
			Log.Info ("需要处理的CMD[" + _cmd + "]");
			needProcCmdDict.Add (_port, _cmd);
			FilterCmdDict [_cmd]._count++;
		}
		if (_cmd == "/yxzh/user_login") {
			_instance = null;	
		}
	}
}
 

