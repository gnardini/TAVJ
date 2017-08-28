using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameController : GameController {
	
	override protected void Start () {
		base.Start();	
	}

	override protected void Update () {
		base.Update();
		BitBuffer bitBuffer = new BitBuffer();
		RemoveDeadAutoAttacks();
		int totalUpdates = _players.Count + _autoAttacks.Count;
		bitBuffer.PutByte((byte)totalUpdates);
		foreach(KeyValuePair<int, Player> playerInfoPair in _players) {
			Vector3 position = playerInfoPair.Value.transform.position;
			PlayerInfo playerInfo = new PlayerInfo(
				playerInfoPair.Key, 
				playerInfoPair.Value.GetHealth(), 
				new PositionInfo(position));
			new PlayerInfoUpdate(playerInfo).PutBytes(bitBuffer);
		}
		foreach(KeyValuePair<int, AutoAttack> autoInfo in _autoAttacks) {
			// TODO FIX
			Vector3 position = autoInfo.Value.transform.position;
			PlayerInfo playerInfo = new PlayerInfo(autoInfo.Key, 0, new PositionInfo(position));
			new PlayerInfoUpdate(playerInfo).PutBytes(bitBuffer);
		}
		bitBuffer.Flip ();
		_channel.SendAll(new ResponsesContainer(bitBuffer.GetByteArray()), false);
	}

	protected override void HandlePacket(Packet packet) {
		BitBuffer bitBuffer = new BitBuffer ();
		bitBuffer.PutBytes(packet.getData ());
		bitBuffer.Flip ();
		PlayerInput input = PlayerInput.fromBytes(bitBuffer);
		switch (input.GetInputType()) {
		case InputType.MOVEMENT:
			_players[input.GetId()].SetTargetPosition(((MovementInput)input).GetPosition());   
			break;
		case InputType.AUTOATTACK:
			AutoAttackInput auto = (AutoAttackInput)input;
			AutoAttack autoAttack = _players[auto.GetId()].SpawnAutoAttack(auto.GetTargetPosition());
			_lastId++;
			_autoAttacks.Add(_lastId, autoAttack);
			_channel.SendAll(new AutoAttackResponse(auto.GetId(), auto.GetTargetPosition()), false);
			break;
		case InputType.START_GAME:
			_lastId++;
			Vector3 startPosition = new Vector3(2f, 1.2f, 0f);
			Player player = CreateServerPlayer(new PlayerInfo(_lastId, startPosition));
			_players.Add(_lastId, player);
			_channel.Send(new PlayerInfoBroadcast(_lastId, _players), packet.getAddress(), true);
			PlayerInfo playerInfo = new PlayerInfo(_lastId, player.GetHealth(), new PositionInfo(startPosition));
			_channel.SendAllExcluding(new NewPlayerEvent(playerInfo), packet.getAddress(), true);
			break;
		}
		packet = _channel.GetPacket();
		bitBuffer.Clear ();
	}

	protected override ReliableChannel CreateChannel() {
		return new ReliableChannel(PORT);
	}
}
