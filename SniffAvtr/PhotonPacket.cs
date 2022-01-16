using System;
using System.IO;
using System.Linq;
using System.Net;

namespace SniffAvtr
{
	internal class PhotonPacket
	{
		private ushort u16PeerID;
		private byte u8CRCEnabled;
		private byte u8CommandCount;
		private uint u32Timestamp;
		private uint u32Challenge;
		private uint u32Extra;
		private int s32Length;

		private byte[] vecPhotonData = new byte[4096];

		public PhotonPacket(byte[] buffer, ushort length)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer, 0, length))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					u16PeerID = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u8CRCEnabled = binaryReader.ReadByte();
					u8CommandCount = binaryReader.ReadByte();
					u32Timestamp = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
					u32Challenge = (uint)binaryReader.ReadInt32();
					u32Extra = (uint)binaryReader.ReadInt32();
					s32Length = length - 16;

					if (length > 16)
						Array.Copy(buffer, 16, vecPhotonData, 0, length - 16);
				}
			}
		}

		public ushort PeerID => u16PeerID;
		public byte CRCEnabled => u8CRCEnabled;
		public byte CommandCount => u8CommandCount;
		public uint TimeStamp => u32Timestamp;
		public uint Challenge => u32Challenge;
		public uint Extra => u32Extra;
		public byte[] Data => vecPhotonData;
		public int Length => s32Length;

		internal class Command
		{
			private byte u8Type;
			private byte u8ChannelID;
			private byte u8Flags;
			private byte u8Reserved;
			private uint u32Length;
			private uint u32ReliableSequenceNumber;
			private byte[] vecCommandData = new byte[4096];

			// PayloadFeilds
			// Acknowledge
			private int s32ReceivedReliableSequenceNumber;
			private uint u32ReceivedSentTimestamp;
			// Connect
			private ushort u16RequestedPeerID;
			private byte[] vecMessageData = new byte[4096];
			// VerifyConnect
			private ushort u16NewPeerID;
			///private byte[] vecMessageData;
			// SendReliable
			///private byte[] vecMessageData;
			// SendUnreliable
			private uint u32UnreliableSequenceNumber;
			///private byte[] vecMessageData;
			// SendReliableFragment
			private int s32StartSequenceNumber;
			private int s32Fragments;
			private int s32Index;
			private int s32TotalLength;
			private int s32Offset;
			///private byte[] (fragmented)vecMessageData;


			public Command(byte[] buffer, int index, int length)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer, index, length))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						u8Type = binaryReader.ReadByte();
						u8ChannelID = binaryReader.ReadByte();
						u8Flags = binaryReader.ReadByte();
						u8Reserved = binaryReader.ReadByte();
						u32Length = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
						u32ReliableSequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

						if (length > u32Length)
							Array.Copy(buffer, index + 12, vecCommandData, 0, u32Length - 12);
					}
				}
			}

			public Type CommandType => (Type)u8Type;
			public byte ChannelID => u8ChannelID;
			public byte Flags => u8Flags;
			public byte Reserved => u8Reserved;
			public uint Length => u32Length;
			public uint ReliableSequenceNumber => u32ReliableSequenceNumber;

			public enum Type
			{
				Acknowledge = 1,
				Connect,
				VerifyConnect,
				Disconnect,
				Ping,
				SendReliable,
				SendUnreliable,
				SendReliableFragment,
				SendUnsequenced,
				ConfigureBandwidthLimit,
				ConfigureThrottling,
				FetchServerTimestamp
			}

			internal void ParsePayload()
			{
				using (MemoryStream memoryStream = new MemoryStream(vecCommandData, 0, (int)u32Length))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						switch (CommandType)
						{
							case Type.Acknowledge:
								if (u32Length != 8)
									break;
								s32ReceivedReliableSequenceNumber = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								u32ReceivedSentTimestamp = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								break;
							case Type.Connect:
								if (u32Length <= 2)
									break;
								u16RequestedPeerID = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
								Array.Copy(vecCommandData, 2, vecMessageData, 0, u32Length - 2);
								break;
							case Type.VerifyConnect:
								if (u32Length <= 2)
									break;
								u16NewPeerID = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
								Array.Copy(vecCommandData, 2, vecMessageData, 0, u32Length - 2);
								break;
							case Type.SendReliable:
								Array.Copy(vecCommandData, 0, vecMessageData, 0, u32Length);
								break;
							case Type.SendUnreliable:
								if (u32Length <= 4)
									break;
								u32UnreliableSequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								Array.Copy(vecCommandData, 4, vecMessageData, 0, u32Length - 4);
								break;
							case Type.SendReliableFragment:
								if (u32Length <= 20)
									break;
								s32StartSequenceNumber = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								s32Fragments = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								s32Index = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								s32TotalLength = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								s32Offset = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
								Array.Copy(vecCommandData, 20, vecMessageData, 0, u32Length - 20);
								break;
						}
					}
				}
			}

			public int ReceivedReliableSequenceNumber => s32ReceivedReliableSequenceNumber;
			public uint ReceivedSentTimestamp => u32ReceivedSentTimestamp;
			public ushort RequestedPeerID => u16RequestedPeerID;
			public byte[] MessageData => vecMessageData;
			public ushort NewPeerID => u16NewPeerID;
			public uint UnreliableSequenceNumber => u32UnreliableSequenceNumber;
			public int StartSequenceNumber => s32StartSequenceNumber;
			public int Fragments => s32Fragments;
			public int Index => s32Index;
			public int TotalLength => s32TotalLength;
			public int Offset => s32Offset;
		}

		internal class Message
		{
			private byte u8Signifier;
			private byte u8Type;
			private byte[] vecParameterData;

			public Message(byte[] buffer, int index, int length)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer, index, length))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						u8Signifier = binaryReader.ReadByte();
						u8Type = binaryReader.ReadByte();

						vecParameterData = new byte[length - 2];
						Array.Copy(buffer, 2, vecParameterData, 0, length - 2);
					}
				}
			}

			public byte Signifier => u8Signifier;
			public Type MessageType => (Type)u8Type;

			public enum Type
			{
				OperationRequest = 2,
				OperationResponse,
				EventData,
				OperationResponse7 = 7
			}

			public byte[] Data => vecParameterData;
		}
	}
}