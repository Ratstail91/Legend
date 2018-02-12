using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour {
	private Durability durability;

	void Start () {
		durability = GetComponent<Durability> ();

//		durability.SetOnDamaged ((dmg) => Debug.Log ("DMG: " + dmg.ToString ()));
		durability.maxHealthPoints = 4;
		durability.healthPoints = 4;
	}
	
	void Update () {
		if (durability.healthPoints <= 0 ) {
			Destroy (gameObject);
		}
	}
}
