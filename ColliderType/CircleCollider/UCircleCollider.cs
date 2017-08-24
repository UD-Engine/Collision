using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UDEngine.Interface;
using UDEngine.Enum;
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
		/// <param name="enabled = true">Is the collider enabled</param>
		/// <param name="layer = 0">Collision layer</param>
		/// <param name="trans = null">Default transform</param>
		/// 
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
				if (this.transform.parent != null) { // tries to use parent transform first
					this.trans = this.transform.parent;
				} else { // fallback
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
		/// <summary>
		/// The suggested max radius.
		/// It is used as a safeguard agains collider against TOO large radius that Spatial Hash could not handle
		/// It is mainly used in Inspector warning
		/// </summary>
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
		/// <summary>
		/// The radius of the collider
		/// </summary>
		public float radius = 1f;

		/// <summary>
		/// trans is the actual transform that should be evaluated
		/// it is usually the PARENT of the object that contains this class
		/// </summary>
		public Transform trans;

		/// <summary>
		/// Collision layer. This simulates the Physic2D layering
		/// </summary>
		public int layer;

		/// <summary>
		/// Whether the collider is enabled
		/// </summary>
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

		// These shorthands are SLOW! Consider using the global KDistance
		public float DistanceFrom(Transform otherTrans) {
			return Vector2.Distance (_Vec3ToVec2(this.trans.position), _Vec3ToVec2(otherTrans.position));
		}
		public float DistanceFrom(Vector3 otherTrans) {
			return Vector2.Distance (_Vec3ToVec2(this.trans.position), _Vec3ToVec2(otherTrans));
		}
		public float DistanceFrom(Vector2 otherTrans) {
			return Vector2.Distance (_Vec3ToVec2(this.trans.position), otherTrans);
		}

		/// <summary>
		/// Determines whether this instance is collided with the specified collider.
		/// </summary>
		/// <returns><c>true</c> if this instance is collided with the specified collider; otherwise, <c>false</c>.</returns>
		/// <param name="collider">Collider.</param>
		public bool IsCollidedWith(UCircleCollider collider) {
			return this.DistanceFrom (collider.GetTransform ()) < this.GetRadius () + collider.GetRadius ();
		}
		// METHODS end
	}
}
