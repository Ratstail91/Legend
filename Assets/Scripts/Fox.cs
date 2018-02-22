using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Destructable))]
[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Unit2D))]

public class Fox : MonoBehaviour {
	//interface structures
	private enum Behaviour {
		NORMAL,
		HUNTING,
		FIGHTING
	}
	private RandomEngine randomEngine;

	//private members
	private Behaviour behaviour;
	private float lastTime;
	private float actionTime;

	private Vector2 lastDirection;
	private bool isAttacking = false;
	public float huntDistance; //public for the inspector
	public float biteDistance; //public for the inspector
	GameObject fightingTarget;

	//child members
	private GameObject biteDamager;

	//components
	Animator animator;
	BoxCollider2D boxCollider;
	Destructable destructable;
	Durability durability;
	Rigidbody2D rigidBody;
	Unit2D pathUnit;

	void Awake() {
		//get components
		randomEngine = new RandomEngine ();
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		destructable = GetComponent<Destructable> ();
		durability = GetComponent<Durability> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		pathUnit = GetComponent<Unit2D> ();

		//get the children
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

		Move ();

		//animation
		if (pathUnit.movement != Vector2.zero) {
			lastDirection = pathUnit.movement;
		}

		//custom fox stuff
		CalculateAttack ();
		UpdateBoxColliders ();

		SendAnimationInfo ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
			//is attacked, start fighting back
			behaviour = Behaviour.FIGHTING;
			fightingTarget = collision.gameObject;
		}
	}

	void HandleBehaviour() {
		//get this pos
		Vector2 thisPos = new Vector2 (transform.position.x, transform.position.y);

		//used for chickens
		Vector2 closest;

		switch(behaviour) {
		case Behaviour.NORMAL:
			actionTime = 2.5f;
			pathUnit.speed = 0.2f;
			isAttacking = false;

			//if there's a chicken within a certain distance (code duplication)
			closest = FindClosestChicken ();
			behaviour = Vector2.Distance(closest, thisPos) < huntDistance ? Behaviour.HUNTING : Behaviour.NORMAL;

			break;

		case Behaviour.HUNTING:
			actionTime = 0.5f;
			pathUnit.speed = 0.4f;

			//if there's a chicken within a certain distance (code duplication)
			closest = FindClosestChicken ();
			behaviour = Vector2.Distance(closest, thisPos) < huntDistance ? Behaviour.HUNTING : Behaviour.NORMAL;

			isAttacking = !isAttacking && Vector2.Distance(closest, thisPos) < biteDistance;

			break;

		case Behaviour.FIGHTING:
			//if the target is dead, calm down
			if (fightingTarget == null) {
				behaviour = Behaviour.NORMAL;
				break;
			}

			actionTime = 0.5f;
			pathUnit.speed = 0.4f;

			Vector2 targetPos = new Vector2 (fightingTarget.transform.position.x, fightingTarget.transform.position.y);

			isAttacking = !isAttacking && Vector2.Distance(targetPos, thisPos) < biteDistance;

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
//			case 0 = force 0
			}
			pathUnit.movement = deltaForce * pathUnit.speed; //no diagonal adjustment for normal behaviour
			break;

		case Behaviour.HUNTING:
			Vector2 closest = FindClosestChicken ();

			//found nothing
			if (closest == Vector2.positiveInfinity) {
				break;
			}

			//pass the movement to the pathfinding code
			pathUnit.FollowPathToPoint (closest);

			break;

		case Behaviour.FIGHTING:
			//if the target is dead, calm down
			if (fightingTarget == null) {
				break;
			}

			Vector2 targetPos = new Vector2 (fightingTarget.transform.position.x, fightingTarget.transform.position.y);

			//pass the movement to the pathfinding code
			pathUnit.FollowPathToPoint (targetPos);
			break;
		}
	}

	void Move() {
		if (behaviour == Behaviour.NORMAL) {
			//force move if not following path
			pathUnit.Move();
		}
	}

	void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", pathUnit.movement.x);
		animator.SetFloat ("ySpeed", pathUnit.movement.y);
		animator.SetFloat ("lastXSpeed", lastDirection.x);
		animator.SetFloat ("lastYSpeed", lastDirection.y);
	}

	//custom stuff below this line

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

	void UpdateBoxColliders () {
		//boxCollider up & down:    0.07 x 0.15
		//boxCollider left & right: 0.31 x 0.15

		if (Mathf.Abs(lastDirection.y) > Mathf.Abs(lastDirection.x)) {
			boxCollider.size = new Vector2 (0.07f, 0.15f);
		} else {
			boxCollider.size = new Vector2 (0.31f, 0.15f);
		}

		//update the biter
		biteDamager.GetComponent<BoxCollider2D> ().size = boxCollider.size;
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
