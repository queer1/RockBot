using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;

namespace RockBot2
{
	public partial class MainWindow : MetroWindow
	{
		private ChangeFilesData GlobalUpdateData = new ChangeFilesData();

		class ChangeFilesData
		{
			public ChangeFilesData()
			{
				OnExit = false;
			}

			public String Proc { get; set; }
			public String Args { get; set; }

			// True if the updater should start this program again.
			public bool OnExit { get; set; }
		}

		private void ChangeFiles(ChangeFilesData data)
		{
			if (data.OnExit)
			{
				data.Args += " \"0\"";
			}

			else
			{
				data.Args += " \"1\"";
			}

			System.Diagnostics.Process.Start(data.Proc, data.Args);
			Application.Current.Shutdown();
		}
	}
}
