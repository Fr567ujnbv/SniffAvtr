using System;
using System.IO;
using System.Net;

namespace SniffAvtr
{
	internal class IPPacket
	{
		//IP Header fields
		private byte u8VersionAndHeaderLength;  //Eight bits for version and header length
		private byte u8DifferentiatedServices;  //Eight bits for differentiated services (TOS)
		private ushort u16TotalLength;           //Sixteen bits for total length of the datagram (header + message)
		private ushort u16Identification;        //Sixteen bits for identification
		private ushort u16FlagsAndOffset;        //Eight bits for flags and fragmentation offset
		private byte u8TTL;                     //Eight bits for TTL (Time To Live)
		private byte u8Protocol;                //Eight bits for the underlying protocol
		private short s16Checksum;                //Sixteen bits containing the checksum of the header
												  //(checksum can be negative so taken as short)
		private uint u32SourceIPAddress;         //Thirty two bit source IP Address
		private uint u32DestinationIPAddress;    //Thirty two bit destination IP Address
												 //End IP Header fields

		private byte u8HeaderLength;             //Header length
		private byte[] vecIPData = new byte[4096];  //Data carried by the datagram

		public IPPacket(byte[] buffer, int length)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer, 0, length))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					u8VersionAndHeaderLength = binaryReader.ReadByte();
					u8DifferentiatedServices = binaryReader.ReadByte();
					u16TotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u16Identification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u16FlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u8TTL = binaryReader.ReadByte();
					u8Protocol = binaryReader.ReadByte();
					s16Checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u32SourceIPAddress = (uint)binaryReader.ReadInt32();
					u32DestinationIPAddress = (uint)binaryReader.ReadInt32();

					u8HeaderLength = u8VersionAndHeaderLength;
					u8HeaderLength <<= 4;
					u8HeaderLength >>= 4;
					u8HeaderLength *= 4;

					if (u16TotalLength > u8HeaderLength)
						Array.Copy(buffer, u8HeaderLength, vecIPData, 0, u16TotalLength - u8HeaderLength);
				}
			}
		}

		public string Version
		{
			get
			{
				int version = u8VersionAndHeaderLength >> 4;
				if (version == 4)
					return "IP v4";
				else if (version == 6)
					return "IP v6";
				else
					return "Unknown";
			}
		}
		public byte HeaderLength => u8HeaderLength;
		public int MessageLength => u16TotalLength - u8HeaderLength;
		public byte DifferentiatedServices => u8DifferentiatedServices;
		public string Flags
		{
			get
			{
				int flags = u16FlagsAndOffset >> 13;
				if (flags == 2)
					return "Don't fragment";
				else if (flags == 1)
					return "More fragments to come";
				else
					return flags.ToString();
			}
		}
		public int FragmentationOffset => u16FlagsAndOffset & 0x1FFF;
		public byte TTL => u8TTL;
		public Protocol ProtocolType => (Protocol)u8Protocol;
		public short Checksum => s16Checksum;
		public IPAddress SourceAddress => new IPAddress(u32SourceIPAddress);
		public IPAddress DestinationAddress => new IPAddress(u32DestinationIPAddress);
		public ushort TotalLength => u16TotalLength;
		public ushort Identification => u16Identification;
		public byte[] Data => vecIPData;

		public enum Protocol
		{
			TCP = 6,
			UDP = 17,
			Unknown = -1
		}
	}
}
