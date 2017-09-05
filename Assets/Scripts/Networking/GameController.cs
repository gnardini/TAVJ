using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameController : MonoBehaviour {

    public Transform playerPrefab;
	public Transform explosionPrefab;

	protected const string SERVER_HOST = "127.0.0.1";// "127.0.0.1";// "181.26.170.150";
    protected const int PORT = 5500;

	protected ReliableChannel _channel;
	protected Dictionary<int, Player> _players;
	protected Dictionary<int, AutoAttack> _autoAttacks;
	protected int _lastPlayerId = 0;
	protected int _lastAbilityId = 0;
	protected int _localId;

    virtual protected void Start () {
		_players = new Dictionary<int, Player>();
		_autoAttacks = new Dictionary<int, AutoAttack>();
		_channel = CreateChannel();     
    }

	virtual protected void Update () {
		RemoveDeadAutoAttacks();
		Packet packet = _channel.GetPacket();
		while (packet != null) {
			HandlePacket(packet);
			packet = _channel.GetPacket();
		}
    }

	protected abstract ReliableChannel CreateChannel();

	protected abstract void HandlePacket(Packet packet);

	public void SendBroadcast(Byteable byteable, bool reliable = false) {
		_channel.SendAll(byteable, reliable);
    }

    protected Player CreateServerPlayer(PlayerInfo playerInfo){
		Player player = InstantiatePlayer(playerInfo);
		player.SetUpdateLoop(new ServerPlayerUpdateLoop(player));
		return player;
    }

	protected Player CreatePlayer(PlayerInfo playerInfo){
		Player player = InstantiatePlayer(playerInfo);
		if (playerInfo.GetId () == _localId) {
			player.SetUpdateLoop(new LocalPlayerUpdateLoop(player, this, _localId));
		} else {
			player.SetUpdateLoop(new MovementUpdateLoop(player));
		}
		return player;
    }

	protected Player InstantiatePlayer(PlayerInfo playerInfo) {
		Player player = Instantiate(playerPrefab, playerInfo.GetPosition(), Quaternion.identity).gameObject.GetComponent<Player>();
		player.SetId(playerInfo.GetId());
		player.SetHealth(playerInfo.GetHealth());
		return player;
	}

	protected AutoAttack SpawnAutoAttack(int id, Vector3 startPosition, Vector3 targetPosition) {
		AutoAttack autoAttack = _players[id].SpawnAutoAttack(startPosition, targetPosition);
		autoAttack.SetGameController(this);
		return autoAttack;
	}

	protected void CreateExplosion(Vector3 position) {
		position.y = explosionPrefab.position.y;
		Instantiate(explosionPrefab, position, Quaternion.identity);
	}

	protected void RemoveDeadAutoAttacks() {
		Dictionary<int, AutoAttack> autoAttacks = new Dictionary<int, AutoAttack>();
		foreach(KeyValuePair<int, AutoAttack> autoInfo in _autoAttacks) {
			if (autoInfo.Value != null) {
				autoAttacks.Add(autoInfo.Key, autoInfo.Value);
			}
		}
		_autoAttacks = autoAttacks;
	}

	void OnDestroy() {
		if (_channel != null) {
			_channel.Destroy();   
		}
	}

}
