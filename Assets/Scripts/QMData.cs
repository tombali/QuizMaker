using UnityEngine;
using UnityEngine.Events;
public interface IQuestionHandler {
	void Process (JSONObject data);
	void Refresh (object[] data);
	void Hide ();
	void OnAnswer (params object[] data);
}

public interface IQuizButton {
	GameObject gameObject {
		get;
	}
	void Set (int index, bool isText, object o, IQuestionHandler callback);
	void SetText (string txt);
	void OnClick ();
}

[System.Serializable]
public class QMBoolEvent : UnityEvent<bool> { }
