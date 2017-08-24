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
	/// to simulate a room of non-simple-rect shape (by adding ubc.SetRecycle() to )
	/// </summary>
	public class URectBulletRegionTrigger : MonoBehaviour, IBulletRegionTrigger {

		// Use this for initialization
		void Start () {
			
		}
	
		// Update is called once per frame
		void Update () {
			
		}

		// PROP begin
		public UnityEvent<UBulletCollider> triggerEvents; // takes a UBulletCollider as argument
		// PROP end

		// METHOD begin
		public void InvokeTrigger(UBulletCollider ubc) {
			
		}

		// METHOD end
	}
}
