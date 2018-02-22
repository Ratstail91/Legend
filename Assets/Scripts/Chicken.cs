using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Destructable))]
[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Liftable))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

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
	private Vector2 moveDelta; //this is so bad...
	private float speed;

	//component members
	Animator animator;
	Destructable destructable;
	Durability durability;
	Liftable liftable;
	Rigidbody2D rigidBody;
	SpriteRenderer spriteRenderer;

	void Awake () {
		//get components
		randomEngine = new RandomEngine ();
		animator = GetComponent<Animator> ();
		destructable = GetComponent<Destructable> ();
		durability = GetComponent<Durability> ();
		liftable = GetComponent<Liftable> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();

		//internal stuff
		lastTime = Time.time;
		behaviour = Behaviour.NORMAL;
		durability.maxHealthPoints = 3;
		durability.healthPoints = 3;

		//DEBUG: scatter
//		transform.position = new Vector3 (
//			transform.position.x + (float)randomEngine.Rand (2) - 1f,
//			transform.position.y + (float)randomEngine.Rand (2) - 1f,
//			0f
//		);

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

		Destructable.callback onDstr = destructable.onDestruction;
		destructable.onDestruction = () => {
			if (onDstr != null) {
				onDstr();
			}
			Instantiate(Resources.Load("Meat_Raw",  typeof(GameObject)), transform.position, Quaternion.identity);
		};
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

	void HandleBehaviour() {
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

	void CalculateMovement() {
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

	void Move() {
		if (behaviour != Behaviour.LIFTED) {
			rigidBody.velocity = Vector2.zero;

			Vector2 impulse = moveDelta * speed;

			if (moveDelta.x != 0 && moveDelta.y != 0) {
				impulse *= 0.71f;
			}

			rigidBody.AddForce (impulse, ForceMode2D.Impulse);
		}
		else {
			//for carrying in the right direction (animation)
			if (rigidBody.velocity != Vector2.zero) {
				moveDelta = rigidBody.velocity;
			}
		}
	}

	void SendAnimationInfo() {
		//send the animation info to the animator
		animator.SetFloat ("xSpeed", moveDelta.x);
		animator.SetFloat ("ySpeed", moveDelta.y);
	}

	//internal stuff
	IEnumerator FlashColor(float r, float g, float b, float seconds) {
		spriteRenderer.color = new Color(r, g, b);
		yield return new WaitForSeconds (seconds);
		spriteRenderer.color = new Color(1, 1, 1);
	}
}
