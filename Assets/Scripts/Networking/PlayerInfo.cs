using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : Byteable {

	private int _id;
	private PositionInfo _positionInfo;

	public PlayerInfo(int id, Player player) : this(id, new PositionInfo(player.transform.position)){
	}

	public PlayerInfo(int id, PositionInfo positionInfo) {
		_id = id;
		_positionInfo = positionInfo;
	}

	public PlayerInfo(int id, Vector3 position) {
		_id = id;
		_positionInfo = new PositionInfo(position);
	}

	public void PutBytes(BitBuffer bitBuffer){
		bitBuffer.PutByte ((byte)_id);
		_positionInfo.PutBytes (bitBuffer);
	}

	public static PlayerInfo fromBytes(BitBuffer bitBuffer){
		int id = bitBuffer.GetByte();
		PositionInfo positionInfo = PositionInfo.fromBytes (bitBuffer);
		return new PlayerInfo (id, positionInfo);
	}

	public int GetId(){
		return _id;
	}

	public Vector3 GetPosition(){
		return _positionInfo.GetPosition ();
	}
}
