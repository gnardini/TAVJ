using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalPlayerUpdateLoop : MovementUpdateLoop {
	
	private GameController _gameController;

	private int _id;
	private Dictionary<AbilityType, float> _cooldowns;
	private Vector3 _targetSignPosition;

	public LocalPlayerUpdateLoop(Player player, GameController gameController, int id) : base(player) {
		_gameController = gameController;
		_id = id;
		_cooldowns = new Dictionary<AbilityType, float>();
		_cooldowns[AbilityType.AUTOATTACK] = 0;
		_cooldowns[AbilityType.FREEZE] = 0;
		_cooldowns[AbilityType.FLASH] = 0;
		_cooldowns[AbilityType.EXPLOSION] = 0;
		player.transform.GetChild(1).gameObject.SetActive(true);
		_targetSignPosition = _player.transform.position;
	}

	override public void Update() {
		base.Update();
		foreach(AbilityType ability in Enum.GetValues(typeof(AbilityType))) {
			_cooldowns[ability] -= Time.deltaTime;
		}
        SendMovement();
        SendAbilities();
		if ((_targetSignPosition - _player.transform.position).magnitude < 0.15f) {
			_player.HideTargetSign();
		}
	}

	void SendMovement() {
		if (Input.GetMouseButton(1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				Vector3 targetPosition = hit.point;
				targetPosition.y = _player.transform.position.y;
				_targetSignPosition = targetPosition;
				_player.SetTargetSign(targetPosition);
				_gameController.SendBroadcast(new MovementInput(_id, new PositionInfo(targetPosition)), true);
			}
		}
	}

	void SendAbilities() {
		if (Input.GetKeyUp(KeyCode.Q) && _cooldowns[AbilityType.AUTOATTACK] <= 0) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				_cooldowns[AbilityType.AUTOATTACK] = _player.autoAttackCooldown;
				Vector3 targetPosition = hit.point;
				targetPosition.y = _player.transform.position.y;
				_gameController.SendBroadcast(new AbilityInput(_id, AbilityType.AUTOATTACK, targetPosition), true);
			}
		}
		if (Input.GetKeyUp(KeyCode.W) && _cooldowns[AbilityType.FREEZE] <= 0) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				_cooldowns[AbilityType.FREEZE] = _player.freezeCooldown;
				Vector3 targetPosition = hit.point;
				_gameController.SendBroadcast(new AbilityInput(_id, AbilityType.FREEZE, targetPosition), true);
			}
		}
		if (Input.GetKeyUp(KeyCode.E) && _cooldowns[AbilityType.FLASH] <= 0) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				_cooldowns[AbilityType.FLASH] = _player.flashCooldown;
				Vector3 targetPosition = hit.point;
				_gameController.SendBroadcast(new AbilityInput(_id, AbilityType.FLASH, targetPosition), true);
			}
		}
	}
}
