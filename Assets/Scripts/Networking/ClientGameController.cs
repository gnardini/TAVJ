using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameController : GameController {
	
	override protected void Start () {
		base.Start();	
		_channel.AddConnection(SERVER_HOST, PORT);
		_players = new Dictionary<int, Player>();
		SendBroadcast(new GameStartInput(), true);
	}

	protected override void Update () {
		base.Update();
	}

	protected override void HandlePacket(Packet packet) {
		BitBuffer bitBuffer = new BitBuffer();
		bitBuffer.PutBytes(packet.getData());
		bitBuffer.Flip ();
		ProcessResponse(bitBuffer);
		bitBuffer.Clear ();
	}

	private void ProcessResponse(BitBuffer bitBuffer) {
		ServerResponse response = ServerResponse.fromBytes(bitBuffer);
		switch (response.GetResponseType()) {
		case ResponseType.PLAYER_UPDATE: {
				PlayerInfoUpdate playerUpdate = (PlayerInfoUpdate)response;
				PlayerInfo playerInfo = playerUpdate.GetPlayerInfo();
				if(_players.ContainsKey(playerInfo.GetId ())){
					_players [playerInfo.GetId ()].SetTargetPosition(playerInfo.GetPosition ());
					_players [playerInfo.GetId ()].SetHealth(playerInfo.GetHealth ());
				} else if (_autoAttacks.ContainsKey(playerInfo.GetId())) {
					_autoAttacks[playerInfo.GetId()].MoveTo(playerInfo.GetPosition());
				}
				break;
			}
		case ResponseType.ABILITY: {
				AbilityResponse auto = (AbilityResponse)response;
				switch(auto.GetAbilityType()) {
				case AbilityType.AUTOATTACK:
					_players [auto.GetId ()].SpawnAutoAttack(auto.GetPosition ());
					break;
				case AbilityType.FREEZE:
					_players[auto.GetId()].SpawnFreeze(auto.GetPosition());
					break;
				}
				break;
			}
		case ResponseType.NEW_PLAYER: {
				NewPlayerEvent newPlayerEvent = (NewPlayerEvent)response;
				PlayerInfo playerInfo = newPlayerEvent.GetPlayerInfo ();
				Player player = CreatePlayer (playerInfo);
				_players.Add (playerInfo.GetId (), player);
				break;
			}
		case ResponseType.PLAYER_INFO_BROADCAST: { 
				PlayerInfoBroadcast playerInfoBroadcast = (PlayerInfoBroadcast)response;
				_localId = playerInfoBroadcast.GetId ();
				foreach (PlayerInfo playerInfo in playerInfoBroadcast.getPlayersInfo()) {
					Player player = CreatePlayer (playerInfo);
					_players.Add (playerInfo.GetId(), player);
				}
				break;
			}
		case ResponseType.RESPONSES_CONTAINER: {
				byte[] responses = ((ResponsesContainer)response).GetData();
				BitBuffer containerBuffer = new BitBuffer();
				containerBuffer.PutBytes(responses);
				containerBuffer.Flip();
				int totalResponses = containerBuffer.GetByte();
				for (int i = 0; i < totalResponses; i++) {
					ProcessResponse(containerBuffer);
				}
				break;
			}
		}
	}

	protected override ReliableChannel CreateChannel() {
		return new ReliableChannel(PORT+1);
	}
}
