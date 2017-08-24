using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UDEngine.Interface;
using UDEngine.Internal;
using UDEngine.Components;
using UDEngine.Components.Collision;

namespace UDEngine.Components.Collision {
	// This class handle all the collisions between the target and the bullets
	public class UCollisionMonitorOld : IMonolike {
		// CONSTRUCTOR begin
		public UCollisionMonitorOld(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
			_bulletColliders = new LinkedList<UBulletCollider> ();
			_targetColliders = new List<UTargetCollider> ();

			_bulletHash = new USpatialHash2DV3<UCircleCollider> (cols, rows, sceneWidth, sceneHeight, minX, minY);
		}
		// CONSTRUCTOR end

		// MONOLIKE FUNC begin
		public void StartFunc() {
		}

		public void UpdateFunc () {
			_bulletHash.ClearBuckets ();

			/*
			// Looping through LinkedList
			LinkedListNode<UBulletCollider> ubcNode = _bulletColliders.First;
			while (ubcNode != null) {
				UBulletCollider ubc = ubcNode.Value;
				LinkedListNode<UBulletCollider> nextNode = ubcNode.Next;

				ubc.InvokeDefaultCallbacks ();

				// TODO: boundary check registers

				if (ubc.IsRecyclable ()) {
					ubc.SetRecyclable (true); // In case that the enabled is still true...

					// TODO: call clearup func here

					_bulletColliders.Remove (ubcNode); // Remove Node from tracking
				} else {
					if (ubc.IsEnabled ()) {
						_bulletHash.Insert (ubc); // Insert to hash ONLY if is enabled
					}
				}

				ubcNode = nextNode; // Move to the next node
			}
			*/


//			foreach (UBulletCollider ubc in _bulletColliders) {
//				ubc.InvokeDefaultCallbacks ();
//				if (ubc.IsEnabled ()) {
//					_bulletHash.Insert (ubc);
//				}
//			}

			foreach (UTargetCollider target in _targetColliders) {
				if (!target.IsEnabled ()) {
					continue;
				} else {
					List<UCircleCollider> nearbyBullets = _bulletHash.GetNearby (target);

					bool isCollided = false;

					/*
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
					*/

					foreach (UBulletCollider ubc in _bulletColliders) {
						ubc.GetActor().InvokeDefaultCallbacks ();

						if (ubc.IsCollidedWith (target)) {
							isCollided = true;
							// If fast detect flag is set, then immediately drop out 
							// and NOT invoking any bullet collision handling events
							if (target.IsFastDetect ()) {
								break;
							}

							ubc.GetActor().InvokeCollisionCallbacks ();
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

		// To make removal faster, changed to LinkedList
		private LinkedList<UBulletCollider> _bulletColliders;
		private List<UTargetCollider> _targetColliders;

		private USpatialHash2DV3<UCircleCollider> _bulletHash;
		// PROP end

		// METHOD begin
		public LinkedList<UBulletCollider> GetBulletColliders() {
			return _bulletColliders;
		}

		public List<UTargetCollider> GetTargetColliders() {
			return _targetColliders;
		}

		public void AddBulletColliders(List<UBulletCollider> colliders) {
			//_bulletColliders.AddRange (colliders);
			for (int i = 0; i < colliders.Count; i++) {
				_bulletColliders.AddLast (colliders [i]);
			}
		}

		public void AddBulletCollider(UBulletCollider collider) {
			//_bulletColliders.Add (collider);
			_bulletColliders.AddLast (collider);
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