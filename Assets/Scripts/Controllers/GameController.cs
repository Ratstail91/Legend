using UnityEngine;

public class GameController : MonoBehaviour {
	void Update () {
		if (Input.GetButton ("Pause")) {
			//TODO: proper pause menu
			Application.Quit ();
		}
	}
}
