using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
public class QMManager : MonoBehaviour {
	[SerializeField]
	private string quizFileName;
	[SerializeField]
	private bool loadOnStart;
	[SerializeField]
	private bool displayQuestionOnStart;
	[SerializeField]
	private bool loadNextQuestionOnAnswer;
	[SerializeField]
	private bool loadNextQuestionOnCorrectAnswer;
	[SerializeField]
	private float breakTime;
	[SerializeField]
	private QMBoolEvent onAnswerEvent;
	[SerializeField]
	private UnityEvent onTimeExpire;
	[SerializeField]
	private UnityEvent onSkipQuestion;
	[SerializeField]
	private UnityEvent onPointsChange;
	[SerializeField]
	private UnityEvent onQuizComplete;

	private bool isQuizDisplayed = false;

	private int totalQuestions = 0;
	private int currentQuestion = 0;
	private int questionTime;
	private int questionPoints;
	private bool isAnswered = false;
	private bool isSkipped = false;

	private JSONObject currentQuiz;
	private Coroutine timerCoroutine;

	private int points;
	public int Points {
		get { return points; }
	}

	private Dictionary<int, bool> stats = new Dictionary<int, bool>();
	public Dictionary<int, bool> Stats {
		get { return stats; }
	}

	public delegate void OnAnswerAction (bool isCorrect);
	public static event OnAnswerAction onAnswer;

	private static QMManager instance;
	public static QMManager Instance {
		get { return instance; }
	}

	void Awake () {
		instance = this;
		ResetSharedElements();
	}

	void Start () {
		if (loadOnStart && !string.IsNullOrEmpty(quizFileName)) {
			LoadQuiz(quizFileName);
		}
	}

	public void DisplayQuizUI (bool state) {
		isQuizDisplayed = state;
		QMUIReference.Instance.QuizUI.SetActive(state);
	}

	public void LoadQuiz (string filename) {
		TextAsset t = Resources.Load(filename) as TextAsset;
		currentQuiz = new JSONObject(t.text);
		currentQuestion = 0;
		totalQuestions = currentQuiz["quiz"].list.Count;
		if (displayQuestionOnStart) {
			DisplayQuestion(currentQuestion);
		}
		points = 0;
		stats.Clear();
	}

	public void DisplayNextQuestion () {
		DisplayQuestion(currentQuestion);
	}

	public void DisplayNextQuestionAdd () {
		DisplayQuestion(currentQuestion);
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.N)) LoadQuiz(quizFileName);
	}

	public void DisplayQuestion (int index) {
		if (currentQuestion < totalQuestions) {
			DisplayQuestion(currentQuiz["quiz"][index]);
		}
		else {
			if (onQuizComplete != null) {
				onQuizComplete.Invoke();
			}
		}
	}

	public void DisplayQuestion (JSONObject json) {
		if (!isQuizDisplayed) {
			DisplayQuizUI(true);
		}

		if (timerCoroutine != null) {
			StopCoroutine(timerCoroutine);
		}

		ResetBooleans();
		ResetSharedElements();

		QMUIReference.Instance.QuestionText.text = json["question_text"].str;
		questionTime = (int)json["time"].n;
		questionPoints = (int)json["points"].n;
		if (questionTime > 0) {
			QMUIReference.Instance.QuestionTimeText.text = questionTime.ToString();
		}
		else QMUIReference.Instance.QuestionTimeText.text = string.Empty;

		switch ((int)json["type"].n) {
			case 0:
				QMUIReference.Instance.TypeA.Process(json["data"]);
				break;
			case 1:
				QMUIReference.Instance.TypeB.Process(json["data"]);
				break;
			case 2:
				QMUIReference.Instance.TypeC.Process(json["data"]);
				break;
			case 3:
				QMUIReference.Instance.TypeD.Process(json["data"]);
				break;
			case 4:
				QMUIReference.Instance.TypeE.Process(json["data"]);
				break;
		}

		if (questionTime > 0) {
			timerCoroutine = StartCoroutine(Timer(questionTime));
		}
	}

	private IEnumerator Timer (int t) {
		while (t > 0 && !isAnswered && !isSkipped) {
			QMUIReference.Instance.QuestionTimeText.text = t.ToString();
			yield return new WaitForSeconds(1);
			t--;			
		}
		if (t <= 0) {
			onTimeExpire.Invoke();
			currentQuestion++;
			if (breakTime <= 0) {
				DisplayQuestion(currentQuestion);
			}
			else {
				StartCoroutine(BreakTime());
			}
		}
	}


	public void SkipQuestion () {
		isSkipped = true;
		onSkipQuestion.Invoke();
		ResetSharedElements();		
	}

	public void OnAnswer (bool isCorrect, IQuestionHandler handler) {
		// add points
		if (isCorrect) {
			points += questionPoints;
			if (onPointsChange != null) {
				onPointsChange.Invoke();
			}
		}

		// fire event
		if (onAnswer != null) {
			onAnswer.Invoke(isCorrect);
		}
		// fire second event
		onAnswerEvent.Invoke(isCorrect);
		// save stats
		stats.Add(currentQuestion, isCorrect);
		// resetting some values
		isAnswered = true;
		handler.Hide();
		ResetSharedElements();
		// increase question index
		currentQuestion++;
		// load next question if set to true
		if (loadNextQuestionOnAnswer) {
			// load next question if answer is correct
			if (loadNextQuestionOnCorrectAnswer && !isCorrect) {
				return;
			}
			if (breakTime <= 0) {
				DisplayQuestion(currentQuestion);
			}
			else {
				StartCoroutine(BreakTime());
			}
		}		
	}

	private IEnumerator BreakTime () {
		float t = breakTime;
		while (t > 0) {
			yield return new WaitForSeconds(1);
			t--;
		}
		DisplayQuestion(currentQuestion);
	}

	private void ResetBooleans () {
		isAnswered = false;
		isSkipped = false;
	}

	private void ResetSharedElements () {
		questionTime = -1;
		QMUIReference.Instance.TypeA.Hide();
		QMUIReference.Instance.TypeB.Hide();
		QMUIReference.Instance.TypeC.Hide();
		QMUIReference.Instance.TypeD.Hide();
		QMUIReference.Instance.TypeE.Hide();
		QMUIReference.Instance.QuestionImage.gameObject.SetActive(false);
		QMUIReference.Instance.QuestionTimeText.text = string.Empty;
		QMUIReference.Instance.QuestionText.text = string.Empty;
	}

	public bool WasAnwseredCorrect (int index) {
		bool value = false;
		if (index < stats.Count) {
			value = stats[index];
		}
		return value;
	}

	public void DebugStats () {
		foreach (KeyValuePair<int, bool> stat in stats) {
			Debug.Log(string.Format("Question {0}. is {1}", stat.Key + 1, stat.Value ? "correct" : "not correct"));
		}
	}
}
