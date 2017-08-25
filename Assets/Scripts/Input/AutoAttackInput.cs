using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackInput : PlayerInput {
    
    private PositionInfo _target;

    public AutoAttackInput(int id, Vector3 target) : base(id) {
        _target = new PositionInfo(target);
    }

    public AutoAttackInput(int id, PositionInfo positionInfo) : base(id) {
        _target = positionInfo;
    }

    public static AutoAttackInput FromBytes(int id, byte[] data, int offset) {
        return new AutoAttackInput(id, PositionInfo.fromBytes(data, offset));
    }

    public Vector3 GetTargetPosition() {
        return _target.GetPosition();
    }

    protected override byte[] ExtraBytes() {
        return _target.toBytes();
    }

    public override InputType GetInputType() {
        return InputType.AUTOATTACK;
    }
}
