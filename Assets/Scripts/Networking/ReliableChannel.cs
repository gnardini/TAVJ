using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class ReliableChannel : Channel {

	private const int ACK = 254;
	private const int NON_RELIABLE = 255;
	private int _reliableSeqNumber;

	private Channel _channel;
	private Dictionary<IPEndPoint , Dictionary<int, byte[]>> _reliableMessages; 

	public ReliableChannel(int port) : base(port){
		_reliableMessages = new Dictionary<IPEndPoint , Dictionary<int, byte[]>> ();
		_reliableSeqNumber = 0;
	}

	public override Packet getPacket(){
		Packet packet = base.getPacket ();
		while (packet != null) {
			byte[] data = packet.getData ();
			int header = data [0];
			switch (header) {
			case ACK:{
					_reliableMessages [packet.getAddress ()].Remove (data [1]);
					break;
				}
			case NON_RELIABLE:{
					return new Packet (createCopy (data, 1), packet.getAddress ());
				}
			default:{
					Send(prepareToSend(ACK, new byte[]{data[0]}), packet.getAddress());
					return new Packet (createCopy (data, 1), packet.getAddress ());
				}
			}
			packet = base.getPacket ();
		}
		return null;
	}


	public byte[] createCopy(byte[] data, int startOffset){
		byte[] new_data = new byte[data.Length - startOffset];
		Buffer.BlockCopy (data, startOffset, new_data, 0, new_data.Length);
		return new_data;
	}

	public static byte[] prepareToSend(int messageType, byte[] data){
		BitBuffer bitBuffer = new BitBuffer ();
		bitBuffer.WriteByte ((byte)messageType);
		bitBuffer.WriteBytes (data);
		return bitBuffer.Read ();
	}
}
