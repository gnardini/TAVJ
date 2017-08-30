using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour {

    public int moveSpeed;
    public int range;
	public int damage;

	private GameController _gameController;
    private Rigidbody _rigidbody;
    private Vector3 _targetPosition;
    private Vector3 _nextPosition;
	private int _ownerId;

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

	public void SetOwnerId(int id) {
		_ownerId = id;
	}

	public int GetOwnerId() {
		return _ownerId;
	}

	public void SetGameController(GameController gameController) {
		_gameController = gameController;
	}

    // This is called in the clients with the position received from the server.
    public void MoveTo(Vector3 position) {
        _nextPosition = position;
    }

	void OnCollisionEnter(Collision collision) {
		GameObject collisionObject = collision.gameObject;
		Player player = collisionObject.GetComponent<Player>();
		if (player != null) {
			if (player.GetId() != _ownerId) {
				player.DoDamage(damage);
			} else {
				return;
			}
		}
		AutoAttack autoAttack = collisionObject.GetComponent<AutoAttack>();
		// The reason for the second condition is so that only one of the autoattacks
		if (autoAttack != null && _ownerId < autoAttack.GetOwnerId()) {
			float xPosition = (transform.position.x + autoAttack.transform.position.x) / 2;
			float zPosition = (transform.position.z + autoAttack.transform.position.z) / 2;
			Vector3 position = new Vector3(xPosition, 1f, zPosition);
			_gameController.SendBroadcast(new AbilityInput(0, AbilityType.EXPLOSION, position), true);
		}
		Destroy(gameObject);
	}
}
