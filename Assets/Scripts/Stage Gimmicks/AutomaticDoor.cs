using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutomaticDoor : MonoBehaviour {
	
	[SerializeField]
	Transform doorOpenedPos;

	Vector3 doorClosedPos;

	[SerializeField]
	public bool allowPlayer = true;

	[SerializeField]
	public bool allowNPCs = true;
	
	bool isOpened = false;
	bool closed = true;
	bool tweening = false;

	const float maxProximity = 3.5f;

	GameObject player;
	GameObject[] NPCs;


	void Start()
	{
		player = GameObject.FindWithTag("Player");
		NPCs = GameObject.FindGameObjectsWithTag("NPC");

		doorClosedPos = transform.position;
	}

    // Update is called once per frame
	void Update () 
	{
		List<Transform> closeBy = new List<Transform>();

		foreach(GameObject npc in NPCs)
		{
			if(Vector3.Distance(npc.transform.position, transform.position) <= maxProximity && allowNPCs)
			{
				closeBy.Add(npc.transform);
			}
		}
		if(Vector3.Distance(player.transform.position, transform.position) <= maxProximity && allowPlayer)
		{
			closeBy.Add(player.transform);
		}



		if(closeBy.Count >0 && !tweening && !isOpened)
		{
			isOpened = true;
		}
		else if (closeBy.Count <=0 && !tweening && isOpened)
        {
            isOpened = false;
        }


		if(isOpened && !tweening && closed)
		{
			OpenTween();
		}
		else if(!isOpened && !tweening && !closed)
		{
			CloseTween();
        }
    }

	void OpenTween()
	{
		tweening = true;
		if(GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().Play();
		}
		iTween.MoveTo(gameObject, iTween.Hash("position", doorOpenedPos.position, 
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
		iTween.MoveTo(gameObject, iTween.Hash("position", doorClosedPos, 
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
