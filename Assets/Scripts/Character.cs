using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Lifter))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class Character : MonoBehaviour {
	//internal variables
	float speed;
	Vector2 deltaForce;
	Vector2 lastDirection;
	bool isMoving = false;
	float lastAttackTime = float.NegativeInfinity;
	float attackInterval = 0.3f;

	//child objects
	GameObject swordDamager;

	//component references
	Animator animator;
	BoxCollider2D boxCollider;
	Durability durability;
	Lifter lifter;
	Rigidbody2D rigidBody;
	SpriteRenderer spriteRenderer;

	void Awake() {
		//get the components
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		durability = GetComponent<Durability> ();
		lifter = GetComponent<Lifter> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();

		//get the sword
		swordDamager = transform.GetChild(0).gameObject;

		//set up the internals
		speed = 0.79f;
		durability.maxHealthPoints = 12;
		durability.healthPoints = 12;
		durability.invincibleWindow = 0.5f;

		//set callbacks
		Durability.callback onDmg = durability.onDamaged;
		durability.onDamaged = (int diff) => {
			if (onDmg != null) {
				onDmg(diff);
			}
			StartCoroutine(FlashColor(1, 0, 0, 0.1f));
		};

		Durability.callback onHld = durability.onHealed;
		durability.onHealed = (int diff) => {
			if (onHld != null) {
				onHld(diff);
			}
			StartCoroutine(FlashColor(0, 1, 0, 0.1f));
		};
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
		if (Input.GetKeyDown("space") && Time.time - lastAttackTime > attackInterval) {
			lastAttackTime = Time.time;
		}
	}

	void Move() {
		//determine how to move the character
		rigidBody.velocity = Vector2.zero;

		Vector2 impulse = deltaForce * speed;

		if (deltaForce.x != 0 && deltaForce.y != 0) {
			impulse *= 0.71f;
		}

		rigidBody.AddForce (impulse, ForceMode2D.Impulse);
	}

	void CalculateAttack() {
		//skip this if not attacking
		if (Time.time - lastAttackTime > attackInterval) {
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
		animator.SetBool ("isAttacking", Time.time - lastAttackTime <= attackInterval);
	}

	IEnumerator FlashColor(float r, float g, float b, float seconds) {
		spriteRenderer.color = new Color(r, g, b);
		yield return new WaitForSeconds (seconds);
		spriteRenderer.color = new Color(1, 1, 1);
	}

	//DEBUGGING
	public bool drawGizmos = true;
	void OnDrawGizmos() {
		if (!drawGizmos) {
			return;
		}
		if (swordDamager.active) {
			Gizmos.color = Color.red;
		} else {
			Gizmos.color = Color.cyan;
		}
		Gizmos.DrawCube (swordDamager.transform.position, swordDamager.GetComponent<BoxCollider2D> ().size);
	}
}
