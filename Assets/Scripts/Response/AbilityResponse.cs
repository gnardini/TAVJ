using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityResponse : ServerResponse {

    private int _id;
	private AbilityType _type;
    private PositionInfo _targetPosition;

	public AbilityResponse(AbilityInput ability) {
		_id = ability.GetId();
		_type = ability.GetAbilityType();
		_targetPosition = new PositionInfo(ability.GetTargetPosition());
    }

    public AbilityResponse(int id, AbilityType type, PositionInfo positionInfo) {
        _id = id;
		_type = type;
        _targetPosition = positionInfo;
    }

	public static AbilityResponse FromBytes(BitBuffer bitBuffer) {
		int id = bitBuffer.GetByte ();
		AbilityType type = (AbilityType) bitBuffer.GetByte();
		return new AbilityResponse(id, type, PositionInfo.fromBytes(bitBuffer));
    }

    public int GetId() {
        return _id;
    }

	public AbilityType GetAbilityType() {
		return _type;
	}

    public Vector3 GetPosition() {
        return _targetPosition.GetPosition();
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
        bitBuffer.PutByte((byte)_id);
		bitBuffer.PutByte((byte)_type);
		_targetPosition.PutBytes(bitBuffer);
    }

    public override ResponseType GetResponseType() {
        return ResponseType.ABILITY;
    }

}
