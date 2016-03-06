using UnityEngine;
using System.Collections;

public class DisplayQuestionOnCollision : MonoBehaviour {
	[SerializeField]
	private int index;

	bool hit = false;
	void OnCollisionEnter (Collision collision) {
		if (collision.transform.tag == "Player" && !hit) {
			hit = true;
			QMManager.Instance.DisplayQuestion(index);
			Destroy(this.gameObject);
		}
	}
}
