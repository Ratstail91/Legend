using UnityEngine;

[RequireComponent(typeof(Durability))]
[RequireComponent(typeof(Liftable))]
public class Pot : MonoBehaviour {
	//components
	private Durability durability;
	private Liftable liftable;

	void Start () {
		durability = GetComponent<Durability> ();
		liftable = GetComponent<Liftable> ();

		durability.maxHealthPoints = 4;
		durability.healthPoints = 4;
	}
	
	void Update () {
		if (durability.healthPoints <= 0 ) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.tag == "Damager") {
			Damager dmgr = collision.collider.gameObject.GetComponent<Damager> ();
			durability.healthPoints += dmgr.damageValue;
		}
	}
}
