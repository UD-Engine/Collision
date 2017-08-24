using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UDEngine;
using UDEngine.Components;
using UDEngine.Components.Collision;

namespace UDEngine.Interface {
	public interface IBulletRegionTrigger {
		void InvokeTrigger(UBulletCollider ubc);
	}
}