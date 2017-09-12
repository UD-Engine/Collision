using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UDEngine;
using UDEngine.Interface;

namespace UDEngine.Interface {
	/// <summary>
	/// Interface for hashable object in SpatialHash2D
	/// </summary>
	public interface ISpatialHashObject2D {
		Vector2 GetPosition();
		float GetRadius();
	}
}
