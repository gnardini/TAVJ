﻿using System;
using System.Collections.Generic;

public class BitBuffer {

    Queue<byte> queue;

    public BitBuffer() {
        queue = new Queue<byte>();
    }

    public void WriteByte(byte b) {
        queue.Enqueue(b);
    }

    public void WriteFloat(float data) {
        byte[] float_data = BitConverter.GetBytes(data);
        foreach (byte b in float_data) {
            queue.Enqueue(b);
        }
    }

    public byte[] Read() {
        byte[] data = new byte[queue.Count];
        int i = 0;
        while (queue.Count > 0) {
            data[i] = queue.Dequeue();
            i++;
        }
        return data;
    }

    public void Clear() {
        queue.Clear();
    }
}
