using System;
using UnityEngine;

public class Durability : MonoBehaviour {
	//public stuff
	public delegate void callback(int i);

	//the important stuff
	private int health;
	private int maxHealth;
	private callback onDamagedCallback;
	private callback onHealedCallback;
	private callback onDestructionCallback;

	//timing members
	float lastTime = float.NegativeInfinity;
	float actionTime = 0;
	public float invincibleWindow {
		get { return actionTime; }
		set { actionTime = value >= 0 ? value : 0; }
	}

	// Update is called once per frame
	void Update () {
		//handle destruction
		if (healthPoints <= 0 ) {
			if (onDestruction != null) {
				onDestruction (0); //callback
			}
			Destroy (gameObject);
		}
	}

	//this handles damage and invincibility windows
	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
			Damager dmgr = collision.collider.gameObject.GetComponent<Damager> ();

			if (lastTime + actionTime < Time.time) {
				healthPoints += dmgr.damageValue;
				lastTime = Time.time;
			}
		}
	}

	//accessors &  mutators
	public int healthPoints {
		get { return health; }
		set {
			int diff = value - health; //diff < 0 means loss of life
			if (diff < 0 && onDamagedCallback != null) {
				onDamagedCallback (diff);
			}
			if (diff > 0 && onHealedCallback != null) {
				onHealedCallback (diff);
			}
			health = value;
			ClampHealth ();
		}
	}

	public callback onDestruction {
		get { return onDestructionCallback; }
		set { onDestructionCallback = value; }
	}

	public callback onDamaged {
		get { return onDamagedCallback; }
		set { onDamagedCallback = value; }
	}

	public callback onHealed {
		get { return onHealedCallback; }
		set { onHealedCallback = value; }
	}

	public int maxHealthPoints {
		get { return maxHealth; }
		set {
			maxHealth = value;
			ClampHealth ();
		}
	}

	//ensure that health is always between 0 and maxHealth
	void ClampHealth() {
		if (maxHealth < 0) {
			throw new Exception ("Duration.maxHealthPoints is out of range");
		}
		if (health < 0) {
			health = 0;
		}
		if (health > maxHealth) {
			health = maxHealth;
		}
	}
}
