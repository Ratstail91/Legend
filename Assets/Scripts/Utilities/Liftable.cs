using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Liftable : MonoBehaviour {
	public bool isLifted = false;
	public Vector2 lastDirection = Vector2.zero; //set in Lifter, used by chicken

//	private Rigidbody2D rigidBody;
	private BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
//		rigidBody = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		boxCollider.enabled = !isLifted;
	}
}
