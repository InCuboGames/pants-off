using UnityEngine;
using System.Collections;

public class PauseScreen : MonoBehaviour {

	PauseManager manager;
	[SerializeField]
	GameObject[] subScreens;

	AudioSource camAudio;

	// Use this for initialization
	void Start () 
	{
		manager = Camera.main.GetComponent<PauseManager>();
		gameObject.SetActive(false);

		camAudio = Camera.main.GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		Time.timeScale = 0;

		var audioSources = GameObject.FindObjectsOfType<AudioSource>();
		
		foreach(AudioSource audio in audioSources)
		{
			if(audio == camAudio)
			{
				audio.volume = 0.4f;
			}
			else
			{
			audio.enabled = false;
			}
		}
	
		Camera.main.GetComponent<MouseOrbit>().enabled = false;
	}

	void OnDisable()
	{
		Time.timeScale = 1;
		
		var audioSources = GameObject.FindObjectsOfType<AudioSource>();
		
		foreach(AudioSource audio in audioSources)
		{
			if(audio == camAudio)
			{
				audio.volume = 1;
			}
			else
			{
				audio.enabled = true;
			}
		}
	}

	public void Unpause()
	{
		manager.paused = false;
		Camera.main.GetComponent<MouseOrbit>().enabled = true;
		Screen.lockCursor = true;
		gameObject.SetActive(false);
	}

	public void OpenSubscreen(int id)
	{
		subScreens[id].SetActive(true);
	}

	public void CloseSubscreens()
	{
		foreach(GameObject screen in subScreens)
		{
			screen.SetActive(false);
		}
	}
}
