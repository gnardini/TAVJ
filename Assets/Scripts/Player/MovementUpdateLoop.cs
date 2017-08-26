using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUpdateLoop : UpdateLoop {

	protected Player _player;
	protected Vector3 _targetPosition;

	public MovementUpdateLoop(Player player) {
		_player = player;
	}

	public virtual void Update() {
		if ((_targetPosition - _player.transform.position).magnitude > 0.1) {
			Vector3 movement = (_targetPosition - _player.transform.position).normalized * _player.moveSpeed * Time.deltaTime;
			_player.MoveTo(_player.transform.position + movement);	
		}
	}

	public virtual void SetTargetPosition(Vector3 position) {
		_targetPosition = position;
	}

}
