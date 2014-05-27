using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockBot2
{
	[Serializable]
	public class CInternalSettings
	{
		public CInternalSettings()
		{
			ircname = "RockBot2";
			channel = "#rockbottestchannel";
			server = "us.quakenet.org";
			reqdelayminutes = 0;
			reqdelayseconds = 30;

			topfontsize = 30;
			topitemheight = 60;
			normalitemfontsize = 20;
			righttoleft = false;
			displaynicks = true;

			nickopacity = 1.0;
			scrollerwidth = 416.0;
		}

		public String ircname { get; set; }
		public String channel { get; set; }
		public String server { get; set; }
		public int reqdelayminutes { get; set; }
		public int reqdelayseconds { get; set; }

		public int topfontsize { get; set; }
		public int topitemheight { get; set; }
		public int normalitemfontsize { get; set; }
		public bool righttoleft { get; set; }
		public bool displaynicks { get; set; }

		public double nickopacity { get; set; }

		public double scrollerwidth { get; set; }
	}
}
