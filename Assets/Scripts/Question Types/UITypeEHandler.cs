using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITypeEHandler : MonoBehaviour, IQuestionHandler {
	[SerializeField]
	private UIButtonMatch[] buttonsGroup1;
	[SerializeField]
	private int buttonHeight = 30;

	private List<string> correct1 = new List<string>();
	private Dictionary<int, string> group1 = new Dictionary<int, string>();

	public void Process (JSONObject data) {
		correct1.Clear();
		group1.Clear();

		for (int i = 0; i < data["answers"].list.Count; i++) {
			correct1.Add(data["answers"][i].str);
		}

		List<string> a = new List<string>();
		for (int i = 0; i < data["answers"].list.Count; i++) {
			a.Add(data["answers"][i].str);
		}

		QMConvert.Shuffle<string>(a);

		for (int i = 0; i < a.Count; i++) {
			group1.Add(i, a[i]);
			buttonsGroup1[i].Set(i, true, a[i], this);
		}

		Refresh(data["answers"].list.Count);
	}

	public void Refresh (params object[] data) {
		GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (int)data[0] * buttonHeight);
	}

	private Vector3 startPos, newPos;
	private UIButtonMatch draggedButton, buttonToSwap;
	public void OnAnswer (params object[] data) {
		if ((bool)data[0] == true) {
			draggedButton = (UIButtonMatch)data[1];
			draggedButton.GetComponent<RectTransform>().SetAsLastSibling();
			startPos = draggedButton.transform.position;
		}
		else {
			draggedButton = null;

			CheckAnswer();
		}
	}

	public void Hide () {
		for (int i = 0; i < buttonsGroup1.Length; i++) {
			buttonsGroup1[i].GameObject.SetActive(false);
		}
	}

	void Update () {
		if (draggedButton == null) {
			return;
		}
		draggedButton.transform.position = new Vector2(draggedButton.transform.position.x, Input.mousePosition.y);

		PointerEventData pe = new PointerEventData(EventSystem.current);
		pe.position = draggedButton.transform.position;
		List<RaycastResult> hits = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pe, hits);
		// loop raycast hits
		foreach (RaycastResult h in hits) {
			if (h.gameObject.GetComponent<UIButtonMatch>()) {
				buttonToSwap = h.gameObject.GetComponent<UIButtonMatch>();
				if (buttonToSwap != draggedButton) {
					// change answers order
					SwapIndex(draggedButton, buttonToSwap);

					// change visual
					newPos = buttonToSwap.transform.position;
					buttonToSwap.transform.position = startPos;
					startPos = newPos;
					draggedButton.SetNewPosition(newPos);
				}
			}
		}
	}

	private void SwapIndex (UIButtonMatch one, UIButtonMatch two) {
		Dictionary<int, string> d = new Dictionary<int, string>();
		d = group1;

		d.Remove(one.Index);
		d.Remove(two.Index);

		int i = one.Index;
		one.SwapIndex(two.Index);
		two.SwapIndex(i);

		d.Add(one.Index, one.GetTextValue);
		d.Add(two.Index, two.GetTextValue);
	}

	private void CheckAnswer () {
		bool correct = true;
		for (int i = 0; i < correct1.Count; i++) {
			if (correct1[i] != group1[i]) {
				correct = false;
			}
			/*
			int index = -1;
			foreach (KeyValuePair<int, string> g in group1) {
				if (correct1[i] == g.Value) {
					if (index != g.Key) {
						correct = false;
					}
				}
			}*/
		}
		Debug.Log(correct);
		if (correct) {
			QMManager.Instance.OnAnswer(true, this);
		}
	}
}
