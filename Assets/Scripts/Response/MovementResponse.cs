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

	public static MovementResponse FromBytes(BitBuffer bitBuffer) {
		int id = bitBuffer.GetByte ();
		return new MovementResponse(id, PositionInfo.fromBytes(bitBuffer));
    }

    public int GetId() {
        return _id;
    }

    public Vector3 GetPosition() {
        return _positionInfo.GetPosition();
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
        bitBuffer.PutByte((byte)_id);
		_positionInfo.PutBytes(bitBuffer);
    }

    public override ResponseType GetResponseType() {
        return ResponseType.POSITIONS;
    }
        
}
