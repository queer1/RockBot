using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO.Compression;
using System.IO;

namespace RockBotUpdater
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 5)
			{
				String src = args[0];
				String dest = args[1];
				String filename = args[2];
				int pid = int.Parse(args[3]);
				int startproc = int.Parse(args[4]);

				Process parentproc = null;
				bool procexists = true;

				try
				{
					parentproc = Process.GetProcessById(pid);
				}

				catch (ArgumentException)
				{
					procexists = false;
				}

				if (procexists)
				{
					DateTime giveuptime = DateTime.Now;
					giveuptime = giveuptime.Add(new TimeSpan(0, 0, 30));

					Console.WriteLine("Waiting for 30 seconds for RockBot2 to close...");

					while (!parentproc.HasExited)
					{
						if (DateTime.Now > giveuptime)
						{
							Console.WriteLine("Unable to catch RockBot2 process shutdown, giving up.");
							Console.ReadLine();
							Environment.Exit(1);
							return;
						}

						Thread.Sleep(200);
					}	
				}

				ZipFile.ExtractToDirectory(src, dest);

				var extractedfiles = System.IO.Directory.GetFiles(dest);

				String newdir = Directory.GetParent(dest).FullName;
				String rootdir = Directory.GetParent(newdir).FullName;

				newdir += "\\";
				rootdir += "\\";

				foreach (var file in extractedfiles)
				{
					var curfilename = Path.GetFileName(file);
					
					if (curfilename != "rbupd.exe")
					{
						File.Delete(rootdir + curfilename);
						File.Move(file, rootdir + curfilename);	
					}
				}

				if (!filename.EndsWith(".exe"))
				{
					filename += ".exe";
				}

				Directory.Delete(newdir, true);

				if (startproc == 1)
				{
					Process.Start(filename);
				}

				Environment.Exit(0);
			}
		}
	}
}
