using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UDEngine;
using UDEngine.Interface;
using UDEngine.Internal;

namespace UDEngine.Internal {

	// Independent, NOT derived from Monobehavior
	/// <summary>
	/// 2D Spatial Hash. type <T> must supply ISpatialHashObject2D's interface requirements
	/// </summary>
	public class SpatialHash2D<T> where T : ISpatialHashObject2D {
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
		public SpatialHash2D(int cols, int rows, float sceneWidth, float sceneHeight, float minX, float minY) {
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
		}

		//Insert an Object to the bucket
		public void Insert(T obj) {
			List<int> cellIDs = GetBucketIDs (obj);
			foreach (int item in cellIDs) {
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

			List<int> bucketIDs = GetBucketIDs(obj);
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
		private void AddBucket(Vector2 vector, float width, List<int> bucketIDs) {
			//The supposed key (ID) to the cell that covers the vector position
			int cellPosition = (int) (
				(Mathf.Floor((vector.x - minX) / cellSizeX)) +
				(Mathf.Floor((vector.y - minY) / cellSizeY)) *
				width
			);
			// Add ONLY if not containing
			if (!bucketIDs.Contains (cellPosition)) {
				bucketIDs.Add (cellPosition);
			}
		}

		/// <summary>
		/// Get the Dictionary Keys of Buckets that may contain bullets that OVERLAPS obj
		/// </summary>
		/// <returns>The bucket I ds.</returns>
		/// <param name="obj">Object.</param>
		private List<int> GetBucketIDs(T obj) {
			List<int> bucketIDs = new List<int>();

			Vector2 min = 
				new Vector2(
					obj.GetPosition().x - (obj.GetRadius()),
					obj.GetPosition().y - (obj.GetRadius())
				);
			Vector2 max = 
				new Vector2(
					obj.GetPosition().x + (obj.GetRadius()),
					obj.GetPosition().y + (obj.GetRadius())
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
	}
}
