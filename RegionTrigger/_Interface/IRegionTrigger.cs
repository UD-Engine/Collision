using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UDEngine;
using UDEngine.Components;
using UDEngine.Components.Collision;

namespace UDEngine.Interface {
	public interface IBulletRegionTrigger {
		IBulletRegionTrigger AddTriggerCallback (UnityAction<UBulletCollider> callback);
		bool IsTriggerable (UBulletCollider ubc);
		bool IsTriggerable(Vector3 pos);
		bool IsTriggerable(Vector2 pos);
		IBulletRegionTrigger InvokeTriggerCallbacks(UBulletCollider ubc);
	}
}