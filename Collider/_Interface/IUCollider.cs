using UnityEngine;
using System.Collections;

namespace UDEngine.Components {
	/// <summary>
	/// Collider Interface
	/// To implement QuadTree, we need to satisfy IQuadTreeObject interface
	/// </summary>
	public interface IUCollider : ISpatialHashObject2D, IQuadTreeObject {
		Vector2 GetPosition();
		EColliderType GetColliderType();
	}
}
