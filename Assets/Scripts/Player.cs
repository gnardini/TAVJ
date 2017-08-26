using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public Transform targetPositionPrefab;
    public Transform autoAttackPrefab;
    public float moveSpeed;
    public int autoAttackCooldown;

    protected Vector3 _targetPosition;
    protected GameObject _targetPositionSign;
    protected Rigidbody _rigidBody;
    public GameController _gameController;
    private int _id;
    private bool _moveLocally;
    private float _autoAttackCooldownRemaining;

	void Start () {
        _targetPosition = transform.position;
        Vector3 startPosition = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _targetPositionSign = Instantiate(targetPositionPrefab, startPosition, Quaternion.identity).gameObject;
        _targetPositionSign.SetActive(false);
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void SetId(int id) {
        _id = id;
    }

    public void SetMoveLocally(bool moveLocally) {
        _moveLocally = moveLocally;
    }

    public void SetGameController(GameController gameController) {
        _gameController = gameController;
    }

    virtual protected void Update () {
        if (_moveLocally) {
            SendMovement();
            SendAbilities();
        }
        FixPosition();
    }

    void FixPosition() {
        transform.position = new Vector3(transform.position.x, 1.2f, transform.position.z);
        transform.rotation = Quaternion.identity;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    public void MakeMovement(Vector3 position) {
        transform.position = position;
        if ((_targetPosition - transform.position).magnitude < 0.1) {
            _targetPositionSign.SetActive(false);
        }
    }

    public AutoAttack SpawnAutoAttack(Vector3 targetPosition) {
        Vector3 relativePosition = targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition);
        Vector3 startPosition = transform.position + 0.8f * relativePosition.normalized;
        return Instantiate(autoAttackPrefab, startPosition, rotation).GetComponent<AutoAttack>();
    }

    void SendMovement() {
        if (Input.GetMouseButton(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                _targetPosition = hit.point;
                _targetPosition.y = transform.position.y;
                _gameController.SendByteable(new MovementInput(_id, new PositionInfo(_targetPosition)));
                _targetPositionSign.transform.position = 
                    new Vector3(_targetPosition.x, _targetPositionSign.transform.position.y, _targetPosition.z);
                _targetPositionSign.SetActive(true);
            }
        }
    }
	
    void SendAbilities() {
        _autoAttackCooldownRemaining -= Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.Space) && _autoAttackCooldownRemaining <= float.Epsilon) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                _autoAttackCooldownRemaining = autoAttackCooldown;
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y;
                _gameController.SendByteable(new AutoAttackInput(_id, targetPosition));
            }
        }
    }

}
