using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class ReliableChannel : Channel {

	private const int ACK = 254;
	private const int NON_RELIABLE = 255;

	private const int MS_TO_DELETE_RELIABLE_MESSAGES = 10000;

	private Dictionary<IPEndPoint, byte> _reliableSeqNumber;
	private Channel _channel;
	private Dictionary<IPEndPoint , Dictionary<byte, byte[]>> _reliableMessages; 
	private Dictionary<IPEndPoint , long[]> _messagesReceived;

	public ReliableChannel(int port) : base(port){
		_reliableMessages = new Dictionary<IPEndPoint , Dictionary<byte, byte[]>> ();
		_reliableSeqNumber = new Dictionary<IPEndPoint, byte> ();
		_messagesReceived = new Dictionary<IPEndPoint , long[]> ();
	}


	public void ResendReliableMessages(){
		lock (_reliableMessages) {
			foreach (KeyValuePair<IPEndPoint , Dictionary<byte, byte[]>> pair in _reliableMessages) {
				foreach (KeyValuePair<byte, byte[]> innerPair in pair.Value) {
					Send (innerPair.Value, pair.Key);
				}
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
			case ACK:{;
					lock (_reliableMessages) {
						_reliableMessages [packet.getAddress ()].Remove (data [1]);
					}
					break;
				}
			case NON_RELIABLE:{
					return new Packet (CreateCopy (data, 1), packet.getAddress ());
				}
			default:{
					Send(PrepareToSend(ACK, new byte[]{data[0]}), packet.getAddress());
					if (!_messagesReceived.ContainsKey (packet.getAddress ())) {
						_messagesReceived.Add (packet.getAddress (), new long[254]);
					}
					long currentTime = CurrentTimeMillis();
					if (currentTime - MS_TO_DELETE_RELIABLE_MESSAGES > _messagesReceived [packet.getAddress ()] [data [0]]) {
						_messagesReceived [packet.getAddress ()] [data [0]] = currentTime;
						return new Packet (CreateCopy (data, 1), packet.getAddress ());	
					}
					break;
				}
			}
			packet = base.GetPacket ();
		}
		return null;
	}

	public byte GetNextReliableSeqNumber(IPEndPoint ip){
		if (!_reliableSeqNumber.ContainsKey (ip)) {
			_reliableSeqNumber.Add (ip, 0);
		}
		int aux = _reliableSeqNumber[ip];
		_reliableSeqNumber[ip]=(byte)((aux+1)%254);
		return (byte)aux;
	}

	public byte[] CreateCopy(byte[] data, int startOffset){
		byte[] new_data = new byte[data.Length - startOffset];
		Buffer.BlockCopy (data, startOffset, new_data, 0, new_data.Length);
		return new_data;
	}


	private byte[] PrepareToSend(int messageType, byte[] data){
		CustomBitBuffer bitBuffer = new CustomBitBuffer ();
		bitBuffer.WriteByte ((byte)messageType);
		bitBuffer.WriteBytes (data);
		return bitBuffer.Read ();
	}

	private byte[] PrepareToSend(byte[] data, IPEndPoint ip, bool reliable){
		if (reliable) {
			byte nextSeqNumber = GetNextReliableSeqNumber (ip);
			data = PrepareToSend (nextSeqNumber, data);
			lock (_reliableMessages) {
				if (!_reliableMessages.ContainsKey (ip)) {
					_reliableMessages.Add (ip, new Dictionary<byte, byte[]> ());
				}
				_reliableMessages [ip].Add (nextSeqNumber, data);
			}
		} else {
			data = PrepareToSend (NON_RELIABLE, data);
		}
		return data;
	}

	public void Send(byte[] data, IPEndPoint ip, bool reliable){
		base.Send(PrepareToSend (data, ip, reliable), ip);
	}

	public void Send(Byteable data, IPEndPoint ip, bool reliable){
		BitBuffer bitBuffer = new BitBuffer ();
		data.PutBytes (bitBuffer);
		bitBuffer.Flip ();
		Send(bitBuffer.GetByteArray(), ip, reliable);
	}

	public void SendAll(byte[] data, bool reliable){
		foreach(IPEndPoint ip in _connections)
			Send(data, ip, reliable);
	}

	public void SendAll(Byteable data, bool reliable){
		BitBuffer bitBuffer = new BitBuffer ();
		data.PutBytes (bitBuffer);
		bitBuffer.Flip ();
		SendAll (bitBuffer.GetByteArray(), reliable);
	}

	public void SendAllExcluding(byte[] data, IPEndPoint ip, bool reliable){
		foreach (IPEndPoint connectedIp in _connections) {
			if (!connectedIp.Equals(ip)) {
				Send(data, connectedIp, reliable);
			}
		}
	}

	public void SendAllExcluding(Byteable data, IPEndPoint ip, bool reliable){
		BitBuffer bitBuffer = new BitBuffer ();
		data.PutBytes (bitBuffer);
		bitBuffer.Flip ();
		SendAllExcluding (bitBuffer.GetByteArray(), ip, reliable);
	}

	private static readonly DateTime Jan1st1970 = new DateTime
		(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static long CurrentTimeMillis()
	{
		return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
	}
}
