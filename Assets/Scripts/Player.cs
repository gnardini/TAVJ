using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Player : MonoBehaviour {

    public Transform targetPositionPrefab;
    public Transform autoAttackPrefab;
    public bool moveLocally;
    public float moveSpeed;
    public int autoAttackCooldown;

    private Vector3 _targetPosition;
    private GameObject _targetPositionSign;
    private Rigidbody _rigidBody;
//    private float autoAttackCooldown;


	void Start () {
        _targetPosition = transform.position;
        Vector3 startPosition = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _targetPositionSign = Instantiate(targetPositionPrefab, startPosition, Quaternion.identity).gameObject;
        _targetPositionSign.SetActive(false);
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update () {
        if (moveLocally) {
            MakeMovement();
            HandleAbilities();
        }
    }

    void MakeMovement() {
        if (Input.GetMouseButton(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                _targetPosition = hit.point;
                // TODO: Improve this.
                _targetPosition.y = transform.position.y;
                _targetPositionSign.transform.position = 
                    new Vector3(_targetPosition.x, _targetPositionSign.transform.position.y, _targetPosition.z);
                _targetPositionSign.SetActive(true);
            }
        }
        if ((_targetPosition - transform.position).magnitude > 0.1) {
            Vector3 movement = (_targetPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            _rigidBody.MovePosition(transform.position + movement);
        } else {
            _targetPositionSign.SetActive(false);
        }
    }
	
    void HandleAbilities() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            Instantiate(autoAttackPrefab, transform.position + Vector3.forward * .5f, transform.rotation);
        }
    }

    public void Write(BitBuffer bitBuffer) {
        bitBuffer.WriteFloat(transform.position.x);
        bitBuffer.WriteFloat(transform.position.y);
        bitBuffer.WriteFloat(transform.position.z);   
    }
}
