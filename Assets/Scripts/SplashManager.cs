using UnityEngine;
using System.Collections;

public class SplashManager : MonoBehaviour {

	bool fading = false;
	public float screenHangTime;
	public bool thanksScreen = false;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		StartCoroutine(StartingFade(1f));

		if(!thanksScreen)
		{
			Invoke("IntroFade",screenHangTime);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKeyDown && !fading && thanksScreen)
		{
			StartCoroutine(FadeScreen(3f));
		}
	
	}

	void IntroFade ()
	{
		StartCoroutine(FadeScreen(1.5f));
	}

	IEnumerator StartingFade(float time)
	{
		fading = true;
		iTween.FadeFrom(gameObject, 0, time);
		
		yield return new WaitForSeconds(time);
		fading = false;
	}

	IEnumerator FadeScreen(float time)
	{
		fading = true;
		iTween.FadeTo(gameObject, 0, time);

		yield return new WaitForSeconds(time);

		if(thanksScreen)
		{
			Application.LoadLevel("Main Menu");
		}
		else
		{
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
