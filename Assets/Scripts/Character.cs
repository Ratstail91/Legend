using System;
using System.Collections;
using UnityEngine;

//TODO: disable damage while attacking - 1 frame of immunity

public class Character : MonoBehaviour {
	//public variables
	public GameObject swordDamager;

	//internal variables
	private float speed;
	private Vector2 deltaForce;
	private Vector2 lastDirection;
	private bool isMoving = false;
	private bool isAttacking = false;

	//component references
	private Rigidbody2D rigidBody;
	private Animator animator;
	private BoxCollider2D boxCollider;
	private Lifter lifter;

	void Start() {
		//get the components
		rigidBody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		lifter = GetComponent<Lifter> ();

		//set up the internals
		speed = 1.0f;
		rigidBody.gravityScale = 0;
		rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void Update() {
		//run each routine in order
		CheckInput ();
		CalculateMovement (deltaForce * speed);
		CalculateAttack ();
		SendAnimationInfo ();
	}

	void CheckInput() {
		//determine the input from the player
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		
		deltaForce = new Vector2 (horizontal, vertical);

		//calculate last direction & isMoving
		isMoving = false;
		if (deltaForce != Vector2.zero) {
			isMoving = true;
			if (!boxCollider.IsTouchingLayers(Physics.AllLayers)) { //TODO: learn more about this
				lastDirection = rigidBody.velocity;
			}
		}

		//Lifter script needs the direction for placement
		lifter.SetLastDirection(lastDirection);

		//if space pressed but not lifting or trying to lift, set attacking to true
		isAttacking = false;
		if (Input.GetKeyDown("space") && !lifter.GetIsLifting() && lifter.GetLiftableObject() == null) {
			isAttacking = true;
		}
	}

	void CalculateMovement(Vector2 force) {
		//determine how to move the character
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce (force, ForceMode2D.Impulse);
	}

	void CalculateAttack() {
		//skip this if not attacking
		if (!isAttacking) {
			swordDamager.SetActive (false);
			return;
		}
		swordDamager.SetActive (true);

		//attack horizontal or vertical
		Vector3 newPos = transform.position;
		if (Math.Abs(lastDirection.x) > Math.Abs(lastDirection.y)) {
			//extra conditionals for not-moving situation
			newPos.x += 0.17f * (lastDirection.x > 0 ? 1 : -1);
		}
		else {
			newPos.y += 0.17f * (lastDirection.y > 0 ? 1 : -1);
		}

		swordDamager.transform.position = newPos;
	}

	void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", rigidBody.velocity.x);
		animator.SetFloat ("ySpeed", rigidBody.velocity.y);
		animator.SetFloat ("lastXSpeed", lastDirection.x);
		animator.SetFloat ("lastYSpeed", lastDirection.y);
		animator.SetBool ("isMoving", isMoving);
		animator.SetBool ("isAttacking", isAttacking);
	}
}
