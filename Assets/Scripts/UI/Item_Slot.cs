using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Slot : MonoBehaviour {
	Image selectedImage;
	Image itemImage;
	Item item;
	bool selected = false;
	
	// Use this for initialization
	void Awake () {
		//children should be like: { BackgroundImage, SelectionImage, ItemImage }
		Image[] imageArray = GetComponentsInChildren<Image> ();
		Debug.Log (imageArray.Length);
		selectedImage = imageArray [1];
		itemImage = imageArray [2];
		UpdateSelected ();
	}
	
	public bool AddItem(Item newItem) {
		if (item != null) {
			return false;
		}

		item = newItem;
		newItem.enabled = false;
		itemImage.sprite = newItem.sprite;
		itemImage.enabled = true;

		return true;
	}

	public Item GetItem() {
		return item;
	}

	public Item RemoveItem() {
		itemImage.sprite = null;
		itemImage.enabled = false;

		Item ret = item;
		item = null;
		if (ret != null) {
			ret.gameObject.SetActive (true);
		}
		return ret;
	}

	public void SetSelected(bool b) {
		selected = b;
		UpdateSelected ();
	}

	public bool GetSelected() {
		return selected;
	}

	void UpdateSelected() {
		Debug.Log (selectedImage);
		selectedImage.enabled = selected;
	}
}
