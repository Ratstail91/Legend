using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Lifter : MonoBehaviour {
	//the private variables
	private bool isLifting = false;

	//the reference to the liftable object
	private GameObject liftableObject;

	//references to the components
	private Rigidbody2D rigidBody;

	void Start () {
		//get the components
		rigidBody = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		//run each routine in order
		CheckInput ("space");
		CalculateMovement ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		//ignore this if already lifting something
		if (isLifting) {
			return;
		}

		//find the liftable object
		if (collision.gameObject.tag == "Liftable") {
			liftableObject = collision.gameObject;
		}
	}

	void OnCollisionExit2D(Collision2D collision) {
		//forget the liftable object
		if (collision.gameObject == liftableObject) {
			if (!liftableObject.GetComponent<Liftable>().isLifted || !isLifting) {
				isLifting = false;
				liftableObject = null;
			}
		}
	}

	void CheckInput(string keyDown) {
		//trying to put down an object
		if (Input.GetKeyDown(keyDown) && isLifting) {
			isLifting = false;
			liftableObject.GetComponent<Liftable> ().isLifted = false;

			//correct placement of the liftable object
			Vector2 placeForce = GetComponent<Character>().deltaForce * 0.1f; //hack into the character script; TODO: more generic please!
			if (placeForce == Vector2.zero) {
				//default to placing south of the character
				placeForce.y = -0.1f;
			}
			//place in front of the lifter
			liftableObject.GetComponent<Rigidbody2D> ().position = rigidBody.position + placeForce;

			//forget the object
			liftableObject = null;
		}

		//trying to pick up object
		if (Input.GetKeyDown(keyDown) && liftableObject != null) {
			isLifting = true;
			liftableObject.GetComponent<Liftable> ().isLifted = true;
		}
	}

	void CalculateMovement() {
		//moving the lifted object
		if (liftableObject != null && isLifting && liftableObject.GetComponent<Liftable>().isLifted) {
			Vector2 carryPos = GetComponent<Rigidbody2D> ().position;
			carryPos.y += 0.2f; //carry on the character's head
			liftableObject.GetComponent<Rigidbody2D> ().position = carryPos;
		}
	}

	public bool GetIsLifting() {
		return isLifting;
	}

	public GameObject GetLiftableObject() {
		return liftableObject;
	}
}
