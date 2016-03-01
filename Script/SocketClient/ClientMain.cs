using System;

namespace XJSocketClient
{
	public class ClientMain
	{
		public ClientMain ()
		{
		}
		public static void Main(string[] args)
		{
			ClientSocketPort cp = new ClientSocketPort ();
			cp.open ("123.59.34.161", 7612);

		}

	}
}

