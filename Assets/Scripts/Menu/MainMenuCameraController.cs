using UnityEngine;
using System.Collections;

public class MainMenuCameraController : MonoBehaviour
{

    [SerializeField]
    public Transform[]
        cameraAnchors;

    [SerializeField]
    MainMenuNPC
        mainMenuNPC;

    [SerializeField]
    GameObject
        fadeSprite;

    [SerializeField]
    MonitorTextWriter
        monitorText;

    [SerializeField]
    ElevatorDoorOpen
        rightDoor, leftDoor;

	[SerializeField]
	Color mainColor;
    
    float scrollValue;
    bool showScrollbar = false;
    Vector3 originalPos;

    int openedSectors = 1;

	public bool onTutorial = false;

    public bool mapScreen
    {
        get;
        set;
    }

    public bool tweening
    {
        get;
        private set;
    }

    public int camIndex
    {
        get;
        private set;
    }

    public string stageToLoad
    {
        get;
        set;
    }

    int indexToSet;

	[SerializeField]
	StageMenuButton[]
		stageButtons;

	float resetTime;
	bool fading = false;

    // Use this for initialization
    void Start()
    {
        Screen.lockCursor = true;

        mainMenuNPC.GetComponent<PlayerControl>().canControl = false;

        transform.position = cameraAnchors [0].position;
        transform.rotation = cameraAnchors [0].rotation;

        fadeSprite.SetActive(true);

        indexToSet = 0;
        camIndex = 0;
        tweening = true;
        TweenTo(cameraAnchors [1], 7.5f);
        monitorText.ChangeText("");

		fading = false;
    }
	
