using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour {
    
    public bool paused
	{
		get;
		set;
	}
    public bool canPause
    {
        get;
        set;
    }

	public bool cameraTweening
	{
		get;
		set;
	}


	GameObject pauseScreen;

    [SerializeField]
    ShameMeter shameScript;
    
    // Use this for initialization
    void Awake () {
        paused = false;
		canPause = true;

		pauseScreen = GameObject.Find("Pause Screen");

    }
    
    // Update is called once per frame
    void Update () {
            
        canPause = !shameScript.onDialogue && !shameScript.gameOver && !shameScript.levelEnded && !cameraTweening;

        if(canPause && !paused)
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) paused = !paused;
        }
        
        if (paused)
        {
			if(!pauseScreen.activeSelf)
            {
				pauseScreen.SetActive(true);
				Screen.lockCursor = false;
            }
        }
        else
        {
			if(pauseScreen.activeSelf)
			{
				pauseScreen.SetActive(false);
			}
        }
    }
}
