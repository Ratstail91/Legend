using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour {
	//components
	public Sprite sprite;

	void Awake() {
		sprite = GetComponent<SpriteRenderer> ().sprite;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		//collided with the character
		if (collider.gameObject.GetComponent<Character> () != null) {
			//dreaded globals!
			GameObject inventoryObject = GameObject.Find ("Inventory");

			if (inventoryObject == null) {
				Debug.LogError ("Failed to find the inventory object");
				return;
			}

			//add self to inventory
			bool success = inventoryObject.GetComponent<Inventory> ().AddItem (this);

			if (success) {
				gameObject.SetActive (false);
			}
		}
	}
}