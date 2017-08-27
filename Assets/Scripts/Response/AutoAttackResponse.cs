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

	public static AutoAttackResponse FromBytes(BitBuffer bitBuffer) {
		int id = bitBuffer.GetByte ();
		return new AutoAttackResponse(id, PositionInfo.fromBytes(bitBuffer));
    }

    public int GetId() {
        return _id;
    }

    public Vector3 GetPosition() {
        return _targetPosition.GetPosition();
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
        bitBuffer.PutByte((byte)_id);
		_targetPosition.PutBytes(bitBuffer);
    }

    public override ResponseType GetResponseType() {
        return ResponseType.AUTOATTACK;
    }

}
