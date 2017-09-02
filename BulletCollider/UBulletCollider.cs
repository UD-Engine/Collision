using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

using UDEngine;
using UDEngine.Internal;
using UDEngine.Components;
using UDEngine.Components.Action;
using UDEngine.Components.Actor;
using UDEngine.Components.Bullet;

namespace UDEngine.Components.Collision {
	/// <summary>
	/// Bullet collider in collision system
	/// </summary>
	public class UBulletCollider : UCircleCollider {

		// CONSTRUCTOR begin
		public UBulletCollider(float radius, bool enabled = true, int layer = 0, Transform trans = null, bool isRecyclable = false) : base(radius, enabled, layer, trans) {
			this.isRecyclable = isRecyclable;
		}
		// CONSTRUCTOR end

		#region UNITYFUNC
		void Start() {
			if (this.bulletObject == null) {
				UDebug.Warning("No bullet object linked from this UBulletCollider");
			}
		}
		#endregion

		#region PROP
		// Enabled means should have collision detection (can still on screen), Recyclable means should REMOVE from screen
		public bool isRecyclable = false;

		public UBulletObject bulletObject = null;
		// actor has been evicted to the bulletObject, which also contains GetActor();
		#endregion

		#region METHOD
		public bool IsRecyclable() {
			return this.isRecyclable;
		}

		public void SetRecyclable(bool cond = true, bool shouldChangeEnabled = true) {
			this.isRecyclable = cond;

			if (shouldChangeEnabled) {
				this.SetEnable (!cond); // enabled should have EXACTEDLY opposite condition normally
			}
		}

		public UBulletActor GetActor() {
			return this.bulletObject.GetActor ();
		}

		public UBulletObject GetObject() {
			return this.bulletObject;
		}
		#endregion
	}
}
