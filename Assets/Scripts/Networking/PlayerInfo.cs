using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : Byteable {

	private int _id;
	private int _health;
	private PositionInfo _positionInfo;

	public PlayerInfo(int id, Player player) : this(id, player.GetHealth(), new PositionInfo(player.transform.position)){
	}

	public PlayerInfo(int id, int health, PositionInfo positionInfo) {
		_id = id;
		_health = health;
		_positionInfo = positionInfo;
	}

	public PlayerInfo(int id, Vector3 position) {
		_id = id;
		_positionInfo = new PositionInfo(position);
	}

	public void PutBytes(BitBuffer bitBuffer){
		bitBuffer.PutByte ((byte)_id);
		bitBuffer.PutByte((byte)_health);
		_positionInfo.PutBytes (bitBuffer);
	}

	public static PlayerInfo FromBytes(BitBuffer bitBuffer){
		int id = bitBuffer.GetByte();
		int health = bitBuffer.GetByte();
		PositionInfo positionInfo = PositionInfo.FromBytes (bitBuffer);
		return new PlayerInfo (id, health, positionInfo);
	}

	public int GetId(){
		return _id;
	}

	public int GetHealth() {
		return _health;
	}

	public Vector3 GetPosition(){
		return _positionInfo.GetPosition ();
	}
}
