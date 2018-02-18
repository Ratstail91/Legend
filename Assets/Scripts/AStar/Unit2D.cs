using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit2D : MonoBehaviour {
	//NOTE: Unit2D now generally handles movement

	//public variables
	public float speed;
	public Vector2 movement;

	//private variables
	Vector2[] path;
	int targetIndex;

	//components
	Rigidbody2D rigidBody;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D> ();
	}

	public void FollowPathToPoint(Vector2 point) {
		PathRequestManager.RequestPath (transform.position, point, OnPathFound);
	}

	public void StopFollowingPath() {
		StopCoroutine ("FollowPath");
	}

	public void Move() {
		//make the actual movement
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce (movement, ForceMode2D.Impulse);
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
			Vector2 currentPosition = new Vector2 (transform.position.x, transform.position.y);
			if (Vector2.Distance(currentPosition, currentWaypoint) < 0.01f) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}

			//don't delete these when upgrading
			Vector2 heading = currentWaypoint - new Vector2 (transform.position.x, transform.position.y);
			movement = heading / heading.magnitude * speed; //normalized * speed

			Move ();

			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (new Vector3 (path [i].x, path [i].y, -1), Vector3.one * 0.1f);

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
