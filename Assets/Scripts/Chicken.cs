using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Liftable))]
public class Chicken : MonoBehaviour {
	//private structures
	public enum Behaviour {
		NORMAL,
		SCARED,
		LIFTED
	};
	private RandomEngine randomEngine;

	//private members
	private float lastTime;
	private float actionTime;
	public float timeUntilCalm;
	private Vector2 movement;
	private float speed;
	public Behaviour behaviour;

	//component members
	Rigidbody2D rigidBody;
	Animator animator;
	Liftable liftable;
	Durability durability;

	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		liftable = GetComponent<Liftable> ();
		durability = GetComponent<Durability> ();
		randomEngine = new RandomEngine ();

		//internal stuff
		lastTime = Time.time;
		behaviour = Behaviour.NORMAL;
		durability.maxHealthPoints = 3;
		durability.healthPoints = 3;
	}
	
	void Update () {
		//handle destruction
		if (durability.healthPoints <= 0 ) {
			Destroy (gameObject);
		}

		HandleBehaviour ();

		//if time interval and not lifted
		if (lastTime + actionTime < Time.time && behaviour != Behaviour.LIFTED) {
			RandomizeMovement (speed);
			lastTime = Time.time;
		}

		Move ();

		SendAnimationInfo ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.tag == "Damager") {
			Damager dmgr = collision.collider.gameObject.GetComponent<Damager> ();
			durability.healthPoints += dmgr.damageValue;

			//slight code duplication
			behaviour = Behaviour.SCARED;
			timeUntilCalm = 5.0f;
			actionTime = 1.0f;
			speed = 1.2f;
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

				RandomizeMovement (speed);
			}
		}
		else if (behaviour == Behaviour.NORMAL) {
			//handle normal behaviour
			actionTime = (float)(1.0 + randomEngine.Rand(3.0)); //shake up the action time for multiple chickens
			speed = 0.1f;
		}
	}

	void RandomizeMovement(float speed) {
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
