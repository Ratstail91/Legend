using UnityEngine;

[RequireComponent(typeof(Durability))]
public class Destructable : MonoBehaviour {
	//components
	Durability durability;

	//internal stuff
	public delegate void OnDestruction();
	public OnDestruction onDestruction {
		get { return onDestructionCallback; }
		set { onDestructionCallback = value; }
	}
	OnDestruction onDestructionCallback;

	// Use this for initialization
	void Start () {
		durability = GetComponent<Durability> ();
	}
	
	// Update is called once per frame
	void Update () {
		//handle destruction
		if (durability.healthPoints <= 0 ) {
			if (onDestruction != null) {
				onDestruction (); //callback
			}
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
			Damager dmgr = collision.collider.gameObject.GetComponent<Damager> ();
			durability.healthPoints += dmgr.damageValue;
		}
	}
}
