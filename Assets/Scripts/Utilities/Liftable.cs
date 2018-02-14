using UnityEngine;

public class Liftable : MonoBehaviour {
	public bool isLifted = false;

	private Rigidbody2D rigidBody;
	private BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		boxCollider.enabled = !isLifted;
		if (!isLifted) {
			rigidBody.velocity = Vector2.zero;
		}
	}
}
