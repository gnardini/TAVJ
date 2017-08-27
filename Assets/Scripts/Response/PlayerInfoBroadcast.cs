using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoBroadcast : ServerResponse {

	private int _id;
	private List<PlayerInfo> _playersInfo;

	public PlayerInfoBroadcast(int id, Dictionary<int, Player> dict) {
		_id = id;
		_playersInfo = new List<PlayerInfo> ();
		foreach (KeyValuePair<int, Player> pair in dict) {
			_playersInfo.Add(new PlayerInfo(pair.Key, pair.Value));
		}
	}


	public PlayerInfoBroadcast(int id, List<PlayerInfo> list) {
		_id = id;
		_playersInfo = list;
	}

	public static PlayerInfoBroadcast FromBytes(BitBuffer bitBuffer) {
		int id = bitBuffer.GetByte();
		int count = bitBuffer.GetByte();
		List<PlayerInfo> list = new List<PlayerInfo> ();
		for (int i = 0; i < count; i++) {
			PlayerInfo playerInfo = PlayerInfo.fromBytes (bitBuffer);
			list.Add (playerInfo);
		}
		return new PlayerInfoBroadcast (id, list);
	}

	public int GetId() {
		return _id;
	}

	public List<PlayerInfo> getPlayersInfo() {
		return _playersInfo;
	}

	protected override void PutExtraBytes(BitBuffer bitBuffer) {
		bitBuffer.PutByte((byte) _id);
		bitBuffer.PutByte ((byte)_playersInfo.Count);
		foreach (PlayerInfo playerInfo in _playersInfo) {
			playerInfo.PutBytes(bitBuffer);
		}
	}

	public override ResponseType GetResponseType() {
		return ResponseType.PLAYER_INFO_BROADCAST;
	}

}
