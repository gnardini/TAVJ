using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementResponse : ServerResponse {

    private int _id;
    private PositionInfo _positionInfo;

    public MovementResponse(int id, Vector3 position) {
        _id = id;
        _positionInfo = new PositionInfo(position);
    }

    public MovementResponse(int id, PositionInfo positionInfo) {
        _id = id;
        _positionInfo = positionInfo;
    }

    public static MovementResponse FromBytes(byte[] data, int offset) {
        return new MovementResponse(data[offset], PositionInfo.fromBytes(data, offset+1));
    }

    public int GetId() {
        return _id;
    }

    public Vector3 GetPosition() {
        return _positionInfo.GetPosition();
    }

    protected override byte[] ExtraBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteByte((byte)_id);
        bitBuffer.WriteBytes(_positionInfo.toBytes());
        return bitBuffer.Read();
    }

    public override ResponseType GetResponseType() {
        return ResponseType.POSITIONS;
    }
        
}
