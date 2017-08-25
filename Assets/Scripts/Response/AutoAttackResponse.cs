using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackResponse : ServerResponse {

    private int _id;
    private PositionInfo _targetPosition;

    public AutoAttackResponse(int id, Vector3 position) {
        _id = id;
        _targetPosition = new PositionInfo(position);
    }

    public AutoAttackResponse(int id, PositionInfo positionInfo) {
        _id = id;
        _targetPosition = positionInfo;
    }

    public static AutoAttackResponse FromBytes(byte[] data, int offset) {
        return new AutoAttackResponse(data[offset], PositionInfo.fromBytes(data, offset+1));
    }

    public int GetId() {
        return _id;
    }

    public Vector3 GetPosition() {
        return _targetPosition.GetPosition();
    }

    protected override byte[] ExtraBytes() {
        BitBuffer bitBuffer = new BitBuffer();
        bitBuffer.WriteByte((byte)_id);
        bitBuffer.WriteBytes(_targetPosition.toBytes());
        return bitBuffer.Read();
    }

    public override ResponseType GetResponseType() {
        return ResponseType.AUTOATTACK;
    }

}
