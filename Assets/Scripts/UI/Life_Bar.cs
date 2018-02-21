using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//designed to work with quartered hearts
public class Life_Bar : MonoBehaviour {
	//public variables
	public Durability durability;

	public Sprite heartZero;
	public Sprite heartOne;
	public Sprite heartTwo;
	public Sprite heartThree;
	public Sprite heartFour;

	//private variables
	Image[] childImages;

	void Awake() {
		childImages = GetComponentsInChildren<Image> ();
		foreach(Image image in childImages) {
			image.enabled = false;
		}

		//BUGFIX: pop the Life_Bar's image
		List<Image> imageList = new List<Image> (childImages);
		imageList.RemoveAt (0);
		childImages = imageList.ToArray ();

		//check the durability fits the heart setup
		if (durability.maxHealthPoints % 4 != 0) {
			Debug.LogError ("Durability.maxHealthPoints is not a multiple of 4");
		}

		durability.SetOnDamaged (UpdateGraphics);
		durability.SetOnHealed (UpdateGraphics);
	}

	//callback
	void UpdateGraphics(int diff) { //NOTE: diff is applied AFTER this function is called
		for (int i = 0; i < durability.maxHealthPoints; i += 4) {
			childImages [i / 4].enabled = true;

			//assign the correct sprite
			if ( (durability.healthPoints+diff) - i >= 4) {
				childImages [i / 4].sprite = heartFour;
			} else {
				switch((durability.healthPoints+diff) - i) {
				case 1:
					childImages [i / 4].sprite = heartOne;
					break;
				case 2:
					childImages [i / 4].sprite = heartTwo;
					break;
				case 3:
					childImages [i / 4].sprite = heartThree;
					break;
				default:
					childImages [i / 4].sprite = heartZero;
					break;
				}
			}
		}
	}
}
