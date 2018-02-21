using System;
using UnityEngine;

public class Durability : MonoBehaviour {
	//the important stuff
	private int health;
	private int maxHealth;
	private Action<int> onDamagedCallback;
	private Action<int> onHealedCallback;

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

	public Action<int> onDamaged {
		get { return onDamagedCallback; }
		set { onDamagedCallback = value; }
	}

	public Action<int> onHealed {
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
