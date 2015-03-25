using UnityEngine;
using System.Collections;

public class GraphicalOptionsButton: MonoBehaviour {

	[SerializeField]
		Sprite hoverSprite, selectedSprite;

	Sprite unselectedSprite;

	GraphicOptionsManager manager;
	MainMenuCameraController controller;

	[SerializeField]
	int option = 0;

	bool selected = false;
	bool bypassCameraIndex = false;

	public bool isQuality = false;

	// Use this for initialization
	void Awake () 
	{
		controller = Camera.main.GetComponent<MainMenuCameraController>();
		manager = Camera.main.GetComponent<GraphicOptionsManager>();
		unselectedSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
	}

	void OnEnable()
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = unselectedSprite;
	}
	
	// Update is called once per frame
	void OnMouseOver () 
	{
		if(!selected && controller.camIndex == 3)gameObject.GetComponent<SpriteRenderer>().sprite = hoverSprite;
	}

	void OnMouseExit () 
	{
		if(!selected && controller.camIndex == 3)gameObject.GetComponent<SpriteRenderer>().sprite = unselectedSprite;
    }

	void OnMouseDown()
	{
		if(!selected && controller.camIndex == 3 || bypassCameraIndex)
		{
			switch(option)
			{
				case 111:
				Screen.SetResolution(640, 360, true);
				break;
				case 112:
				Screen.SetResolution(640, 360, false);
	        	break;

				case 121:
				Screen.SetResolution(960, 540, true);
				break;
				case 122:
				Screen.SetResolution(960, 540, false);
	        break;

				case 131:
				Screen.SetResolution(1280, 720, true);
				break;
				case 132:
				Screen.SetResolution(1280, 720, false);
	        break;

				case 141:
				Screen.SetResolution(1920, 1080, true);
				break;
				case 142:
				Screen.SetResolution(1920, 1080, false);
	        break;
	            
	        case 21:
				QualitySettings.SetQualityLevel(0, true);
				break;
				case 22:
				QualitySettings.SetQualityLevel(1, true);
				break;
				case 23:
				QualitySettings.SetQualityLevel(2, true);
	            break;
	      		case 24:
				QualitySettings.SetQualityLevel(3, true);
	        break;
	    	}

			if(isQuality)
			{
				manager.ClearQuality();
			}
			else
			{
				manager.ClearResolution();
			}

			selected = true;
			gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite;
		}
	}

	public void Unselect()
	{
		selected = false;
        OnMouseExit();
    }

	public void Select()
	{
		bypassCameraIndex = true;
		OnMouseDown();
		bypassCameraIndex = false;
    }
}
