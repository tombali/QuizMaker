using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QMRunModeEditor : MonoBehaviour {
	private static QMRunModeEditor instance;
	public static QMRunModeEditor Instance {
		get { return instance; }
	}

	[SerializeField]
	private GameObject editor;
	[SerializeField]
	private InputField question_title, question_time, question_points;
	[SerializeField]
	private QMRunModeRow[] rows;
	[SerializeField]
	private GameObject correctButtonsParent;
	[SerializeField]
	private QMRunModeSetCorrect[] correctButtons;
	[SerializeField]
	private InputField shortAnswer;
	[SerializeField]
	private GameObject previousButton, nextButton;
	[SerializeField]
	private Dropdown dropdown;
	[SerializeField]
	private Text infoLabel;

	private int question_type = 0, question_correct = 0;
	private string[] texts1 = new string[5];
	private string[] texts2 = new string[5];

	private JSONObject quiz;
	private int currentQuestion;

	public string CurrentQuiz {
		get { return quiz.ToString(); }
	}

	void Awake () {
		instance = this;
	}

	public void CreateNew () {
		JSONObject o = new JSONObject();
		JSONObject a = new JSONObject(JSONObject.Type.ARRAY);
		o.AddField("quiz", a);
		quiz = o;
	}

	public void Load (string json) {
		quiz = new JSONObject(json);
		Clean();
		editor.SetActive(true);
		currentQuestion = 0;
		SetInfoLabel();
		LoadFromJSON(quiz["quiz"][currentQuestion]);
	}

	public void Save () {
		if (!CheckInputField(question_title) || !CheckInputField(question_time) || !CheckInputField(question_points)) {
			return;
		}

		RowToArray();

		SaveToJSON();
	}

	private bool CheckInputField (InputField field) {
		if (string.IsNullOrEmpty(field.text)) {
			field.image.color = Color.red;
			return false;
		}
		else {
			field.image.color = Color.white;
			return true;
		}
	}

	void LoadFromJSON (JSONObject o) {
		if (o == null) return;
		question_type = (int)o["type"].n;
		question_time.text = o["time"].n.ToString();
		question_points.text = o["points"].n.ToString();
		question_title.text = o["question_text"].str;
		switch (question_type) {
			case 0:				
				RowsDisplayOne();
				correctButtonsParent.SetActive(true);
				OneText(o["data"]);
				shortAnswer.gameObject.SetActive(false);
				break;
			case 1:				
				RowsDisplayBoth();
				TwoText(o["data"]);
				shortAnswer.gameObject.SetActive(false);
				correctButtonsParent.SetActive(false);
				break;
			case 4:				
				RowsDisplayOne();
				Sort(o["data"]);
				shortAnswer.gameObject.SetActive(false);
				correctButtonsParent.SetActive(false);
				break;
			case 5:
				RowsDisplayNone();
				shortAnswer.gameObject.SetActive(true);
				correctButtonsParent.SetActive(false);
				ShortAnswer(o["data"]);
				break;
		}
	}

	void SaveToJSON () {
		switch (question_type) {
			case 0:
				quiz["quiz"][currentQuestion] = QMConvert.TypeAToJSON(question_title.text, int.Parse(question_time.text), int.Parse(question_points.text), question_correct, null, texts1);
				break;
			case 1:
				quiz["quiz"][currentQuestion] = QMConvert.TypeBToJSON(question_title.text, int.Parse(question_time.text), int.Parse(question_points.text), texts1, texts2);
				break;
			case 4:
				quiz["quiz"][currentQuestion] = QMConvert.TypeEToJSON(question_title.text, int.Parse(question_time.text), int.Parse(question_points.text), texts1);
				break;
			case 5:
				quiz["quiz"][currentQuestion] = QMConvert.TypeFToJSON(question_title.text, int.Parse(question_time.text), int.Parse(question_points.text), shortAnswer.text);
				break;
		}

		Debug.Log(quiz.ToString());
	}

	public void NewQuestion (int index) {
		Clean();
		editor.SetActive(true);

		question_type = index;
		JSONObject o = new JSONObject();
		if (index == 0) {
			RowsDisplayOne();
			correctButtonsParent.SetActive(true);
			shortAnswer.gameObject.SetActive(false);
			o = QMConvert.TypeAToJSON(string.Empty, 0, 0, 0, null, new string[5]);
		}
		else if (index == 1) {
			correctButtonsParent.SetActive(false);
			RowsDisplayBoth();
			shortAnswer.gameObject.SetActive(false);
			o = QMConvert.TypeBToJSON(string.Empty, 0, 0, new string[5], new string[5]);
		}
		else if (index == 4) {
			correctButtonsParent.SetActive(false);
			RowsDisplayOne();
			shortAnswer.gameObject.SetActive(false);
			o = QMConvert.TypeEToJSON(string.Empty, 0, 0, new string[5]);
		}
		else if (index == 5) {
			correctButtonsParent.SetActive(false);
			RowsDisplayNone();
			shortAnswer.gameObject.SetActive(true);
			o = QMConvert.TypeFToJSON(string.Empty, 0, 0, "");
		}

		quiz["quiz"].Add(o);
		currentQuestion = quiz["quiz"].list.Count - 1;
		
		SetInfoLabel();
	}

	void OneText (JSONObject o) {
		question_correct = (int)o["correct"].n;
		for (int i = 0; i < o["answers"].list.Count; i++) {
			texts1[i] = o["answers"].list[i].str;
			rows[i].Field1.text = texts1[i];
		}
		SetCorrect(question_correct);
	}

	void TwoText (JSONObject o) {
		for (int i = 0; i < o["group1"].list.Count; i++) {
			texts1[i] = o["group1"].list[i].str;
			rows[i].Field1.text = texts1[i];
		}
		for (int i = 0; i < o["group2"].list.Count; i++) {
			texts2[i] = o["group2"].list[i].str;
			rows[i].Field2.text = texts2[i];
		}
	}

	void Sort (JSONObject o) {
		for (int i = 0; i < o["answers"].list.Count; i++) {
			texts1[i] = o["answers"].list[i].str;
			rows[i].Field1.text = texts1[i];
		}
	}

	void ShortAnswer (JSONObject o) {
		shortAnswer.text = o["answer"].str;
	}

	private void RowToArray () {
		for (int i = 0; i < rows.Length; i++) {
			texts1[i] = rows[i].Field1.text;
			texts2[i] = rows[i].Field2.text;
		}
	}

	private void RowsDisplayOne () {
		for (int i = 0; i < rows.Length; i++) {
			rows[i].DisplayOne();
		}
	}

	private void RowsDisplayBoth () {
		for (int i = 0; i < rows.Length; i++) {
			rows[i].DisplayBoth();
		}
	}

	private void RowsDisplayNone () {
		for (int i = 0; i < rows.Length; i++) {
			rows[i].DisplayNone();
		}
	}

	public void SetCorrect (int correct) {
		question_correct = correct;
		for (int i = 0; i < correctButtons.Length; i++) {
			if (correctButtons[i].Index != correct) {
				correctButtons[i].GetComponent<Image>().color = Color.white;
			}
			else if (correctButtons[i].Index == correct) {
				correctButtons[i].GetComponent<Image>().color = Color.green;
			}
		}
	}

	private void Clean () {
		question_title.text = string.Empty;
		question_time.text = string.Empty;
		question_points.text = string.Empty;
		shortAnswer.text = string.Empty;
	}

	public void ShowPrevious () {
		if (currentQuestion > 0) {
			currentQuestion--;
		}

		LoadFromJSON(quiz["quiz"][currentQuestion]);

		SetInfoLabel();
	}

	public void ShowNext () {
		if (currentQuestion < quiz["quiz"].list.Count - 1) {
			currentQuestion++;
		}

		LoadFromJSON(quiz["quiz"][currentQuestion]);

		SetInfoLabel();
	}

	public void ShowQuestion (int index) {
		currentQuestion = index;

		LoadFromJSON(quiz["quiz"][currentQuestion]);

		SetInfoLabel();
	}

	public void SetInfoLabel () {
		infoLabel.text = string.Format("Current question: {0}/{1}", currentQuestion + 1, quiz["quiz"].list.Count);
		SetDropdown();
	}

	private void SetDropdown () {
		System.Collections.Generic.List<Dropdown.OptionData> options = new System.Collections.Generic.List<Dropdown.OptionData>();
		for (int i = 0; i < quiz["quiz"].list.Count; i++) {
			options.Add(new Dropdown.OptionData((i + 1).ToString()));
		}
		dropdown.options = options;
	}

	public void Delete () {
		quiz["quiz"].list.RemoveAt(currentQuestion);
		if (currentQuestion > 0) {
			currentQuestion--;
		}
	}
}
