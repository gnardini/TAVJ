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
	private Dictionary<IPEndPoint , Dictionary<byte, byte[]>> _reliableMessages; 

	public ReliableChannel(int port) : base(port){
		_reliableMessages = new Dictionary<IPEndPoint , Dictionary<byte, byte[]>> ();
		_reliableSeqNumber = 0;
	}


	public void ResendReliableMessages(){
		foreach (KeyValuePair<IPEndPoint , Dictionary<byte, byte[]>> pair in _reliableMessages) {
			foreach (KeyValuePair<byte, byte[]> innerPair in pair.Value) {
				Send (innerPair.Value, pair.Key, true);
			}
		}
	}

	public override Packet GetPacket(){

		// Esto deberia ir en otro lado.
		 ResendReliableMessages ();

		Packet packet = base.GetPacket ();
		while (packet != null) {
			byte[] data = packet.getData ();
			int header = data [0];
			switch (header) {
			case ACK:{
					Debug.Log ("Ack received");
					_reliableMessages [packet.getAddress ()].Remove (data [1]);
					break;
				}
			case NON_RELIABLE:{
					return new Packet (CreateCopy (data, 1), packet.getAddress ());
				}
			default:{
					Debug.Log ("Reliable received");
					Send(PrepareToSend(ACK, new byte[]{data[0]}), packet.getAddress());
					return new Packet (CreateCopy (data, 1), packet.getAddress ());
				}
			}
			packet = base.GetPacket ();
		}
		return null;
	}

	public byte GetNextReliableSeqNumber(){
		int aux = _reliableSeqNumber;
		_reliableSeqNumber=(_reliableSeqNumber+1)%254;
		return (byte)aux;
	}

	public byte[] CreateCopy(byte[] data, int startOffset){
		byte[] new_data = new byte[data.Length - startOffset];
		Buffer.BlockCopy (data, startOffset, new_data, 0, new_data.Length);
		return new_data;
	}


	public byte[] PrepareToSend(int messageType, byte[] data){
		BitBuffer bitBuffer = new BitBuffer ();
		bitBuffer.WriteByte ((byte)messageType);
		bitBuffer.WriteBytes (data);
		return bitBuffer.Read ();
	}

	public void Send(byte[] data, IPEndPoint ip, bool reliable){
		if (reliable) {
			byte nextSeqNumber = GetNextReliableSeqNumber ();
			data = PrepareToSend (nextSeqNumber, data);
			if (!_reliableMessages.ContainsKey (ip)) {
				_reliableMessages [ip] = new Dictionary<byte, byte[]> ();
			}
			_reliableMessages [ip] [nextSeqNumber] = data;
		} else {
			data = PrepareToSend (NON_RELIABLE, data);
		}
		base.Send(data, ip);
	}

	public void Send(Byteable data, IPEndPoint ip, bool reliable){
		Send(data.toBytes(), ip, reliable);
	}

	public void SendAll(byte[] data, bool reliable){
		foreach(IPEndPoint ip in _connections)
			Send(data, ip, reliable);
	}

	public void SendAll(Byteable data, bool reliable){
		SendAll (data.toBytes(), reliable);
	}

	public void SendAllExcluding(byte[] data, IPEndPoint ip, bool reliable){
		foreach (IPEndPoint connectedIp in _connections) {
			if (!connectedIp.Equals(ip)) {
				Send(data, connectedIp, reliable);
			}
		}
	}

	public void SendAllExcluding(Byteable data, IPEndPoint ip, bool reliable){
		SendAllExcluding (data.toBytes(), ip, reliable);
	}
}
