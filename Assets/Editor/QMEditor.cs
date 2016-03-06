using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class QMEditor : EditorWindow {
	[MenuItem("Quiz Maker/Editor")]

	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(QMEditor));
	}

	private JSONObject quiz;
	private int currentQuestion = 0;
	private bool fetchedQuiz = false;
	private bool isLoaded = false, newQuestion = false;
	void OnGUI () {
		if (Selection.activeObject != null) {
			if (Selection.activeObject.GetType() != typeof(UnityEngine.TextAsset)) {
				Warning();
			}
			else {
				GUILayout.Label("Editing: " + Selection.activeObject.name);
				if (!fetchedQuiz) {
					fetchedQuiz = true;
					TextAsset t = Selection.activeObject as TextAsset;
					quiz = new JSONObject(t.text);
				}
				EditorUI();
			}
		}
		else Warning();
	}

	void Warning () {
		GUILayout.Label("Select existing quiz file or create new quiz file");
		if (GUILayout.Button("Create")) {

		}
	}

	void EditorUI () {
		GUILayout.BeginHorizontal();
		QuestionList(quiz);
		QuestionEditor(quiz["quiz"][currentQuestion]);
		GUILayout.EndHorizontal();
	}

	void QuestionList (JSONObject quiz) {
		GUILayout.BeginVertical(GUILayout.Width(200));
		GUI.color = Color.yellow;
		if (GUILayout.Button("New ABCDE question")) {
			newQuestion = true;
			isLoaded = false;
			JSONObject o = QMConvert.TypeAToJSON(string.Empty, 0, 0, 0, null, new string[5]);
			NewQuestion(o);
		}
		if (GUILayout.Button("New text match")) {
			newQuestion = true;
			isLoaded = false;
			JSONObject o = QMConvert.TypeBToJSON(string.Empty, 0, 0, new string[5], new string[5]);
			NewQuestion(o);
		}
		if (GUILayout.Button("New images match")) {
			newQuestion = true;
			isLoaded = false;
			Clean();
			JSONObject o = QMConvert.TypeCToJSON(string.Empty, 0, 0, new Sprite[5], new Sprite[5]);
			NewQuestion(o);
		}
		if (GUILayout.Button("New text/image match")) {
			newQuestion = true;
			isLoaded = false;
			JSONObject o = QMConvert.TypeDToJSON(string.Empty, 0, 0, new string[5], new Sprite[5]);
			NewQuestion(o);
		}
		if (GUILayout.Button("New sorting question")) {
			newQuestion = true;
			isLoaded = false;
			JSONObject o = QMConvert.TypeEToJSON(string.Empty, 0, 0, new string[5]);
			NewQuestion(o);
		}
		GUI.color = Color.white;
		GUILayout.Label("Questions in quiz");
		for (int i = 0; i < quiz["quiz"].list.Count; i++) {
			if (i == currentQuestion) {
				GUI.color = Color.red;
			}
			else {
				GUI.color = Color.white;
			}
			if (GUILayout.Button("Question " + (i + 1).ToString())) {
				GUI.FocusControl("Dummy");
				isLoaded = false;
				Clean();
				currentQuestion = i;
			}
		}
		GUI.color = Color.white;
		GUILayout.EndVertical();
	}

	void NewQuestion (JSONObject o) {
		GUI.FocusControl("Dummy");
		quiz["quiz"].Add(o);
		currentQuestion = quiz["quiz"].list.Count - 1;
		Clean();
		newQuestion = false;
	}

	private int question_type;
	private string question_question_text;
	private int question_time, question_points, question_correct;
	private string[] texts1 = new string[5], texts2 = new string[5];
	private Sprite[] sprites1 = new Sprite[5], sprites2 = new Sprite[5];
	private Sprite questionImage;
	void QuestionEditor (JSONObject o) {
		if (newQuestion) return;

		GUILayout.BeginVertical(GUILayout.Width(600));
		if (!isLoaded) {
			ConvertFromJSON(o);
		}
		else {
			GUILayout.Label("Question text");
			question_question_text = EditorGUILayout.TextField(question_question_text);
			GUILayout.Label("Time to answer");
			question_time = EditorGUILayout.IntField(question_time);
			GUILayout.Label("Points");
			question_points = EditorGUILayout.IntField(question_points);
			GUILayout.Label("Answers");
			switch (question_type) {
				case 0:
					DisplayOneText();
					break;
				case 1:
					DisplayTwoText();
					break;
				case 2:
					DisplayTwoSprites();
					break;
				case 3:
					DisplayOneTextOneSprite();
					break;
				case 4:
					DisplaySort();
					break;
			}

			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUI.color = Color.green;
			if (GUILayout.Button("Save", GUILayout.Width(200))) {
				SaveToJSON();
			}
			GUILayout.Space(200);
			GUI.color = Color.red;
			if (GUILayout.Button("Delete", GUILayout.Width(200))) {
				quiz["quiz"].list.RemoveAt(currentQuestion);
				if (currentQuestion > 0) {
					currentQuestion--;
				}
				WriteFile();
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		GUILayout.Space(20);
		GUILayout.Label("Current question JSON Output");
		EditorGUILayout.TextArea(quiz["quiz"][currentQuestion].ToString(), GUILayout.Height(200));
		GUILayout.EndVertical();
	}

	void SaveToJSON () {
		switch (question_type) {
			case 0:
				quiz["quiz"][currentQuestion] = QMConvert.TypeAToJSON(question_question_text, question_time, question_points, question_correct, questionImage, texts1);
				break;
			case 1:
				quiz["quiz"][currentQuestion] = QMConvert.TypeBToJSON(question_question_text, question_time, question_points, texts1, texts2);
				break;
			case 2:
				quiz["quiz"][currentQuestion] = QMConvert.TypeCToJSON(question_question_text, question_time, question_points, sprites1, sprites2);
				break;
			case 3:
				quiz["quiz"][currentQuestion] = QMConvert.TypeDToJSON(question_question_text, question_time, question_points, texts1, sprites1);
				break;
			case 4:
				quiz["quiz"][currentQuestion] = QMConvert.TypeEToJSON(question_question_text, question_time, question_points, texts1);
				break;

		}
		WriteFile();
	}

	void WriteFile () {
		System.IO.File.WriteAllText(Application.dataPath + "/Resources/" + Selection.activeObject.name + ".txt", quiz.ToString());
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		fetchedQuiz = false;
	}

	void ConvertFromJSON (JSONObject o) {
		question_type = (int)o["type"].n;
		question_time = (int)o["time"].n;
		question_points = (int)o["points"].n;
		question_question_text = o["question_text"].str;

		switch (question_type) {
			case 0:
				OneText(o["data"]);
				break;
			case 1:
				TwoText(o["data"]);
				break;
			case 2:
				TwoSprite(o["data"]);
				break;
			case 3:
				OneTextOneSprite(o["data"]);
				break;
			case 4:
				Sort(o["data"]);
				break;
		}

		isLoaded = true;
	}

	void OneText (JSONObject o) {
		question_correct = (int)o["correct"].n;
		for (int i = 0; i < o["answers"].list.Count; i++) {
			texts1[i] = o["answers"].list[i].str;
		}
		if (o.HasField("image")) {
			questionImage = Resources.Load<Sprite>(o["image"].str) as Sprite;
		}
	}

	void DisplayOneText () {
		for (int i = 0; i < texts1.Length; i++) {
			GUILayout.BeginHorizontal();
			if (i == question_correct) {
				GUI.color = Color.green;
			}
			if (GUILayout.Button((i + 1).ToString() + ".", GUILayout.Width(30))) {
				if (!string.IsNullOrEmpty(texts1[i])) {
					question_correct = i;
				}
			}
			GUI.color = Color.white;
			texts1[i] = EditorGUILayout.TextField(texts1[i]);
			GUILayout.EndHorizontal();
		}
		GUILayout.Label("Image");
		questionImage = (Sprite)EditorGUILayout.ObjectField(questionImage, typeof(Sprite));
	}

	void TwoText (JSONObject o) {
		for (int i = 0; i < o["group1"].list.Count; i++) {
			texts1[i] = o["group1"].list[i].str;
		}
		for (int i = 0; i < o["group2"].list.Count; i++) {
			texts2[i] = o["group2"].list[i].str;
		}
	}

	void DisplayTwoText () {
		for (int i = 0; i < texts1.Length; i++) {
			GUILayout.BeginHorizontal();
			texts1[i] = EditorGUILayout.TextField(texts1[i]);
			texts2[i] = EditorGUILayout.TextField(texts2[i]);
			GUILayout.EndHorizontal();
		}
	}

	void TwoSprite (JSONObject o) {
		for (int i = 0; i < o["group1"].list.Count; i++) {
			sprites1[i] = Resources.Load<Sprite>(o["group1"].list[i].str) as Sprite;
		}
		for (int i = 0; i < o["group2"].list.Count; i++) {
			sprites2[i] = Resources.Load<Sprite>(o["group2"].list[i].str) as Sprite;
		}
	}

	void DisplayTwoSprites () {
		for (int i = 0; i < sprites1.Length; i++) {
			GUILayout.BeginHorizontal();
			sprites1[i] = (Sprite)EditorGUILayout.ObjectField(sprites1[i], typeof(Sprite));
			sprites2[i] = (Sprite)EditorGUILayout.ObjectField(sprites2[i], typeof(Sprite));
			GUILayout.EndHorizontal();
		}
	}

	void OneTextOneSprite (JSONObject o) {
		for (int i = 0; i < o["group1"].list.Count; i++) {
			texts1[i] = o["group1"].list[i].str;
		}
		for (int i = 0; i < o["group2"].list.Count; i++) {
			sprites1[i] = Resources.Load<Sprite>(o["group2"].list[i].str) as Sprite;
		}
	}

	void DisplayOneTextOneSprite () {
		for (int i = 0; i < texts1.Length; i++) {
			GUILayout.BeginHorizontal();
			texts1[i] = EditorGUILayout.TextField(texts1[i]);
			sprites1[i] = (Sprite)EditorGUILayout.ObjectField(sprites1[i], typeof(Sprite));
			GUILayout.EndHorizontal();
		}
	}

	void Sort (JSONObject o) {
		for (int i = 0; i < o["answers"].list.Count; i++) {
			texts1[i] = o["answers"].list[i].str;
		}
	}

	void DisplaySort () {
		for (int i = 0; i < texts1.Length; i++) {
			texts1[i] = EditorGUILayout.TextField(texts1[i]);
		}
	}

	void Clean () {
		question_type = -1;
		question_correct = -1;
		question_points = 0;
		question_question_text = string.Empty;
		question_time = 0;
		questionImage = null;
		texts1 = new string[5];
		texts2 = new string[5];
		sprites1 = new Sprite[5];
		sprites2 = new Sprite[5];
	}
}