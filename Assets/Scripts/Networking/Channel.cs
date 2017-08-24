using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Channel {

    private UdpClient udpClient;
    private IPEndPoint ipEndpoint;

    public Channel() {
        udpClient = new UdpClient();
    }

    public Channel(int port) {
        udpClient = new UdpClient(port);
    }

    public void Connect(string host, int port) {
        ipEndpoint = new IPEndPoint(IPAddress.Parse(host), port);
    }

    public void Send(BitBuffer bitBuffer) {
        byte[] data = bitBuffer.Read();
        udpClient.Send(data, data.Length, ipEndpoint);
    }

    public byte[] Read() {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = udpClient.Receive(ref remoteIpEndPoint);
        Debug.Log("This message was sent from " +
            remoteIpEndPoint.Address.ToString() +
            " on their port number " +
            remoteIpEndPoint.Port.ToString());
        return data;
    }

}

