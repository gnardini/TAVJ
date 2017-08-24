using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public Transform playerPrefab;
    public bool isServer;
    public Player player;

    private const string SERVER_HOST = "";
    private const int PORT = 5000;
    private Channel _channel;
    private List<ServerPlayer> _players;

	void Start () {
        if (isServer) {
            _channel = new Channel(PORT);
            _players.Add(Instantiate(playerPrefab, new Vector3(0f, 1.2f, 0f), Quaternion.identity).gameObject.GetComponent<ServerPlayer>());
        } else {
            _channel = new Channel();
            _channel.AddConnection(SERVER_HOST, PORT);
        }
	}
	
	void Update () {
        if (isServer) {
            byte[] bytes = _channel.getPacket();
            while (bytes != null) {
                PlayerInput input = PlayerInput.fromBytes(bytes);
                _players[0].MoveTo(input.GetTargetPosition());
                bytes = _channel.getPacket();
            }
            foreach(ServerPlayer serverPlayer in _players) {
                PositionInfo info = new PositionInfo(serverPlayer.transform.position);
                _channel.SendAll(info);
            }
        } else {
            byte[] bytes = _channel.getPacket();
            PositionInfo info = PositionInfo.fromBytes(bytes);
            player.MakeMovement(info);
        }
	}

    public void SendByteable(Byteable byteable) {
        _channel.SendAll(byteable);
    }
}
