using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public Transform clientPlayerPrefab;
    public Transform serverPlayerPrefab;
    public bool isServer;

	private const string SERVER_HOST = "181.26.156.227"; //"127.0.0.1";
    private const int PORT = 5500;
    private Channel _channel;
    private Dictionary<int, ServerPlayer> _players;
    private Dictionary<int, Player> _clientPlayers;
    private int _lastId = 0;
    private int _localId;

	void Start () {
        if (isServer) {
            _channel = new Channel(PORT);
            _players = new Dictionary<int, ServerPlayer>();
        } else {
			_channel = new Channel(PORT+1);
            _channel.AddConnection(SERVER_HOST, PORT);
            _clientPlayers = new Dictionary<int, Player>();
            SendByteable(new GameStartInput());
        }
	}
	
	void Update () {
        if (isServer) {
            Packet packet = _channel.getPacket();
            while (packet != null) {
                byte[] bytes = packet.getData ();
                PlayerInput input = PlayerInput.fromBytes(bytes);
                switch (input.GetInputType()) {
                case InputType.MOVEMENT:
                    _players[input.GetId()].MoveTo(((MovementInput)input).GetPosition());   
                    break;
                case InputType.AUTOATTACK:
                    AutoAttackInput auto = (AutoAttackInput)input;
                    _players[auto.GetId()].SpawnAutoAttack(auto.GetTargetPosition());
                    _channel.SendAll(new AutoAttackResponse(auto.GetId(), auto.GetTargetPosition()));
                    break;
				case InputType.START_GAME:
					_lastId++;
					Vector3 startPosition = new Vector3 (2f, 1.2f, 0f);
					ServerPlayer serverPlayer = createServerPlayer(new PlayerInfo(_lastId, startPosition));
						_players.Add (_lastId, serverPlayer);
					_channel.Send (new PlayerInfoBroadcast (_lastId, _players), packet.getAddress());
					//_channel.SendAll (new PlayerInfoBroadcast (_lastId, _players));
                    _channel.SendAllExcluding(new NewPlayerEvent(_lastId, startPosition), packet.getAddress());
                    break;
                }
                packet = _channel.getPacket();
            }
            foreach(KeyValuePair<int, ServerPlayer> playerInfo in _players) {
                _channel.SendAll(new MovementResponse(playerInfo.Key, playerInfo.Value.transform.position));
            }
        } else {
            Packet packet = _channel.getPacket();
            while (packet != null) {
                byte[] bytes = packet.getData();
                ServerResponse response = ServerResponse.fromBytes(bytes);
                switch (response.GetResponseType()) {
				case ResponseType.POSITIONS: {
						MovementResponse movementResponse = (MovementResponse)response;
						if(_clientPlayers.ContainsKey(movementResponse.GetId ())){
							_clientPlayers [movementResponse.GetId ()].MakeMovement (movementResponse.GetPosition ());
						}
						break;
					}
				case ResponseType.AUTOATTACK: {
						AutoAttackResponse auto = (AutoAttackResponse)response;
						_clientPlayers [auto.GetId ()].SpawnAutoAttack (auto.GetPosition ());
						break;
					}
				case ResponseType.NEW_PLAYER: {
						NewPlayerEvent newPlayerEvent = (NewPlayerEvent)response;
						PlayerInfo playerInfo = newPlayerEvent.GetPlayerInfo ();
						Player player = createPlayer (playerInfo);
						_clientPlayers.Add (playerInfo.GetId (), player);
						break;
					}
				case ResponseType.PLAYER_INFO_BROADCAST: { 
						PlayerInfoBroadcast playerInfoBroadcast = (PlayerInfoBroadcast)response;
						_localId = playerInfoBroadcast.GetId ();
						foreach (PlayerInfo playerInfo in playerInfoBroadcast.getPlayersInfo()) {
							Player player = createPlayer (playerInfo);
							if (playerInfo.GetId () == _localId) {
								player.SetId (_localId);
								player.SetMoveLocally (true);
								player.SetGameController (this);
								player.gameObject.transform.GetChild (0).gameObject.SetActive (true);
							} 
							_clientPlayers.Add (playerInfo.GetId(), player);
						}
						break;
					}
                }

                packet = _channel.getPacket();
            }
        }
	}

    public void SendByteable(Byteable byteable) {
        _channel.SendAll(byteable);
    }

    void OnDestroy() {
        _channel.Destroy();   
    }

	private ServerPlayer createServerPlayer(PlayerInfo playerInfo){
		return Instantiate(serverPlayerPrefab, playerInfo.GetPosition(), Quaternion.identity).gameObject.GetComponent<ServerPlayer>();
	}

	private Player createPlayer(PlayerInfo playerInfo){
		return Instantiate(clientPlayerPrefab, playerInfo.GetPosition(), Quaternion.identity).gameObject.GetComponent<Player>();
	}
}

