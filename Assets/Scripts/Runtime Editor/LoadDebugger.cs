using UnityEngine;
using System.Collections;

public class LoadDebugger : MonoBehaviour {
	[SerializeField]
	private string json; 

	void Start() {
		QMRunModeEditor.Instance.Load(json);
	}
}
