using UnityEngine;
using System.Collections;

public class RankingScreen : MonoBehaviour {

	GameObject HUDOverlay;

	public GameObject LevelEndText;

	public GameObject textFinalPos;

	public GameObject Aurelio;
	public GameObject medalColumn;
	public GameObject AurelioModel;
	public GameObject buttonGroup;

	public TextMesh time;
	public TextMesh exposures;
	public TextMesh postits;

	public GameObject[] medals;
	public GameObject[] failParticles;

	StageManager stageManager;

	[SerializeField] AudioClip bleep;

	float loopDelay = 0.04f;
	int loop = 0;
	int maxLoops = 25;

	float waitDelay = 1f;

	string maxMinutes, maxSeconds, elapsedMinutes, elapsedSeconds;
	float timeElapsed;
	int timesSpotted = 0;
	int postItsFound = 0;

	int medalsEarned = 0;

	public AudioClip perfect, good, bad, fail;

	bool allowInput;

	void OnEnable () 
	{
		stageManager = GameObject.Find("Stage Manager").GetComponent<StageManager>();

		if(GameObject.Find("HUD Overlay"))
		{
			HUDOverlay = GameObject.Find("HUD Overlay");
			HUDOverlay.SetActive(false);
		}

		Aurelio.SetActive(false);
		medalColumn.SetActive(false);
		AurelioModel.SetActive(false);
		buttonGroup.SetActive(false);

		Camera.main.GetComponent<MusicManager>().PlayRankingScreen();

		iTween.MoveTo(LevelEndText, iTween.Hash(
			"position", textFinalPos.transform.position,
			"time", 2.5f,
			"easetype", iTween.EaseType.easeInOutQuad));

		iTween.ScaleTo(LevelEndText, iTween.Hash(
			"scale", textFinalPos.transform.localScale,
			"time", 2.5f,
			"easetype", iTween.EaseType.easeInOutQuad));

		timeElapsed = stageManager.timeElapsed;
		timesSpotted = stageManager.timesSpotted;

		elapsedMinutes = Mathf.Floor(timeElapsed / 60).ToString("00");
		elapsedSeconds = Mathf.Floor(timeElapsed % 60).ToString("00");

		maxMinutes = Mathf.Floor(stageManager.stageTimeLimit / 60).ToString("00");
		maxSeconds = Mathf.Floor(stageManager.stageTimeLimit % 60).ToString("00");

		time.text = string.Format(0 + ":" + 00 + " / " + maxMinutes + ":" + maxSeconds);

		postItsFound = stageManager.greenNotesFound.Count;

		StartCoroutine(RankingAnimation());
	}

	void Update()
	{
//		if(Input.GetKeyDown(KeyCode.Return) && !fastFoward)
//		{
//			fastFoward = true;
//			waitDelay = waitDelay/4;
//			loopDelay = loopDelay/4;
//			maxLoops = maxLoops/4;
//		}

		if(Input.GetKeyDown(KeyCode.T) && allowInput) {
			RetryStage ();

		}

		if(Input.GetKeyDown(KeyCode.E) && allowInput) {
			NextStage ();

		}

		if(Input.GetKeyDown(KeyCode.M) && allowInput) {
			ReturnToMenu ();
		}
	}

	public void NextStage ()
	{
		allowInput = false;
		EndStageSave (0);
	}

	public void RetryStage ()
	{
		allowInput = false;
		EndStageSave (1);
	}

	public void ReturnToMenu ()
	{
		allowInput = false;
		EndStageSave (2);
	}

	IEnumerator RankingAnimation()
	{
		yield return new WaitForSeconds(1f);

		medalColumn.SetActive(true);
		iTween.MoveFrom(medalColumn, iTween.Hash(
			"position", new Vector3(medalColumn.transform.position.x, medalColumn.transform.position.y + 1.2f, medalColumn.transform.position.z),
			"time", 1.3f,
			"easetype", iTween.EaseType.easeOutBounce));

		AurelioModel.SetActive(true);
		iTween.MoveFrom(AurelioModel, iTween.Hash(
			"position", new Vector3(AurelioModel.transform.position.x, AurelioModel.transform.position.y - 1.2f, AurelioModel.transform.position.z),
			"time", 2.5f,
			"easetype", iTween.EaseType.easeOutQuad));

		yield return new WaitForSeconds(waitDelay * 0.3f);

		Aurelio.SetActive(true);
		iTween.MoveFrom(Aurelio, iTween.Hash(
			"position", new Vector3(Aurelio.transform.position.x - 3, Aurelio.transform.position.y, Aurelio.transform.position.z),
			"time", 2f,
			"easetype", iTween.EaseType.easeInOutExpo));

		yield return new WaitForSeconds(waitDelay);

		StartCoroutine(RollStat(1, 0, 200));

		yield return new WaitForSeconds(waitDelay * 2f);
		
		StartCoroutine(RollStat(2, 0, 20));

		yield return new WaitForSeconds(waitDelay * 2f);
		
		StartCoroutine(RollStat(3, 0, 3));

		yield return new WaitForSeconds(waitDelay * 2.5f);

		EvaluateResults();


	}

