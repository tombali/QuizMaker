using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UITypeAHandler : MonoBehaviour, IQuestionHandler {
	[SerializeField]
	private UIButtonWithText[] buttons;
	[SerializeField]
	private int buttonHeight = 30;

	private int correct = -1;

	public void Process (JSONObject data) {
		correct = (int)data["correct"].n;
		for (int i = 0; i < data["answers"].list.Count; i++) {
			buttons[i].Set(i, true, data["answers"][i].str, this);
		}

		if (data.HasField("image")) {
			QMUIReference.Instance.QuestionImage.sprite = Resources.Load<Sprite>(data["image"].str) as Sprite;
			QMUIReference.Instance.QuestionImage.gameObject.SetActive(true);
		}

		Refresh(data["answers"].list.Count);
	}

	public void Refresh (params object[] data) {
		GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (int)data[0] * buttonHeight);
	}


	public void OnAnswer (params object[] data) {
		Hide();

		if ((int)data[0] == correct) {
			QMManager.Instance.OnAnswer(true, this);
		}
		else QMManager.Instance.OnAnswer(false, this);
	}

	public void Hide () {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].GameObject.SetActive(false);
		}
	}
}
