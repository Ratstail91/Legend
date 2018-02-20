using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Destructable))]
[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Liftable))]
[RequireComponent(typeof(Rigidbody2D))]

public class Chicken : MonoBehaviour {
	//private structures
	private enum Behaviour {
		NORMAL,
		SCARED,
		LIFTED
	}
	private RandomEngine randomEngine;

	//private members
	private Behaviour behaviour;
	private float lastTime;
	private float actionTime;

	private float timeUntilCalm;
	private Vector2 movement;
	private float speed;

	//component members
	Animator animator;
	Destructable destructable;
	Durability durability;
	Liftable liftable;
	Rigidbody2D rigidBody;

	void Awake () {
		//get components
		randomEngine = new RandomEngine ();
		animator = GetComponent<Animator> ();
		destructable = GetComponent<Destructable> ();
		durability = GetComponent<Durability> ();
		liftable = GetComponent<Liftable> ();
		rigidBody = GetComponent<Rigidbody2D> ();

		//internal stuff
		lastTime = Time.time;
		behaviour = Behaviour.NORMAL;
		durability.maxHealthPoints = 3;
		durability.healthPoints = 3;

		//DEBUG: scatter
		transform.position = new Vector3 (
			transform.position.x + (float)randomEngine.Rand (2) - 1f,
			transform.position.y + (float)randomEngine.Rand (2) - 1f,
			0f
		);
	}
	
	void Update () {
		HandleBehaviour ();

		//if time interval and not lifted
		if (lastTime + actionTime < Time.time && behaviour != Behaviour.LIFTED) {
			CalculateMovement ();
			lastTime = Time.time;
		}

		Move ();

		SendAnimationInfo ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
			//slight code duplication
			behaviour = Behaviour.SCARED;
			timeUntilCalm = 5.0f;
			actionTime = 1.0f;
			speed = 1.2f;

			//run AWAY
			Vector3 otherPos3 = collision.collider.gameObject.transform.position;
			Vector2 otherPos = new Vector2 (otherPos3.x, otherPos3.y);
			Vector2 direction = new Vector2 (transform.position.x, transform.position.y) - otherPos;
			//similar to deltaForce
			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
				direction.x = direction.x > 0 ? 1 : -1;
				direction.y = 0;
			} else {
				direction.x = 0;
				direction.y = direction.y > 0 ? 1 : -1;
			}
			movement = direction * speed;
		}
	}

	void HandleBehaviour() {
		//handle passiveness
		if (liftable.isLifted) {
			behaviour = Behaviour.LIFTED;
		}
		else if (!liftable.isLifted && behaviour == Behaviour.LIFTED) {
			//handle placement
			//slight code duplication
			behaviour = Behaviour.SCARED;
			timeUntilCalm = 5.0f;
			lastTime = Time.time;
			actionTime = 1.0f;
			speed = 1.2f;
			movement = liftable.lastDirection * speed;
		}
		else if (behaviour == Behaviour.SCARED) {
			//handle scaredness
			timeUntilCalm -= Time.deltaTime;
			if (timeUntilCalm <= 0) {
				behaviour = Behaviour.NORMAL;
				lastTime = Time.time;
				speed = 0.1f;

				CalculateMovement ();
			}
		}
		else if (behaviour == Behaviour.NORMAL) {
			//handle normal behaviour
			actionTime = (float)(1.0 + randomEngine.Rand(3.0)); //shake up the action time for multiple chickens
			speed = 0.1f;
		}
	}

	void CalculateMovement() {
		Vector2 deltaForce = new Vector2 (0, 0);

		int direction = (int)randomEngine.Rand(5.0);

		//handle scared = no stopping
		if (behaviour == Behaviour.SCARED && direction == 0) {
			direction = 4; //DOWN
		}

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
		if (behaviour != Behaviour.LIFTED) {
			rigidBody.velocity = Vector2.zero;
			rigidBody.AddForce (movement, ForceMode2D.Impulse);
		}
		else {
			//for carrying in the right direction
			if (rigidBody.velocity != Vector2.zero) {
				movement = rigidBody.velocity;
			}
		}
	}

	void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", movement.x);
		animator.SetFloat ("ySpeed", movement.y);
	}
}
