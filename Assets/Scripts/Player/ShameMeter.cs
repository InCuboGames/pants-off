using UnityEngine;
using System.Collections;

public class ShameMeter : MonoBehaviour {

    private int maxShame = 100;
    private int floorShame = 0;
    [System.NonSerialized]
    public float shame;
    [System.NonSerialized]
    public int shameLevel;
    private float shameFraction;

    GameObject shameMeter;
    GameObject shameBar;
    GameObject HUDCircleFade;
    public Texture2D barVexame;

    AudioSource musicSource;

    GameObject GameOverText;
	GameObject LevelEndText;
	public GameObject RankingScreen;

    GameObject shameText;
	public Texture2D[] shameTexts;
	//textEncabulado, textEnvergonhado, textConstrangido, textHumilhado, textVexame;

	GameObject shameBorder;
	public Texture2D[] shameBorders;
	
    GameObject shameFace;
	public Texture2D[] shameFaces;
	//faceEncabulado, faceEnvergonhado, faceConstrangido, faceHumilhado, faceVexame;

	[SerializeField]
	GameObject
		playerModel;

	[HideInInspector]
	public bool gameOver;
	[HideInInspector]
	public bool levelEnded;

//    private bool reset = false;
//    private float resetTimer = 0;
//    private bool fadingText, cutoffFade = false;

	[HideInInspector]
    public bool onDialogue = false;
	
    StageManager stageManager;

	void Awake () 
    {
		if(GameObject.Find("Stage Manager"))
		{
			stageManager = GameObject.Find("Stage Manager").GetComponent<StageManager>();
		}

		shameMeter = GameObject.Find("Shame Meter");
		shameBar = GameObject.Find("MeterBar");
		HUDCircleFade = GameObject.Find("HUD Circle Fade");

		musicSource = Camera.main.GetComponent<AudioSource>();

		GameOverText = GameObject.Find("GameOverText");
		LevelEndText = GameObject.Find("LevelEndText");

		shameText = GameObject.Find("ShameText");
		shameBorder = GameObject.Find("ShameBorder");
		shameFace = GameObject.Find("ShameFace");

        shame = 0;

        GameOverText.SetActive(false);
		LevelEndText.SetActive(false);
	}
	
	void Update ()
    {
		foreach(SkinnedMeshRenderer mesh in playerModel.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			mesh.enabled = !onDialogue;
		}

        if (shameLevel <= 3 && !onDialogue)
        {
            shame -= Time.deltaTime;// *2;
        }

        var percent = (shame / maxShame);
        shameBar.renderer.material.SetFloat("_Cutoff", Mathf.Clamp(1 - percent, 0.001f, 0.999f));

        shame = Mathf.Clamp(shame, floorShame, maxShame);

        #region Níveis de Vergonha
        if (shame >= 20 && shame < 50 && shameLevel != 1)
        {
            floorShame = 20;
            ShameLevelUp();
        }
        else if (shame >= 50 && shame < 75 && shameLevel != 2)
        {
            floorShame = 50;
            ShameLevelUp();
        }
        else if (shame >= 80 && shame < 100 && shameLevel != 3)
        {
            floorShame = 80;
            ShameLevelUp();
        }
        else if (shame >= 100 && shameLevel != 4)
        {
            ShameLevelUp();
            GameOver();
        }
        #endregion

        #region Texto e Rosto
        {
			if (shameLevel == 0 && shameText.renderer.material.mainTexture != shameTexts[0])
            {
				shameText.renderer.material.mainTexture = shameTexts[shameLevel];
				shameBorder.renderer.material.mainTexture = shameBorders[shameLevel];
				shameFace.renderer.material.mainTexture = shameFaces[shameLevel];
            }
			else if (shameLevel == 1 && shameText.renderer.material.mainTexture != shameTexts[1])
            {
				shameText.renderer.material.mainTexture = shameTexts[shameLevel];
				shameBorder.renderer.material.mainTexture = shameBorders[shameLevel];
				shameFace.renderer.material.mainTexture = shameFaces[shameLevel];
            }
			else if (shameLevel == 2 && shameText.renderer.material.mainTexture != shameTexts[2])
            {
				shameText.renderer.material.mainTexture = shameTexts[shameLevel];
				shameBorder.renderer.material.mainTexture = shameBorders[shameLevel];
				shameFace.renderer.material.mainTexture = shameFaces[shameLevel];
            }
			else if (shameLevel == 3 && shameText.renderer.material.mainTexture != shameTexts[3])
            {
				shameText.renderer.material.mainTexture = shameTexts[shameLevel];
				shameBorder.renderer.material.mainTexture = shameBorders[shameLevel];
				shameFace.renderer.material.mainTexture = shameFaces[shameLevel];
            }
			else if (shameLevel == 4 && shameText.renderer.material.mainTexture != shameTexts[4])
            {
				shameText.renderer.material.mainTexture = shameTexts[shameLevel];
				shameBorder.renderer.material.mainTexture = shameBorders[shameLevel];
				shameFace.renderer.material.mainTexture = shameFaces[shameLevel];
                shameBar.renderer.material.mainTexture = barVexame;
            }
        }
        #endregion

		#region CHEATS

		if(GlobalDataManager.enableCheats)
		{
	        if (Input.GetKeyDown(KeyCode.M))
	        {
	            DrinkTea();
	        }

			if(Input.GetKeyDown(KeyCode.H))
			{
				LevelEnd();
			}

			if(Input.GetKeyDown(KeyCode.P))
			{
				Application.LoadLevel(Application.loadedLevel+1);
			}
		}

		#endregion
    }

