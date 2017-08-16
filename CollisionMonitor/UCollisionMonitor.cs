using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UDEngine.Internal;
using UDEngine.Components;
using UDEngine.Components.Collision;

namespace UDEngine.Components.Collision {
	// This class handle all the collisions between the target and the bullets
	public class UCollisionMonitor : IMonolike {
		// CONSTRUCTOR begin
		public UCollisionMonitor(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
			_bulletColliders = new List<UBulletCollider> ();
			_targetColliders = new List<UTargetCollider> ();

			_bulletHash = new SpatialHash2D<UCircleCollider> (cols, rows, sceneWidth, sceneHeight, minX, minY);
		}
		// CONSTRUCTOR end

		// MONOLIKE FUNC begin
		public void StartFunc() {
		}

		public void UpdateFunc () {
			_bulletHash.ClearBuckets ();
			foreach (UBulletCollider ubc in _bulletColliders) {
				ubc.InvokeDefaultCallbacks ();
				if (ubc.IsEnabled ()) {
					_bulletHash.Insert (ubc);
				}
			}

			foreach (UTargetCollider target in _targetColliders) {
				if (!target.IsEnabled ()) {
					continue;
				} else {
					List<UCircleCollider> nearbyBullets = _bulletHash.GetNearby (target);

					bool isCollided = false;
					foreach (UCircleCollider uc in nearbyBullets) {
						if (uc.IsCollidedWith (target)) {
							isCollided = true;
							// If fast detect flag is set, then immediately drop out 
							// and NOT invoking any bullet collision handling events
							if (target.IsFastDetect ()) {
								break;
							}

							UBulletCollider ubc = uc as UBulletCollider;
							ubc.InvokeCollisionCallbacks ();
						}
					}

					if (isCollided) {
						target.InvokeCollisionCallbacks ();
					}
				}
			}
		}
		// UNITYFUNC end

		// PROP begin
		private List<UBulletCollider> _bulletColliders;
		private List<UTargetCollider> _targetColliders;

		private SpatialHash2D<UCircleCollider> _bulletHash;
		// PROP end

		// METHOD begin
		public List<UBulletCollider> GetBulletColliders() {
			return _bulletColliders;
		}

		public List<UTargetCollider> GetTargetColliders() {
			return _targetColliders;
		}

		public void AddBulletColliders(List<UBulletCollider> colliders) {
			_bulletColliders.AddRange (colliders);
		}

		public void AddBulletCollider(UBulletCollider collider) {
			_bulletColliders.Add (collider);
		}

		public void AddTargetColliders(List<UTargetCollider> colliders) {
			_targetColliders.AddRange (colliders);
		}

		public void AddTargetCollider(UTargetCollider collider) {
			_targetColliders.Add (collider);
		}

		public void ClearBulletColliders() {
			_bulletColliders.Clear ();
		}

		public void ClearTargetColliders() {
			_targetColliders.Clear ();
		}

		public void RefillBulletColliders(List<UBulletCollider> colliders) {
			ClearBulletColliders ();
			AddBulletColliders (colliders);
		}

		public void RefillTargetColliders(List<UTargetCollider> colliders) {
			ClearTargetColliders ();
			AddTargetColliders (colliders);
		}

		public void RemoveBulletCollider(UBulletCollider collider) {
			_bulletColliders.Remove (collider);
		}

		public void RemoveTargetCollider(UTargetCollider collider) {
			_targetColliders.Remove (collider);
		}
		// METHOD end
	}
}