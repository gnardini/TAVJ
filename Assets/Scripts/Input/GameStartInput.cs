using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartInput : PlayerInput {

    public GameStartInput() : base(0) {
    }

    public static GameStartInput FromBytes(int id, byte[] data, int offset) {
        return new GameStartInput();
    }

    protected override byte[] ExtraBytes() {
        return new byte[0];
    }

    public override InputType GetInputType() {
        return InputType.START_GAME;
    }
}