using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour 
{
    #region Variáveis
	[SerializeField]
	GameObject questionBox, answerGroup;


    int humor
    {
        get;
        set;
    }

    private int answer;
    private string strAnswer;

    float elapsedTime;

    public bool answered
    {
        get;
        set;
    }

//    [SerializeField]
//    TextMesh questionSubject;
    
    [SerializeField]
    TextMesh questionText;

    [SerializeField]
    GameObject timerMeter;

    float timeTaken;
  
    [SerializeField]
    GameObject alt1Button;
    [SerializeField]
    GameObject alt2Button;
    [SerializeField]
    GameObject alt3Button;
    [SerializeField]
    GameObject alt4Button;

    [SerializeField]
    TextMesh alt1Text;
    [SerializeField]
    TextMesh alt2Text;
    [SerializeField]
    TextMesh alt3Text;
    [SerializeField]
    TextMesh alt4Text;

	Vector3 originalPos;
	Vector3 originalSize;

    IA receptor;

	int wrongTries = 0;

	ShameMeter playerMeter;

    #endregion

    #region Behaviours do Unity
	// Use this for initialization
	void Awake () 
    {
		originalPos = answerGroup.transform.position;
		originalSize = questionBox.transform.localScale;

		playerMeter = GameObject.FindGameObjectWithTag("Player").GetComponent<ShameMeter>();

        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void OnEnable () 
    {
//        transform.position = origin;
//
//        timeBar.transform.localScale = new Vector3(barSize,
//                                                 timeBar.transform.localScale.y,
//                                                 timeBar.transform.localScale.z);

		questionBox.transform.localScale = originalSize;
        iTween.ScaleFrom(questionBox, iTween.Hash(
            "scale", Vector3.zero,
            "time", 0.8f,
            "onupdatetarget", gameObject,
            "easetype", iTween.EaseType.easeOutBack));

		answerGroup.transform.position = originalPos;
		iTween.MoveFrom(answerGroup, iTween.Hash(
			"position", new Vector3(answerGroup.transform.position.x + 0.8f, answerGroup.transform.position.y, answerGroup.transform.position.z),
			"time", 0.8f,
			"onupdatetarget", gameObject,
			"easetype", iTween.EaseType.easeOutCubic));

		wrongTries = 0;
	}

    void OnDisable()
    {
        if(receptor)
        {
            receptor.ResetAnswer();
        }
    }

    void Update()
    {
        if(!answered)
        {
            elapsedTime += Time.deltaTime;

			var percent = (elapsedTime / receptor.paciencia);
			timerMeter.renderer.material.SetFloat("_Cutoff", Mathf.Clamp(1 - percent, 0.001f, 1));
		
            if(elapsedTime >= receptor.paciencia)
            {
                answered = true;

                receptor.DamagePlayerDialogue();
                
                StartCoroutine(WrongAnswerMove());
                StartCoroutine(WrongAnswerShake());
                StartCoroutine(WrongAnswerBlink());
            }

			if(playerMeter.gameOver)
			{
				answered = true;

				StartCoroutine(WrongAnswerMove());
				StartCoroutine(WrongAnswerShake());
				StartCoroutine(WrongAnswerBlink());
			}
        }
    }
    #endregion

    #region Processamento de Questões
    public void Initialize(int humor, IA NPC)
    {
        receptor = NPC;

        answered = false;

        elapsedTime = 0;

        int randomQuestion = Random.Range(0, (DialogueDB.dialogueArray.Length / 5));

        questionText.text = DialogueDB.dialogueArray[randomQuestion, 0];

		//questionSubject.text = "Palpite <color=#0066FF>" + DialogueDB.dialogueArray[randomQuestion, 5] + "</color>!";


        string[] arrAnswers = {DialogueDB.dialogueArray[randomQuestion, 1],
                               DialogueDB.dialogueArray[randomQuestion, 2],
                               DialogueDB.dialogueArray[randomQuestion, 3],
                               DialogueDB.dialogueArray[randomQuestion, 4]};

        string [] randomizedAnswers = RandomizedArray(arrAnswers);

        alt1Text.text = randomizedAnswers[0];
        alt2Text.text = randomizedAnswers[1];
        alt3Text.text = randomizedAnswers[2];
        alt4Text.text = randomizedAnswers[3];

        answer = humor + 1;
        if(humor >= arrAnswers.Length)
        {
            strAnswer = "ANY";
        }
        else
        {
            strAnswer = arrAnswers[humor];
        }
    }

    public static string[] RandomizedArray(string[] sequence)
    {
        string[] outputArray = sequence.ToArray();

        for (int i = 0; i < outputArray.Length - 1; i += 1)
        {
            int swapIndex = Random.Range(i, outputArray.Length);
            if (swapIndex != i) {
                string temp = outputArray[i];
                outputArray[i] = outputArray[swapIndex];
                outputArray[swapIndex] = temp;
            }
        }
        return outputArray;
    }
    #endregion

    #region Output e Feedback de Respostas
    public void ReceiveAnswer(TextMesh receivedAnswer)
    {
        answered = true;

        if(receivedAnswer.text == strAnswer || answer >= 5)
        {
			foreach(QuestionAltButton btn in answerGroup.GetComponentsInChildren<QuestionAltButton>())
			{
				btn.collider.enabled = true;
				btn.gameObject.renderer.material.color = Color.white;
			}

            receptor.dialogueSuccess = true;
        }
        else
        {
            receptor.DamagePlayerDialogue();
			receptor.anim.SetTrigger("wrongAnswer");
            
			wrongTries++;
			if(wrongTries < 3 && !playerMeter.gameOver )
			{
				receivedAnswer.transform.parent.collider.enabled = false;
				receivedAnswer.transform.parent.renderer.material.color = Color.red;
				receivedAnswer.GetComponentInParent<QuestionAltButton>().numKey = 0;
				answered = false;
				elapsedTime = 0;

				StartCoroutine(WrongAnswerShake());
				StartCoroutine(WrongAnswerBlink());

			}
			else if (wrongTries >= 3 || playerMeter.gameOver)
			{
	            StartCoroutine(WrongAnswerMove());
	            StartCoroutine(WrongAnswerShake());
	            StartCoroutine(WrongAnswerBlink());
			}
        }
    }

    IEnumerator WrongAnswerShake()
    {
		iTween.ShakePosition(questionBox, new Vector3(0.03f, 0.03f), 0.4f);
		iTween.ShakePosition(answerGroup, new Vector3(0.03f, 0.03f), 0.4f);
        yield return null;
    }

    IEnumerator WrongAnswerMove()
    {
		iTween.ScaleTo(questionBox, iTween.Hash(
			"scale", Vector3.zero,
			"time", 1f,
			"onupdatetarget", questionBox,
			"easetype", iTween.EaseType.easeInExpo));

		iTween.MoveTo(answerGroup, iTween.Hash(
			"position", new Vector3(answerGroup.transform.position.x + 0.8f, answerGroup.transform.position.y, answerGroup.transform.position.z),
			"time", 1f,
			"onupdatetarget", answerGroup,
			"oncomplete", "RespawnWindow",
			"oncompletetarget", gameObject,
			"easetype", iTween.EaseType.easeInExpo));

		foreach(QuestionAltButton btn in answerGroup.GetComponentsInChildren<QuestionAltButton>())
		{
			btn.collider.enabled = true;
			btn.gameObject.renderer.material.color = Color.white;
		}

        yield return null;
    }

    IEnumerator WrongAnswerBlink () 
    {
		iTween.ColorFrom(questionBox.transform.Find("QuestionBG").gameObject, iTween.Hash(
			"color", Color.red,
            "time", 0.8f,
			"onupdatetarget", questionBox.transform.Find("QuestionBG").gameObject));

		foreach(QuestionAltButton btn in answerGroup.GetComponentsInChildren<QuestionAltButton>())
		{
			if(btn.numKey != 0)
			{
				btn.renderer.material.color = Color.white;
			}
		}

		iTween.ColorFrom(answerGroup, iTween.Hash(
			"color", Color.red,
			"time", 0.8f,
			"onupdatetarget", answerGroup));
        yield return null;
    }
    #endregion

    #region Funções de Reinício da Janela
    void ResetWindow()
    {
        if(receptor)
        {
            receptor.ResetAnswer();
        }
    }
    
    void RespawnWindow()
    {
		iTween.Stop(questionBox);
		iTween.Stop(answerGroup);

        gameObject.SetActive(false);
    }
    #endregion
}
