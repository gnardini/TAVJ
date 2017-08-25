using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer : Player {

    override protected void Update() {
		base.Update ();
        MakeMovement();
    }

    public void MoveTo(Vector3 position) {
        _targetPosition = position;    
    }

    public void MakeMovement() {
        if ((_targetPosition - transform.position).magnitude > 0.1) {
            Vector3 movement = (_targetPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            _rigidBody.MovePosition(transform.position + movement);
        }
    }

}
