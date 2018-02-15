using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Destructable))]
[RequireComponent(typeof(Durability))]
public class Fox : MonoBehaviour {
	//public structures
	public enum Behaviour {
		NORMAL,
		HUNTING,
		FIGHTING
	};
	private RandomEngine randomEngine;

	//private members
	private float lastTime;
	private float actionTime;
	private Vector2 movement;
	private float speed;
	private Vector2 lastDirection;
	private Behaviour behaviour;
	private bool isAttacking = false;

	//child members
	private GameObject biteDamager;

	//components
	Animator animator;
	Rigidbody2D rigidBody;
	BoxCollider2D boxCollider;
	Durability durability;

	void Start() {
		//get components
		animator = GetComponent<Animator> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		durability = GetComponent<Durability> ();
		randomEngine = new RandomEngine ();

		//get the biter
		biteDamager = transform.GetChild(0).gameObject;

		//internal stuff
		lastTime = Time.time;
		behaviour = Behaviour.NORMAL;
		durability.maxHealthPoints = 3;
		durability.healthPoints = 3;
	}

	void Update() {
		HandleBehaviour ();

		//if time interval and normal behaviour
		if (lastTime + actionTime < Time.time && behaviour == Behaviour.NORMAL) {
			RandomizeMovement (speed);
			lastTime = Time.time;
		}

		Move ();
		CalculateAttack ();
		UpdateBoxCollider ();

		SendAnimationInfo ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
			//TODO: fight
		}
	}

	void HandleBehaviour() {
		switch(behaviour) {
		case Behaviour.NORMAL:
			actionTime = 2.5f;
			speed = 0.2f;
			break;
		case Behaviour.HUNTING:
			//TODO
			break;
		case Behaviour.FIGHTING:
			//TODO
			break;
		}
	}

	void RandomizeMovement(float speed) {
		Vector2 deltaForce = new Vector2 (0, 0);

		int direction = (int)randomEngine.Rand(4.0)+1;

		switch (direction) {
		case 1:
			deltaForce = new Vector2 (1, 0);
			break;
		case 2:
			deltaForce = new Vector2 (-1, 0);
			break;
		case 3:
			deltaForce = new Vector2 (0, 1);
			break;
		case 4:
			deltaForce = new Vector2 (0, -1);
			break;
			//		case 0 = force 0
		}

		movement = deltaForce * speed;
	}

	void Move() {
		//simple movement
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce (movement, ForceMode2D.Impulse);
		if (movement != Vector2.zero) {
			lastDirection = movement;
		}
	}

	void CalculateAttack () {
		if (!isAttacking) {
			biteDamager.SetActive (false);
			return;
		}
		biteDamager.SetActive (true);

		//attack horizontal or vertical
		Vector3 newPos = transform.position;
		if (Math.Abs(lastDirection.x) > Math.Abs(lastDirection.y)) {
			//extra conditionals for not-moving situation
			newPos.x += 0.1f * (lastDirection.x > 0 ? 1 : -1);
		}
		else {
			newPos.y += 0.1f * (lastDirection.y > 0 ? 1 : -1);
		}

		biteDamager.transform.position = newPos;
	}

	void UpdateBoxCollider () {
		//boxCollider up & down: 0.07 * 0.15
		//boxCollider left & right: 0.31 * 0.15

		if (lastDirection.y != 0) {
			boxCollider.size = new Vector2 (0.07f, 0.15f);
		}
		else if (lastDirection.x != 0) {
			boxCollider.size = new Vector2 (0.31f, 0.15f);
		}

		//update the biter
		biteDamager.GetComponent<BoxCollider2D> ().size = boxCollider.size;
	}

	void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", movement.x);
		animator.SetFloat ("ySpeed", movement.y);
		animator.SetFloat ("lastXSpeed", lastDirection.x);
		animator.SetFloat ("lastYSpeed", lastDirection.y);
	}
}
