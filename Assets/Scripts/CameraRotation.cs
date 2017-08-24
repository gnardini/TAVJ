using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    public Transform target;
    public float speed = 12.0f;

	void Start () {	
	}
	
	void LateUpdate () { 
        if (Input.GetMouseButton(1)) {
            float x = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
            transform.RotateAround(target.position, transform.up, x);
        }
	}
}
