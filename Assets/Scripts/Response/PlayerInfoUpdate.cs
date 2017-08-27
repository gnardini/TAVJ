using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUpdate : ServerResponse {

	private PlayerInfo _playerInfo;

	public PlayerInfoUpdate(PlayerInfo playerInfo) {
		_playerInfo = playerInfo;
    }

//    public PlayerInfoUpdate(int id, int health, PositionInfo positionInfo) {
//        _id = id;
//        _positionInfo = positionInfo;
//    }

	public static PlayerInfoUpdate FromBytes(BitBuffer bitBuffer) {
		return new PlayerInfoUpdate(PlayerInfo.FromBytes(bitBuffer));
    }

    public PlayerInfo GetPlayerInfo() {
		return _playerInfo;
    }

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
		_playerInfo.PutBytes(bitBuffer);
    }

    public override ResponseType GetResponseType() {
        return ResponseType.PLAYER_UPDATE;
    }
        
}
