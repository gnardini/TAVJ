using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServerResponse : Byteable {

	public static ServerResponse fromBytes(BitBuffer bitBuffer) {
		ResponseType type = (ResponseType)bitBuffer.GetByte ();
		switch (type) {
        case ResponseType.PLAYER_UPDATE:
			return PlayerInfoUpdate.FromBytes(bitBuffer);
        case ResponseType.ABILITY:
			return AbilityResponse.FromBytes(bitBuffer);
        case ResponseType.NEW_PLAYER:
			return NewPlayerEvent.FromBytes(bitBuffer);
		case ResponseType.PLAYER_INFO_BROADCAST:
			return PlayerInfoBroadcast.FromBytes (bitBuffer);
		case ResponseType.RESPONSES_CONTAINER:
			return ResponsesContainer.FromBytes(bitBuffer);
        }

        return null;
    }

	public void PutBytes(BitBuffer bitBuffer) {
        bitBuffer.PutByte((byte) GetResponseType());
		PutExtraBytes(bitBuffer);
    }

	protected abstract void PutExtraBytes(BitBuffer bitBuffer);

    public abstract ResponseType GetResponseType();
}
