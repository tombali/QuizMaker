using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static class QMConvert {
	// pick A, B, C, D, E answer
	public static JSONObject TypeAToJSON (string text, int time, int points, int correct, Sprite questionImage, params string[] answers) {
		JSONObject json = new JSONObject();
		json.AddField("type", 0);
		json.AddField("question_text", text);
		json.AddField("time", time);
		json.AddField("points", points);

		JSONObject data = new JSONObject();
		if (questionImage != null) {
			data.AddField("image", questionImage.name);
		}
		data.AddField("correct", correct);
		JSONObject array = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < answers.Length; i++) {
			if (!string.IsNullOrEmpty(answers[i])) {
				array.Add(answers[i]);
			}
		}
		data.AddField("answers", array);
		json.AddField("data", data);
		return json;
	}

	// pairs text
	public static JSONObject TypeBToJSON (string text, int time, int points, string[] group1, string[] group2) {
		JSONObject json = new JSONObject();
		json.AddField("type", 1);
		json.AddField("question_text", text);
		json.AddField("time", time);
		json.AddField("points", points);

		JSONObject data = new JSONObject();
		data.AddField("isText", true);
		JSONObject array1 = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < group1.Length; i++) {
			if (!string.IsNullOrEmpty(group1[i])) {
				array1.Add(group1[i]);
			}
		}
		data.AddField("group1", array1);
		JSONObject array2 = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < group2.Length; i++) {
			if (!string.IsNullOrEmpty(group2[i])) {
				array2.Add(group2[i]);
			}
		}
		data.AddField("group2", array2);
		json.AddField("data", data);

		return json;
	}

	// pairs images
	public static JSONObject TypeCToJSON (string text, int time, int points, Sprite[] group1, Sprite[] group2) {
		JSONObject json = new JSONObject();
		json.AddField("type", 2);
		json.AddField("question_text", text);
		json.AddField("time", time);
		json.AddField("points", points);

		JSONObject data = new JSONObject();
		data.AddField("isText", false);
		JSONObject array1 = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < group1.Length; i++) {
			if (group1[i] != null) {
				array1.Add(group1[i].name);
			}
		}
		data.AddField("group1", array1);
		JSONObject array2 = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < group2.Length; i++) {
			if (group2[i] != null) {
				array2.Add(group2[i].name);
			}
		}
		data.AddField("group2", array2);
		json.AddField("data", data);

		return json;
	}

	// pairs text and image
	public static JSONObject TypeDToJSON (string text, int time, int points, string[] group1, Sprite[] group2) {
		JSONObject json = new JSONObject();
		json.AddField("type", 3);
		json.AddField("question_text", text);
		json.AddField("time", time);
		json.AddField("points", points);

		JSONObject data = new JSONObject();
		data.AddField("isText", false);
		JSONObject array1 = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < group1.Length; i++) {
			if (!string.IsNullOrEmpty(group1[i])) {
				array1.Add(group1[i]);
			}
		}
		data.AddField("group1", array1);
		JSONObject array2 = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < group2.Length; i++) {
			if (group2[i] != null) {
				array2.Add(group2[i].name);
			}
		}
		data.AddField("group2", array2);
		json.AddField("data", data);

		return json;
	}

	public static JSONObject TypeEToJSON (string text, int time, int points, params string[] answers) {
		JSONObject json = new JSONObject();
		json.AddField("type", 4);
		json.AddField("question_text", text);
		json.AddField("time", time);
		json.AddField("points", points);

		JSONObject data = new JSONObject();
		data.AddField("isText", true);
		JSONObject array = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < answers.Length; i++) {
			if (!string.IsNullOrEmpty(answers[i])) {
				array.Add(answers[i]);
			}
		}
		data.AddField("answers", array);

		json.AddField("data", data);

		return json;
	}

	public static JSONObject TypeFToJSON (string text, int time, int points, string answer) {
		JSONObject json = new JSONObject();
		json.AddField("type", 5);
		json.AddField("question_text", text);
		json.AddField("time", time);
		json.AddField("points", points);

		JSONObject data = new JSONObject();
		data.AddField("isText", true);
		data.AddField("answer", answer);

		json.AddField("data", data);

		return json;
	}

	// random hash
	public static string GetHash () {
		return System.IO.Path.GetRandomFileName().Replace(".", "");
	}

	// randomize List
	private static System.Random rng = new System.Random();
	public static void Shuffle<T> (this IList<T> list) {
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	// swap items in list
	public static IList<T> Swap<T> (this IList<T> list, int indexA, int indexB) {
		if (indexB > -1 && indexB < list.Count) {
			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}
		return list;
	}
}