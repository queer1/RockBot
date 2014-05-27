using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using ChatSharp;
using MahApps.Metro.Controls;
using System.Windows.Media.Animation;
using System.Net;
using MahApps.Metro.Controls.Dialogs;

namespace RockBot2
{
	public partial class MainWindow : MetroWindow
	{
		private CInternalSettings InternalSettings = new CInternalSettings();
		private IrcClient client = null;
		private bool CanRequest = true;

		private ObservableCollection<RockSmithRequestEntry> Requests = new ObservableCollection<RockSmithRequestEntry>();

		public MainWindow()
		{
			InitializeComponent();

			skipsong_hook.RegisterHotKey(0, System.Windows.Forms.Keys.F2);
			skipsong_hook.KeyPressed += SkipFirst;

			togglereq_hook.RegisterHotKey(0, System.Windows.Forms.Keys.F3);
			togglereq_hook.KeyPressed += ToggleRequestsCallback;

			MainWindow1.Closing += (sender, e) =>
			{
				Save();
			};

			Load();

			UpdateTitleActiveRequests();

			lstRequests.DataContext = Requests;

			btnCheckForUpdates.DataContext = this;
		}

		private void SkipFirst(object sender, KeyPressedEventArgs e)
		{
			if (Requests.Count > 0)
			{
				Requests.RemoveAt(0);
				lstRequests.UpdateLayout();
			}

			UpdateTitleActiveRequests();
		}

		private void numSeconds_ValueChanged(object sender, RoutedEventArgs e)
		{
			var num = sender as NumericUpDown;

			if (num != null)
			{
				if (num.Value > 59)
				{
					num.Value = 0;
				}

				if (num.Value < 0)
				{
					num.Value = 59;
				}
			}
		}

		private void btnConnectIRC_Click(object sender, RoutedEventArgs e)
		{
			if (!String.IsNullOrEmpty(InternalSettings.ircname) && 
				!String.IsNullOrEmpty(InternalSettings.channel) &&
				!String.IsNullOrEmpty(InternalSettings.server))
			{
				if (client == null)
				{
					client = new IrcClient(InternalSettings.server, new IrcUser(InternalSettings.ircname, InternalSettings.ircname));
				}

				client.ConnectionComplete += (s, irc_e) =>
				{
					client.JoinChannel(InternalSettings.channel);
				};

				client.NetworkError += (s, irc_e) =>
				{
					MessageBox.Show("Error: " + irc_e.SocketError);
					btnConnectIRC.IsEnabled = true;
				};
				
				client.UserMessageRecieved += (s, irc_e) =>
				{
					SongRequest(irc_e.PrivateMessage);
				};

				client.ChannelMessageRecieved += (s, irc_e) =>
				{
					SongRequest(irc_e.PrivateMessage);
				};
				
				client.ConnectAsync();
			}

			btnConnectIRC.IsEnabled = false;
		}

		private void UpdateTitleActiveRequests()
		{
			Dispatcher.Invoke((() =>
			{
				lblRequestsCount.Content = Requests.Count + " Requests";
			}));
		}

		private void btnDisableRequests_Click(object sender, RoutedEventArgs e)
		{
			ToggleRequests();
		}

		private void lstRequests_MouseDown_1(object sender, MouseButtonEventArgs e)
		{
			lstRequests.UnselectAll();
		}

		private void btnAddCustomEntry_Click(object sender, RoutedEventArgs e)
		{
			RockSmithRequestEntry test = new RockSmithRequestEntry();
			test.FromUser = new IrcUser(InternalSettings.ircname, InternalSettings.ircname);
			Requests.Add(test);
			lstRequests.SelectedItem = test;
			lstRequests.ScrollIntoView(test);
			SelectListboxTextbox();
			UpdateTitleActiveRequests();
		}

		private void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			var rockentry = btn.DataContext as RockSmithRequestEntry;
			Requests.Remove(rockentry);
			UpdateTitleActiveRequests();
		}

