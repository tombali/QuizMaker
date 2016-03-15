using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class QMRunModeSetCorrect : MonoBehaviour {
	[SerializeField]
	private int index;
	public int Index {
		get { return index; }
	}

	void Start () {
		GetComponent<Button>().onClick.AddListener(() => OnClick());
	}

	void OnClick () {
		QMRunModeEditor.Instance.SetCorrect(index);
		GetComponent<Image>().color = Color.green;
	}
}
