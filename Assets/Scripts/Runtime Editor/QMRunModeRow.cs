using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QMRunModeRow : MonoBehaviour {
	[SerializeField]
	private int index;
	[SerializeField]
	private InputField field1, field2;
	public InputField Field1 {
		get { return field1; }
	}
	public InputField Field2 {
		get { return field2; }
	}

	public void DisplayOne () {
		field1.text = string.Empty;
		field2.text = string.Empty;
		field1.gameObject.SetActive(true);
		field2.gameObject.SetActive(false);
	}

	public void DisplayBoth () {
		field1.text = string.Empty;
		field2.text = string.Empty;
		field1.gameObject.SetActive(true);
		field2.gameObject.SetActive(true);
	}

	public void DisplayNone () {
		field1.text = string.Empty;
		field2.text = string.Empty;
		field1.gameObject.SetActive(false);
		field2.gameObject.SetActive(false);
	}
}
