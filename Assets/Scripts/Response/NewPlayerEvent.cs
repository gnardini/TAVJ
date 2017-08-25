using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerEvent : ServerResponse {

    private int _id;
    private bool _isOwner;
    private PositionInfo _positionInfo;

    public NewPlayerEvent(int id, bool isOwner, Vector3 position) : this(id, isOwner, new PositionInfo(position)) {
    }

    public NewPlayerEvent(int id, bool isOwner, PositionInfo positionInfo) {
        _id = id;
        _isOwner = isOwner;
        _positionInfo = positionInfo;
    }

    public static NewPlayerEvent FromBytes(byte[] data, int offset) {
        return new NewPlayerEvent((int) data[0], data[1] == 1, PositionInfo.fromBytes(data, offset+2));
    }

    public Vector3 GetPosition() {
        return _positionInfo.GetPosition();
    }

    public int GetId() {
        return _id;
    }

    public bool IsOwner() {
        return _isOwner;
    }

    protected override byte[] ExtraBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteByte((byte) _id);
        bitBuffer.WriteByte((byte) (_isOwner ? 1 : 0));
        bitBuffer.WriteBytes(_positionInfo.toBytes());
        return bitBuffer.Read();
    }

    public override ResponseType GetResponseType() {
        return ResponseType.NEW_PLAYER;
    }

}
