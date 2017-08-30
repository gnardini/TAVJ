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
		int totalUpdates = _players.Count;
		bitBuffer.PutByte((byte)totalUpdates);
		foreach(KeyValuePair<int, Player> playerInfoPair in _players) {
			Vector3 position = playerInfoPair.Value.transform.position;
			PlayerInfo playerInfo = new PlayerInfo(
				playerInfoPair.Key, 
				playerInfoPair.Value.GetHealth(), 
				new PositionInfo(position));
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
		case InputType.ABILITY:
			AbilityInput ability = (AbilityInput)input;
			Vector3 abilityStartPosition = Vector3.zero;
			switch (ability.GetAbilityType()) {
			case AbilityType.AUTOATTACK:
				abilityStartPosition = _players[ability.GetId()].transform.position;
				AutoAttack autoAttack = SpawnAutoAttack(ability.GetId(), abilityStartPosition, ability.GetTargetPosition());
				_lastAbilityId = (_lastAbilityId + 1) % 255;
				_autoAttacks.Add(_lastAbilityId, autoAttack);
				break;
			case AbilityType.FREEZE:
				_players[ability.GetId()].SpawnFreeze(ability.GetTargetPosition());
				break;
			case AbilityType.FLASH:
				_players[ability.GetId()].MoveTo(ability.GetTargetPosition());
				break;
			case AbilityType.EXPLOSION:
				CreateExplosion(ability.GetTargetPosition());
				break;
			}
			_channel.SendAll(new AbilityResponse(ability, abilityStartPosition), true);
			break;
		case InputType.START_GAME:
			_lastPlayerId++;
			Vector3 startPosition = new Vector3(2f, 1.2f, 0f);
			Player player = CreateServerPlayer(new PlayerInfo(_lastPlayerId, startPosition));
			_players.Add(_lastPlayerId, player);
			_channel.Send(new PlayerInfoBroadcast(_lastPlayerId, _players), packet.getAddress(), true);
			PlayerInfo playerInfo = new PlayerInfo(_lastPlayerId, player.GetHealth(), new PositionInfo(startPosition));
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
