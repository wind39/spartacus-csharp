/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace Spartacus.Tools.WebServer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Spartacus.Net.WebServer v_server = null;
			int v_port;

			if (args.Length == 2 && args[1] == "--debug" && int.TryParse(args[0], out v_port))
				v_server = new Spartacus.Net.WebServer(v_port);
			else
			{
				if (args.Length == 1 && args[0] == "--debug")
					v_server = new Spartacus.Net.WebServer(9000);
				else
				{
					if (args.Length == 1 && int.TryParse(args[0], out v_port))
						v_server = new Spartacus.Net.WebServer(v_port);
					else
					{
						if (args.Length == 0)
							v_server = new Spartacus.Net.WebServer(9000);
						else
						{
							Console.WriteLine("Usage: WebServer.exe [port] [--debug]");
							System.Environment.Exit(0);
						}
					}
				}
			}

			v_server.Start();

			Console.WriteLine();
			Console.Read();

			v_server.Stop();
		}
	}
}
