using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour {

    public int moveSpeed;
    public int range;

    private Rigidbody _rigidbody;
    private Vector3 _targetPosition;
    private Vector3 _nextPosition;

	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _targetPosition = transform.position + transform.forward * range;
        _nextPosition = _targetPosition;
	}
	
	void Update () {
        if ((_targetPosition - transform.position).magnitude < 0.1) {
            Destroy(gameObject);
        } else {
            Vector3 movement = (_nextPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            _rigidbody.MovePosition(transform.position + movement);
        }
            
	}

    // This is called in the clients with the position received from the server.
    public void MoveTo(Vector3 position) {
        _nextPosition = position;
    }
}
