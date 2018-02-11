using UnityEngine;

public class Sword : MonoBehaviour {
	private Animator animator;

	void SetDirection(Vector2 v) {
		string d;
		if (v.x > 0)
			d = "right";
		else if (v.x < 0)
			d = "left";
		else if (v.y > 0)
			d = "up";
		else
			d = "down"; //default

		animator = GetComponent<Animator> ();
		animator.SetBool (d, true);

		Vector3 pos = gameObject.transform.position;
		Vector3 newPos = pos + (new Vector3 (v.x, v.y, 0) * 20);
		gameObject.transform.position = newPos;
	}

	void Update () {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Done")) {
			Destroy (gameObject);
		}
	}
}