		private void lstRequests_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				lstRequests.UpdateLayout();
				lstRequests.SelectedItem = null;
				lstRequests.SelectedIndex = -1;
				lstRequests.UnselectAll();
				e.Handled = true;
			}

			if (e.Key == Key.Tab && Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift))
			{
				int newindex = lstRequests.SelectedIndex - 1;

				if (newindex >= 0)
				{
					lstRequests.SelectedIndex = newindex;
					SelectListboxTextbox();
				}

				else
				{
					lstRequests.UnselectAll();
				}

				e.Handled = true;
				return;
			}

			if (e.Key == Key.Tab)
			{
				int newindex = lstRequests.SelectedIndex + 1;

				if (newindex < Requests.Count)
				{
					lstRequests.SelectedIndex = newindex;
					SelectListboxTextbox();
				}

				else
				{
					lstRequests.UnselectAll();
				}

				e.Handled = true;
			}
		}

		private void SelectListboxTextbox()
		{
			Dispatcher.Invoke((() =>
			{
				lstRequests.UpdateLayout();
				ListBoxItem item = lstRequests.ItemContainerGenerator.ContainerFromItem(lstRequests.SelectedItem) as ListBoxItem;

				if (item != null)
				{
					var tbox = FindVisualChild<TextBox>(item);

					if (tbox != null)
					{
						tbox.Focus();	
					}
				}
			}));
		}

		private void UpdateRequestAvailability()
		{
			Dispatcher.Invoke((() =>
			{
				CanRequest = !CanRequest;

				if (CanRequest)
				{
					btnDisableRequests.Content = "Disable Requests";
				}

				else
				{
					btnDisableRequests.Content = "Enable Requests";
				}
			}));
		}

		private void lstRequests_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				SelectListboxTextbox();
				e.Handled = true;
			}
		}

		private void lstRequests_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var pos = e.GetPosition(lstRequests);
			var result = lstRequests.InputHitTest(pos);

			// Sort of hacky but this is the only element that blocks
			if (result is TextBlock)
			{
				var item = FindVisualParent<ListBoxItem>(result as TextBlock);

				if (item != null)
				{
					lstRequests.SelectedItem = item.DataContext;
				}
			}

			// Allows clicking on an empty space in the listbox
			if (result is ScrollViewer)
			{
				lstRequests.UnselectAll();
			}
		}

		private async void btnCheckForUpdates_Click(object sender, RoutedEventArgs e)
		{
			var mainprogressdlg = await this.ShowProgressAsync("Please wait...", "Checking for updates");
			mainprogressdlg.SetCancelable(true);

			String downloadlocationtarget = "http://cf2.vacker.me/RockBot/version/";

			using (var updatecheck = new WebClient())
			{
				updatecheck.DownloadStringCompleted += async (cl_sender, cl_event) =>
				{
					if (mainprogressdlg.IsCanceled || cl_event.Cancelled)
					{
						return;
					}

					if (cl_event.Error != null)
					{
						await mainprogressdlg.CloseAsync();
						updatecheck.CancelAsync();
						updatecheck.DownloadProgressChanged -= null;

						var dlgres = await this.ShowMessageAsync("Error", cl_event.Error.Message, MessageDialogStyle.Affirmative);
						return;
					}

					String result = cl_event.Result;
					double newversion = double.Parse(cl_event.Result, CultureInfo.InvariantCulture);

					if (newversion > Version.FileVersion)
					{
						mainprogressdlg.SetIndeterminate();
						mainprogressdlg.SetMessage("Downloading RockBot version " + newversion.ToString(CultureInfo.InvariantCulture));

						String currentdir = Environment.CurrentDirectory;
						String targetdir = currentdir + "\\new";
						String targetfilename = targetdir + "\\RockBot" + newversion + ".zip";

						if (!Directory.Exists(targetdir))
						{
							Directory.CreateDirectory(targetdir);	
						}

						updatecheck.DownloadFileAsync(new Uri(downloadlocationtarget + "RockBot.zip"), targetfilename);

						updatecheck.DownloadFileCompleted += async (zip_sender, zip_event) =>
						{
							if (zip_event.Cancelled)
							{
								mainprogressdlg.SetMessage("Canceling...");
								await mainprogressdlg.CloseAsync();
								updatecheck.CancelAsync();
								updatecheck.DownloadProgressChanged -= null;

								if (Directory.Exists(targetdir))
								{
									Directory.Delete(targetdir, true);
								}

								return;
							}

							mainprogressdlg.SetMessage("Updating...");
							mainprogressdlg.SetIndeterminate();

							await mainprogressdlg.CloseAsync();
							updatecheck.CancelAsync();

							var choicedlg = await this.ShowMessageAsync("New RockBot", "You have updated to the latest version.\nRestart now?",
										MessageDialogStyle.AffirmativeAndNegative);

							GlobalUpdateData.Args = String.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"",
									targetfilename, // Zip to be extracted
									currentdir + "\\new\\files", // To this folder
									"RockBot", // And then launch this program
									System.Diagnostics.Process.GetCurrentProcess().Id.ToString()); // If this process doesnt exist

							GlobalUpdateData.Proc = currentdir + "\\rbupd.exe";
							GlobalUpdateData.OnExit = false;

							if (choicedlg == MessageDialogResult.Affirmative)
							{
								ChangeFiles(GlobalUpdateData);
							}

							else if (choicedlg == MessageDialogResult.Negative)
							{
								GlobalUpdateData.OnExit = true;
							}
						};
					}

					else
					{
						await mainprogressdlg.CloseAsync();
						updatecheck.CancelAsync();
						updatecheck.DownloadProgressChanged -= null;
						var dlg = await this.ShowMessageAsync("No update required.", "You have the latest version.", MessageDialogStyle.Affirmative);
					}
				};

				updatecheck.DownloadProgressChanged += (cl_sender, cl_event) =>
				{
					mainprogressdlg.SetMessage("" + cl_event.ProgressPercentage + "%");
					mainprogressdlg.SetProgress(cl_event.ProgressPercentage / 100.0);

					if (mainprogressdlg.IsCanceled)
					{
						mainprogressdlg.CloseAsync();
						updatecheck.CancelAsync();
						updatecheck.DownloadProgressChanged -= null;
						return;
					}
				};

				updatecheck.DownloadStringAsync(new Uri(downloadlocationtarget + "ver.txt"));	
			}
		}

		private void settingsscrviewer_mousedown(object sender, MouseButtonEventArgs e)
		{
			lstRequests.UnselectAll();
		}
	}
}
