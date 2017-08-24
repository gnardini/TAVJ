using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System;

public class ReadPositions : MonoBehaviour {

    private readonly object lockObj = new object();

    public Player otherPlayer;

    private Vector3? otherPosition;

	void Start () {
        Thread th = new Thread(new ThreadStart(ReadThread));
        th.IsBackground = true;
        th.Start();
	}
	
	void Update () {
        lock(lockObj) {
            if (otherPosition != null) {
                otherPlayer.transform.position = (Vector3) otherPosition;
                otherPosition = null;
            }
        }
	}

    void ReadThread() {
        Channel channel = new Channel(4000);
        while (true) {
            byte[] data = channel.Read();
            float x = System.BitConverter.ToSingle(data, 0);
            float y = System.BitConverter.ToSingle(data, 4);
            float z = System.BitConverter.ToSingle(data, 8);
            lock(lockObj) {
                otherPosition = new Vector3(x, y, z);
            }
        }
    }
}
