using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInput : PlayerInput {
    
	private AbilityType _type;
    private PositionInfo _target;

    public AbilityInput(int id, AbilityType type, Vector3 target) : base(id) {
        _target = new PositionInfo(target);
		_type = type;
    }

	public AbilityInput(int id, AbilityType type, PositionInfo positionInfo) : base(id) {
        _target = positionInfo;
		_type = type;
    }

	public static AbilityInput FromBytes(int id, BitBuffer bitBuffer) {
		return new AbilityInput(id, (AbilityType)bitBuffer.GetByte(), PositionInfo.fromBytes(bitBuffer));
    }

    public Vector3 GetTargetPosition() {
        return _target.GetPosition();
    }

	public AbilityType GetAbilityType() {
		return _type;
	}

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
		bitBuffer.PutByte((byte) _type);
		_target.PutBytes(bitBuffer);
    }

    public override InputType GetInputType() {
		return InputType.ABILITY;
    }
}
