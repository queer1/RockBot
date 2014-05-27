using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MahApps.Metro.Controls;

namespace RockBot2
{
	public partial class MainWindow : MetroWindow
	{
		private const String filename = "settings.json";

		private void Save()
		{
			using (var writer = new StreamWriter(filename))
			{
				writer.Write(new JavaScriptSerializer().Serialize(InternalSettings));
			}

			if (GlobalUpdateData.OnExit)
			{
				ChangeFiles(GlobalUpdateData);	
			}
		}

		private void Load()
		{
			if (!File.Exists(filename))
			{
				MainWindow1.DataContext = InternalSettings;
				return;
			}

			using (var reader = new StreamReader(filename))
			{
				InternalSettings = new JavaScriptSerializer().Deserialize<CInternalSettings>(reader.ReadToEnd());
			}

			if (InternalSettings == null)
			{
				InternalSettings = new CInternalSettings();
			}

			MainWindow1.DataContext = InternalSettings;
		}
	}
}
