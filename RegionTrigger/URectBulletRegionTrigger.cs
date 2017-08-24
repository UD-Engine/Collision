using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UDEngine;
using UDEngine.Components;
using UDEngine.Components.Collision;
using UDEngine.Components.Actor;
using UDEngine.Internal;
using UDEngine.Interface;
using UDEngine.Enum;

namespace UDEngine.Components.Collision {
	/// <summary>
	/// Rect Region Trigger for Bullets. It is used to create something like
	/// invisible bounce walls, or, combined with original boundary settings of UCollisionMonitor
	/// to simulate a room of non-simple-rect shape (by adding ubc.SetRecycle() to the callbacks)
	/// 
	/// Also, Region Trigger would simply check ONLY position, but NOT radius, as in effect it is not helpful
	/// </summary>
	public class URectBulletRegionTrigger : MonoBehaviour, IBulletRegionTrigger {

		// Use this for initialization
		void Start () {
			
		}
	
		// Update is called once per frame
		void Update () {
			
		}

		// PROP begin

		// Positional
		public float xMin;
		public float yMin;
		public float width;
		public float height;


		public UBulletRegionTriggerEvent triggerEvents; // takes a UBulletCollider as argument
		// PROP end

		// METHOD begin
		public void AddTriggerCallback(UnityAction<UBulletCollider> callback) {
			if (triggerEvents == null) { // Lazy init
				triggerEvents = new UBulletRegionTriggerEvent();
			}
			triggerEvents.AddListener (callback);
		}

		public bool IsTriggerable(UBulletCollider ubc) {
			return IsTriggerable (ubc.GetPosition ());
		}

		public bool IsTriggerable(Vector3 pos) {
			return (
				(pos.x > xMin) &&
				(pos.x < xMin + width) &&
				(pos.y > yMin) &&
				(pos.y < yMin + height)
			);
		}

		public bool IsTriggerable(Vector2 pos) {
			return (
				(pos.x > xMin) &&
				(pos.x < xMin + width) &&
				(pos.y > yMin) &&
				(pos.y < yMin + height)
			);
		}

		public void InvokeTriggerCallbacks(UBulletCollider ubc) {
			if (triggerEvents == null) {
				// Do nothing
			} else {
				triggerEvents.Invoke (ubc);
			}
		}
		// METHOD end
	}
}
