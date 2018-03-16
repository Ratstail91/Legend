using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	public const int itemSlotCount = 4;
	int selectedSlot = 0;
	float lastPress = float.NegativeInfinity;
	float pressDelay = 0.5f;

	public Item_Slot[] itemSlots = new Item_Slot[itemSlotCount];

	void Awake() {
		//get the child Item_Slots
		Transform[] childTransforms = GetComponentsInChildren<Transform> ();
		//format: {Inventory, ItemSlot {background, selection, item}, ...}
		for (int i = 0; i < itemSlotCount; i++) {
			itemSlots [i] = childTransforms [1 + i * 4].gameObject.GetComponent<Item_Slot> ();
		}

		SetSelection (0);
	}

	void Update() {
		if (Input.GetButton("Inventory Up") && Time.time - lastPress > pressDelay) {
			this.SelectionDown();
			lastPress = Time.time;
		}
		if ((Input.GetButton("Inventory Down") || Input.GetAxis("Inventory Down") > 0) && Time.time - lastPress > pressDelay) {
			this.SelectionUp();
			lastPress = Time.time;
		}

		if (!Input.GetButton("Inventory Up") && !Input.GetButton("Inventory Down") && Input.GetAxis("Inventory Down") == 0) {
			lastPress = float.NegativeInfinity;
		}

		if (Input.GetButton("Drop")) {
			Item item = RemoveItem (selectedSlot);
			if (item != null) {
				item.transform.position = GameObject.Find ("Character").transform.position;
			}
		}
	}

	public bool AddItem(Item newItem) {
		for (int i = 0; i < itemSlotCount; i++) {
			if (itemSlots [i].GetItem() == null) {
				itemSlots [i].AddItem (newItem);
				return true;
			}
		}
		return false;
	}

	public Item RemoveItem(int index) {
		return itemSlots [index].RemoveItem ();
	}

	//selection controllers
	public void SetSelection(int i) {
		selectedSlot = i;
		CheckSelectionBounds ();
		UpdateSelection ();
	}

	public void SelectionDown() {
		selectedSlot--;
		CheckSelectionBounds ();
		UpdateSelection ();
	}

	public void SelectionUp() {
		selectedSlot++;
		CheckSelectionBounds ();
		UpdateSelection ();
	}

	void CheckSelectionBounds() {
		if (selectedSlot < 0) {
			selectedSlot = 0;
		}
		if (selectedSlot >= itemSlotCount) {
			selectedSlot = itemSlotCount - 1;
		}
	}

	void UpdateSelection() {
		foreach (Item_Slot slot in itemSlots) {
			slot.SetSelected (false);
		}
		itemSlots [selectedSlot].SetSelected (true);
	}
}
