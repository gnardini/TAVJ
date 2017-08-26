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

	public byte[] toBytes(){
		BitBuffer bitBuffer = new BitBuffer ();
		bitBuffer.WriteByte ((byte)_id);
		bitBuffer.WriteBytes (_positionInfo.toBytes ());
		return bitBuffer.Read ();
	}

	public static PlayerInfo fromBytes(byte[] data, int offset){
		int id = data [offset];
		PositionInfo positionInfo = PositionInfo.fromBytes (data, offset + 1);
		return new PlayerInfo (id, positionInfo);
	}

	public int GetId(){
		return _id;
	}

	public Vector3 GetPosition(){
		return _positionInfo.GetPosition ();
	}
}
