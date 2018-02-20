using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid2D : MonoBehaviour {
	//public variables
	public LayerMask unwalkableMask;
	public Vector2 worldGridSize; //real size of the grid
	public float nodeRadius;
	public int obsticleProximityPenalty;
	public TerrainType[] walkableRegions;
	Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int> (); //optimization

	//private members
	LayerMask walkableMask;
	Node2D[,] grid;
	float nodeDiameter;
	int gridSizeX, gridSizeY; //the number of nodes stored on an axis

	int minPenalty = int.MaxValue;
	int maxPenalty = int.MinValue;

	void Awake() {
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(worldGridSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(worldGridSize.y / nodeDiameter);

		foreach(TerrainType region in walkableRegions) {
			walkableMask.value |= region.terrainMask.value;
			walkableRegionsDictionary.Add ((int)Mathf.Log(region.terrainMask.value, 2), region.penalty);
		}

		CreateGrid ();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	public bool displayGridGizmos = true;
	void OnDrawGizmos() {
		Gizmos.DrawWireCube (transform.position, new Vector3 (worldGridSize.x, worldGridSize.y, 1));
		if (grid != null && displayGridGizmos) {
			foreach(Node2D n in grid) {
				Gizmos.color = Color.Lerp (Color.white, Color.black, Mathf.InverseLerp (minPenalty, maxPenalty, n.penalty));
				Gizmos.color = (n.walkable ? Gizmos.color : Color.red);
				Gizmos.DrawCube (new Vector3(n.worldPosition.x, n.worldPosition.y, 0), Vector3.one * (nodeDiameter * 0.5f));
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

				int movementPenalty = 0;

				RaycastHit2D hit = Physics2D.Raycast (worldPoint, Vector2.zero, 0, walkableMask);
				if (hit.collider != null) {
					walkableRegionsDictionary.TryGetValue (hit.collider.gameObject.layer, out movementPenalty);
				}

				if (!walkable) {
					movementPenalty += obsticleProximityPenalty;
				}

				grid [i, j] = new Node2D (walkable, worldPoint, i, j, movementPenalty);
			}
		}

		BlurPenaltyMap (3);
	}

	void BlurPenaltyMap(int blurSize) {
		int kernelSize = blurSize * 2 + 1;
		int kernelExtents = (kernelSize - 1) / 2;

		int[,] penaltiesHorizontalPass = new int[gridSizeX,gridSizeY];
		int[,] penaltiesVerticalPass = new int[gridSizeX,gridSizeY];

		//horizontal pass
		for (int y = 0; y < gridSizeY; y++) {
			//row 0
			for (int x = -kernelExtents; x <= kernelExtents; x++) {
				int sampleX = Mathf.Clamp(x, 0, kernelExtents);
				penaltiesHorizontalPass[0,y] += grid[sampleX, y].penalty;
			}

			for (int x = 1; x < gridSizeX; x++) {
				int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
				int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX -1);

				penaltiesHorizontalPass[x,y] = penaltiesHorizontalPass[x-1, y] - grid[removeIndex,y].penalty + grid[addIndex,y].penalty;
			}
		}

		//vertical pass
		for (int x = 0; x < gridSizeX; x++) {
			//row 0
			for (int y = -kernelExtents; y <= kernelExtents; y++) {
				int sampleY = Mathf.Clamp(x, 0, kernelExtents);
				penaltiesVerticalPass[x,0] += penaltiesHorizontalPass[x, sampleY];
			}

			int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass [x, 0] / (kernelSize * kernelSize));
			grid [x, 0].penalty = blurredPenalty;

			for (int y = 1; y < gridSizeY; y++) {
				int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
				int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY -1);

				penaltiesVerticalPass[x,y] = penaltiesVerticalPass[x, y-1] - penaltiesHorizontalPass[x,removeIndex] + penaltiesHorizontalPass[x,addIndex];
				blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass [x, y] / (kernelSize * kernelSize));
				grid [x, y].penalty = blurredPenalty;

				//gizmo stuff
				if (blurredPenalty > maxPenalty) {
					maxPenalty = blurredPenalty;
				}
				if (blurredPenalty < minPenalty) {
					minPenalty = blurredPenalty;
				}
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

	[System.Serializable]
	public class TerrainType {
		public LayerMask terrainMask;
		public int penalty;
	}
}
