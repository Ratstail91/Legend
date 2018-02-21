using UnityEngine;
using UnityEditor;

//So what does this do exactly?
//I'm not sure, but it works
[CustomEditor(typeof(Inventory))]
public class Inventory_Editor : Editor {
	private bool[] showItemSlots = new bool[Inventory.itemSlotCount];

	private SerializedProperty itemImagesProperty;
	private SerializedProperty itemsProperty;

	private const string inventoryPropItemImagesName = "itemImages";
	private const string inventoryPropItemsName = "items";

	private void OnEnable() {
		itemImagesProperty = serializedObject.FindProperty (inventoryPropItemImagesName);
		itemsProperty = serializedObject.FindProperty (inventoryPropItemsName);
	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();

		for (int i = 0; i < Inventory.itemSlotCount; i++) {
			ItemSlotGUI (i);
		}

		serializedObject.ApplyModifiedProperties ();
	}

	private void ItemSlotGUI(int index) {
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;

		showItemSlots [index] = EditorGUI.Foldout (GUILayoutUtility.GetRect (150, 16), showItemSlots[index], "Item Slot " + index);
		if (showItemSlots[index]) {
			EditorGUILayout.PropertyField (itemImagesProperty.GetArrayElementAtIndex(index));
			EditorGUILayout.PropertyField (itemsProperty.GetArrayElementAtIndex(index));
		}

		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();
	}
}
