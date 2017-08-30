using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UpdateLoop {
	
    void Update();

	void SetTargetPosition(Vector3 position);

	void CancelMovement();
}
