using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UDEngine.Interface;
using UDEngine.Internal;
using UDEngine.Components;
using UDEngine.Components.Actor;
using UDEngine.Components.Collision;

using KSM;

namespace UDEngine.Components.Collision {
	// This class handle all the collisions between the target and the bullets
	public class UCollisionMonitor : IMonolike {
		// CONSTRUCTOR begin
		public UCollisionMonitor(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
			_bulletColliders = new LinkedList<UBulletCollider> ();
			_targetColliders = new List<UTargetCollider> ();

			_boundXMin = minX;
			_boundXMax = minX + sceneWidth;
			_boundYMin = minY;
			_boundYMax = minY + sceneHeight;
		}
		// CONSTRUCTOR end

		// MONOLIKE FUNC begin
		public void StartFunc() {
		}

		public void UpdateFunc () {
			// Create a simple minimal structure to avoid meaningless repetitive target.IsEnabled() check
			List<UTargetCollider> enabledTargets = new List<UTargetCollider>();
			foreach (UTargetCollider target in _targetColliders) {
				if (target.IsEnabled()) {
					enabledTargets.Add (target);
				}
			}
			int enabledTargetsLen = enabledTargets.Count; // Save for better performance
			bool[] targetIsCollidedMap = new bool[enabledTargetsLen]; // default(bool) in C# is false

			LinkedListNode<UBulletCollider> ubcNode = _bulletColliders.First;
			while (ubcNode != null) {
				UBulletCollider ubc = ubcNode.Value;
				LinkedListNode<UBulletCollider> nextNode = ubcNode.Next;

				// Caching position will be VERY useful in later steps, including boundary collider check and target collision check
				Vector3 ubcPosition = ubc.trans.position;

				// boundary check registers
				if (!IsInBoundary (ubcPosition)) {
					ubc.GetActor ().InvokeBoundaryCallbacks (); // Invoke boundary callbacks
				}

				if (ubc.IsRecyclable ()) {
					ubc.SetRecyclable (true); // In case that the enabled is still true...

					// TODO: call clearup func here

					_bulletColliders.Remove (ubcNode); // Remove Node from tracking
				}

				ubc.GetActor().InvokeDefaultCallbacks (); // Don't invoke before checking recycling

				bool hasFastDetect = false;
				for (int i = 0; i < enabledTargetsLen; i++) {
					UTargetCollider target = enabledTargets[i];

					// This exposition of the underlying collision logic is ugly, but... it is for performance's sake...
					if (KVector3.DistanceXY(target.trans.position, ubcPosition) < ubc.GetRadius() + target.GetRadius()) {
						// If fast detect flag is set, then immediately drop out 
						// and NOT invoking any bullet collision handling events
						targetIsCollidedMap[i] = true;

						if (target.IsFastDetect ()) {
							hasFastDetect = true;
							break;
						}

						ubc.GetActor().InvokeCollisionCallbacks ();
					}
				}
				if (hasFastDetect) {
					break;
				}

				ubcNode = nextNode; // Move to the next node
			}

			for (int i = 0; i < enabledTargetsLen; i++) {
				if (targetIsCollidedMap [i]) {
					enabledTargets [i].InvokeCollisionCallbacks ();
				}
			}

		}
		// UNITYFUNC end

		// PROP begin

		// Monitor Boundary
		private float _boundXMin;
		private float _boundXMax;
		private float _boundYMin;
		private float _boundYMax;

		// To make removal faster, changed to LinkedList
		private LinkedList<UBulletCollider> _bulletColliders;
		private List<UTargetCollider> _targetColliders;

		// PROP end

		// METHOD begin
		public bool IsInBoundary(Vector3 pos) {
			return (
				(pos.x >= _boundXMin) &&
				(pos.x <= _boundXMax) &&
				(pos.y >= _boundYMin) &&
				(pos.y <= _boundYMax)
			);
		}

		public bool IsInBoundary(Vector2 pos) {
			return (
				(pos.x >= _boundXMin) &&
				(pos.x <= _boundXMax) &&
				(pos.y >= _boundYMin) &&
				(pos.y <= _boundYMax)
			);
		}


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