using System;
using System.Collections;

public class FPortManager
{
	private static FPortManager _Instance;
	private static bool _singleton = true;
	private Hashtable ports;

	public FPortManager()
	{
		if (_singleton)
		{
			throw new Exception("this is singleton!");
		}
	}

	public void clearPorts()
	{
		if (this.ports != null)
		{
			this.ports.Clear();
		}
	}

	public T getFPort<T>() where T: BaseFPort
	{
		System.Type type = typeof(T);
		string name = type.Name;
		if (this.ports == null)
		{
			this.ports = new Hashtable();
		}
		if (this.ports[name] == null)
		{
			object obj2 = Activator.CreateInstance(type);
			this.ports.Add(name, obj2);
		}
		return (this.ports[name] as T);
	}

	public BaseFPort getFPort(string str)
	{
		if (this.ports == null)
		{
			this.ports = new Hashtable();
		}
		if (this.ports[str] == null)
		{
			this.ports.Add(str, DomainAccess.getObject(str));
		}
		return (this.ports[str] as BaseFPort);
	}

	public static FPortManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_singleton = false;
				_Instance = new FPortManager();
				_singleton = true;
				return _Instance;
			}
			return _Instance;
		}
		set
		{
			_Instance = value;
		}
	}
}

