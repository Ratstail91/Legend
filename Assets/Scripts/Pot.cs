using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Destructable))]
[RequireComponent(typeof(Durability))]
public class Pot : MonoBehaviour {
	//components
	private Rigidbody2D rigidBody;
	private Durability durability;

	void Awake () {
		rigidBody = GetComponent<Rigidbody2D> ();
		durability = GetComponent<Durability> ();

		durability.maxHealthPoints = 4;
		durability.healthPoints = 4;
	}
	
	void Update () {
		//stop movement
		rigidBody.velocity = Vector2.zero;
	}
}
