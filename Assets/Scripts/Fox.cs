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
	Unit2D pathUnit;

	void Awake() {
		//get components
		animator = GetComponent<Animator> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		durability = GetComponent<Durability> ();
		pathUnit = GetComponent<Unit2D> ();
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

		//if time interval
		if (lastTime + actionTime < Time.time) {
			CalculateMovement ();
			lastTime = Time.time;
		}

		if (behaviour == Behaviour.NORMAL) {
			//force move if not following path
			pathUnit.Move();
		}

		//animation
		if (pathUnit.movement != Vector2.zero) {
			lastDirection = pathUnit.movement;
		}

		CalculateAttack ();
		UpdateBoxCollider ();

		SendAnimationInfo ();
	}

//	void OnCollisionEnter2D(Collision2D collision) {
//		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
//			//TODO: fight
//		}
//	}

	void HandleBehaviour() {
		//if there's a chicken within 10 units
		Vector3 closest = FindClosestChicken ();
		behaviour = Vector2.Distance(closest, new Vector2(transform.position.x, transform.position.y)) < 10 ? Behaviour.HUNTING : Behaviour.NORMAL;

		switch(behaviour) {
		case Behaviour.NORMAL:
			actionTime = 2.5f;
			pathUnit.speed = 0.2f;
			break;
		case Behaviour.HUNTING:
			actionTime = 0.5f;
			pathUnit.speed = 0.4f;
			break;
		case Behaviour.FIGHTING:
			//TODO
			break;
		}
	}

	void CalculateMovement() {
		//heavy handed
		pathUnit.StopFollowingPath ();

		switch (behaviour) {
		case Behaviour.NORMAL:
			//meander around at will
			Vector2 deltaForce = new Vector2 (0, 0);
			int direction = (int)randomEngine.Rand (4.0) + 1;
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
			pathUnit.movement = deltaForce * pathUnit.speed;
			break;

		case Behaviour.HUNTING:
			Vector2 closest = FindClosestChicken ();

			//found nothing
			if (closest == Vector2.positiveInfinity) {
				behaviour = Behaviour.NORMAL;
				break;
			}

			//pass the movement to the pathfinding code
			pathUnit.FollowPathToPoint (closest);

			isAttacking = !isAttacking && Vector2.Distance(new Vector2(transform.position.x, transform.position.y), closest) < 0.3f;

			break;

//		case Behaviour.FIGHTING:
//			//TODO: fighting behaviour
//			break;
		}
	}

	void CalculateAttack () {
		//TODO: distance to chicken

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
		animator.SetFloat ("xSpeed", pathUnit.movement.x);
		animator.SetFloat ("ySpeed", pathUnit.movement.y);
		animator.SetFloat ("lastXSpeed", lastDirection.x);
		animator.SetFloat ("lastYSpeed", lastDirection.y);
	}

	Vector2 FindClosestChicken() {
		//find the closest chicken
		UnityEngine.Object[] objects = FindObjectsOfType (typeof(Chicken));

		Vector2 closest = Vector2.positiveInfinity;
		float closestDist = float.PositiveInfinity;

		foreach (UnityEngine.Object o in objects) {
			Vector3 chickenPos = ((Chicken)o).gameObject.transform.position;
			float dist = Vector2.Distance (new Vector2 (transform.position.x, transform.position.y), new Vector2 (chickenPos.x, chickenPos.y));

			if (dist < closestDist) {
				closest = new Vector2 (chickenPos.x, chickenPos.y);
				closestDist = dist;
			}
		}

		return closest;
	}
}
