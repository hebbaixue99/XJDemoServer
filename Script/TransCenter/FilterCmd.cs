using System;

public class FilterCmd
{
	public String _cmd="";
	public int _count=0;
	public int _beginCount=1;
	public BaseFPort _fport=null ;

	public FilterCmd ()
	{
	}
	public FilterCmd (string cmd,int beginCount)
	{
		this._cmd = cmd;
		this._beginCount = beginCount;
	}
}


