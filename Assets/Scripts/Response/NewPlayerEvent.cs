using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerEvent : ServerResponse {
	
    private PlayerInfo _playerInfo;

	public NewPlayerEvent(PlayerInfo playerInfo) {
		_playerInfo = playerInfo;
    }

	public NewPlayerEvent(int id, Vector3 position) {
		_playerInfo = new PlayerInfo (id, new PositionInfo(position));
	}

    public NewPlayerEvent(int id, PositionInfo positionInfo) {
		_playerInfo = new PlayerInfo (id, positionInfo);
    }

    public static NewPlayerEvent FromBytes(byte[] data, int offset) {
        return new NewPlayerEvent((int) data[offset], PositionInfo.fromBytes(data, offset+1));
    }

    public PlayerInfo GetPlayerInfo() {
        return _playerInfo;
    }

    protected override byte[] ExtraBytes() {
        BitBuffer bitBuffer = new BitBuffer();
		bitBuffer.WriteBytes (_playerInfo.toBytes ());
        return bitBuffer.Read();
    }

    public override ResponseType GetResponseType() {
        return ResponseType.NEW_PLAYER;
    }

}
