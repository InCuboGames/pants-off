using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {

	[SerializeField]
		Sprite selectSprite;

	Sprite unselectedSprite;

	[SerializeField]
	PauseScreen pauseScreen;

	[SerializeField]
	int option = 0;

	// Use this for initialization
	void Awake () 
	{
		unselectedSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
	}

	void OnEnable()
	{

		gameObject.GetComponent<SpriteRenderer>().sprite = unselectedSprite;
	}
	
	// Update is called once per frame
	void OnMouseOver () 
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = selectSprite;
	}

	void OnMouseExit () 
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = unselectedSprite;
    }

	void OnMouseDown()
	{
		if(option < 5)
		{
			switch(option)
			{
				case 1:
				pauseScreen.Unpause();
				break;
				case 2:
				pauseScreen.OpenSubscreen(0);
	        	break;
				case 3:
				pauseScreen.OpenSubscreen(1);
	            break;
				case 4:
				pauseScreen.OpenSubscreen(2);
	            break;
			}
		}
		else if (option == 22 || option == 32  || option == 42)
		{
			pauseScreen.CloseSubscreens();
		}
		else
		{
			switch(option)
			{
				case 21:
				//pauseScreen.Unpause();
		        Application.LoadLevel(Application.loadedLevelName);
				break;
				case 31:
				//pauseScreen.Unpause();
				Application.LoadLevel("Main Menu");
				break;
	        	case 41:
				Application.Quit();
	            break;
            }
        }
	}
}
