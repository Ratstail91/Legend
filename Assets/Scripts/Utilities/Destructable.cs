using UnityEngine;

[RequireComponent(typeof(Durability))]
public class Destructable : MonoBehaviour {
	//components
	Durability durability;

	//members
	float lastTime;
	float actionTime;
	public float invincibleWindow {
		get { return actionTime; }
		set { actionTime = value >= 0 ? value : 0; }
	}

	//internal stuff
	public delegate void OnDestruction();
	public OnDestruction onDestruction {
		get { return onDestructionCallback; }
		set { onDestructionCallback = value; }
	}
	OnDestruction onDestructionCallback;

	// Use this for initialization
	void Awake () {
		durability = GetComponent<Durability> ();
		lastTime = Time.time;
		actionTime = 0;
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

	//this handles damage and invincibility windows
	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.gameObject.GetComponent<Damager> () != null) {
			Damager dmgr = collision.collider.gameObject.GetComponent<Damager> ();

			if (lastTime + actionTime < Time.time) { 
				durability.healthPoints += dmgr.damageValue;
				lastTime = Time.time;
			}
		}
	}
}
