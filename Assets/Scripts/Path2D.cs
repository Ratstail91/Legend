using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path2D {
	public readonly Vector2[] lookPoints;
	public readonly Line2D[] turnBoundries;
	public readonly int finishLineIndex;

	public Path2D(Vector2[] waypoints, Vector2 startPos, float turnDistance) {
		lookPoints = waypoints;
		turnBoundries = new Line2D[lookPoints.Length];
		finishLineIndex = turnBoundries.Length - 1;

		Vector2 previousPoint = startPos;
		for (int i = 0; i < lookPoints.Length; i++) {
			Vector2 currentPoint = lookPoints [i];
			Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
			Vector2 turnBoundryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDistance;
			turnBoundries [i] = new Line2D (turnBoundryPoint, previousPoint - dirToCurrentPoint * turnDistance);
			previousPoint = turnBoundryPoint;
		}
	}

	public void DrawWithGizmos() {
		Gizmos.color = Color.black;
		foreach(Vector2 p in lookPoints) {
			Gizmos.DrawCube (new Vector3 (p.x, p.y, -2), Vector3.one);
		}

		Gizmos.color = Color.white;
		foreach(Line2D l in turnBoundries) {
			l.DrawWithGizmos (5);
		}
	}
}
