using UnityEngine;
using System.Collections;

public class MovieManager : MonoBehaviour {

	public MovieTexture movie;
	public float movieDuration = 1;
	bool fading = false;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		movie.Play();
		movie.loop = false;

		Invoke("FinishMovie",movieDuration);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKeyDown && !fading)
		{
			StartCoroutine(FadeMovie(1f));
		}
	
	}

	IEnumerator FadeMovie(float time)
	{
		fading = true;
		iTween.FadeTo(gameObject, 0, time);

		yield return new WaitForSeconds(time);

		FinishMovie();
	}

	void FinishMovie()
	{
		Application.LoadLevel(Application.loadedLevel+1);
	}
}
