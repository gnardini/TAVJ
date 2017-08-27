using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerEvent : ServerResponse {
	
    private PlayerInfo _playerInfo;

	public NewPlayerEvent(PlayerInfo playerInfo) {
		_playerInfo = playerInfo;
    }

//	public NewPlayerEvent(int id, Vector3 position) {
//		_playerInfo = new PlayerInfo (id, new PositionInfo(position));
//	}
//
//    public NewPlayerEvent(int id, int health, PositionInfo positionInfo) {
//		_playerInfo = new PlayerInfo (id, positionInfo);
//    }

	public static NewPlayerEvent FromBytes(BitBuffer bitBuffer) {
		return new NewPlayerEvent(PlayerInfo.FromBytes(bitBuffer));
    }

    public PlayerInfo GetPlayerInfo() {
        return _playerInfo;
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
		_playerInfo.PutBytes (bitBuffer);
    }

    public override ResponseType GetResponseType() {
        return ResponseType.NEW_PLAYER;
    }

}
