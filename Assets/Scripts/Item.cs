using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour {
	//components
	public Sprite sprite;

	void Awake() {
		sprite = GetComponent<SpriteRenderer> ().sprite;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		//collided with the character
		if (collision.gameObject.GetComponent<Character> () != null) {
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