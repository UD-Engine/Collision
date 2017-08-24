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

		// UNITYFUNC begin
		void Start() {
			if (this.actor == null) {
				this.actor = new UBulletActor (this);
			}
		}
		// UNITYFUNC end

		// PROP begin

		// Enabled means should have collision detection (can still on screen), Recyclable means should REMOVE from screen
		public bool isRecyclable = false;

		public UBulletActor actor = null;
		// PROP end

		// METHOD begin
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
			if (this.actor == null) {
				this.actor = new UBulletActor (this);
			}
			return this.actor;
		}
		// METHOD end
	}
}
