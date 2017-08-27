using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInfo : Byteable {

    private Vector3 _position;

    public PositionInfo(Vector3 position) {
        _position = position;
    }

    public Vector3 GetPosition() {
        return _position;
    }

	public static PositionInfo fromBytes(BitBuffer bitBuffer) {
		float x = bitBuffer.GetFloat ();
		float y = bitBuffer.GetFloat ();
		float z = bitBuffer.GetFloat ();
        return new PositionInfo(new Vector3(x, y, z));
    }

	public void PutBytes(BitBuffer bitBuffer) {
        bitBuffer.PutFloat(_position.x);
        bitBuffer.PutFloat(_position.y);
        bitBuffer.PutFloat(_position.z);
    }

}
