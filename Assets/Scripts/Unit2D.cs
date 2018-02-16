using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit2D : MonoBehaviour {

	public Transform target;
	public float speed = 1;
	public float turnDistance = 0.5f;

	Path2D path;

	void Start() {
		PathRequestManager.RequestPath (transform.position, target.transform.position, OnPathFound);
	}

	public void OnPathFound(Vector2[] waypoints, bool success) {
		if (success) {
			path = new Path2D(waypoints, transform.position, turnDistance);
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		while(true) {
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			path.DrawWithGizmos ();
		}
	}
}
