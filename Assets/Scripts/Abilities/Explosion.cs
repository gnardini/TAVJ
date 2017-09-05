using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public int timeDuration;
	public int damage;

	private float _timePast;
	private List<Player> _affectedPlayers;

	void Start () {
		_affectedPlayers = new List<Player>();
	}

	void Update () {
		_timePast += Time.deltaTime;
		if (_timePast >= timeDuration) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider collider) {
		GameObject collisionObject = collider.gameObject;
		Player player = collisionObject.GetComponent<Player>();
		if (player != null && !_affectedPlayers.Contains(player)) {
			_affectedPlayers.Add(player);
			// TODO: Fix the direction
			player.DoDamage(damage, Vector3.zero);
		}
	}

}
