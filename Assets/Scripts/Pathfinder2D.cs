using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NodeGrid2D))]
public class Pathfinder2D : MonoBehaviour {
	NodeGrid2D nodeGrid;
	public Transform seeker, target;

	void Awake() {
		nodeGrid = GetComponent<NodeGrid2D> ();
	}

	void Update() {
		FindPath (new Vector2(seeker.position.x, seeker.position.y), new Vector2(target.position.x, target.position.y));
	}

	void FindPath(Vector2 startPos, Vector2 targetPos) {
		//start and end points
		Node2D startNode = nodeGrid.GetNode2DFromWorldPoint (startPos);
		Node2D targetNode = nodeGrid.GetNode2DFromWorldPoint (targetPos);

		//open and closed lists
		List<Node2D> openNodes = new List<Node2D> ();
		HashSet<Node2D> closedNodes = new HashSet<Node2D> ();

		//begin with the start node in the open list
		openNodes.Add (startNode);

		//find the open node with the lowest heuristic
		while(openNodes.Count > 0) {
			Node2D currentNode = openNodes [0];
			for (int i = 1; i < openNodes.Count; i++) {
				if (openNodes[i].fCost < currentNode.fCost || (openNodes[i].fCost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost)) {
					currentNode = openNodes [i];
				}
			}

			//move that node to the closed list
			openNodes.Remove (currentNode);
			closedNodes.Add (currentNode);

			//if found the target, return
			if (currentNode == targetNode) {
				RetracePath (startNode, targetNode);
				return;
			}

			foreach(Node2D neighbourNode in nodeGrid.GetNeighbours(currentNode)) {
				if (!neighbourNode.walkable || closedNodes.Contains(neighbourNode)) {
					continue;
				}

				int movementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbourNode);
				if (movementCostToNeighbour < neighbourNode.gCost || !openNodes.Contains(neighbourNode)) {
					neighbourNode.gCost = movementCostToNeighbour;
					neighbourNode.hCost = GetDistance (neighbourNode, targetNode);
					neighbourNode.parent = currentNode;

					if (!openNodes.Contains(neighbourNode)) {
						openNodes.Add (neighbourNode);
					}
				}
			}
		}
	}

	void RetracePath(Node2D startNode, Node2D endNode) {
		List<Node2D> path = new List<Node2D> ();
		Node2D currentNode = endNode;

		while(currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();
		nodeGrid.path = path;
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
