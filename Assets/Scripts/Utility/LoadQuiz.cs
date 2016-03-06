using UnityEngine;
using System.Collections;

public class LoadQuiz : MonoBehaviour {

	// Use this for initialization
	void Start () {
		QMManager.Instance.LoadQuiz("test_quiz");
		QMManager.Instance.DisplayNextQuestion();
	}
}
