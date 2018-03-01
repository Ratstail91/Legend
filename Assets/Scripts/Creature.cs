using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public abstract class Creature : MonoBehaviour {
	//timing members
	protected float lastTime;
	protected float actionTime;

	//components
	protected Animator animator;
	protected BoxCollider2D boxCollider;
	protected Durability durability;
	protected RandomEngine randomEngine;
	protected Rigidbody2D rigidBody;
	protected SpriteRenderer spriteRenderer;

	protected virtual void Awake() {
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		durability = GetComponent<Durability> ();
		randomEngine = new RandomEngine ();
		rigidBody = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();

		lastTime = Time.time;
	}

	protected virtual void Update() {
		HandleBehaviour ();

		//if time interval
		if (lastTime + actionTime < Time.time) {
			CalculateMovement ();
			lastTime = Time.time;
		}

		Move ();

		SendAnimationInfo ();
	}

	abstract protected void HandleBehaviour ();

	abstract protected void CalculateMovement ();

	abstract protected void Move ();

	abstract protected void SendAnimationInfo ();

	protected void FlashColor(float r, float g, float b, float seconds) {
		StartCoroutine (FlashColorCoroutine (r, g, b, seconds));
	}

	IEnumerator FlashColorCoroutine(float r, float g, float b, float seconds) {
		spriteRenderer.color = new Color(r, g, b);
		yield return new WaitForSeconds (seconds);
		spriteRenderer.color = new Color(1, 1, 1);
	}
}
