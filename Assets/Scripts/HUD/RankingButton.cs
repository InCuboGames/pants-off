using UnityEngine;
using System.Collections;

public class RankingButton : MonoBehaviour {

	[SerializeField]
		Sprite selectSprite;

	Sprite unselectedSprite;

	[SerializeField]
	RankingScreen rankingScreen;

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
		switch(option)
		{
			case 1:
			rankingScreen.RetryStage();
			break;
			case 2:
			rankingScreen.NextStage();
        	break;
			case 3:
			rankingScreen.ReturnToMenu();
            break;
		}
	}
}
