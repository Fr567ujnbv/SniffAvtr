using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SniffAvtr
{
	public partial class MainForm : Form
	{
		private bool ContinueCapturing = false;
		private Socket MainStocket;
		private byte[] Buffer = new byte[4096];
		private ConcurrentQueue<ListViewItem> NewListViewItems = new ConcurrentQueue<ListViewItem>();
		private List<PhotonPacket.Command> FragmentedMessages = new List<PhotonPacket.Command>();
		public MainForm()
		{
			InitializeComponent();
			Logger.WriteLine($"{Text} started");

			IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress address in hostEntry.AddressList)
			{
				ComboBox_Interface.Items.Add(address.ToString());
			}

			UpdateTimer.Start();
		}

		private void Button_StartStop_Click(object sender, EventArgs e)
		{
			if (ComboBox_Interface.Text == "")
			{
				MessageBox.Show("Select an Interface to capture from.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			try
			{
				if (!ContinueCapturing)
				{
					SetupAndStart_MainSocket();
				}
				else
				{
					Stop_MainSocket();
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"An error occurred changing capture state: {ex.Message}\n{ex.StackTrace}");
			}
		}

		private void SetupAndStart_MainSocket()
		{
			Logger.WriteLine("Starting capture");
			Button_StartStop.Text = "&Stop";
			ContinueCapturing = true;
			MainStocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
			MainStocket.Bind(new IPEndPoint(IPAddress.Parse(ComboBox_Interface.Text), 0));
			MainStocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
			MainStocket.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });

			ReceiveNextPacket();
		}

		private void Stop_MainSocket()
		{
			Logger.WriteLine("Stoping capture");
			Button_StartStop.Text = "&Start";
			ContinueCapturing = false;
			MainStocket.Close();
		}

		private void ReceiveNextPacket()
		{
			Buffer = new byte[1024 * 64]; // 4kb was too small and MainStocket.EndReceive(result) was throw errors
			MainStocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
		}

		private void OnReceive(IAsyncResult result)
		{
			try
			{
				int count = MainStocket.EndReceive(result);
				ParseData(Buffer, count);

				if (ContinueCapturing)
				{
					ReceiveNextPacket();
				}
			}
			catch (ObjectDisposedException) { }
			catch (Exception ex)
			{
				Logger.WriteLine($"An error occurred receiving data: {ex.Message}\n{ex.StackTrace}");
			}
		}

		private void ParseData(byte[] buffer, int count)
		{
			// Parse data as IP packet
			IPPacket ip = new IPPacket(buffer, count);
			// Filter UDP traffic
			if (ip.ProtocolType != IPPacket.Protocol.UDP)
			{
				return;
			}
			// Parse payload as UDP packet
			UDPPacket udp = new UDPPacket(ip.Data, ip.MessageLength);
			// Filter port 5056 traffic
			if (/*udp.DestinationPort == 5056 || */udp.SourcePort != 5056)
			{
				return;
			}
			// Parse payload as Photon package packet
			PhotonPacket photon = new PhotonPacket(udp.Data, udp.Length);

			List<PhotonPacket.Message> messages = new List<PhotonPacket.Message>();

			// Parse each Photon command
			int commandOffset = 0;
			for (int i = 0; i < photon.CommandCount; i++)
			{
				PhotonPacket.Command command = new PhotonPacket.Command(photon.Data, commandOffset, photon.Length);
				commandOffset += (int)command.Length;

				if (command.CommandType == PhotonPacket.Command.Type.SendReliable)
				{
					command.ParsePayload();
					PhotonPacket.Message message = new PhotonPacket.Message(command.MessageData, 0, (int)command.Length);
					messages.Add(message);
				}
				else if (command.CommandType == PhotonPacket.Command.Type.SendReliableFragment)
				{
					command.ParsePayload();
					FragmentedMessages.Add(command);

					// Check if full set of fragments now exists
					var matching = FragmentedMessages.FindAll(x => x.StartSequenceNumber == command.StartSequenceNumber && x.TotalLength == command.TotalLength);
					if (matching.Count >= command.Fragments)
					{
						bool complete = true;
						for (int j = 0; j < command.Fragments; j++)
						{
							if (!matching.Exists(x => x.Index == j))
							{
								complete = false;
								break;
							}
						}

						if (!complete)
						{
							continue;
						}
						// Reassemble all fragments into message
						FragmentedMessages.RemoveAll(x => x.StartSequenceNumber == command.StartSequenceNumber);

						byte[] messageBuffer = new byte[command.TotalLength];
						for (int j = 0; j < command.Fragments; j++)
						{
							var fragment = matching.Find(x => x.Index == j);
							Array.Copy(fragment.MessageData, 0, messageBuffer, fragment.Offset, fragment.Length - 32);
						}


						// Parse reassembled message
						PhotonPacket.Message message = new PhotonPacket.Message(messageBuffer, 0, command.TotalLength);
						messages.Add(message);
					}
				}
			}

			foreach (PhotonPacket.Message message in messages)
			{
				byte code = 0;
				ParameterDictionary parameters = null;
				short returnCode = 0;
				string debug = "";

				try//deserializing message using Photon SDK
				{
					var protocol = new Protocol18();
					var streamBuffer = new StreamBuffer(message.Data);

					switch (message.MessageType)
					{
						case PhotonPacket.Message.Type.OperationRequest:
							var operationRequest = protocol.DeserializeOperationRequest(streamBuffer);
							code = operationRequest.OperationCode;
							parameters = operationRequest.Parameters;
							break;
						case PhotonPacket.Message.Type.OperationResponse:
						case PhotonPacket.Message.Type.OperationResponse7:
							var operationResponse = protocol.DeserializeOperationResponse(streamBuffer);
							code = operationResponse.OperationCode;
							parameters = operationResponse.Parameters;
							debug = operationResponse.DebugMessage;
							returnCode = operationResponse.ReturnCode;
							break;
						case PhotonPacket.Message.Type.EventData:
							var eventData = protocol.DeserializeEventData(streamBuffer);
							code = eventData.Code;
							parameters = eventData.Parameters;
							break;
					}
				}
				catch (Exception ex)
				{
					Logger.WriteLine($"An error occurred parsing message data: {ex.Message}\n{ex.StackTrace}");
					Logger.WriteLine($"Message info: {message.Signifier}, {message.MessageType}, {code}, {returnCode}, {debug}, {parameters?.Count}, " + Convert.ToBase64String(message.Data, 0, message.Data.Length));
				}

				if (CheckBox_LogRawPackets.Checked && (byte)message.MessageType < 0x80)
					Logger.WriteLine($"{message.Signifier}, {message.MessageType}, {code}, {returnCode}, {debug}, {parameters?.Count}, " + Convert.ToBase64String(message.Data, 0, message.Data.Length));

				if (parameters != null)
				{
					if (CheckBox_LogRawPackets.Checked)
						Logger.WriteLine($"{message.Signifier}, {message.MessageType}, {code}, {returnCode}, {debug}, {parameters.Count}, {parameters.ToStringFull()}");

					// Go through each parameter to find property record
					foreach (var parameter in parameters)
					{
						if (parameter.Value is IDictionary paramDict)
						{
							if (CheckBox_LogRawPackets.Checked)
								Logger.WriteLine($"{message.Signifier}:{parameter.Key}: " + SupportClass.DictionaryToString(paramDict));

							if (!TryParseProperty(paramDict))
							{
								// Failed to parse. Try as list of props
								foreach (DictionaryEntry subItem in paramDict)
								{
									if (subItem.Value is IDictionary subDict)
									{
										if (CheckBox_LogRawPackets.Checked)
											Logger.WriteLine($"{message.Signifier}:{subItem.Key}: " + SupportClass.DictionaryToString(subDict));

										if (!TryParseProperty(subDict))
										{
											Debugger.Break();
										}
									}
									else
									{
										if (CheckBox_LogRawPackets.Checked)
											Logger.WriteLine($"{message.Signifier}:{subItem.Key}: {subItem.Value}");
									}
								}
							}
						}
						else
						{
							if (CheckBox_LogRawPackets.Checked)
								Logger.WriteLine($"{message.Signifier}:{parameter.Key}: {parameter.Value}");
						}
					}
				}
			}
		}

		private string DumpHex(byte[] buffer, int maxlength)
		{
			string output = "";
			for (int i = 0; i < maxlength; i++)
			{
				byte b = buffer[i];
				output += $"{b:X2} ";
			}
			return output;
		}

		private bool TryParseProperty(IDictionary propertyTable)
		{
			bool isValidTable = propertyTable["user"] != null && propertyTable["avatarDict"] != null;
			if (isValidTable)
			{
				var userDict = propertyTable["user"] as IDictionary;
				var avatarDict = propertyTable["avatarDict"] as IDictionary;

				// Build list of asset variants
				var unityPackages = avatarDict?["unityPackages"];
				var assetList = "";
				if (unityPackages is object[])
				{
					foreach (Dictionary<string, object> item in unityPackages as object[])
					{
						if (assetList.Length > 0)
						{
							assetList += ",";
						}
						assetList += $"{item["platform"]}-{item["unityVersion"]}={item["assetUrl"]}";
					}
				}

				var subitemstrings = new string[]
				{
					userDict["displayName"].ToString(),
					userDict["id"].ToString(),
					userDict["last_platform"].ToString(),
					avatarDict["name"].ToString(),
					avatarDict["description"].ToString(),
					avatarDict["id"].ToString(),
					avatarDict["releaseStatus"].ToString(),
					avatarDict["updated_at"].ToString(),
					avatarDict["authorName"].ToString(),
					avatarDict["authorId"].ToString(),
					avatarDict["assetUrl"].ToString(),
					assetList
				};
				Logger.WriteLine(string.Join(", ", subitemstrings));
				NewListViewItems.Enqueue(new ListViewItem(subitemstrings, -1));
			}
			return isValidTable;
		}

		private int FindOffset(byte[] buffer, string key, int after = 0)
		{
			if (after != 0)
			{
				buffer = buffer.Skip(after).ToArray();
			}
			int len = key.Length;
			string pattern = Encoding.Default.GetString(To7BitEncodedInt(len)) + key;
			return IndexInByteArray(buffer, pattern) + len + 2 + after;
		}

		private byte[] To7BitEncodedInt(int value)
		{
			byte[] output;
			if (value < 0x80)
				output = new byte[] { (byte)value };
			else if (value < 0x4000)
				output = new byte[] { (byte)(value | 0x80), (byte)(value >> 7) };
			else if (value < 0x200000)
				output = new byte[] { (byte)(value | 0x80), (byte)(value >> 7 | 0x80), (byte)(value >> 14) };
			else if (value < 0x1000000)
				output = new byte[] { (byte)(value | 0x80), (byte)(value >> 7 | 0x80), (byte)(value >> 14 | 0x80), (byte)(value >> 21) };
			else
				output = new byte[] { (byte)(value | 0x80), (byte)(value >> 7 | 0x80), (byte)(value >> 14 | 0x80), (byte)(value >> 21 | 0x80), (byte)((uint)value >> 28) };
			return output;
		}

		private int From7BitEncodedInt(byte[] buffer)
		{
			// Needs to also signal the amount of bytes consumed. Should probably just use BinaryReader
			throw new NotImplementedException();
		}

		private int IndexInByteArray(byte[] buffer, string pattern) => IndexInByteArray(buffer, pattern.Select(c => (byte)c).ToArray());
		private int IndexInByteArray(byte[] buffer, byte[] pattern)
		{
			int maxFirstCharSlot = buffer.Length - pattern.Length + 1;
			for (int i = 0; i < maxFirstCharSlot; i++)
			{
				if (buffer[i] != pattern[0]) // compare only first byte
					continue;

				// found a match on first byte, now try to match rest of the pattern
				for (int j = pattern.Length - 1; j >= 1; j--)
				{
					if (buffer[i + j] != pattern[j]) break;
					if (j == 1) return i;
				}
			}
			return -1;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Logger.WriteLine($"{Text} closing");
			if (ContinueCapturing)
			{
				MainStocket.Close();
			}
			Logger.Flush();
		}

		private void Button_Clear_Click(object sender, EventArgs e)
		{
			Invoke((MethodInvoker)delegate () { MainListView.Items.Clear(); });
		}

		private void UpdateTimer_Tick(object sender, EventArgs e)
		{
			if (!NewListViewItems.IsEmpty)
			{
				MainListView.BeginUpdate();
				while (NewListViewItems.TryDequeue(out ListViewItem result))
				{
					bool skip = false;
					if (CheckBox_SkipDuplicates.Checked)
					{
						foreach (ListViewItem item in MainListView.Items)
						{
							if (item.SubItems[4].Text == result.SubItems[4].Text)
							{
								if (item.SubItems[1].Text == result.SubItems[1].Text)
								{
									skip = true;
									break;
								}
							}
						}
					}
					if (!skip)
					{
						MainListView.Items.Add(result);
					}
				}

				/*if (MainListView.Items.Count > 1000) // Stop at limit
				{
					Stop_MainSocket();
				}*/
				while (MainListView.Items.Count > 1000)
				{
					MainListView.Items.RemoveAt(0);
				}

				if (CheckBox_AutoScroll.Checked)
					MainListView.Items[MainListView.Items.Count - 1].EnsureVisible();

				MainListView.EndUpdate();
			}
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MainListView.SelectedItems.Count == 1)
			{
				ListViewItem selectedItem = MainListView.SelectedItems[0];
				string toCopy = "";
				if (sender == copyUserDisplaynameToolStripMenuItem)
				{
					toCopy = selectedItem.Text;
				}
				else if (sender == copyUserIDToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[1].Text;
				}
				else if (sender == copyAvatarNameToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[3].Text;
				}
				else if (sender == copyAvatarDescriptionToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[4].Text;
				}
				else if (sender == copyAvatarIDToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[5].Text;
				}
				else if (sender == copyAuthorDisplaynameToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[8].Text;
				}
				else if (sender == copyAuthorIDToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[9].Text;
				}
				else if ((sender is ToolStripMenuItem) && (sender as ToolStripMenuItem).Name.StartsWith("http"))
				{
					toCopy = (sender as ToolStripMenuItem).Name;
				}
				if (!string.IsNullOrEmpty(toCopy))
					Clipboard.SetText(toCopy);
			}
		}

		private void ContextMenuStrip_ListView_Opening(object sender, CancelEventArgs e)
		{
			if (MainListView.SelectedItems.Count == 1)
			{
				copyAssetURLToolStripMenuItem.DropDownItems.Clear();
				ListViewItem selectedItem = MainListView.SelectedItems[0];
				copyAssetURLToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("default", null, copyToolStripMenuItem_Click, selectedItem.SubItems[10].Text));
				foreach (string url in selectedItem.SubItems[11].Text.Split(','))
				{
					var pair = url.Split('=');
					copyAssetURLToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(pair[0], null, copyToolStripMenuItem_Click, pair[1]));
				}
			}
		}
	}
}
