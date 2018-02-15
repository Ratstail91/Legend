using UnityEngine;

public class GameController : MonoBehaviour {
	void Update () {
		if (Input.GetKey ("escape")) {
			Application.Quit ();
		}
	}
}
