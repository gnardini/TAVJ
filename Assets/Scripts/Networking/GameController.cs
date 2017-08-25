using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public Transform clientPlayerPrefab;
    public Transform serverPlayerPrefab;
    public bool isServer;

    private const string SERVER_HOST = "127.0.0.1";
    private const int PORT = 5000;
    private Channel _channel;
    private Dictionary<int, ServerPlayer> _players;
    private Dictionary<int, Player> _clientPlayers;
    private int _lastId = 0;
    private Player _player;
    private int _localId;

	void Start () {
        if (isServer) {
            _channel = new Channel(PORT);
            _players = new Dictionary<int, ServerPlayer>();
        } else {
			_channel = new Channel(PORT+1);
            _channel.AddConnection(SERVER_HOST, PORT);
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
                case InputType.START_GAME:
                    _lastId++;
                    Vector3 startPosition = new Vector3(2f, 1.2f, 0f);
                    _players.Add(_lastId, Instantiate(serverPlayerPrefab, startPosition, Quaternion.identity).gameObject.GetComponent<ServerPlayer>());
                    _channel.Send(new NewPlayerEvent(_lastId, true, startPosition), packet.getAddress());
                    _channel.SendAllExcluding(new NewPlayerEvent(_lastId, false, startPosition), packet.getAddress());
                    break;
                }
                packet = _channel.getPacket();
            }
            foreach(ServerPlayer serverPlayer in _players.Values) {
                PositionInfo info = new PositionInfo(serverPlayer.transform.position);
                _channel.SendAll(info);
            }
        } else {
            Packet packet = _channel.getPacket();
            while (packet != null) {
                byte[] bytes = _channel.getPacket().getData();
                ServerResponse response = ServerResponse.fromBytes(bytes);
                switch (response.GetResponseType()) {
                case ResponseType.POSITIONS:
                    MovementResponse movementResponse = (MovementResponse) response;
                    _clientPlayers[movementResponse.GetId()].MakeMovement(movementResponse.GetPosition());
                    break;
                case ResponseType.NEW_PLAYER:
                    NewPlayerEvent newPlayerEvent = ((NewPlayerEvent)response);
                    Player player = Instantiate(clientPlayerPrefab, newPlayerEvent.GetPosition(), Quaternion.identity).gameObject.GetComponent<Player>();
                    if (newPlayerEvent.IsOwner()) {
                        _localId = newPlayerEvent.GetId();
                        player.SetId(_localId);
                        player.SetMoveLocally(true);
                        player.GetComponentInChildren<Camera>().enabled = true;
                    }
                    _clientPlayers.Add(_lastId, player);
                    break;
                }

                packet = _channel.getPacket();
            }
        }
	}

    public void SendByteable(Byteable byteable) {
        _channel.SendAll(byteable);
    }
}
