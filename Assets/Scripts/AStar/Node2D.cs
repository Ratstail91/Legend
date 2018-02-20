using UnityEngine;

public class Node2D : IHeapItem<Node2D> {
	public bool walkable;
	public Vector2 worldPosition;
	public int gridX;
	public int gridY;
	public int penalty;

	public int gCost;
	public int hCost;
	public int fCost {
		get {
			return gCost + hCost;
		}
	}
	public Node2D parent;

	private int index;

	public Node2D(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY, int _penalty) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		penalty = _penalty;
	}

	//interface stuff
	public int heapIndex {
		get { return index; }
		set { index = value; }
	}

	public int CompareTo(Node2D other) {
		int compare = fCost.CompareTo (other.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo (other.hCost);
		}
		return -compare;
	}
}
