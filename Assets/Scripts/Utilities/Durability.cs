using System;
using UnityEngine;

public class Durability : MonoBehaviour {
	//the important stuff
	private int health;
	private int maxHealth;
	private Action<int> onDamaged;
	private Action<int> onHealed;

	//accessors
	public int healthPoints {
		get { return health; }
		set {
			int diff = value - health; //diff < 0 means loss of life
			if (diff < 0 && onDamaged != null) {
				onDamaged (diff);
			}
			if (diff > 0 && onHealed != null) {
				onHealed (diff);
			}
			health = value;
			ClampHealth ();
		}
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

	public void SetOnDamaged(Action<int> f) {
		onDamaged = f;
	}

	public void SetOnHealed(Action<int> f) {
		onHealed = f;
	}
}
