using System;

public class AutoCmd
{
	public String _cmd="";
	public int _count=0;
	public int _beginCount=1;
	public BaseFPort _fport=null ;
	public String _beginTime="00:00:00" ;
	public String _endTime = "23:59:59" ;

	public AutoCmd ()
	{
	}
	public AutoCmd (string cmd,int beginCount)
	{
		this._cmd = cmd;
		this._beginCount = beginCount;
	}
	public AutoCmd (string cmd,int beginCount,BaseFPort fport)
	{
		this._cmd = cmd;
		this._beginCount = beginCount;
		this._fport = fport;
	}
}


