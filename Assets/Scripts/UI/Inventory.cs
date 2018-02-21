using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	public const int itemSlotCount = 4;

	public Image[] itemImages = new Image[itemSlotCount];
	public Item[] items = new Item[itemSlotCount];

	void Awake() {
		//HACK: get the references to the itemImages
		//children should be like: ItemSlot { BackgroundImage, ItemImage }
		Image[] imageArray = GetComponentsInChildren<Image> ();
		for (int i = 0; i < itemSlotCount; i++) {
			itemImages[i] = imageArray [i * 2 + 1];
		}
	}

	public bool AddItem(Item newItem) {
		for (int i = 0; i < itemSlotCount; i++) {
			//empty slot
			if (items[i] == null) {
				items [i] = newItem;
				itemImages [i].sprite = newItem.sprite;
				itemImages [i].enabled = true;
				return true;
			}
		}
		return false;
	}

	public bool RemoveItem(Item removeItem) {
		for (int i = 0; i < itemSlotCount; i++) {
			if (items[i] == removeItem) {
				items [i] = null;
				itemImages [i].sprite = null;
				itemImages [i].enabled = false;
				return true;
			}
		}
		return false;
	}
}
