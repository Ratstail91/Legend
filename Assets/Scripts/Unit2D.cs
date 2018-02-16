using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit2D : MonoBehaviour {

	public Transform target;
	float speed = 1;
	Vector2[] path;
	int targetIndex;

	void Start() {
		PathRequestManager.RequestPath (transform.position, target.transform.position, OnPathFound);
	}

	public void OnPathFound(Vector2[] newPath, bool success) {
		if (success) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		targetIndex = 0;
		Vector2 currentWaypoint = path [0];
		while(true) {
			if (new Vector2(transform.position.x, transform.position.y) == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}
			transform.position = Vector2.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (new Vector3 (path [i].x, path [i].y, -1), Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, new Vector3 (path [i].x, path [i].y, -1));
				}
				else {
					Gizmos.DrawLine (new Vector3 (path [i - 1].x, path [i - 1].y, -1), new Vector3 (path [i].x, path [i].y, -1));
				}
			}
		}
	}
}
