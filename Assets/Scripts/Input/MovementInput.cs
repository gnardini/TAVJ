using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : PlayerInput {

    private PositionInfo _positionInfo;

    public MovementInput(int id, Vector3 position) : base(id) {
        _positionInfo = new PositionInfo(position);
    }

    public MovementInput(int id, PositionInfo positionInfo) : base(id) {
        _positionInfo = positionInfo;
    }

    public static MovementInput FromBytes(int id, byte[] data, int offset) {
        return new MovementInput(id, PositionInfo.fromBytes(data, offset));
    }

    public Vector3 GetPosition() {
        return _positionInfo.GetPosition();
    }

    protected override byte[] ExtraBytes() {
        return _positionInfo.toBytes();
    }

    public override InputType GetInputType() {
        return InputType.MOVEMENT;
    }
}
