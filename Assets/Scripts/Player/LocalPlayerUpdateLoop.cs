using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerUpdateLoop : MovementUpdateLoop {
	
	private GameController _gameController;

	private int _id;
	private float _autoAttackCooldownRemaining;

	public LocalPlayerUpdateLoop(Player player, GameController gameController, int id) : base(player) {
		_gameController = gameController;
		_id = id;
		_autoAttackCooldownRemaining = player.autoAttackCooldown;
		player.transform.GetChild(1).gameObject.SetActive(true);

	}

	override public void Update() {
		base.Update();
        SendMovement();
        SendAbilities();
	}

	void SendMovement() {
		if (Input.GetMouseButton(1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				Vector3 targetPosition = hit.point;
				targetPosition.y = _player.transform.position.y;
				_gameController.SendBroadcast(new MovementInput(_id, new PositionInfo(targetPosition)), true);
			}
		}
	}

	void SendAbilities() {
		_autoAttackCooldownRemaining -= Time.deltaTime;
		if (Input.GetKeyUp(KeyCode.Space) && _autoAttackCooldownRemaining <= float.Epsilon) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				_autoAttackCooldownRemaining = _player.autoAttackCooldown;
				Vector3 targetPosition = hit.point;
				targetPosition.y = _player.transform.position.y;
				_gameController.SendBroadcast(new AutoAttackInput(_id, targetPosition), true);
			}
		}
	}
}
