using System;
using System.Collections;
using UnityEngine;

//TODO: disable damage while attacking - 1 frame of immunity

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Destructable))]
[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Lifter))]
[RequireComponent(typeof(Rigidbody2D))]

public class Character : MonoBehaviour {
	//internal variables
	float speed;
	Vector2 deltaForce;
	Vector2 lastDirection;
	bool isMoving = false;
	bool isAttacking = false;

	//child objects
	GameObject swordDamager;

	//component references
	Animator animator;
	BoxCollider2D boxCollider;
	Destructable destructable;
	Durability durability;
	Lifter lifter;
	Rigidbody2D rigidBody;

	void Awake() {
		//get the components
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		destructable = GetComponent<Destructable> ();
		durability = GetComponent<Durability> ();
		lifter = GetComponent<Lifter> ();
		rigidBody = GetComponent<Rigidbody2D> ();

		//get the sword
		swordDamager = transform.GetChild(0).gameObject;

		//set up the internals
		speed = 1.0f;
		durability.maxHealthPoints = 12;
		durability.healthPoints = 12;
		destructable.invincibleWindow = 0.5f;
	}

	void Update() {
		//run each routine in order
		CheckInput ();
		Move ();
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
			lastDirection = rigidBody.velocity;
		}

		//Lifter script needs the direction for placement
		lifter.SetLastDirection(lastDirection);

		//if space pressed but not lifting or trying to lift, set attacking to true
		isAttacking = false;
		if (Input.GetKeyDown("space")) {
			isAttacking = true;
		}
	}

	void Move() {
		//determine how to move the character
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce (deltaForce * speed, ForceMode2D.Impulse);
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
		animator.SetBool ("isMoving", isMoving); //TODO: remove this from the animation system
		animator.SetBool ("isAttacking", isAttacking);
	}
}
