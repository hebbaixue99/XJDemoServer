using System;
using NLog;
public class ThreadKit
{
	public readonly Logger Log = LogManager.GetLogger("DemoMain");
    public static void dumpStack()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception exception)
        {
           // Log.error(null, exception);
        }
    }
}

