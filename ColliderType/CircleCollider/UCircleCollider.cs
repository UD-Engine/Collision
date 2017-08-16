using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Error Logging
using UDEngine.Internal;

namespace UDEngine.Components.Collision {

	//Notice all editors require inheritance from UnityEngine.Object
	//MonoBehavior provides: transform.position
	public class UCircleCollider : MonoBehaviour, IUCollider {

		// CONSTRUCTOR begin
		/// <summary>
		/// Initializes a new instance of the <see cref="UDEngine.UCircleCollider"/> class.
		/// Set radius to 1f on error.
		/// </summary>
		/// <param name="radius">Radius.</param>
		public UCircleCollider(float radius, bool enabled = true, int layer = 0, Transform trans = null) {
			if (radius <= 0f) {
				UDebug.Error ("collider radius cannot be negative or 0. Set to 1f instead");
				this.radius = 1f;
			} else {
				this.radius = radius;
			}

			// Default use the current parent as the trans, as the bullet layout goes as
			// UBulletPrefab
			//  |_ UBulletSprite
			//  |_ UBulletComponents (collider resides here)
			//  |_ UBulletChildren (used if I want to use this bullet as a bullet group, empty by default)
			if (trans == null) {
				if (this.transform.parent != null) {
					this.trans = this.transform.parent;
				} else {
					this.trans = this.transform;
				}
			} else {
				this.trans = trans;
			}

			// layer is used as tag to know whether the bullet should be detected
			this.layer = layer;
			this._enabled = enabled;
		}
		// CONSTRUCTOR end


		// STATIC begin
		private static float _suggestedMaxRadius = 5f;
		public static float GetSuggestedMaxRadius() {
			return _suggestedMaxRadius;
		}
		public static void SetSuggestedMaxRadius(float radius) {
			if (radius <= 0f) {
				UDebug.Error ("collider radius cannot be negative or 0");
			} else {
				_suggestedMaxRadius = radius;
			}
		}
		// STATIC end


		// PROP begin
		public float radius = 1f;
		public Transform trans;
		public int layer;
		private bool _enabled = true;
		// PROP end


		// INTERFACE begin
		public Vector2 GetPosition() {
			//Convert by initializing is considered the fastest;
			return new Vector2(this.trans.position.x, this.trans.position.y);
		}
		public EColliderType GetColliderType() {
			return EColliderType.CIRCLE;
		}
		public bool IsEnabled() {
			return _enabled;
		}
		public void SetEnable(bool condition = true) {
			_enabled = condition;
		}
		// INTERFACE end


		// METHODS begin
		private Vector2 _Vec3ToVec2(Vector3 vec) {
			return new Vector2 (vec.x, vec.y);
		}


		public float GetRadius() {
			return radius;
		}
		public void SetRadius(float radius) {
			if (radius <= 0f) {
				UDebug.Error ("collider radius cannot be negative or 0");
			} else {
				this.radius = radius;
			}
		}
		public Transform GetTransform() {
			return this.trans;
		}
		public void SetTransform(Transform trans) {
			this.trans = trans;
		}
		public int GetLayer() {
			return this.layer;
		}
		public void SetLayer(int layer) {
			this.layer = layer;
		}

		public float DistanceFrom(Transform otherTrans) {
			return Vector2.Distance (_Vec3ToVec2(this.trans.position), _Vec3ToVec2(otherTrans.position));
		}
		public float DistanceFrom(Vector3 otherTrans) {
			return Vector2.Distance (_Vec3ToVec2(this.trans.position), _Vec3ToVec2(otherTrans));
		}
		public float DistanceFrom(Vector2 otherTrans) {
			return Vector2.Distance (_Vec3ToVec2(this.trans.position), otherTrans);
		}

		public bool IsCollidedWith(UCircleCollider collider) {
			return this.DistanceFrom (collider.GetTransform ()) < this.GetRadius () + collider.GetRadius ();
		}
		// METHODS end
	}
}
