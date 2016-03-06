using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITypeCHandler : MonoBehaviour, IQuestionHandler {
	[SerializeField]
	private UIButtonMatch[] buttonsGroup1, buttonsGroup2;
	[SerializeField]
	private int buttonHeight = 30;

	private List<string> correct1 = new List<string>();
	private List<string> correct2 = new List<string>();
	private Dictionary<int, string> group1 = new Dictionary<int, string>();
	private Dictionary<int, string> group2 = new Dictionary<int, string>();

	public void Process (JSONObject data) {
		correct1.Clear();
		correct2.Clear();
		group1.Clear();
		group2.Clear();

		for (int i = 0; i < data["group1"].list.Count; i++) {
			correct1.Add(data["group1"][i].str);
			correct2.Add(data["group2"][i].str);
		}

		List<string> a = new List<string>();
		List<string> b = new List<string>();
		for (int i = 0; i < data["group1"].list.Count; i++) {
			a.Add(data["group1"][i].str);
			b.Add(data["group2"][i].str);
		}

		QMConvert.Shuffle<string>(a);
		QMConvert.Shuffle<string>(b);

		bool isText = data["isText"].b;

		for (int i = 0; i < a.Count; i++) {
			group1.Add(i, a[i]);
			buttonsGroup1[i].Set(i, isText, a[i], this);
			group2.Add(i, b[i]);
			buttonsGroup2[i].Set(i, isText, b[i], this);
		}

		Refresh(data["group1"].list.Count);
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
			buttonsGroup2[i].GameObject.SetActive(false);
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
		if (one.IsLeftSide && two.IsLeftSide) {
			d = group1;
		}
		else d = group2;

		d.Remove(one.Index);
		d.Remove(two.Index);

		int i = one.Index;
		one.SwapIndex(two.Index);
		two.SwapIndex(i);

		d.Add(one.Index, one.Hash);
		d.Add(two.Index, two.Hash);
	}

	private void CheckAnswer () {
		bool correct = true;
		for (int i = 0; i < correct1.Count; i++) {
			int index = -1;
			foreach (KeyValuePair<int, string> g in group1) {
				if (correct1[i] == g.Value) {
					index = g.Key;
					foreach (KeyValuePair<int, string> g2 in group2) {
						if (correct2[i] == g2.Value) {
							if (index != g2.Key) {
								correct = false;
							}
						}
					}

				}
			}
		}

		if (correct) {
			QMManager.Instance.OnAnswer(true, this);
		}
	}
}
