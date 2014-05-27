using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;

namespace RockBot2
{
	public class RockSmithRequestEntry
	{
		public IrcUser FromUser { get; set; }
		public String Request { get; set; }
		public DateTime NextRequestTime { get; set; }
		public bool CanRequest { get; set; }

		public RockSmithRequestEntry()
		{
			CanRequest = true;
		}
	}
}
