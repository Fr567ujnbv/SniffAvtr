using System;
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

			// Parse each Photon command
			int commandOffset = 0;
			for (int i = 0; i < photon.CommandCount; i++)
			{
				PhotonPacket.Command command = new PhotonPacket.Command(photon.Data, commandOffset, photon.Length);
				commandOffset += (int)command.Length;

				// Filter only fragmented messages (packets bigger than ~1154 bytes)
				if (command.CommandType != PhotonPacket.Command.Type.SendReliableFragment)
				{
					continue;
				}
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
					byte[] data = message.Data;

					if (CheckBox_LogRawPackets.Checked)
						Logger.WriteLine($"{message.Signifier}, {message.MessageType}, {message.OperationCode}, {message.OperationReturnCode}, {message.OperationDebug}, {message.EventCode}, {message.ParameterCount}, " + Convert.ToBase64String(data, 0, data.Length));

					while (true)
					{
						if (data == null)
						{
							break;
						}

						var ptrUser = FindOffset(data, "user");
						if (ptrUser == 5) // Message doesn't have user record
						{
							// Abort parsing command
							break;
						}
						var ptrDisplayName = FindOffset(data, "displayName", after: ptrUser);
						var ptrUserId = FindOffset(data, "id", after: ptrUser);

						var ptrAvatarDict = FindOffset(data, "avatarDict");
						if (ptrAvatarDict == 11) // Message doesn't have avatar record
						{
							// Abort parsing command
							break;
						}
						var ptrAvatarName = FindOffset(data, "name", after: ptrAvatarDict);
						var ptrAvatarDescription = FindOffset(data, "description", after: ptrAvatarDict);
						var ptrAvatarId = FindOffset(data, "id", after: ptrAvatarDict);
						var ptrAvatarUpdatedAt = FindOffset(data, "updated_at", after: ptrAvatarDict);
						var ptrAvatarAuthor = FindOffset(data, "authorName", after: ptrAvatarDict);
						var ptrAvatarAuthorId = FindOffset(data, "authorId", after: ptrAvatarDict);
						var ptrAvatarAssetUrl = FindOffset(data, "assetUrl", after: ptrAvatarDict);

						var subitemstrings = new string[]
						{
							Encoding.UTF8.GetString(data.Skip(ptrDisplayName + 1).Take(data[ptrDisplayName]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrUserId + 1).Take(data[ptrUserId]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarName + 1).Take(data[ptrAvatarName]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarDescription + 1).Take(data[ptrAvatarDescription]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarId + 1).Take(data[ptrAvatarId]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarUpdatedAt + 1).Take(data[ptrAvatarUpdatedAt]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarAuthor + 1).Take(data[ptrAvatarAuthor]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarAuthorId + 1).Take(data[ptrAvatarAuthorId]).ToArray()),
							Encoding.UTF8.GetString(data.Skip(ptrAvatarAssetUrl + 1).Take(data[ptrAvatarAssetUrl]).ToArray())
						};
						Logger.WriteLine(string.Join(", ", subitemstrings));
						NewListViewItems.Enqueue(new ListViewItem(subitemstrings, -1));

						var tail = ptrAvatarDict + IndexInByteArray(data.Skip(ptrAvatarDict).ToArray(), "\x0003\x00ff\x0008\x000b");
						if (tail == -1)
						{
							break;
						}
						data = data.Skip(tail).ToArray();
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
					toCopy = selectedItem.SubItems[2].Text;
				}
				else if (sender == copyAvatarDescriptionToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[3].Text;
				}
				else if (sender == copyAvatarIDToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[4].Text;
				}
				else if (sender == copyAuthorDisplaynameToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[6].Text;
				}
				else if (sender == copyAuthorIDToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[7].Text;
				}
				else if (sender == copyAssetURLToolStripMenuItem)
				{
					toCopy = selectedItem.SubItems[8].Text;
				}
				Clipboard.SetText(toCopy);
			}
		}
	}
}
