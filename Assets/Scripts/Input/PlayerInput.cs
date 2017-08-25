using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput : Byteable {

    private int _id;

    public PlayerInput(int id) {
        _id = id;
    }

    public static PlayerInput fromBytes(byte[] bytes) {
        int id = bytes[0];
        switch ((InputType) bytes[1]) {
        case InputType.MOVEMENT:
            return MovementInput.FromBytes(id, bytes, 2);
        case InputType.AUTOATTACK:
            return null;
        case InputType.START_GAME:
            return GameStartInput.FromBytes(id, bytes, 2);
        }
        return null;
    }

    public int GetId() {
        return _id;
    }

    public byte[] toBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteByte((byte) _id);
        bitBuffer.WriteByte((byte) GetInputType());
        bitBuffer.WriteBytes(ExtraBytes());
        return bitBuffer.Read();
    }

    protected abstract byte[] ExtraBytes();

    public abstract InputType GetInputType();
}
