using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;
using MahApps.Metro.Controls;

namespace RockBot2
{
	public partial class MainWindow : MetroWindow
	{
		private KeyboardHook skipsong_hook = new KeyboardHook();
		private KeyboardHook togglereq_hook = new KeyboardHook();

		private void SongRequest(PrivateMessage requestfull)
		{
			if (requestfull.Message.StartsWith("!req"))
			{
				if (!CanRequest)
				{
					client.SendMessage("Sorry! Requesting is currently disabled.", requestfull.User.Nick);
					return;
				}

				Dispatcher.Invoke((() =>
				{
					foreach (RockSmithRequestEntry entry in Requests)
					{
						if (DateTime.Now > entry.NextRequestTime)
						{
							entry.CanRequest = true;
						}

						if (entry.FromUser.Hostname == requestfull.User.Hostname && !entry.CanRequest)
						{
							var difference = entry.NextRequestTime - DateTime.Now;
							String endstr;

							if (difference.Minutes > 0)
							{
								endstr = String.Format("{0} minutes and {1} seconds", difference.Minutes, difference.Seconds);
							}

							else
							{
								endstr = String.Format("{0} seconds", difference.Seconds);
							}

							client.SendMessage(String.Format("Sorry! Try again in {0}", endstr), entry.FromUser.Nick);
							return;
						}
					}

					var newentry = new RockSmithRequestEntry();
					String reqstr = requestfull.Message.Substring(4);

					const int maxlength = 72;

					if (reqstr.Length > maxlength)
					{
						reqstr = reqstr.Substring(0, maxlength);
					}

					reqstr = reqstr.Trim();

					newentry.Request = reqstr;
					newentry.FromUser = requestfull.User;
					newentry.NextRequestTime = DateTime.Now;
					newentry.NextRequestTime =
						newentry.NextRequestTime.Add(new TimeSpan(0, InternalSettings.reqdelayminutes,
							InternalSettings.reqdelayseconds));
					newentry.CanRequest = false;

					Requests.Add(newentry);

					client.SendMessage("\"" + reqstr + "\" Added", requestfull.User.Nick);
				}));

				UpdateTitleActiveRequests();
			}
		}

		private void ToggleRequests()
		{
			if (client != null)
			{
				if (CanRequest)
				{
					client.SendMessage("Requesting now disabled.", InternalSettings.channel);
				}

				else
				{
					client.SendMessage("Requesting now enabled.", InternalSettings.channel);
				}

				UpdateRequestAvailability();
			}
		}

		private void ToggleRequestsCallback(object sender, KeyPressedEventArgs e)
		{
			ToggleRequests();
		}
	}
}
