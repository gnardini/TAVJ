﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;

public class Player : MonoBehaviour {

	[Serializable]
	public class AbilityInfo {
		public float cooldown;
		public float range;
		public KeyCode keyCode;
	}
    
    public Transform targetPositionPrefab;
    public Transform autoAttackPrefab;
	public Transform freezePrefab;
	public Transform rangeHelper;
    public float moveSpeed;
	public AbilityInfo autoAttackInfo;
	public AbilityInfo freezeInfo;
	public AbilityInfo flashInfo;
    public int maxHealth;

	protected GameObject _targetPositionSign;

    private Rigidbody _rigidBody;
    private HealthBar _healthBar;
	private int _id;
    private int _healthLeft;
	private float _speedMultiplier;
    private UpdateLoop _updateLoop;

	void Start () {
        Vector3 startPosition = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _targetPositionSign = Instantiate(targetPositionPrefab, startPosition, Quaternion.identity).gameObject;
        _targetPositionSign.SetActive(false);
        _rigidBody = GetComponentInParent<Rigidbody>();
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthLeft = maxHealth;
		_speedMultiplier = 1f;
    }

    public void SetUpdateLoop(UpdateLoop updateLoop) {
        _updateLoop = updateLoop;
    }

    virtual protected void Update () {
        _updateLoop.Update();
        FixPosition();
		_healthBar.SetHealthPercent(_healthLeft / (float) maxHealth);
    }

	public void SetId(int id) {
		_id = id;
	}

	public int GetId() {
		return _id;
	}

	public void SetSpeedMultiplier(float multiplier) {
		_speedMultiplier = multiplier;
	}

	public float GetMoveSpeed() {
		return moveSpeed * _speedMultiplier;
	}

	public void SetRangeHelperScale(float scale) {
		Vector3 rangeHelperScale = rangeHelper.localScale;
		rangeHelperScale.x = scale;
		rangeHelperScale.z = scale;
		rangeHelper.localScale = rangeHelperScale;
	}

    void FixPosition() {
        transform.position = new Vector3(transform.position.x, 1.2f, transform.position.z);
        transform.rotation = Quaternion.identity;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    public void MoveTo(Vector3 position) {
		transform.position = position;
//		_rigidBody.MovePosition(position);
    }

	public void CancelMovement() {
		_updateLoop.CancelMovement();
		HideTargetSign();
	}

    public void SetTargetPosition(Vector3 position) {
        _updateLoop.SetTargetPosition(position);
    }

	public int GetHealth() {
		return _healthLeft;
	}

	public void SetHealth(int health) {
		_healthLeft = health;
	}

	public void DoDamage(int damage) {
		_healthLeft -= damage;
		if (_healthLeft < 0) {
			_healthLeft = 0;
		}
	}

    public AutoAttack SpawnAutoAttack(Vector3 startPosition, Vector3 targetPosition) {
        Vector3 relativePosition = targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition);
		startPosition = startPosition + 0.8f * relativePosition.normalized;
		AutoAttack auto = Instantiate(autoAttackPrefab, startPosition, rotation).GetComponent<AutoAttack>();
		auto.SetOwnerId(_id);
		return auto;
    }

	public Freeze SpawnFreeze(Vector3 targetPosition) {
		targetPosition.y = freezePrefab.position.y;
		Freeze freeze = Instantiate(freezePrefab, targetPosition, Quaternion.identity).GetComponent<Freeze>();
		freeze.SetOwnerId(_id);
		return freeze;	
	}

	public void SetTargetSign(Vector3 position) {
		_targetPositionSign.transform.position = 
			new Vector3(position.x, _targetPositionSign.transform.position.y, position.z);
		_targetPositionSign.SetActive(true);
	}

	public void HideTargetSign() {
		_targetPositionSign.SetActive(false);
	}

}
