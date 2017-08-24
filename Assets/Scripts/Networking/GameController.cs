using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public Transform playerPrefab;
    public bool isServer;
    public Player player;

    private const string SERVER_HOST = "10.17.48.182";//"10.23.22.92";
    private const int PORT = 5000;
    private Channel _channel;
    private List<ServerPlayer> _players;

	void Start () {
        if (isServer) {
            _channel = new Channel(PORT);
            _players = new List<ServerPlayer>();
            _players.Add(Instantiate(playerPrefab, new Vector3(2f, 1.2f, 0f), Quaternion.identity).gameObject.GetComponent<ServerPlayer>());
        } else {
			_channel = new Channel(PORT+1);
            _channel.AddConnection(SERVER_HOST, PORT);
        }
	}
	
	void Update () {
        if (isServer) {
			byte[] bytes = _channel.getPacket ().getData ();
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
			byte[] bytes = _channel.getPacket().getData();

            if (bytes != null) {
                PositionInfo info = PositionInfo.fromBytes(bytes);
                player.MakeMovement(info);   
            }
        }
	}

    public void SendByteable(Byteable byteable) {
        _channel.SendAll(byteable);
    }
}