    void ShameLevelUp()
    {
        shameLevel++;
        iTween.ShakePosition(shameMeter, new Vector3(0.02f, 0.02f), 0.6f);
        shame++;
    }

    void DrinkTea()
    {
        if (shame < 20)
        {
            floorShame = 0;
            shame = 0;
        }
        else if (shame >= 20 && shame < 50)
        {
            floorShame = 0;
            shame = 19;

        }
        else if (shame >= 50 && shame < 75)
        {
            floorShame = 20;
            shame = 49;
        }
        else if (shame >= 80 && shame < 100)
        {
            floorShame = 50;
            shame = 79;
        }

        if (shameLevel > 0)
        {
            shameLevel--;
        }
    }

    void GameOver()
    {
		gameOver = true;

        foreach (IA npc in stageManager.arrayNPC)
        {
            npc.gameObject.GetComponent<NavMeshAgent>().Stop();
            npc.StopAllCoroutines();
        }

		var audioSources = GameObject.FindObjectsOfType<AudioSource>();
		
		foreach(AudioSource audio in audioSources)
		{
			audio.enabled = false;
		}
		
		Camera.main.GetComponent<AudioSource>().enabled = true;
		if(Camera.main.GetComponent<AudioLowPassFilter>())Camera.main.GetComponent<AudioLowPassFilter>().enabled = false;

        musicSource.GetComponent<MusicManager>().PlayGameOver();

        GetComponent<PlayerControl>().canControl = false;
        Camera.main.GetComponent<PauseManager>().enabled = false;

		GetComponent<PlayerControl>().anim.SetTrigger("gameOver");

        StartCoroutine(GameOverSequence());
    }

    #region Sequência de Game Over
    IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(3);
        CutoffCircleFade();

        yield return new WaitForSeconds(1.5f);
        GameOverText.SetActive(true);

        yield return new WaitForSeconds(3);
            FadeGameOverText();

        yield return new WaitForSeconds(3);
        Application.LoadLevel(Application.loadedLevel);

        yield return null;
    }
	#endregion

	public void LevelEnd()
	{
		levelEnded = true;

		foreach (IA npc in stageManager.arrayNPC)
		{
			npc.gameObject.GetComponent<NavMeshAgent>().Stop();
			npc.StopAllCoroutines();
		}

		var audioSources = GameObject.FindObjectsOfType<AudioSource>();
		
		foreach(AudioSource audio in audioSources)
		{
			audio.enabled = false;
		}
		
		Camera.main.GetComponent<AudioSource>().enabled = true;
		if(Camera.main.GetComponent<CameraWaterEffects>())Camera.main.GetComponent<CameraWaterEffects>().enabled = false;
		if(Camera.main.GetComponent<AudioLowPassFilter>())Camera.main.GetComponent<AudioLowPassFilter>().enabled = false;

		musicSource.GetComponent<MusicManager>().PlayLevelEnd();
		
		StartCoroutine(LevelEndSequence());
	}
	
	#region Sequência de Level End
	IEnumerator LevelEndSequence()
	{
		yield return new WaitForSeconds(1.5f);
		CutoffCircleFade();
		
		yield return new WaitForSeconds(1.5f);
		LevelEndText.SetActive(true);
		
		yield return new WaitForSeconds(2);
		RankingScreen.SetActive(true);
		HUDCircleFade.SetActive(false);

		yield return null;
	}
    #endregion
    
    void CutoffCircleFade()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
        "from", 1,
        "to", 0,
        "time", 1f,
        "onupdatetarget", gameObject,
        "onupdate", "SetCircleFadeCutoff",
        "easetype", iTween.EaseType.easeOutExpo));
    }

    void SetCircleFadeCutoff(float tweenedValue)
    {
        HUDCircleFade.renderer.material.SetFloat("_Cutoff", tweenedValue);
    }

    void FadeGameOverText()
    {
        iTween.ValueTo(GameOverText, iTween.Hash(
        "from", 0.5f,
        "to", 0,
        "time", 1f,
        "onupdatetarget", gameObject,
        "onupdate", "SetGameOverTextAlpha",
        "easetype", iTween.EaseType.easeOutSine));
    }

    void SetGameOverTextAlpha(float tweenedValue)
    {
        GameOverText.renderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, tweenedValue));
    }
    
}
