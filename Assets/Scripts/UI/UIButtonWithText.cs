using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIButtonWithText : MonoBehaviour, IQuizButton {
	[SerializeField]
	private Text label;

	private int index;
	private IQuestionHandler callback;

	public GameObject GameObject {
		get { return this.gameObject; }
	}

	void Start () {
		GetComponent<Button>().onClick.AddListener(() => OnClick());
	}

	public void OnClick () {
		callback.OnAnswer(index);
	}

	public void Set (int index, bool isText, object txt, IQuestionHandler callback) {
		this.callback = callback;

		this.index = index;
		SetText(txt.ToString());

		gameObject.SetActive(true);
	}

	public void SetText (string txt) {
		label.text = txt;
	}
}
