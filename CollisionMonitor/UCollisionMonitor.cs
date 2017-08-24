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
	public class UCollisionMonitor : MonoBehaviour {
		// CONSTRUCTOR begin
		// Removed as UCollisionMonitor changed from IMonolike to MonoBehaviour descendant
		/*
		public UCollisionMonitor(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
			_bulletColliders = new LinkedList<UBulletCollider> ();
			_targetColliders = new List<UTargetCollider> ();

			boundXMin = minX;
			boundWidth = sceneWidth;
			boundYMin = minY;
			boundHeight = sceneHeight;
		}
		*/
		// CONSTRUCTOR end

		// MONOLIKE FUNC begin
		void Start() {
			_bulletColliders = new LinkedList<UBulletCollider> ();
			_targetColliders = new List<UTargetCollider> ();
			_bulletRegionTriggers = new List<IBulletRegionTrigger> ();
		}

		void Update () {
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

				// region trigger check
				int regionTriggerLen = _bulletRegionTriggers.Count;
				for (int i = 0; i < regionTriggerLen; i++) {
					IBulletRegionTrigger trigger = _bulletRegionTriggers [i];
					// Invoke regional trigger collider callbacks on collision
					if (trigger.IsTriggerable (ubc)) {
						trigger.InvokeTriggerCallbacks (ubc);
					}
				}

				if (ubc.IsRecyclable ()) {
					ubc.SetRecyclable (true); // In case that the enabled is still true...

					// TODO: call clearup func here

					_bulletColliders.Remove (ubcNode); // Remove Node from tracking
				}

				// DEFAULT CALLBACK INVOCATION
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
		public float boundXMin;
		public float boundWidth;
		public float boundYMin;
		public float boundHeight;

		// To make removal faster, changed to LinkedList
		private LinkedList<UBulletCollider> _bulletColliders; // This is still private, as it will be TERRIBLE to display this

		// This is made public now, for easier configuration
		private List<UTargetCollider> _targetColliders;

		private List<IBulletRegionTrigger> _bulletRegionTriggers;

		// PROP end

		// METHOD begin
		public bool IsInBoundary(Vector3 pos) {
			return (
				(pos.x >= boundXMin) &&
				(pos.x <= boundXMin + boundWidth) &&
				(pos.y >= boundYMin) &&
				(pos.y <= boundYMin + boundHeight)
			);
		}

		public bool IsInBoundary(Vector2 pos) {
			return (
				(pos.x >= boundXMin) &&
				(pos.x <= boundXMin + boundWidth) &&
				(pos.y >= boundYMin) &&
				(pos.y <= boundYMin + boundHeight)
			);
		}

		// Safety check not yet added (e.g. initialize on use) as no problem currently met yet
		public LinkedList<UBulletCollider> GetBulletColliders() {
			return _bulletColliders;
		}

		public List<UTargetCollider> GetTargetColliders() {
			return _targetColliders;
		}

		public List<IBulletRegionTrigger> GetBulletRegionTriggers() {
			return _bulletRegionTriggers;
		}

		public void AddBulletColliders(List<UBulletCollider> colliders) {
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

		public void AddBulletRegionTrigger(IBulletRegionTrigger trigger) {
			_bulletRegionTriggers.Add (trigger);
		}

		public void AddBulletRegionTriggers(List<IBulletRegionTrigger> triggers) {
			_bulletRegionTriggers.AddRange (triggers);
		}

		public void ClearBulletColliders() {
			_bulletColliders.Clear ();
		}

		public void ClearTargetColliders() {
			_targetColliders.Clear ();
		}

		public void ClearBulletRegionTriggers() {
			_bulletRegionTriggers.Clear ();
		}

		// These remove methods are EXTREMELY slow (especially the RemoveBulletCollider()); AVOID them
		public void RemoveBulletCollider(UBulletCollider collider) {
			_bulletColliders.Remove (collider);
		}

		public void RemoveTargetCollider(UTargetCollider collider) {
			_targetColliders.Remove (collider);
		}

		public void RemoveBulletRegionTrigger(IBulletRegionTrigger trigger) {
			_bulletRegionTriggers.Remove (trigger);
		}
		// METHOD end
	}
}