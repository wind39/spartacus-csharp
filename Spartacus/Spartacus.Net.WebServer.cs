/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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

namespace Spartacus.Net
{
	public class WebServer
	{
		public int v_port;

		private Spartacus.ThirdParty.WebServer.ApplicationServer v_server;

		public WebServer(int p_port)
		{
			this.v_port = p_port;
			this.v_server = null;
		}

		public void Start()
		{
			string v_filename;

			v_filename = System.Reflection.Assembly.GetExecutingAssembly().Location;

			try
			{
				if (! System.IO.Directory.Exists("bin"))
					System.IO.Directory.CreateDirectory("bin");

				System.IO.File.Copy(v_filename, "bin/" + System.IO.Path.GetFileName(v_filename), true);
			}
			catch
			{
				
			}

			this.v_server = Spartacus.ThirdParty.WebServer.XSP.Server.Instantiate(new [] { "--applications", "/:.", "--port", this.v_port.ToString(), "--nonstop" });
		}

		public void Stop()
		{
			if (this.v_server != null)
				this.v_server.Stop();
		}
	}
}
