using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public bool isServer;
    public List<ServerPlayer> players;
    public Player player;

    private const string SERVER_HOST = "";
    private const int PORT = 5000;
    private Channel _channel;

	void Start () {
        if (isServer) {
            _channel = new Channel(PORT);   
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
                players[0].MoveTo(input.GetTargetPosition());
                bytes = _channel.getPacket();
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
