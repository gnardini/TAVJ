using System;
using System.Net;
using System.Net.Sockets;

public class Packet
{
	private byte[] _data;
	private IPEndPoint _ip;

	public Packet(byte[] data, IPEndPoint ip){
		_data = data;
		_ip = ip;
	}

	public byte[] getData(){
		return _data;
	}

	public IPEndPoint getAddress(){
		return _ip;
	}
}

