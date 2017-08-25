using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServerResponse : Byteable {

    public static ServerResponse fromBytes(byte[] bytes) {
        switch ((ResponseType) bytes[0]) {
        case ResponseType.POSITIONS:
            return MovementResponse.FromBytes(bytes, 1);
        case ResponseType.AUTOATTACK:
            return AutoAttackResponse.FromBytes(bytes, 1);
        case ResponseType.NEW_PLAYER:
            return NewPlayerEvent.FromBytes(bytes, 1);
        }
        return null;
    }

    public byte[] toBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteByte((byte) GetResponseType());
        bitBuffer.WriteBytes(ExtraBytes());
        return bitBuffer.Read();
    }

    protected abstract byte[] ExtraBytes();

    public abstract ResponseType GetResponseType();
}
