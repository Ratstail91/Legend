using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid2D : MonoBehaviour {
	//public variables
	public LayerMask unwalkableMask;
	public Vector2 worldGridSize; //real size of the grid
	public float nodeRadius;

	//private members
	Node2D[,] grid;
	float nodeDiameter;
	int gridSizeX, gridSizeY; //the number of nodes stored on an axis

	void Start() {
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(worldGridSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(worldGridSize.y / nodeDiameter);
		CreateGrid ();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	public List<Node2D> path;
	void OnDrawGizmos() {
		Gizmos.DrawWireCube (transform.position, new Vector3 (worldGridSize.x, worldGridSize.y, 1));
		if (grid != null) {
			foreach(Node2D n in grid) {
				Gizmos.color = (n.walkable ? Color.white : Color.red);
				if (path != null && path.Contains(n)) {
					Gizmos.color = Color.black;
				}
				Gizmos.DrawCube (new Vector3(n.worldPosition.x, n.worldPosition.y, 1), Vector3.one * (nodeDiameter - 0.1f));
			}
		}
	}

	public Node2D GetNode2DFromWorldPoint(Vector2 worldPosition) {
		float percentX = (worldPosition.x + worldGridSize.x / 2) / worldGridSize.x;
		float percentY = (worldPosition.y + worldGridSize.y / 2) / worldGridSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid [x, y];
	}

	void CreateGrid() {
		grid = new Node2D[gridSizeX, gridSizeY];
		Vector2 worldBottomLeft;
		worldBottomLeft.x = transform.position.x - (Vector2.right * worldGridSize.x / 2).x;
		worldBottomLeft.y = transform.position.y - (Vector2.up * worldGridSize.y / 2).y;

		for (int i = 0; i < gridSizeX; i++) {
			for (int j = 0; j < gridSizeY; j++) {
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (i * nodeDiameter + nodeRadius) + Vector2.up * (j * nodeDiameter + nodeRadius);
				bool walkable = !(Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask));

				grid [i, j] = new Node2D (walkable, worldPoint, i, j);
			}
		}
	}

	public List<Node2D> GetNeighbours(Node2D node) {
		List<Node2D> neighbours = new List<Node2D> ();

		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (i == 0 && j == 0) {
					continue;
				}

				int checkX = node.gridX + i;
				int checkY = node.gridY + j;

				if (checkX < 0 || checkX >= gridSizeX || checkY < 0 || checkY >= gridSizeY) {
					continue;
				}

				neighbours.Add (grid [checkX, checkY]);
			}
		}
		return neighbours;
	}
}
