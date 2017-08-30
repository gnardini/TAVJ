using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalPlayerUpdateLoop : MovementUpdateLoop {

	private GameController _gameController;

	private int _id;
	private Dictionary<AbilityType, Player.AbilityInfo> _abilities;
	private Vector3 _targetSignPosition;

	public LocalPlayerUpdateLoop(Player player, GameController gameController, int id) : base(player) {
		_gameController = gameController;
		_id = id;
		_abilities = new Dictionary<AbilityType, Player.AbilityInfo>();
		_abilities[AbilityType.AUTOATTACK] = Copy(player.autoAttackInfo);
		_abilities[AbilityType.FREEZE] = Copy(player.freezeInfo);
		_abilities[AbilityType.FLASH] = Copy(player.flashInfo);
		player.transform.GetChild(1).gameObject.SetActive(true);
		_targetSignPosition = _player.transform.position;
	}

	override public void Update() {
		base.Update();
		foreach(Player.AbilityInfo info in _abilities.Values) {
			info.cooldown -= Time.deltaTime;
		}
		UpdateRangeHelper();
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
		foreach(KeyValuePair<AbilityType, Player.AbilityInfo> abilityPair in _abilities) {
			Player.AbilityInfo abilityInfo = abilityPair.Value;
			if (Input.GetKeyUp(abilityInfo.keyCode) && abilityInfo.cooldown <= 0) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit)) {
					Vector3 targetPosition = hit.point;
					switch(abilityPair.Key) {
					case AbilityType.AUTOATTACK:
						targetPosition.y = _player.transform.position.y;
						SendAutoAttack(abilityInfo, targetPosition);
						break;
					case AbilityType.FREEZE:
						SendFreeze(abilityInfo, targetPosition);
						break;
					case AbilityType.FLASH:
						SendFlash(abilityInfo, targetPosition);
						break;
					}
				}
			}
		}
	}

	private void SendAutoAttack(Player.AbilityInfo ability, Vector3 targetPosition) {
		ability.cooldown = _player.autoAttackInfo.cooldown;
		targetPosition.y = _player.transform.position.y;
		_gameController.SendBroadcast(new AbilityInput(_id, AbilityType.AUTOATTACK, targetPosition), true);
	}

	private void SendFreeze(Player.AbilityInfo ability, Vector3 targetPosition) {
		if (IsInRange(targetPosition, ability.range)) {
			ability.cooldown = _player.freezeInfo.cooldown;
			_gameController.SendBroadcast(new AbilityInput(_id, AbilityType.FREEZE, targetPosition), true);
		}
	}

	private void SendFlash(Player.AbilityInfo ability, Vector3 targetPosition) {
		if (IsInRange(targetPosition, ability.range)) {
			ability.cooldown = _player.flashInfo.cooldown;
			_gameController.SendBroadcast(new AbilityInput(_id, AbilityType.FLASH, targetPosition), true);
		}
	}

	private bool IsInRange(Vector3 position, float distance) {
		float x = position.x - _player.transform.position.x;
		float z = position.z - _player.transform.position.z;
		return x * x + z * z <= distance * distance;
	}

	private void UpdateRangeHelper() {
		foreach (Player.AbilityInfo abilityInfo in _abilities.Values) {
			if (Input.GetKeyDown(abilityInfo.keyCode)) {
				_player.SetRangeHelperScale(abilityInfo.range * 2);
			} else if (Input.GetKeyUp(abilityInfo.keyCode)) {
				_player.SetRangeHelperScale(0);
			}
		}
	}

	private Player.AbilityInfo Copy(Player.AbilityInfo abilityInfo) {
		Player.AbilityInfo copy = new Player.AbilityInfo();
		copy.cooldown = 0;
		copy.keyCode = abilityInfo.keyCode;
		copy.range = abilityInfo.range;
		return copy;
	}
}
