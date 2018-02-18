using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder2D))]
public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	Pathfinder2D pathfinder;

	bool isProcessing = false;

	void Awake() {
		instance = this;
		pathfinder = GetComponent<Pathfinder2D> ();
	}

	public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback) {
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}

	void TryProcessNext() {
		if (!isProcessing && pathRequestQueue.Count	> 0) {
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessing = true;
			pathfinder.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector2[] path, bool success) {
		currentPathRequest.callback (path, success);
		isProcessing = false;
		TryProcessNext ();
	}

	struct PathRequest {
		public Vector2 pathStart;
		public Vector2 pathEnd;
		public Action<Vector2[], bool> callback;

		public PathRequest(Vector2 _pathStart, Vector2 _pathEnd, Action<Vector2[], bool> _callback) {
			pathStart = _pathStart;
			pathEnd = _pathEnd;
			callback = _callback;
		}
	}
}
