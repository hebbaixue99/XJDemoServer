using System;
using System.Reflection;

public class DomainAccess
{
	public static object getObject(string classStr)
	{
		System.Type type = getType(classStr);
		if (type == null)
		{
			return null;
		}
		return loadObject(type);
	}

	private static System.Type getType(string classStr)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		System.Type type = null;
		foreach (Assembly assembly in assemblies)
		{
			type = assembly.GetType(classStr);
			if (type != null)
			{
				return type;
			}
		}
		return type;
	}

	private static object loadObject(System.Type type)
	{
		try
		{
			//Log.info(type.GetType().Name);
			Object o = Activator.CreateInstance(type) ;
			//Log.info(o);
			return o;
		}
		catch (Exception exception)
		{
			 
			return null;
		}
	}
}

