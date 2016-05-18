using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITypeFHandler : MonoBehaviour, IQuestionHandler {
	[SerializeField]
	private InputField input;
	[SerializeField]
	private Button submit;

	private string answer = string.Empty;

	public void Process (JSONObject data) {
		answer = data["answer"].str;

		input.gameObject.SetActive(true);
		submit.gameObject.SetActive(true);
	}

	public void Refresh (params object[] data) {

	}

	public void Hide () {
		input.gameObject.SetActive(false);
		submit.gameObject.SetActive(false);
	}

	public void OnAnswer (params object[] data) {
		CheckAnswer();
	}

	public void CheckAnswer () {
		bool correct = true;
		if (answer.ToLower() != input.text.ToLower()) {
			correct = false;
		}

		QMManager.Instance.OnAnswer(correct, this);
	}
}
