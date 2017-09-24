using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UDEngine.Interface;
using UDEngine.Internal;
using UDEngine.Core;
using UDEngine.Core.Actor;
using UDEngine.Core.Collision;

using KSM;

namespace UDEngine.Core.Collision {
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

		#region UNITYFUNC
		void Awake() {
			if (_bulletColliders == null) {
				_bulletColliders = new LinkedList<UBulletCollider> ();
			}
			if (_targetColliders == null) {
				_targetColliders = new List<UTargetCollider> ();
			}
			if (_bulletRegionTriggers == null) {
				_bulletRegionTriggers = new List<IBulletRegionTrigger> ();
			}
			if (_targetColliderPositions == null) {
				_targetColliderPositions = new List<Vector3> ();
			}
		}

		void Start() {
		}

		void Update () {
			// If should NOT update, immediately return
			if (!shouldUpdate) {
				return;
			}
			//Debug.Log (_bulletColliders.Count.ToString () + "..." + _targetColliders.Count.ToString ());


			// Create a simple minimal structure to avoid meaningless repetitive target.IsEnabled() check
			List<UTargetCollider> enabledTargets = new List<UTargetCollider> ();
			List<Vector3> enabledTargetPositions = new List<Vector3> (); // caching enabled targets' positions to avoid calling transform.position twice

			int targetCollidersLen = _targetColliders.Count;
			for (int i = 0; i < targetCollidersLen; i++) {
				UTargetCollider target = _targetColliders [i];
				// Caching target position
				Vector3 targetPos = target.trans.position;
				_targetColliderPositions [i] = targetPos;
				if (target.IsEnabled()) {
					// ALWAYS TOGETHER
					enabledTargets.Add (target);
					enabledTargetPositions.Add (targetPos);
				}
				// The reason to add in both _targetColliderPositions and enabledTargetPositions
				// is to allow possibly ANY even inactive target be aimed for better future effects
			}

			/*
			foreach (UTargetCollider target in _targetColliders) {
				if (target.IsEnabled()) {
					enabledTargets.Add (target);
				}
			}
			*/

			int enabledTargetsLen = enabledTargets.Count; // Save for better performance
			bool[] targetIsCollidedMap = new bool[enabledTargetsLen]; // default(bool) in C# is false

			LinkedListNode<UBulletCollider> ubcNode = _bulletColliders.First;

			while (ubcNode != null) {
				UBulletCollider ubc = ubcNode.Value;
				LinkedListNode<UBulletCollider> nextNode = ubcNode.Next;

				// Caching position will be VERY useful in later steps, including boundary collider check and target collision check
				Vector3 ubcPosition = ubc.trans.position;
				UBulletActor ubcActor = ubc.actor;

				// boundary check registers
				if (!IsInBoundary (ubcPosition)) {
					ubcActor.InvokeBoundaryCallbacks (); // Invoke boundary callbacks
				}

				// region trigger check
				int regionTriggerLen = _bulletRegionTriggers.Count;
				for (int i = 0; i < regionTriggerLen; i++) {
					IBulletRegionTrigger trigger = _bulletRegionTriggers [i];
					// Invoke regional trigger collider callbacks on collision
					if (trigger.IsTriggerable (ubcPosition)) { // Use cached ubcPosition instead of ubc to boost performance
						trigger.InvokeTriggerCallbacks (ubc);
					}
				}

				if (ubc.IsRecyclable ()) {
					ubc.SetRecyclable (true); // In case that the enabled is still true...

					// TODO: call clearup func here
					// TODO: I decide NOT to put Recycle() here. 
					// CollisionMonitor should be mostly oblivious of any sort of tasks that is not focused on Collision itself
					// Thus the ONLY task for Recycle() done that would be handled here is the Remove from LinkedList task
					// Other clearups will be done by inserting Recycle() into AddBoundaryCallback();

					_bulletColliders.Remove (ubcNode); // Remove Node from tracking

					// FIX: avoid calling the default callback of the should be recycled bullet
					ubcNode = nextNode; // Move to the next node
					continue;
				}

				// DEFAULT CALLBACK INVOCATION
				ubcActor.InvokeDefaultCallbacks (); // Don't invoke before checking recycling


				bool hasFastDetect = false;
				for (int i = 0; i < enabledTargetsLen; i++) {
					UTargetCollider target = enabledTargets[i];

					// This exposition of the underlying collision logic is ugly, but... it is for performance's sake...
					// Also, here the enabledTargets have already their positions cached, so we can avoid calling transform.position again
					if (KVector3.DistanceXY(enabledTargetPositions[i], ubcPosition) < ubc.GetRadius() + target.GetRadius()) {
					//if (KVector3.DistanceXY(target.trans.position, ubcPosition) < ubc.GetRadius() + target.GetRadius()) {
						// If fast detect flag is set, then immediately drop out 
						// and NOT invoking any bullet collision handling events
						targetIsCollidedMap[i] = true;

						if (target.IsFastDetect ()) {
							hasFastDetect = true;
							break;
						}

						ubcActor.InvokeCollisionCallbacks ();
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
				} else {
					// ADD: what target may need to do if NOT collided ever
					enabledTargets [i].InvokeNoHitCallbacks ();
				}
			}

		}


		void OnDisable() { // On disable, clear all records
			// Originally, I want to add some DisableCallbacks.
			// However, this actually add some unneeded pressure of callbacks on monitor
			// This would be further 
			ClearBulletColliders ();
			ClearTargetColliders ();
			ClearBulletRegionTriggers ();
		}
		#endregion

		#region PROP
		// Should update, global update lock
		public bool shouldUpdate;

		// Monitor Boundary
		public float boundXMin;
		public float boundWidth;
		public float boundYMin;
		public float boundHeight;



		// To make removal faster, changed to LinkedList
		private LinkedList<UBulletCollider> _bulletColliders; // This is still private, as it will be TERRIBLE to display this

		// remove for targetColliders is RARE, thus we can use simple List (with .RemoveAt(index) of O(n))
		private List<UTargetCollider> _targetColliders;

		private List<IBulletRegionTrigger> _bulletRegionTriggers;

		// This stores current target collider position. It updates every update
		// It is used to cache position and provide for any target-aiming bullet attacks
		private List<Vector3> _targetColliderPositions; 

		#endregion

		#region METHOD
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

		// Get target positions (cached) so that later bullet aiming functions could be used
		// Due to the caching nature, it can be much faster
		public Vector3 GetTargetPosition(int index = 0) {
			if (index > _targetColliderPositions.Count) {
				UDebug.Warning ("no matching target found, using (0, 0, 0) as fallback");
				return new Vector3 (0, 0, 0);
			}
			return _targetColliderPositions [index];
		}


		// UPDATE: remove *s version as they are not so useful, and could be done with for-loop instead

		public void AddBulletCollider(UBulletCollider collider) {
			//_bulletColliders.Add (collider);
			if (_bulletColliders == null) {
				_bulletColliders = new LinkedList<UBulletCollider> ();
			}
			_bulletColliders.AddLast (collider);
		}

		public void AddTargetCollider(UTargetCollider collider) {
			/*
			if (_targetColliders == null) {
				_targetColliders = new List<UTargetCollider> ();
			}
			if (_targetColliderPositions == null) {
				_targetColliderPositions = new List<Vector3> ();
			}
			*/
			// ALWAYS TOGETHER
			_targetColliders.Add (collider);
			_targetColliderPositions.Add (collider.trans.position); // updating cached positions
		}

		public void AddBulletRegionTrigger(IBulletRegionTrigger trigger) {
			if (_bulletRegionTriggers == null) {
				_bulletRegionTriggers = new List<IBulletRegionTrigger> ();
			}
			_bulletRegionTriggers.Add (trigger);
		}

		public void ClearBulletColliders() {
			_bulletColliders.Clear ();
		}

		public void ClearTargetColliders() {
			// ALWAYS TOGETHER
			_targetColliders.Clear ();
			_targetColliderPositions.Clear ();
		}

		public void ClearBulletRegionTriggers() {
			_bulletRegionTriggers.Clear ();
		}

		// These remove methods are EXTREMELY slow (especially the RemoveBulletCollider()); AVOID them
		public void RemoveBulletCollider(UBulletCollider collider) {
			_bulletColliders.Remove (collider);
		}

		public void RemoveTargetCollider(UTargetCollider collider) {
			// This is SLOW (O(n) * 3). But removal of target colliders is RARE compared to bullet colliders
			int index = _targetColliders.IndexOf(collider);
			if (index > 0) { // found
				// ALWAYS TOGETHER
				_targetColliders.RemoveAt(index);
				_targetColliderPositions.RemoveAt (index); // remove cached position slot
			}
			//_targetColliders.Remove (collider);
		}

		public void RemoveBulletRegionTrigger(IBulletRegionTrigger trigger) {
			_bulletRegionTriggers.Remove (trigger);
		}

		public void SetUpdate(bool condition = true) {
			shouldUpdate = condition;
		}
		#endregion
	}
}
