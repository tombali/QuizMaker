using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class QMUIReference : MonoBehaviour {
	[SerializeField]
	private GameObject quizUI;
	public GameObject QuizUI {
		get { return quizUI; }
	}

	[SerializeField]
	private Text questionText, questionTimeText;
	public Text QuestionText {
		get { return questionText; }
	}
	public Text QuestionTimeText {
		get { return questionTimeText; }
	}

	[SerializeField]
	private Image questionImage;
	public Image QuestionImage {
		get { return questionImage; }
	}

	[SerializeField]
	private UITypeAHandler typeA;
	public UITypeAHandler TypeA {
		get { return typeA; }
	}

	[SerializeField]
	private UITypeBHandler typeB;
	public UITypeBHandler TypeB {
		get { return typeB; }
	}

	[SerializeField]
	private UITypeCHandler typeC;
	public UITypeCHandler TypeC {
		get { return typeC; }
	}

	[SerializeField]
	private UITypeDHandler typeD;
	public UITypeDHandler TypeD {
		get { return typeD; }
	}

	[SerializeField]
	private UITypeEHandler typeE;
	public UITypeEHandler TypeE {
		get { return typeE; }
	}

	private static QMUIReference instance;
	public static QMUIReference Instance {
		get { return instance; }
	}

	void Awake () {
		instance = this;
	}
}
