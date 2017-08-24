using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoAndDieAbility : MonoBehaviour {

    public int moveSpeed;
    public int range;

    private Rigidbody _rigidbody;
    private Vector3 _targetPosition;

	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _targetPosition = transform.position + Vector3.forward * range;
	}
	
	void Update () {
        if ((_targetPosition - transform.position).magnitude > 0.1) {
            Vector3 movement = (_targetPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            _rigidbody.MovePosition(transform.position + movement);
        } else {
            Destroy(gameObject);
        }
	}
}
