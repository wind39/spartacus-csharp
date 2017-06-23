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

namespace Spartacus.Tools.Compressor
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Spartacus.Utils.FileExplorer v_explorer;
			Spartacus.Utils.File v_directory;
			Spartacus.Utils.ProgressEventClass v_progress;

			v_explorer = new Utils.FileExplorer(".");
			v_directory = new Spartacus.Utils.File(Spartacus.Utils.FileType.DIRECTORY, "./Data");
			v_progress = new Spartacus.Utils.ProgressEventClass();
			v_progress.ProgressEvent += OnProgress;

			Console.Write("Compressing into Data_ZipStorer.zip...");
			v_explorer.CompressDirectory("Data_ZipStorer.zip", v_directory);
			Console.WriteLine();
			Console.WriteLine();

			Console.Write("Compressing into Data_DotNetZip.zip...");
			v_explorer.CompressDirectory("Data_DotNetZip.zip", v_directory, v_progress);
			Console.WriteLine();
		}

		public static void OnProgress(Spartacus.Utils.ProgressEventClass obj, Spartacus.Utils.ProgressEventArgs e)
		{
			Console.WriteLine(string.Format("{0} - {1}", e.v_percentage, e.v_message));
		}
	}
}
