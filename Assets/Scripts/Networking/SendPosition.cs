using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPosition : MonoBehaviour {

    private const string HOST = "127.0.0.1";
    private const int PORT = 4001;

    public Player localPlayer;

    private Channel channel;
    private BitBuffer bitBuffer;

	void Start () {	
        channel = new Channel();
        channel.Connect(HOST, PORT);
        bitBuffer = new BitBuffer();
	}
	
	void Update () {
        bitBuffer.Clear();
        localPlayer.Write(bitBuffer);
        channel.Send(bitBuffer);
	}
}
