using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Liftable))]

public class Chicken : Creature {
	//private structures
	private enum Behaviour {
		NORMAL,
		SCARED,
		LIFTED
	}

	//private members
	private Behaviour behaviour;

	private float timeUntilCalm;
	private Vector2 moveDelta;
	private float speed;

	//conponents
	Liftable liftable;

	protected override void Awake () {
		base.Awake ();

		//get components
		liftable = GetComponent<Liftable> ();

		//internal stuff
		behaviour = Behaviour.NORMAL;
		durability.maxHealthPoints = 3;
		durability.healthPoints = 3;
		durability.invincibleWindow = 0.5f;

		//set callbacks
		Durability.callback onDmg = durability.onDamaged;
		durability.onDamaged = (int diff) => {
			if (onDmg != null) {
				onDmg(diff);
			}
			FlashColor(1, 0, 0, 0.1f);
		};

		Durability.callback onHld = durability.onHealed;
		durability.onHealed = (int diff) => {
			if (onHld != null) {
				onHld(diff);
			}
			FlashColor(0, 1, 0, 0.1f);
		};

		Durability.callback onDstr = durability.onDestruction;
		durability.onDestruction = (int i) => {
			if (onDstr != null) {
				onDstr(i);
			}
			Instantiate(Resources.Load("Meat_Raw",  typeof(GameObject)), transform.position, Quaternion.identity);
		};
	}
	
	protected override void Update () {
		HandleBehaviour ();

		//if time interval and not lifted
		if (lastTime + actionTime < Time.time && !liftable.isLifted) {
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

			//determine moveDelta
			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
				moveDelta.x = direction.x > 0 ? 1 : -1;
				moveDelta.y = 0;
			} else {
				moveDelta.x = 0;
				moveDelta.y = direction.y > 0 ? 1 : -1;
			}
		}
	}

	protected override void HandleBehaviour() {
		//handle passiveness
		if (liftable.isLifted) {
			behaviour = Behaviour.LIFTED;
		}
		else if (!liftable.isLifted && behaviour == Behaviour.LIFTED) {
			//handle placement
			behaviour = Behaviour.SCARED;
			timeUntilCalm = 5.0f;
			lastTime = Time.time;
			actionTime = 1.0f;
			speed = 1.2f;

			//run
			moveDelta = liftable.lastDirection;
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

	protected override void CalculateMovement() {
		int direction = (int)randomEngine.Rand(5.0);

		//handle scared = no stopping
		if (behaviour == Behaviour.SCARED && direction == 0) {
			direction = 4; //DOWN
		}

		switch (direction) {
		case 0:
			moveDelta = new Vector2 (0, 0);
			break;
		case 1:
			moveDelta = new Vector2 (1, 0);
			break;
		case 2:
			moveDelta = new Vector2 (-1, 0);
			break;
		case 3:
			moveDelta = new Vector2 (0, 1);
			break;
		case 4:
			moveDelta = new Vector2 (0, -1);
			break;
		}
	}

	protected override void Move() {
		if (behaviour == Behaviour.LIFTED) {
			//for carrying in the right direction (animation)
			if (rigidBody.velocity != Vector2.zero) {
				moveDelta = rigidBody.velocity;
			}
		} else {
			//normal movement
			rigidBody.velocity = Vector2.zero;

			Vector2 impulse = moveDelta * speed;

			if (moveDelta.x != 0 && moveDelta.y != 0) {
				impulse *= 0.71f; //diagonal
			}

			rigidBody.AddForce (impulse, ForceMode2D.Impulse);
		}
	}

	protected override void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", moveDelta.x);
		animator.SetFloat ("ySpeed", moveDelta.y);
	}
}
