using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameController gameController;
    public Transform targetPositionPrefab;
    public Transform autoAttackPrefab;
    public bool moveLocally;
    public float moveSpeed;
    public int autoAttackCooldown;

    protected Vector3 _targetPosition;
    protected GameObject _targetPositionSign;
    protected Rigidbody _rigidBody;
//    private float autoAttackCooldown;

	void Start () {
        _targetPosition = transform.position;
        Vector3 startPosition = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _targetPositionSign = Instantiate(targetPositionPrefab, startPosition, Quaternion.identity).gameObject;
        _targetPositionSign.SetActive(false);
        _rigidBody = GetComponent<Rigidbody>();
    }

    virtual protected void Update () {
        if (moveLocally) {
            SendMovement();
            HandleAbilities();
            FixPosition();
        }
    }

    void FixPosition() {
        transform.position = new Vector3(transform.position.x, 1.2f, transform.position.z);
        transform.rotation = Quaternion.identity;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    public void MakeMovement(PositionInfo positionInfo) {
        transform.position = positionInfo.GetPosition();
        if ((_targetPosition - transform.position).magnitude < 0.1) {
            _targetPositionSign.SetActive(false);
        }
    }

    void SendMovement() {
        if (Input.GetMouseButton(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                _targetPosition = hit.point;
                _targetPosition.y = transform.position.y;
                gameController.SendByteable(new PlayerInput(InputType.MOVEMENT, _targetPosition));
                _targetPositionSign.transform.position = 
                    new Vector3(_targetPosition.x, _targetPositionSign.transform.position.y, _targetPosition.z);
                _targetPositionSign.SetActive(true);
            }
        }
    }
	
    void HandleAbilities() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y;
                Vector3 relativePosition = targetPosition - transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePosition);
                Instantiate(autoAttackPrefab, transform.position + 0.8f * relativePosition.normalized, rotation);
            }
        }
    }

}
