using System;
using System.IO;
using System.Net;

namespace SniffAvtr
{
	internal class UDPPacket
	{
		//UDP header fields
		private ushort u16SourcePort;           //Sixteen bits for the source port number
		private ushort u16DestinationPort;      //Sixteen bits for the destination port number
		private ushort u16Length;               //Length of the UDP header
		private short s16Checksum;              //Sixteen bits for the checksum
												//(checksum can be negative so taken as short)
		//End UDP header fields

		private byte[] vecUDPData = new byte[4096];  //Data carried by the UDP packet
		public UDPPacket(byte[] buffer, int length)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer, 0, length))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					u16SourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u16DestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					u16Length = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
					s16Checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

					if (length > 8)
						Array.Copy(buffer, 8, vecUDPData, 0, length - 8);
				}
			}
		}

		public ushort SourcePort => u16SourcePort;
		public ushort DestinationPort => u16DestinationPort;
		public ushort Length => u16Length;
		public short Checksum => s16Checksum;
		public byte[] Data => vecUDPData;
	}
}