    // Update is called once per frame
    void Update()
    {

        if ((Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            if (camIndex == 1 && !tweening)
            {
                TweenTo(cameraAnchors [camIndex + 1], 1.5f);
                if (!mainMenuNPC.WalkingAround)
                {
                    Screen.lockCursor = false;
                    mainMenuNPC.WalkingAround = true;
                }
            } else if (camIndex == 0)
            {
                iTween.Stop(gameObject);
                fadeSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                transform.position = cameraAnchors [1].position;
                transform.rotation = cameraAnchors [1].rotation;
                indexToSet = 1;
                SetIndex();
            }
        }

		if(camIndex == 1 && !tweening)
		{
			if(resetTime < 20f)
			{
				resetTime+= Time.deltaTime;
				Debug.Log(resetTime);
			}
			else
			{
				if(!fading)
				{
					StartCoroutine(FadeBackToIntro());
					fading = true;
				}
			}
		}
		else
		{
			if(resetTime > 0)
			{
				resetTime = 0;
			}
		}
    }

	IEnumerator FadeBackToIntro()
	{
		iTween.AudioTo(gameObject, 0, 1, 3f);
		fadeSprite.GetComponent<MainMenuFadeIn>().FadeOut();
		yield return  new WaitForSeconds(4);
		Application.LoadLevel(2);
	}

    public void MenuOption(int optionIndex)
    {
		if(fading)
		{
			return;
		}

        indexToSet = optionIndex;
        switch (optionIndex)
        {
            case 1:
                TweenTo(cameraAnchors [optionIndex], 1f, 1f, false);
                if (camIndex == 2 && mainMenuNPC.WalkingAround)
                {
					onTutorial = false;
                    mainMenuNPC.WalkingAround = false;
					monitorText.ChangeText("");
                }
                break;
            case 2:
                camIndex = 1;
				if(onTutorial)
				{
                	TweenTo(cameraAnchors [optionIndex], 3f);
					onTutorial = false;
				}
				else
				{
					TweenTo(cameraAnchors [optionIndex], 1.5f);
				}

                if (!mainMenuNPC.WalkingAround)
                {
                    Screen.lockCursor = false;
                    mainMenuNPC.WalkingAround = true;
                }
                break;
            case 4:
                camIndex = 3;
                StartCoroutine(rightDoor.OpenDoor());
                StartCoroutine(leftDoor.OpenDoor());
                TweenTo(cameraAnchors [optionIndex], 7f, 2f);
                FadeOutVolume();
                break;
            case 5:
                TweenTo(cameraAnchors [optionIndex], 3f);
                break;
            case 7:
				onTutorial = true;
				mainMenuNPC.WalkingAround = false;
				TweenTo(cameraAnchors [7], 6f, 6f);
                break;
            default:
                TweenTo(cameraAnchors [optionIndex], 1.5f);
                break;
        }

        if (!mapScreen)
        {
            showScrollbar = false;
            scrollValue = 0;
        }
    }

    void TweenTo(Transform target, float moveTime = 1, float rotTime = 0, bool forward = true)
    {
        tweening = true;

        if (rotTime == 0)
            rotTime = moveTime;

        if (forward)
        {
            indexToSet = camIndex + 1;
        } else
        {
            indexToSet = camIndex - 1;
        }

        iTween.MoveTo(gameObject, iTween.Hash("position", target.position, "time", moveTime, "easetype", iTween.EaseType.easeOutCubic, "oncomplete", "SetIndex", "oncompletetarget", gameObject));
        iTween.RotateTo(gameObject, iTween.Hash("rotation", target.localEulerAngles, "time", rotTime, "easetype", iTween.EaseType.easeOutCubic));
    }

    void SetIndex()
    {
        camIndex = indexToSet;
        tweening = false;

        if (camIndex == 1)
        {
            Screen.lockCursor = true;
        } else if (camIndex == 2)
        {
            monitorText.ChangeText("Menu Principal");
			monitorText.gameObject.renderer.material.color = mainColor;
            
        } else if (camIndex == 3)
        {
            monitorText.ChangeText("");
            originalPos = transform.position;
        } else
        {
            monitorText.ChangeText("");
        }

        
        if (camIndex == 4)
        {
            iTween.Stop(gameObject);
            if (stageToLoad != string.Empty)
            {
                Application.LoadLevel(stageToLoad);
            } else
            {
                Application.LoadLevel("Main Menu");
            }
        }
    }

	public void CalculateOpenedSectors()
	{
		int farthestStage = GlobalDataManager.GameData.UnlockedStages[0];

		foreach (int stage in GlobalDataManager.GameData.UnlockedStages)
		{
			farthestStage = stage > farthestStage ? stage : farthestStage;
		}

		if (farthestStage >= 0)openedSectors = 1;
		if (farthestStage >= 5)openedSectors = 2;
		if (farthestStage >= 10)openedSectors = 3;
		if (farthestStage >= 15)openedSectors = 4;
		if (farthestStage >= 20)openedSectors = 5;

		foreach(StageMenuButton btn in stageButtons)
		{
			btn.RefreshState();
		}
	}

    void OnGUI()
    {
        float maxScroll;

        switch (openedSectors)
        {
            case 1:
                maxScroll = 0;
                break;
            case 2:
                maxScroll = 4.5f;
                break;
            case 3:
                maxScroll = 9.5f;
                break;
            case 4:
                maxScroll = 14.5f;
                break;
            case 5:
                maxScroll = 21f;
                break;
            default:
                maxScroll = 0;
                break;
        }

        if (mapScreen && camIndex == 3)
        {
            if (maxScroll <= 0)
            {
                scrollValue = 0;
                transform.position = new Vector3(originalPos.x, originalPos.y, originalPos.z); 
                showScrollbar = false;
            } else
            {
                showScrollbar = true;
            }

            if (showScrollbar)
            {
                GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.05f;
                GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.05f;

                scrollValue += Input.GetAxis("Mouse ScrollWheel") * 2;

                scrollValue = GUI.VerticalScrollbar(new Rect(Screen.width * 0.9f, Screen.height * 0.05f, Screen.width * 0.05f, Screen.height * 0.9f), scrollValue, 1F, 1.0F, -maxScroll);

                transform.position = new Vector3(originalPos.x, originalPos.y + (scrollValue / 10), originalPos.z); 
            }
        }
    }

    void FadeOutVolume()
    {
        iTween.AudioTo(gameObject, 0, 1, 6f);
    }

    public void StartTutorial()
    {
        Screen.lockCursor = true;

        mainMenuNPC.GetComponent<NavMeshAgent>().Stop();

        mainMenuNPC.transform.position = new Vector3(mainMenuNPC.transform.position.x, mainMenuNPC.transform.position.y + 0.3f, mainMenuNPC.transform.position.z);
        mainMenuNPC.gameObject.GetComponent<CharacterMotor>().enabled = true;
        //mainMenuNPC.gameObject.GetComponent<ShameMeter>().enabled = true;
        mainMenuNPC.gameObject.GetComponent<PlayerControl>().canControl = true;
        mainMenuNPC.gameObject.GetComponent<PlatformInputController>().enabled = true;
        mainMenuNPC.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        mainMenuNPC.gameObject.GetComponent<MainMenuNPC>().enabled = false;

        GetComponent<MouseOrbit>().enabled = true;
    }

    public void EndTutorial()
    {
        GetComponent<MouseOrbit>().enabled = false;

        Screen.lockCursor = false;

        //mainMenuNPC.transform.position = new Vector3(mainMenuNPC.transform.position.x, mainMenuNPC.transform.position.y + 0.3f, mainMenuNPC.transform.position.z);
        mainMenuNPC.gameObject.GetComponent<CharacterMotor>().enabled = false;
        //mainMenuNPC.gameObject.GetComponent<ShameMeter>().enabled = true;
        mainMenuNPC.gameObject.GetComponent<PlayerControl>().canControl = false;
        mainMenuNPC.gameObject.GetComponent<PlatformInputController>().enabled = false;
        mainMenuNPC.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        mainMenuNPC.gameObject.GetComponent<MainMenuNPC>().enabled = true;

        MenuOption(2);
    }
}
