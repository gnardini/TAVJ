using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput : Byteable {

    private int _id;

    public PlayerInput(int id) {
        _id = id;
    }

	public static PlayerInput fromBytes(BitBuffer bitBuffer) {
		int id = bitBuffer.GetByte();
		InputType type = (InputType)bitBuffer.GetByte ();
		switch (type) {
        case InputType.MOVEMENT:
			return MovementInput.FromBytes(id, bitBuffer);
        case InputType.ABILITY:
			return AbilityInput.FromBytes(id, bitBuffer);
        case InputType.START_GAME:
			return GameStartInput.FromBytes(id, bitBuffer);
        }
        return null;
    }

    public int GetId() {
        return _id;
    }

	public void PutBytes(BitBuffer bitBuffer) {
        bitBuffer.PutByte((byte) _id);
        bitBuffer.PutByte((byte) GetInputType());
		PutExtraBytes(bitBuffer);
    }

	protected abstract void PutExtraBytes(BitBuffer bitBuffer);

    public abstract InputType GetInputType();
}
