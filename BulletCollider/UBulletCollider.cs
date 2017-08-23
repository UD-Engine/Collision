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
	/// Bullet collider in collision system
	/// </summary>
	public class UBulletCollider : UCircleCollider {

		// CONSTRUCTOR begin
		public UBulletCollider(float radius, bool enabled = true, int layer = 0, Transform trans = null, bool isRecyclable = false) : base(radius, enabled, layer, trans) {
			this.isRecyclable = isRecyclable;
		}
		// CONSTRUCTOR end

		// PROP begin
		//private Sequence _collisionActionSequence = null;

		// Enabled means should have collision detection (can still on screen), Recyclable means should REMOVE from screen
		public bool isRecyclable = false;

		public UnityEvent collisionEvent = null;
		public UnityEvent defaultEvent = null;
		// PROP end

		// METHOD begin
		/*
		public Sequence GetCollisionActionSequence() {
			if (_collisionActionSequence == null) {
				_collisionActionSequence = DOTween.Sequence ();
			}
			return _collisionActionSequence;
		}*/
		public bool IsRecyclable() {
			return this.isRecyclable;
		}

		public void SetRecyclable(bool cond = true, bool shouldChangeEnabled = true) {
			this.isRecyclable = cond;

			if (shouldChangeEnabled) {
				this.SetEnable (!cond); // enabled should have EXACTEDLY opposite condition normally
			}
		}

		public void AddCollisionCallback(UnityAction callback) {
			if (collisionEvent == null) {
				collisionEvent = new UnityEvent ();
			}
			collisionEvent.AddListener (callback);
		}

		public void InvokeCollisionCallbacks() {
			if (collisionEvent == null) {
				// DO NOTHING, and don't warn anything, as this might be useful
			} else {
				collisionEvent.Invoke ();
			}
		}

		public void ClearCollisionCallbacks() {
			collisionEvent = null;
		}

		public void AddDefaultCallback(UnityAction callback) {
			if (defaultEvent == null) {
				defaultEvent = new UnityEvent ();
			}
			defaultEvent.AddListener (callback);
		}

		public void InvokeDefaultCallbacks() {
			if (defaultEvent == null) {
				// DO NOTHING, and don't warn anything, as this might be useful
			} else {
				defaultEvent.Invoke ();
			}
		}

		public void ClearDefaultCallbacks() {
			defaultEvent = null;
		}
		// METHOD end
	}
}
