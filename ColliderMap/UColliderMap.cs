using UnityEngine;
using System.Collections;
using UDEngine.Internal;

/// <summary>
/// Map that stores all the instances of active colliders
/// Implemented internally using Quadtree
/// </summary>
namespace UDEngine.Components {
	public class UColliderMap {
		private static int _colliderTreeMapMaxObjectCount = 4;
		public static void SetColliderTreeMapMaxObjectCount(int num) {
			if (num <= 0) {
				UDebug.Error ("max object count cannot be negative or 0");
			} else {
				_colliderTreeMapMaxObjectCount = num;
			}
		}

		public UColliderMap(Rect mapBound) {
			_colliderTreeMap = new QuadTree<IUCollider> (_colliderTreeMapMaxObjectCount, mapBound);
		}

		QuadTree<IUCollider> _colliderTreeMap;


	}
}
