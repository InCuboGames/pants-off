using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimedDoor : MonoBehaviour {

	[SerializeField]
	GameObject doorGroup;

	[SerializeField]
	Transform doorOpenedPos;
	[SerializeField]
	Light[] lights;

	[SerializeField]
	MeshRenderer[] lightSprites;

	[SerializeField]
	Sprite greenLightSprite;

	[SerializeField]
	Sprite yellowLightSprite;

	[SerializeField]
	Sprite redLightSprite;

	[SerializeField]
	float cycleTime;

	[SerializeField]
	AudioClip doorBleep;

	Vector3 doorClosedPos;

	bool isOpened = false;
	bool closed = true;
	bool tweening = false;

	GameObject player;
	GameObject[] NPCs;


	void Start()
	{
		player = GameObject.FindWithTag("Player");
		NPCs = GameObject.FindGameObjectsWithTag("NPC");

		doorClosedPos = doorGroup.transform.position;
		StartCoroutine(TimedOpen());
	}

	public IEnumerator TimedOpen()
	{
		yield return new WaitForSeconds(cycleTime/2);

		foreach (MeshRenderer rend in lightSprites)
		{
			rend.material.mainTexture = yellowLightSprite.texture;
		}
		
		foreach (Light l in lights)
		{
			l.color = new Color(1,1,0.3f);
		}

		for(int i = 0; i < 3; i++)
		{
			GetComponent<AudioSource>().PlayOneShot(doorBleep);
			
			foreach (MeshRenderer rend in lightSprites)
			{
				rend.material.mainTexture = greenLightSprite.texture;
			}
			
			foreach (Light l in lights)
			{
				l.color = new Color(0,1,0.3f);
			}
			
			yield return new WaitForSeconds((cycleTime/2)/6);
			
			foreach (MeshRenderer rend in lightSprites)
			{
				rend.material.mainTexture = yellowLightSprite.texture;
			}
			
			foreach (Light l in lights)
			{
				l.color = new Color(1,1,0.3f);
			}
			
			yield return new WaitForSeconds((cycleTime/2)/6);
		}


		foreach (MeshRenderer rend in lightSprites)
		{
			rend.material.mainTexture = greenLightSprite.texture;
		}
		
		foreach (Light l in lights)
		{
			l.color = new Color(0,1,0.3f);
		}

		OpenTween();
		isOpened = false;
		StartCoroutine(TimedClose());
	}

	public IEnumerator TimedClose()
	{
		yield return new WaitForSeconds(cycleTime/2);
		
		foreach (MeshRenderer rend in lightSprites)
		{
			rend.material.mainTexture = yellowLightSprite.texture;
		}
		
		foreach (Light l in lights)
		{
			l.color = new Color(1,1,0.3f);
		}
		
		for(int i = 0; i < 3; i++)
		{
			GetComponent<AudioSource>().PlayOneShot(doorBleep);

			foreach (MeshRenderer rend in lightSprites)
			{
				rend.material.mainTexture = redLightSprite.texture;
			}
			
			foreach (Light l in lights)
			{
				l.color = new Color(1,0,0.3f);
			}

			yield return new WaitForSeconds((cycleTime/2)/6);

			foreach (MeshRenderer rend in lightSprites)
			{
				rend.material.mainTexture = yellowLightSprite.texture;
			}
			
			foreach (Light l in lights)
			{
				l.color = new Color(1,1,0.3f);
			}

			yield return new WaitForSeconds((cycleTime/2)/6);
		}
		
		foreach (MeshRenderer rend in lightSprites)
		{
			rend.material.mainTexture = redLightSprite.texture;
		}
		
		foreach (Light l in lights)
		{
			l.color = new Color(1,0,0.3f);
		}
		
		CloseTween();
		isOpened = true;
		StartCoroutine(TimedOpen());
	}

	void OpenTween()
	{
		tweening = true;
		if(GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().Play();
		}
		iTween.MoveTo(doorGroup, iTween.Hash("position", doorOpenedPos.position, 
		                                      "time", 1, 
		                                      "easetype", iTween.EaseType.easeInOutCubic, 
		                                      "oncomplete", "TurnOffTween"));
	}

	void CloseTween()
	{
		tweening = true;
		if(GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().Play();
		}
		iTween.MoveTo(doorGroup, iTween.Hash("position", doorClosedPos, 
		                                      "time", 1, 
		                                      "easetype", iTween.EaseType.easeInOutCubic, 
		                                      "oncomplete", "TurnOffTween"));
	}


	void TurnOffTween()
	{
		tweening = false;
		if(isOpened)
		{
			closed = false;
		}
		else
		{
			closed = true;
		}
	}

	void OnDisable()
	{
		iTween.Stop(gameObject);
	}
}