	IEnumerator RollStat(int id, float min, float max)
	{
		yield return new WaitForSeconds(loopDelay);
		
		if (loop < maxLoops)
		{
			Camera.main.GetComponent<AudioSource>().PlayOneShot(bleep);
			switch (id)
			{
				case 1:
				var randomMinutes = Mathf.Floor(Random.Range(min, max) / 60).ToString("00");
				var randomSeconds = Mathf.Floor(Random.Range(min, max) % 60).ToString("00");
				time.text = string.Format(randomMinutes + ":" + randomSeconds + " / " + maxMinutes + ":" + maxSeconds);
				loop++;
				StartCoroutine(RollStat(1, 0, 200));
				break;

				case 2:
				var randomExposures = (int)Random.Range(min, max);
				exposures.text = randomExposures.ToString();
				loop++;
				StartCoroutine(RollStat(2, 0, 20));
				break;

				case 3:
				var randomPostIts = (int)Random.Range(min, max);
				postits.text = string.Format(randomPostIts + " / " + 3);
				loop++;
				StartCoroutine(RollStat(3, 0, 3));
				break;
			}

		}
		else
		{
			switch (id)
			{
				case 1:
				time.text = string.Format(elapsedMinutes + ":" + elapsedSeconds + " / " + maxMinutes + ":" + maxSeconds);

				if(timeElapsed <= stageManager.stageTimeLimit)
				{
					medals[0].SetActive(true);
					medalsEarned++;
				}
				else
				{
					failParticles[0].SetActive(true);
				}
				break;
				case 2:
				exposures.text = timesSpotted.ToString();
				
				if(timesSpotted <= 0)
				{
					medals[1].SetActive(true);
					medalsEarned++;
				}
				else
				{
					failParticles[1].SetActive(true);
				}
				break;
				case 3:
				postits.text = string.Format(postItsFound + " / " + 3);
				
				if(postItsFound >= 3)
				{
					medals[2].SetActive(true);
					medalsEarned++;
				}
				else
				{
					failParticles[2].SetActive(true);
				}
				break;
			}

			loop = 0;
			yield return null;
		}
	}

	void EvaluateResults ()
	{
		switch(medalsEarned)
		{
			case 0:
			Camera.main.GetComponent<AudioSource>().PlayOneShot(fail);
			AurelioModel.GetComponent<Animator>().SetTrigger("fail");
			break;
			case 1:
			Camera.main.GetComponent<AudioSource>().PlayOneShot(bad);
			AurelioModel.GetComponent<Animator>().SetTrigger("bad");
			break;
			case 2:
			Camera.main.GetComponent<AudioSource>().PlayOneShot(good);
			AurelioModel.GetComponent<Animator>().SetTrigger("good");
			break;
			case 3:
			Camera.main.GetComponent<AudioSource>().PlayOneShot(perfect);
			AurelioModel.GetComponent<Animator>().SetTrigger("perfect");
			break;
		}

		allowInput = true;
		Screen.lockCursor = false;
		
		buttonGroup.SetActive(true);
		iTween.MoveFrom(buttonGroup, iTween.Hash(
			"position", new Vector3(buttonGroup.transform.position.x, buttonGroup.transform.position.y - 1.5f, buttonGroup.transform.position.z),
			"time", 0.5f,
			"easetype", iTween.EaseType.easeOutQuad));
	}

 	void EndStageSave(int nextScreen)
	{
		int num;
		bool parsedStageInt = int.TryParse(Application.loadedLevelName, out num);
		if(parsedStageInt && GlobalDataManager.enableSaving)
		{
			int stageNum = num;
			if(stageNum < GlobalDataManager.UnlockableStages[GlobalDataManager.UnlockableStages.Count - 1])
			{
				//Save Score
				var currentStageIndex = GlobalDataManager.UnlockableStages.IndexOf(stageNum);
				var unlockedStage = (GlobalDataManager.UnlockableStages[currentStageIndex + 1]);
				print (unlockedStage);

				if(!GlobalDataManager.GameData.UnlockedStages.Contains(unlockedStage))
			  	{
					GlobalDataManager.GameData.UnlockedStages.Add(unlockedStage);
					SaveLoad.Instance.Save();
				}


				//SAVE SCORE HERE



				if(nextScreen == 0)
				{
					Application.LoadLevel(unlockedStage.ToString());
					//Application.LoadLevel(Application.loadedLevel + 1);
				}
				else if (nextScreen == 1)
				{
					Application.LoadLevel(Application.loadedLevelName);
				}
				else if (nextScreen == 2)
				{
					Application.LoadLevel("Main Menu");
				}
			}
			else if (GlobalDataManager.enableSaving)
			{
				//SAVE SCORE HERE
				SaveLoad.Instance.Save();
//				Application.LoadLevel("Main Menu");
				Application.LoadLevel(Application.loadedLevel + 1);
			}
			else
			{
//				Application.LoadLevel("Main Menu");
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
		else
		{
			Application.LoadLevel("Main Menu");
		}

	}
}
