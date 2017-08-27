using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;


public class Channel{
	
	private UdpClient _socket;
	private Queue<Packet> _packets;
	protected HashSet<IPEndPoint> _connections;
    private Thread _thread;
	
	public Channel(int port){
		_socket = new UdpClient (port);
		_packets = new Queue<Packet> ();
		_connections = new HashSet<IPEndPoint> ();
        _thread = new Thread (new ThreadStart(ListeningIncomingMessages));
		_thread.IsBackground = true;
        _thread.Start ();
	}

    public void AddConnection(string host, int port) {
        _connections.Add(new IPEndPoint(IPAddress.Parse(host), port));
    }

	private void ListeningIncomingMessages() {
        while (true) {
			IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
			byte[] b = _socket.Receive (ref ip);
			_connections.Add (ip);
			lock (_packets) {
				_packets.Enqueue (new Packet(b, ip));
			}
		}
	}

	public virtual Packet GetPacket(){
		lock (_packets) {
			if (_packets.Count == 0) {
				return null;
			} else {
				return _packets.Dequeue ();
			}
		}
	}

	public void Send(byte[] data, IPEndPoint ip){
		_socket.Send(data, data.Length, ip);
	}

	public void Send(Byteable data, IPEndPoint ip){
        Send(data.toBytes(), ip);
	}
	/*
	public void SendAll(byte[] data){
		foreach(IPEndPoint ip in _connections)
			Send(data, ip);
	}

	public void SendAll(Byteable data){
		SendAll (data.toBytes());
	}

    public void SendAllExcluding(byte[] data, IPEndPoint ip){
        foreach (IPEndPoint connectedIp in _connections) {
            if (!connectedIp.Equals(ip)) {
				Send(data, connectedIp);
            }
        }
    }

    public void SendAllExcluding(Byteable data, IPEndPoint ip){
        SendAllExcluding (data.toBytes(), ip);
    }*/

    public void Destroy() {
        _thread.Abort();
        _socket.Close();
    }
}
