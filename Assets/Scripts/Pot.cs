using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Durability))]
public class Pot : MonoBehaviour {
	//components
	private Rigidbody2D rigidBody;
	private Durability durability;

	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		durability = GetComponent<Durability> ();

		durability.maxHealthPoints = 4;
		durability.healthPoints = 4;
	}
	
	void Update () {
		//stop movement
		rigidBody.velocity = Vector2.zero;

		//handle destruction
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
