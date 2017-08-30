using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour {

	public int timeDuration;
	public float speedMultiplier;

	private int _ownerId;
	private float _timePast;
	private List<Player> _slowedPlayers;

	void Start () {
		_slowedPlayers = new List<Player>();
	}

	void Update () {
		_timePast += Time.deltaTime;
		if (_timePast >= timeDuration) {
			foreach(Player player in _slowedPlayers) {
				player.SetSpeedMultiplier(1f);
			}
			Destroy(gameObject);
		}
	}

	public void SetOwnerId(int id) {
		_ownerId = id;
	}

	void OnTriggerEnter(Collider collider) {
		GameObject collisionObject = collider.gameObject;
		Player player = collisionObject.GetComponent<Player>();
		if (player != null) {
			if (player.GetId() != _ownerId) {
				player.SetSpeedMultiplier(speedMultiplier);
				_slowedPlayers.Add(player);
			}
		}
	}

	void OnTriggerExit(Collider collider) {
		GameObject collisionObject = collider.gameObject;
		Player player = collisionObject.GetComponent<Player>();
		if (player != null) {
			if (player.GetId() != _ownerId) {
				player.SetSpeedMultiplier(1f);
				_slowedPlayers.Remove(player);
			}
		}
	}
}
