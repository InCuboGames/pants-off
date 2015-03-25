using UnityEngine;
using System.Collections;

public class IntroPanCamera : MonoBehaviour {

    [SerializeField]
    Transform objective;
    
    [SerializeField]
    float time;

    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject HUDCam;

    [SerializeField]
    GameObject introCam;

    [SerializeField]
    GameObject upperBar, lowerBar;

    Transform playerStartPos;

    bool panning = false;
    bool canSkip = false;

    void Start()
    {
		playerStartPos = GameObject.Find("Player Start").transform;

        Screen.lockCursor = true;
        GetComponent<MouseOrbit>().enabled = false;
        GetComponent<PauseManager>().enabled = false;
        player.GetComponent<PlayerControl>().canControl = false;
        HUDCam.SetActive(false);

        transform.position = new Vector3(objective.position.x, objective.position.y + 9, objective.position.z - 5);
        transform.LookAt(objective);
        
        StartCoroutine(PanCamera());

        GoUp(upperBar);
        GoDown(lowerBar);
    }

    void Update()
    {
        if(Input.anyKeyDown ||Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            if(canSkip)
            {
                EndPan();
            }
        }
    }
	
	IEnumerator PanCamera () 
    {
        panning = true;
		Vector3 endPos = new Vector3(playerStartPos.position.x, playerStartPos.position.y + 9, playerStartPos.position.z - 4);

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", endPos, 
            "time", time, 
            "easetype", iTween.EaseType.easeInOutCubic,
            "oncomplete", "EndPan", 
            "oncompletetarget", gameObject));

        yield return new WaitForSeconds(5);
        if(panning)
        {
            player.GetComponent<AutoControl>().enabled = true;
        }

        yield return null;
	}

    void EndPan()
    {
        panning = false;
        canSkip = false;

        GoUp(lowerBar);
        GoDown(upperBar);

//        yield return new WaitForSeconds(1);
//
//        //StartStage();
    }


    void StartStage()
    {
        player.GetComponent<NavMeshAgent>().Stop();
        player.GetComponent<AutoControl>().enabled = false;
        player.GetComponent<NavMeshAgent>().enabled = false;
        player.GetComponent<PlayerControl>().canControl = true;
        player.GetComponent<PlatformInputController>().enabled = true;
        player.GetComponent<NavMeshObstacle>().enabled = true;
        player.transform.position = playerStartPos.position;
        HUDCam.SetActive(true);
        introCam.SetActive(false);
        GetComponent<MouseOrbit>().enabled = true;
        GetComponent<PauseManager>().enabled = true;

		if(GameObject.Find("Stage Manager").GetComponent<StageManager>().VA == 3)
		{
			GetComponent<CameraWaterEffects>().enabled = true;
		}

        iTween.Stop(gameObject);
        this.enabled = false;
    }

    void GoUp (GameObject go) {
        
        iTween.MoveTo(go, iTween.Hash("y", go.transform.position.y + 0.4f,
                                        "time", 1, "easetype", iTween.EaseType.easeInOutQuad));                              
    }

    void GoDown (GameObject go) {
        
        iTween.MoveTo(go, iTween.Hash("y", go.transform.position.y - 0.4f,
                                      "time", 1, "easetype", iTween.EaseType.easeInOutQuad,
                                      "oncomplete", "BarEnded", "oncompletetarget", gameObject));                                  
    }

    void BarEnded()
    {
        canSkip = true;

        if(!panning)
        {
            StartStage();
        }
    }
}
