using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NodeGrid2D))]
[RequireComponent(typeof(PathRequestManager))]
public class Pathfinder2D : MonoBehaviour {
	NodeGrid2D nodeGrid;
	PathRequestManager pathReqeustManager;

	void Awake() {
		nodeGrid = GetComponent<NodeGrid2D> ();
		pathReqeustManager = GetComponent<PathRequestManager> ();
	}

	public void StartFindPath(Vector2 startPos, Vector2 targetPos) {
		StartCoroutine (FindPath(startPos, targetPos));
	}

	IEnumerator FindPath(Vector2 startPos, Vector2 targetPos) {
		//"return" value
		Vector2[] waypoints = new Vector2[0];
		bool success = false;

		//start and end points
		Node2D startNode = nodeGrid.GetNode2DFromWorldPoint (startPos);
		Node2D targetNode = nodeGrid.GetNode2DFromWorldPoint (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			//open and closed lists
			Heap<Node2D> openNodes = new Heap<Node2D> (nodeGrid.MaxSize);
			HashSet<Node2D> closedNodes = new HashSet<Node2D> ();

			//begin with the start node in the open list
			openNodes.Add (startNode);

			//find the open node with the lowest heuristic
			while (openNodes.Count > 0) {
				Node2D currentNode = openNodes.RemoveFirst ();
				closedNodes.Add (currentNode);

				//if found the target, return
				if (currentNode == targetNode) {
					success = true;
					break;
				}

				foreach (Node2D neighbourNode in nodeGrid.GetNeighbours(currentNode)) {
					if (!neighbourNode.walkable || closedNodes.Contains (neighbourNode)) {
						continue;
					}

					int movementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbourNode) + neighbourNode.penalty;
					if (movementCostToNeighbour < neighbourNode.gCost || !openNodes.Contains (neighbourNode)) {
						neighbourNode.gCost = movementCostToNeighbour;
						neighbourNode.hCost = GetDistance (neighbourNode, targetNode);
						neighbourNode.parent = currentNode;

						if (!openNodes.Contains (neighbourNode)) {
							openNodes.Add (neighbourNode);
						} else {
							openNodes.UpdateItem (neighbourNode);
						}
					}
				}
			}
		}
		yield return null;
		if (success) {
			waypoints = RetracePath (startNode, targetNode);
		}
		pathReqeustManager.FinishedProcessingPath (waypoints, success);
	}

	Vector2[] RetracePath(Node2D startNode, Node2D endNode) {
		List<Node2D> path = new List<Node2D> ();
		Node2D currentNode = endNode;

		while(currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Add (startNode);
		Vector2[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;
	}

	Vector2[] SimplifyPath(List<Node2D> path) {
		List<Vector2> waypoints = new List<Vector2> ();
		Vector2 directionOld = Vector2.zero;

		//youtube comment fix
		if (path.Count == 1) {
			waypoints.Add (path [0].worldPosition);
			return waypoints.ToArray ();
		}

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add (path [i-1].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray ();
	}

	int GetDistance(Node2D nodeA, Node2D nodeB) {
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);
		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		}
		else {
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
}
