using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public Transform targetPositionPrefab;
    public Transform autoAttackPrefab;
    public float moveSpeed;
    public float autoAttackCooldown;
    public float maxHealth;

	protected GameObject _targetPositionSign;

    private Vector3 _targetPosition;
    private Rigidbody _rigidBody;
    private HealthBar _healthBar;
    private float _healthLeft;
    private UpdateLoop _updateLoop;

	void Start () {
        _targetPosition = transform.position;
        Vector3 startPosition = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _targetPositionSign = Instantiate(targetPositionPrefab, startPosition, Quaternion.identity).gameObject;
        _targetPositionSign.SetActive(false);
        _rigidBody = GetComponentInParent<Rigidbody>();
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthLeft = maxHealth;
    }

    public void SetUpdateLoop(UpdateLoop updateLoop) {
        _updateLoop = updateLoop;
    }

    virtual protected void Update () {
        _updateLoop.Update();
        FixPosition();
        _healthBar.SetHealthPercent(_healthLeft / maxHealth);
    }

    void FixPosition() {
        transform.position = new Vector3(transform.position.x, 1.2f, transform.position.z);
        transform.rotation = Quaternion.identity;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    public void MoveTo(Vector3 position) {
		_rigidBody.MovePosition(position);
        if ((_targetPosition - transform.position).magnitude < 0.1) {
            _targetPositionSign.SetActive(false);
        }
    }

    public void SetTargetPosition(Vector3 position) {
		_targetPosition = position;
        _updateLoop.SetTargetPosition(position);
    }

    public AutoAttack SpawnAutoAttack(Vector3 targetPosition) {
        Vector3 relativePosition = targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition);
        Vector3 startPosition = transform.position + 0.8f * relativePosition.normalized;
        return Instantiate(autoAttackPrefab, startPosition, rotation).GetComponent<AutoAttack>();
    }

	public void SetTargetSign(Vector3 position) {
		_targetPositionSign.transform.position = 
			new Vector3(position.x, _targetPositionSign.transform.position.y, position.z);
		_targetPositionSign.SetActive(true);
	}

}
