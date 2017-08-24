using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInfo : Byteable {

    private Vector3 _position;

    public PositionInfo(Vector3 position) {
        _position = position;
    }

    public Vector3 GetPosition() {
        return _position;
    }

    public static PositionInfo fromBytes(byte[] bytes) {
        float x = System.BitConverter.ToSingle(bytes, 0);
        float y = System.BitConverter.ToSingle(bytes, 4);
        float z = System.BitConverter.ToSingle(bytes, 8);
        return new PositionInfo(new Vector3(x, y, z));
    }

    public byte[] toBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteFloat(_position.x);
        bitBuffer.WriteFloat(_position.y);
        bitBuffer.WriteFloat(_position.z);
        return bitBuffer.Read();
    }

}
