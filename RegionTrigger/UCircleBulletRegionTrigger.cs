using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UDEngine;
using UDEngine.Core;
using UDEngine.Core.Collision;
using UDEngine.Core.Actor;
using UDEngine.Internal;
using UDEngine.Interface;
using UDEngine.Enum;

using KSM;

namespace UDEngine.Core.Collision {
	/// <summary>
	/// Rect Region Trigger for Bullets. It is used to create something like
	/// invisible bounce walls, or, combined with original boundary settings of UCollisionMonitor
	/// to simulate a room of non-simple-rect shape (by adding ubc.SetRecycle() to the callbacks)
	/// 
	/// Also, Region Trigger would simply check ONLY position, but NOT radius, as in effect it is not helpful
	/// </summary>
	public class UCircleBulletRegionTrigger : MonoBehaviour, IBulletRegionTrigger {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		#region PROP

		// Positional
		public float centerX;
		public float centerY;
		public float radius;


		public UBulletRegionTriggerEvent triggerEvents; // takes a UBulletCollider as argument
		#endregion

		#region METHOD
		public IBulletRegionTrigger AddTriggerCallback(UnityAction<UBulletCollider> callback) {
			if (triggerEvents == null) { // Lazy init
				triggerEvents = new UBulletRegionTriggerEvent();
			}
			triggerEvents.AddListener (callback);

			return this;
		}

		public bool IsTriggerable(UBulletCollider ubc) {
			return IsTriggerable (ubc.GetPosition ());
		}

		public bool IsTriggerable(Vector3 pos) {
			float xDiff = pos.x - centerX;
			float yDiff = pos.y - centerY;

			return (
				Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff) < radius
			);
		}

		public bool IsTriggerable(Vector2 pos) {
			float xDiff = pos.x - centerX;
			float yDiff = pos.y - centerY;

			return (
				Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff) < radius
			);
		}

		public IBulletRegionTrigger InvokeTriggerCallbacks(UBulletCollider ubc) {
			if (triggerEvents == null) {
				// Do nothing
			} else {
				triggerEvents.Invoke (ubc);
			}

			return this;
		}
		#endregion
	}
}
