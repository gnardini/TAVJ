using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : Byteable {

//    private int id;
    private Vector3 _targetPosition;
    private InputType _inputType;

    public PlayerInput(InputType type, Vector3 targetPosition) {
        _targetPosition = targetPosition;
        _inputType = type;
    }

    public Vector3 GetTargetPosition() {
        return _targetPosition;
    }

    public static PlayerInput fromBytes(byte[] bytes) {
        float x = System.BitConverter.ToSingle(bytes, 1);
        float y = System.BitConverter.ToSingle(bytes, 5);
        float z = System.BitConverter.ToSingle(bytes, 9);
        return new PlayerInput((InputType) bytes[0], new Vector3(x, y, z));
    }

    public byte[] toBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteByte((byte) _inputType);
        bitBuffer.WriteFloat(_targetPosition.x);
        bitBuffer.WriteFloat(_targetPosition.y);
        bitBuffer.WriteFloat(_targetPosition.z);
        return bitBuffer.Read();
    }
}
