using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;


public class Channel{
	
	private UdpClient socket;
	private Queue<byte[]> packets;
	private HashSet<IPEndPoint> connections;

	public Channel(){
		socket = new UdpClient ();
		packets = new Queue<byte[]> ();
		connections = new HashSet<IPEndPoint> ();
		Thread t = new Thread (ListeningIncomingMessages);
		t.Start ();
	}
	
	public Channel(int port){
		socket = new UdpClient (port);
		packets = new Queue<byte[]> ();
		connections = new HashSet<IPEndPoint> ();
		Thread t = new Thread (ListeningIncomingMessages);
		t.Start ();
	}

	private void ListeningIncomingMessages(){
		while (true) {
			IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
			byte[] b = socket.Receive (ref ip);
			connections.Add (ip);
			lock (packets) {
				packets.Enqueue (b);
			}
		}
	}

	public byte[] getPacket(){
		lock (packets) {
			if (packets.Count == 0) {
				return null;
			} else {
				return packets.Dequeue ();
			}
		}
	}

	public void Send(byte[] data, IPEndPoint ip){
		socket.Send(data, data.Length, ip);
	}

	public void Send(Byteable data, IPEndPoint ip){
		socket.Send(data.toBytes(), ip);
	}

	public void SendAll(byte[] data){
		foreach(IPEndPoint ip in connections)
			socket.Send(data, data.Length, ip);
	}

	public void SendAll(Byteable data){
		SendAll (data.toBytes ());
	}
}
