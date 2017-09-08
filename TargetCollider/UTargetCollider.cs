using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

using UDEngine;
using UDEngine.Internal;
using UDEngine.Components;
using UDEngine.Components.Action;

namespace UDEngine.Components.Collision {
	/// <summary>
	/// Target Collider in a collision system (usually Player)
	/// </summary>
	public class UTargetCollider : UCircleCollider {
		// CONSTRUCTOR begin
		public UTargetCollider(float radius, bool enabled = true, int layer = 0, Transform trans = null) : base(radius, enabled, layer, trans) {
		}
		// CONSTRUCTOR end

		// PROP begin
		public UnityEvent collisionEvent = null;

		// This controls if the target should immediately drop out of collision detection loop if collision is found
		// It improves speed of handling, but causes all bullet collision events to be ignored (not invoked)
		private bool _shouldFastDetect = false; // This is useful in UCollisionMonitor module
		// PROP end

		// METHOD begin
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
		// METHOD end
	}
}
