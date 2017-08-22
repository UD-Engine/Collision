using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

using UDEngine;
using UDEngine.Interface;
using UDEngine.Internal;

// Adapted from https://unionassets.com/blog/spatial-hashing-295

namespace UDEngine.Internal {

	public class USpatialHash2DPlus<T> where T : ISpatialHashObject2D {

		private Dictionary<int, List<T>> dict;
		private Dictionary<T, int> objects;
		private int cellSize;


		public float sceneWidth;
		public float sceneHeight;
		public float cellSizeX;
		public float cellSizeY;
		//2D Coordinates of the down left corner of 
		public float minX, minY;
		public int cols;
		public int rows;
		public USpatialHash2DPlus(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
			this.sceneWidth = sceneWidth;
			this.sceneHeight = sceneHeight;
			this.cols = cols;
			this.rows = rows;

			this.minX = minX;
			this.minY = minY;

			this.cellSizeX = sceneWidth / cols;
			this.cellSizeY = sceneHeight / rows;

			dict = new Dictionary<int, List<T>>();
			objects = new Dictionary<T, int>();
		}

		public void Insert(T obj) {
			var key = Key(obj.GetPosition());
			if (dict.ContainsKey(key)) {
				dict[key].Add(obj);
			}
			else {
				dict[key] = new List<T> { obj };
			}
			objects[obj] = key;
		}

//		public void UpdatePosition(Vector3 vector, T obj) {
//			if (objects.ContainsKey(obj)) {
//				dict[objects[obj]].Remove(obj);
//			}
//			Insert(vector, obj);
//		}

		public List<T> GetNearby(Vector2 vector) {
			var key = Key(vector);
			return dict.ContainsKey(key) ? dict[key] : new List<T>();
		}

		public List<T> GetNearby(T obj) {
			var key = Key(obj.GetPosition());
			return dict.ContainsKey(key) ? dict[key] : new List<T>();
		}

		public bool ContainsKey(Vector2 vector) {
			return dict.ContainsKey(Key(vector));
		}

		public void ClearBuckets() {
			int[] keys = dict.Keys.ToArray();
			for (var i = 0; i < keys.Length; i++)
				dict[keys[i]].Clear();
			objects.Clear();
		}

		public void Reset() {
			dict.Clear();
			objects.Clear();
		}

		private const int BIG_ENOUGH_INT = 16 * 1024;
		private const double BIG_ENOUGH_FLOOR = BIG_ENOUGH_INT + 0.0000;

		private static int FastFloor(float f) {
			return (int)(f + BIG_ENOUGH_FLOOR) - BIG_ENOUGH_INT;
		}

		private int Key(Vector2 v) {
			return ((FastFloor(v.x / cellSizeX) * 73856093) ^
				(FastFloor(v.y / cellSizeY) * 19349663));
		}
	}
}
