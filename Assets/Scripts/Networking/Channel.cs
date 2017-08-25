using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;


public class Channel{
	
	private UdpClient socket;
	private Queue<Packet> packets;
	private HashSet<IPEndPoint> connections;
    private Thread _thread;
	
	public Channel(int port){
		socket = new UdpClient (port);
		packets = new Queue<Packet> ();
		connections = new HashSet<IPEndPoint> ();
        _thread = new Thread (new ThreadStart(ListeningIncomingMessages));
        _thread.Start ();
	}

    public void AddConnection(string host, int port) {
        connections.Add(new IPEndPoint(IPAddress.Parse(host), port));
    }

	private void ListeningIncomingMessages() {
        while (true) {
			IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
			byte[] b = socket.Receive (ref ip);
			connections.Add (ip);
			lock (packets) {
				packets.Enqueue (new Packet(b, ip));
			}
		}
	}

	public Packet getPacket(){
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
        Send(data.toBytes(), ip);
	}

	public void SendAll(byte[] data){
		foreach(IPEndPoint ip in connections)
			socket.Send(data, data.Length, ip);
	}

	public void SendAll(Byteable data){
		SendAll (data.toBytes());
	}

    public void SendAllExcluding(byte[] data, IPEndPoint ip){
        foreach (IPEndPoint connectedIp in connections) {
            if (!connectedIp.Equals(ip)) {
                socket.Send(data, data.Length, ip);
            }
        }
    }

    public void SendAllExcluding(Byteable data, IPEndPoint ip){
        SendAllExcluding (data.toBytes(), ip);
    }

    public void Destroy() {
        _thread.Abort();
        socket.Close();
    }
}
