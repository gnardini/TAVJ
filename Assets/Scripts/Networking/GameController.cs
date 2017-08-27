﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

    public Transform clientPlayerPrefab;
    public Transform serverPlayerPrefab;
    public Transform autoAttackPrefab;
    public bool isServer;

    private const string SERVER_HOST = "181.26.156.227";//"192.168.0.18"; // "181.26.156.227";
    private const int PORT = 5500;
	private ReliableChannel _channel;
    private Dictionary<int, ServerPlayer> _players;
    private Dictionary<int, Player> _clientPlayers;
    private Dictionary<int, AutoAttack> _autoAttacks;
    private int _lastId = 0;
    private int _localId;

    void Start () {
        if (isServer) {
            _channel = new ReliableChannel(PORT);
            _players = new Dictionary<int, ServerPlayer>();
        } else {
			_channel = new ReliableChannel(PORT+1);
            _channel.AddConnection(SERVER_HOST, PORT);
            _clientPlayers = new Dictionary<int, Player>();
            SendBroadcast(new GameStartInput());
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
                    _players[input.GetId()].MoveTo(((MovementInput)input).GetPosition());   
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
                    Vector3 startPosition = new Vector3 (2f, 1.2f, 0f);
                    ServerPlayer serverPlayer = createServerPlayer(new PlayerInfo(_lastId, startPosition));
                    _players.Add (_lastId, serverPlayer);
                    _channel.Send (new PlayerInfoBroadcast (_lastId, _players), packet.getAddress(), true);
                    _channel.SendAllExcluding(new NewPlayerEvent(_lastId, startPosition), packet.getAddress(), true);
                    break;
                }
                packet = _channel.GetPacket();
				bitBuffer.Clear ();
            }
            foreach(KeyValuePair<int, ServerPlayer> playerInfo in _players) {
				new MovementResponse (playerInfo.Key, playerInfo.Value.transform.position).PutBytes (bitBuffer);
            }
			bitBuffer.Flip ();
			_channel.SendAll(bitBuffer.GetByteArray(), false);

			bitBuffer.Clear ();
            foreach(KeyValuePair<int, AutoAttack> autoInfo in _autoAttacks) {
				new MovementResponse(autoInfo.Key, autoInfo.Value.transform.position).PutBytes(bitBuffer);
            }
			bitBuffer.Flip ();
			_channel.SendAll(bitBuffer.GetByteArray(), false);

        } else {
            Packet packet = _channel.GetPacket();
			BitBuffer bitBuffer = new BitBuffer ();
            while (packet != null) {
				byte[] a = packet.getData();
				Debug.Log ("response size " + a.Length);
				for(int i =0; i< a.Length; i++){
					Debug.Log("array elem["+i+"]: "+a[i]+"      "+System.DateTime.Now.Millisecond);
				}
				bitBuffer.PutBytes(packet.getData());
				bitBuffer.Flip ();
				ServerResponse response = ServerResponse.fromBytes(bitBuffer);
                switch (response.GetResponseType()) {
                case ResponseType.POSITIONS: {
                        MovementResponse movementResponse = (MovementResponse)response;
                        if(_clientPlayers.ContainsKey(movementResponse.GetId ())){
                            _clientPlayers [movementResponse.GetId ()].MakeMovement (movementResponse.GetPosition ());
                        } else if (_autoAttacks.ContainsKey(movementResponse.GetId())) {
                            _autoAttacks[movementResponse.GetId()].MoveTo(movementResponse.GetPosition());
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
                        if (!_clientPlayers.ContainsKey(playerInfo.GetId())) {
                            Player player = createPlayer (playerInfo);
                            _clientPlayers.Add (playerInfo.GetId (), player);   
                        }
                        break;
                    }
                case ResponseType.PLAYER_INFO_BROADCAST: { 
                        PlayerInfoBroadcast playerInfoBroadcast = (PlayerInfoBroadcast)response;
                        _localId = playerInfoBroadcast.GetId ();
                        foreach (PlayerInfo playerInfo in playerInfoBroadcast.getPlayersInfo()) {
							if (!_clientPlayers.ContainsKey (playerInfo.GetId ())) {
		                        Player player = createPlayer (playerInfo);
		                        if (playerInfo.GetId () == _localId) {
		                            player.SetId (_localId);
		                            player.SetMoveLocally (true);
		                            player.SetGameController (this);
		                            player.gameObject.transform.GetChild (0).gameObject.SetActive (true);
		                        } 
								_clientPlayers.Add (playerInfo.GetId (), player);
							}
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

