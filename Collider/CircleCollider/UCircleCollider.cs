using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Error Logging
using UDEngine.Internal;

namespace UDEngine.Components {

	//Notice all editors require inheritance from UnityEngine.Object
	//MonoBehavior provides: transform.position
	public class UCircleCollider : MonoBehaviour, IUCollider {

		/* CONSTRUCTOR */
		/// <summary>
		/// Initializes a new instance of the <see cref="UDEngine.UCircleCollider"/> class.
		/// Set radius to 1f on error.
		/// </summary>
		/// <param name="radius">Radius.</param>
		public UCircleCollider(float radius) {
			if (radius <= 0f) {
				UDebug.Error ("collider radius cannot be negative or 0. Set to 1f instead");
				this.radius = 1f;
			} else {
				this.radius = radius;
			}
		}


		/* STATIC */

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


		/* INSTANCE */
		public float radius = 1f;

		//Getter
		//Matching IUCollider Interface
		public Vector2 GetPosition() {
			//Convert by initializing is considered the fastest;
			return new Vector2(transform.position.x, transform.position.y);
		}
		public EColliderType GetColliderType() {
			return EColliderType.CIRCLE;
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
	}
}
