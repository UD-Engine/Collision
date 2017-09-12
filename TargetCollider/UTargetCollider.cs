using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

using UDEngine;
using UDEngine.Internal;
using UDEngine.Core;

namespace UDEngine.Core.Collision {
	/// <summary>
	/// Target Collider in a collision system (usually Player)
	/// </summary>
	public class UTargetCollider : UCircleCollider {
		#region CONSTRUCTOR
		public UTargetCollider(float radius, bool enabled = true, int layer = 0, Transform trans = null) : base(radius, enabled, layer, trans) {
		}
		#endregion

		#region PROP
		public UnityEvent collisionEvent = null;
		public UnityEvent noHitEvent = null; // event triggered if no collision happens

		// This controls if the target should immediately drop out of collision detection loop if collision is found
		// It improves speed of handling, but causes all bullet collision events to be ignored (not invoked)
		private bool _shouldFastDetect = false; // This is useful in UCollisionMonitor module
		#endregion

		#region METHOD
		public bool IsFastDetect() {
			return _shouldFastDetect;
		}
		public UTargetCollider SetFastDetect(bool condition = true) {
			_shouldFastDetect = condition;

			return this;
		}


		public UTargetCollider AddCollisionCallback(UnityAction callback) {
			if (collisionEvent == null) {
				collisionEvent = new UnityEvent ();
			}
			collisionEvent.AddListener (callback);

			return this;
		}

		public UTargetCollider InvokeCollisionCallbacks() {
			if (collisionEvent == null) {
				// DO NOTHING, and don't warn anything, as this might be useful
			} else {
				collisionEvent.Invoke ();
			}

			return this;
		}

		public UTargetCollider ClearCollisionCallbacks() {
			collisionEvent = null;

			return this;
		}


		public UTargetCollider AddNoHitCallback(UnityAction callback) {
			if (noHitEvent == null) {
				noHitEvent = new UnityEvent ();
			}
			noHitEvent.AddListener (callback);

			return this;
		}

		public UTargetCollider InvokeNoHitCallbacks() {
			if (noHitEvent == null) {
				// DO NOTHING, and don't warn anything, as this might be useful
			} else {
				noHitEvent.Invoke ();
			}

			return this;
		}

		public UTargetCollider ClearNoHitCallbacks() {
			noHitEvent = null;

			return this;
		}
		#endregion
	}
}
