using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerUpdateLoop : MovementUpdateLoop {
    
	public ServerPlayerUpdateLoop(Player player) : base(player) {
	}

	override public void Update () {
		base.Update();
	}

	override public void SetTargetPosition(Vector3 position) {
		base.SetTargetPosition(position);
	}
}
