using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class Character : MonoBehaviour {
	//public variables
	private float speed;
	private Vector2 deltaForce;

	//internal variables
	private Vector2 lastDirection;
	private bool isMoving = false;

	//component references
	private Rigidbody2D rigidBody;
	private Animator animator;
	private BoxCollider2D boxCollider;

	void Start() {
		//get the components
		rigidBody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();

		//set up the internals
		speed = 1.0f;
		rigidBody.gravityScale = 0;
		rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void Update() {
		//run each routine in order
		CheckInput ();
		CalculateMovement (deltaForce * speed);
		SendAnimationInfo ();
	}

	void CheckInput() {
		//determine the input from the player
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		
		deltaForce = new Vector2 (horizontal, vertical);

		//pass deltaForce to the lifter script
		GetComponent<Lifter>().SetDeltaForce(deltaForce);
		
		isMoving = false;
		if (deltaForce != Vector2.zero) {
			isMoving = true;
			if (!boxCollider.IsTouchingLayers(Physics.AllLayers)) { //TODO: learn more about this
				lastDirection = rigidBody.velocity;
			}
		}
	}

	void CalculateMovement(Vector2 force) {
		//determine how to move the character
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce (force, ForceMode2D.Impulse);
	}

	void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", rigidBody.velocity.x);
		animator.SetFloat ("ySpeed", rigidBody.velocity.y);
		animator.SetFloat ("lastXSpeed", lastDirection.x);
		animator.SetFloat ("lastYSpeed", lastDirection.y);
		animator.SetBool ("isMoving", isMoving);
//		animator.SetBool ("isAttacking", isAttacking); //TODO
	}
}
