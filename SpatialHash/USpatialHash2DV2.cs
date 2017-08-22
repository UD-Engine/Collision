using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

using UDEngine;
using UDEngine.Interface;
using UDEngine.Internal;

// Adapted from https://unionassets.com/blog/spatial-hashing-295
// Adapted from https://conkerjo.wordpress.com/2009/06/13/spatial-hashing-implementation-for-fast-2d-collisions/

namespace UDEngine.Internal {

	// Independent, NOT derived from Monobehavior
	/// <summary>
	/// 2D Spatial Hash. type <T> must supply ISpatialHashObject2D's interface requirements
	/// </summary>
	public class USpatialHash2DV2<T> where T : ISpatialHashObject2D {
		public float sceneWidth;
		public float sceneHeight;
		public float cellSizeX;
		public float cellSizeY;

		//2D Coordinates of the down left corner of 
		public float minX, minY;

		public int cols;
		public int rows;
		//Buckets: store actual bullets
		public Dictionary<int, List<T>> buckets;
		//Max bucket size, avoid calculated bucket out of range bug (see below)
		public int bucketSize;

		/// <summary>
		/// Initializes the <see cref="SpatialHash2D`1"/> Object.
		/// </summary>
		/// <param name="cols">Cols.</param>
		/// <param name="rows">Rows.</param>
		/// <param name="sceneWidth">Scene width.</param>
		/// <param name="sceneHeight">Scene height.</param>
		/// <param name="minX">Minimum x.</param>
		/// <param name="minY">Minimum y.</param>
		public USpatialHash2DV2(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
			this.sceneWidth = sceneWidth;
			this.sceneHeight = sceneHeight;
			this.cols = cols;
			this.rows = rows;

			this.minX = minX;
			this.minY = minY;

			this.cellSizeX = sceneWidth / cols;
			this.cellSizeY = sceneHeight / rows;

			this.buckets = new Dictionary<int, List<T>> (this.cols * this.rows);

			for (int i = 0; i < cols * rows; i++) {
				this.buckets.Add (i, new List<T> ());
			}
			this.bucketSize = buckets.Count;

			//this.bucketIDs = new HashSet<int> ();
		}

		//Insert an Object to the bucket
		public void Insert(T obj) {
			//HashSet<int> cellIDs = GetBucketIDs (obj);
			int[] cellIDs = GetBucketIDs (obj).ToArray(); // Array is faster to iterate

			for (int i = 0; i < cellIDs.Length; i++) {
				int item = cellIDs [i];
				//Avoid OutOfBounds Error (since obj may move OUTSIDE of the scene limits, generating unwanted "item" value, e.g. -1)
				if (item >= bucketSize || item < 0) {
					continue;
				}
				buckets[item].Add(obj);
			}
		}

		//Okay
		public List<T> GetNearby(T obj) {
			List<T> objects = new List<T>();

			HashSet<int> bucketIDs = GetBucketIDs(obj);
			foreach (int item in bucketIDs) {
				//Avoid OutOfBounds Error (since obj may move OUTSIDE of the scene limits, generating unwanted "item" value, e.g. -1)
				if (item >= bucketSize || item < 0) {
					continue;
				}
				objects.AddRange(buckets[item]);
			}
			return objects;
		}

		/// <summary>
		/// Clears the buckets. MUST be called once per frame!!!
		/// </summary>
		public void ClearBuckets() {
			//Clear every single bucket
			for (int i = 0; i < cols * rows; i++) {
				this.buckets[i].Clear();
			}
		}

		/// <summary>
		/// Add possible dictionary keys to bucketIDs
		/// </summary>
		/// <param name="vector">Vector.</param>
		/// <param name="width">Width.</param>
		/// <param name="bucketIDs">Bucket I ds.</param>
		private void AddBucket(Vector2 vector, float width, HashSet<int> bucketIDs) {
			//The supposed key (ID) to the cell that covers the vector position
			//			int cellPosition = (int) (
			//				(Mathf.Floor((vector.x - minX) / cellSizeX)) +
			//				(Mathf.Floor((vector.y - minY) / cellSizeY)) *
			//				width
			//			);
			int cellPosition = (int) (
				(_FastFloor((vector.x - minX) / cellSizeX)) +
				(_FastFloor((vector.y - minY) / cellSizeY)) *
				width
			);

			// Add ONLY if not containing
			//if (!bucketIDs.Contains (cellPosition)) { // Removed as it is not needed (HashSet ensures uniqueness)
			bucketIDs.Add (cellPosition);
			//}
		}


		//HashSet<int> bucketIDs;
		/// <summary>
		/// Get the Dictionary Keys of Buckets that may contain bullets that OVERLAPS obj
		/// </summary>
		/// <returns>The bucket I ds.</returns>
		/// <param name="obj">Object.</param>
		private HashSet<int> GetBucketIDs(T obj) {
			//			if (bucketIDs == null) {
			//				bucketIDs = new HashSet<int> ();
			//			} else {
			//				bucketIDs.Clear ();
			//			}
			HashSet<int> bucketIDs = new HashSet<int>();

			// This HashSet might NOT be necessary! after all, we only have 4 possible objects...

			// Caching to avoid repetitive bad operations
			Vector2 objPos = obj.GetPosition();
			float objRadius = obj.GetRadius ();

			Vector2 min = 
				new Vector2(
					objPos.x - objRadius,
					objPos.y - objRadius
				);
			Vector2 max = 
				new Vector2(
					objPos.x + objRadius,
					objPos.y + objRadius
				);

			//TopLeft
			AddBucket(min, cols, bucketIDs);
			//TopRight
			AddBucket(new Vector2(max.x, min.y), cols, bucketIDs);
			//BottomRight
			AddBucket(max, cols, bucketIDs);
			//BottomLeft
			AddBucket(new Vector2(min.x, max.y), cols, bucketIDs);

			return bucketIDs;    
		}



		// Better implementation of Floor, which boosts the floor performance greatly
		private const int _BIG_ENOUGH_INT = 16 * 1024;
		private const double _BIG_ENOUGH_FLOOR = _BIG_ENOUGH_INT + 0.0000;

		private static int _FastFloor(float f) {
			return (int)(f + _BIG_ENOUGH_FLOOR) - _BIG_ENOUGH_INT;
		}
	}
}
