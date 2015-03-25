using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager: MonoBehaviour {

    public GameObject NPCContainer;

    public IA[] arrayNPC
    {
        get;
        set;
    }
	
    public float timeElapsed
	{
		get;
		set;
	}

	[SerializeField] public int stageTimeLimit;

    public int timesSpotted
    {
        get;
        set;
    }

	//Stage-specific values
	[HideInInspector]
	public int VA = 0;

	[HideInInspector]
	public List<PopupNote> greenNotesFound;

	public Light NorthLight, SouthLight;
	float lowLightFactor = 4;
	float iceLightFactor = 2f;
	float floodLightFactor = 1f;

	[SerializeField]
	Color sector1Tint, sector2Tint, sector3Tint, sector4Tint, sector5Tint;

	PlayerControl player;

	GameObject seenIcon, postItIcon, ghostIcon, clockIcon;

    public void Awake()
    {
        arrayNPC = NPCContainer.GetComponentsInChildren<IA>();

		player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();

		seenIcon= GameObject.Find("Seen Icon");
		clockIcon = GameObject.Find("Clock Icon");
		ghostIcon = GameObject.Find("Ghost Icon");
		postItIcon = GameObject.Find("PostIt Icon");

		seenIcon.SetActive(false);
		postItIcon.SetActive(false);

		var mainCam = Camera.main;

		int stageNum;
		bool parsedStageInt = int.TryParse(Application.loadedLevelName, out stageNum);

		if(parsedStageInt)
		{
			if(stageNum > 5 && stageNum <= 10)
			{
				RenderSettings.ambientLight = mainCam.backgroundColor = sector2Tint;
				 
			}
			else if(stageNum > 10 && stageNum <= 15)
			{
				RenderSettings.ambientLight = mainCam.backgroundColor = sector3Tint;
			}
			else if(stageNum > 15 && stageNum <= 20)
			{
				RenderSettings.ambientLight = mainCam.backgroundColor = sector4Tint;
			}
			else if(stageNum > 20 && stageNum <= 25)
			{
				RenderSettings.ambientLight = mainCam.backgroundColor = sector5Tint;
			}
			else if (stageNum <=5)
			{
				RenderSettings.ambientLight = mainCam.backgroundColor = sector1Tint;
			}
		}
		else
		{
			RenderSettings.ambientLight = mainCam.backgroundColor = sector1Tint;
		}

		if(VA == 1)
		{
			var lowLight = RenderSettings.ambientLight;
			lowLight.r = lowLight.r/lowLightFactor;
			lowLight.g = lowLight.g/lowLightFactor;
			lowLight.b = lowLight.b/lowLightFactor;
			RenderSettings.ambientLight  = mainCam.backgroundColor = lowLight;

			NorthLight.intensity = 0.04f;
			SouthLight.intensity = 0.04f;
		}else if(VA == 2)
		{
			var iceLight = RenderSettings.ambientLight;
			iceLight.r = iceLight.r * iceLightFactor;
			iceLight.g = iceLight.g * iceLightFactor;
			iceLight.b = iceLight.b * iceLightFactor;
			RenderSettings.ambientLight  = mainCam.backgroundColor = iceLight;
			
			NorthLight.intensity = 0.13f;
			SouthLight.intensity = 0.13f;
		}
		else if(VA == 3)
		{
			var floodLight = RenderSettings.ambientLight;
			floodLight.r = floodLight.r/(floodLightFactor);
			floodLight.g = floodLight.g/(floodLightFactor)* 1.2f;
			floodLight.b = floodLight.b/(floodLightFactor)* 1.2f;
			RenderSettings.ambientLight = mainCam.backgroundColor = floodLight;
			
			NorthLight.intensity = 0.08f;
			SouthLight.intensity = 0.08f;
		}

		if(stageNum == 15)
		{
			var lowLight = RenderSettings.ambientLight;
			lowLight.r = lowLight.r/(lowLightFactor/2)* 0.7f;
			lowLight.g = lowLight.g/lowLightFactor * 0.7f;
			lowLight.b = lowLight.b/(lowLightFactor/4)* 0.7f;
			RenderSettings.ambientLight = mainCam.backgroundColor = lowLight;
			
			NorthLight.intensity = 0.04f;
			SouthLight.intensity = 0.04f;
		}
    }

    public void Update()
    {
        timeElapsed = timeElapsed + Time.deltaTime;

        string minutes = Mathf.Floor(timeElapsed / 60).ToString("00");
        string seconds = Mathf.Floor(timeElapsed % 60).ToString("00");

//        print("Tempo: " + string.Format("{0:0}:{1:00}", minutes, seconds) + 
//              "|Exposições: " + timesSpotted );

		seenIcon.SetActive(player.beingSeen);

		if(timesSpotted > 0 && ghostIcon.activeSelf)
		{
			ghostIcon.SetActive(false);
		}

		if(timeElapsed > stageTimeLimit && clockIcon.activeSelf)
		{
			clockIcon.SetActive(false);
		}

		if(greenNotesFound.Count == 3 && !postItIcon.activeSelf)
		{
			postItIcon.SetActive(true);
		}
    }
}
