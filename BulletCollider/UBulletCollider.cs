using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

using UDEngine;
using UDEngine.Internal;
using UDEngine.Core;
using UDEngine.Core.Actor;
using UDEngine.Core.Bullet;

namespace UDEngine.Core.Collision {
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
				UDebug.Error("No bullet object linked from this UBulletCollider");
			}
			if (this.actor == null) {
				UDebug.Error("No bullet actor linked from this UBulletCollider");
			}
		}
		#endregion

		#region PROP
		// Enabled means should have collision detection (can still on screen), Recyclable means should REMOVE from screen
		public bool isRecyclable = false;

		public UBulletObject bulletObject = null;
		public UBulletActor actor = null; // re-inserted as the modified version proves terrible performance
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
			return this.actor;
		}

		public UBulletObject GetObject() {
			return this.bulletObject;
		}
		#endregion
	}
}
