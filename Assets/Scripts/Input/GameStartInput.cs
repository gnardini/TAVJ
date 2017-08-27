using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartInput : PlayerInput {

    public GameStartInput() : base(0) {
    }

	public static GameStartInput FromBytes(int id, BitBuffer bitBuffer) {
        return new GameStartInput();
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
    }

    public override InputType GetInputType() {
        return InputType.START_GAME;
    }
}