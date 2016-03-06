using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIButtonMatch : MonoBehaviour, IQuizButton, IPointerDownHandler, IPointerUpHandler {
	[SerializeField]
	private Text label;

	[SerializeField]
	private bool isLeftSide;
	public bool IsLeftSide {
		get { return isLeftSide; }
	}
	[SerializeField]
	private int index;
	public int Index {
		get { return index; }
	}

	[SerializeField]
	private string hash;
	public string Hash {
		get { return hash; }
	}

	private IQuestionHandler callback;

	public GameObject GameObject {
		get { return this.gameObject; }
	}

	private Vector3 positonAfterDrag;
	public string GetTextValue {
		get { return label.text; }
	}

	void Start () {
		//GetComponent<Button>().onClick.AddListener(() => OnClick());
	}

	void OnEnable () {
		positonAfterDrag = transform.position;
	}

	public void OnClick () {
		//callback.OnAnswer(index);
	}

	public void Set (int index, bool isText, object o, IQuestionHandler callback) {
		this.callback = callback;

		this.index = index;
		if (isText) {
			SetText(o.ToString());
		}
		else {
		//	SetText(o.ToString());
			hash = o.ToString();
			label.gameObject.SetActive(false);
			GetComponent<Image>().sprite = Resources.Load<Sprite>(o.ToString()) as Sprite;
		}

		gameObject.SetActive(true);
	}

	public void SetText (string txt) {
		label.text = txt;
	}

	public void OnPointerDown (PointerEventData eventData) {
		PointerEventData pe = new PointerEventData(EventSystem.current);
		pe.position = Input.mousePosition;
		List<RaycastResult> hits = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pe, hits);
		foreach (RaycastResult h in hits) {
			if (h.gameObject.GetComponent<UIButtonMatch>()) {
				callback.OnAnswer(true, h.gameObject.GetComponent<UIButtonMatch>());
			}
		}
	}

	public void OnPointerUp (PointerEventData eventData) {
		transform.position = positonAfterDrag;
		callback.OnAnswer(false, null);
	}

	public void SetNewPosition (Vector3 newPos) {
		positonAfterDrag = newPos;
	}

	public void SwapIndex (int index) {
		this.index = index;
	}
}