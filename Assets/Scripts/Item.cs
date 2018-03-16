using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour {
	//components
	public Sprite sprite;
	GameObject inventory;

	void Awake() {
		sprite = GetComponent<SpriteRenderer> ().sprite;
		inventory = GameObject.Find ("Inventory"); //dreaded globals!

		if (inventory == null) {
			Debug.LogError ("Failed to find the inventory object");
		}
	}

	void OnTriggerStay2D(Collider2D collider) {
		//collided with the characterw
		if (collider.gameObject.GetComponent<Character> () != null && Input.GetButton("Use")) {
			//add self to inventory
			bool success = inventory.GetComponent<Inventory> ().AddItem (this);

			if (success) {
				gameObject.SetActive (false);
			}
		}
	}
}