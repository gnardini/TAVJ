using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityResponse : ServerResponse {

    private int _id;
	private AbilityType _type;
	// Start position is only used in the autoattack for now.
	private PositionInfo _startPosition;
	private PositionInfo _targetPosition;

	public AbilityResponse(AbilityInput ability, Vector3 startPosition) {
		_id = ability.GetId();
		_type = ability.GetAbilityType();
		_startPosition = new PositionInfo(startPosition);
		_targetPosition = new PositionInfo(ability.GetTargetPosition());
    }

    public AbilityResponse(int id, AbilityType type, PositionInfo startPosition, PositionInfo targetPosition) {
        _id = id;
		_type = type;
		_startPosition = startPosition;
		_targetPosition = targetPosition;
    }

	public static AbilityResponse FromBytes(BitBuffer bitBuffer) {
		int id = bitBuffer.GetByte ();
		AbilityType type = (AbilityType) bitBuffer.GetByte();
		PositionInfo startPosition = null;
		if (type == AbilityType.AUTOATTACK) {
			startPosition = PositionInfo.FromBytes(bitBuffer);
		}
		return new AbilityResponse(id, type, startPosition, PositionInfo.FromBytes(bitBuffer));
    }

    public int GetId() {
        return _id;
    }

	public AbilityType GetAbilityType() {
		return _type;
	}

	public Vector3 GetStartPosition() {
		return _startPosition.GetPosition();
	}

    public Vector3 GetPosition() {
        return _targetPosition.GetPosition();
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
        bitBuffer.PutByte((byte)_id);
		bitBuffer.PutByte((byte)_type);
		if (_type == AbilityType.AUTOATTACK) {
			_startPosition.PutBytes(bitBuffer);
		}
		_targetPosition.PutBytes(bitBuffer);
    }

    public override ResponseType GetResponseType() {
        return ResponseType.ABILITY;
    }

}
