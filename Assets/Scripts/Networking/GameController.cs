using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

    public Transform playerPrefab;
    public Transform autoAttackPrefab;
    public bool isServer;

	private const string SERVER_HOST = "181.26.156.227"; // "192.168.0.18";

    private const int PORT = 5500;
	private ReliableChannel _channel;
	private Dictionary<int, Player> _players;
    private Dictionary<int, AutoAttack> _autoAttacks;
    private int _lastId = 0;
    private int _localId;

    void Start () {
        if (isServer) {
            _channel = new ReliableChannel(PORT);
            _players = new Dictionary<int, Player>();
        } else {
			_channel = new ReliableChannel(PORT+1);
            _channel.AddConnection(SERVER_HOST, PORT);
            _players = new Dictionary<int, Player>();
            SendBroadcast(new GameStartInput(), true);
        }
        _autoAttacks = new Dictionary<int, AutoAttack>();
    }

    void Update () {
        if (isServer) {
            Packet packet = _channel.GetPacket();
			BitBuffer bitBuffer = new BitBuffer ();
            while (packet != null) {
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
            foreach(KeyValuePair<int, Player> playerInfoPair in _players) {
				Vector3 position = playerInfoPair.Value.transform.position;
				PlayerInfo playerInfo = new PlayerInfo(
					playerInfoPair.Key, 
					playerInfoPair.Value.GetHealth(), 
					new PositionInfo(position));
				_channel.SendAll(new PlayerInfoUpdate(playerInfo), false);
//				new MovementResponse (playerInfo.Key, playerInfo.Value.transform.position).PutBytes (bitBuffer);
            }
//			bitBuffer.Flip ();
//			_channel.SendAll(bitBuffer.GetByteArray(), false);

//			bitBuffer.Clear ();
			RemoveDeadAutoAttacks();
            foreach(KeyValuePair<int, AutoAttack> autoInfo in _autoAttacks) {
				// TODO FIX
				Vector3 position = autoInfo.Value.transform.position;
				PlayerInfo playerInfo = new PlayerInfo(autoInfo.Key, 0, new PositionInfo(position));
				_channel.SendAll(new PlayerInfoUpdate(playerInfo), false);
//				new MovementResponse(autoInfo.Key, autoInfo.Value.transform.position).PutBytes(bitBuffer);
            }
//			bitBuffer.Flip ();
//			_channel.SendAll(bitBuffer.GetByteArray(), false);

        } else {
            Packet packet = _channel.GetPacket();
			BitBuffer bitBuffer = new BitBuffer ();
            while (packet != null) {
				bitBuffer.PutBytes(packet.getData());
				bitBuffer.Flip ();
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
				case ResponseType.AUTOATTACK: {
						AutoAttackResponse auto = (AutoAttackResponse)response;
						_players [auto.GetId ()].SpawnAutoAttack (auto.GetPosition ());
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
                }
                packet = _channel.GetPacket();
				bitBuffer.Clear ();
            }
        }
    }

	public void SendBroadcast(Byteable byteable, bool reliable = false) {
		_channel.SendAll(byteable, reliable);
    }

    private Player CreateServerPlayer(PlayerInfo playerInfo){
		Player player = InstantiatePlayer(playerInfo);
		player.SetUpdateLoop(new ServerPlayerUpdateLoop(player));
		return player;
    }

    private Player CreatePlayer(PlayerInfo playerInfo){
		Player player = InstantiatePlayer(playerInfo);
		if (playerInfo.GetId () == _localId) {
			player.SetUpdateLoop(new LocalPlayerUpdateLoop(player, this, _localId));
		} else {
			player.SetUpdateLoop(new MovementUpdateLoop(player));
		}
		return player;
    }

	private Player InstantiatePlayer(PlayerInfo playerInfo) {
		Player player = Instantiate(playerPrefab, playerInfo.GetPosition(), Quaternion.identity).gameObject.GetComponent<Player>();
		player.SetId(playerInfo.GetId());
		player.SetHealth(playerInfo.GetHealth());
		return player;
	}

	void RemoveDeadAutoAttacks() {
		Dictionary<int, AutoAttack> autoAttacks = new Dictionary<int, AutoAttack>();
		foreach(KeyValuePair<int, AutoAttack> autoInfo in _autoAttacks) {
			if (autoInfo.Value != null) {
				autoAttacks.Add(autoInfo.Key, autoInfo.Value);
			}
		}
		_autoAttacks = autoAttacks;
	}

	void OnDestroy() {
		_channel.Destroy();   
	}

}

