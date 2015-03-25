using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonControlledDoor : MonoBehaviour {

	[SerializeField]
	GameObject doorGroup;

	[SerializeField]
	Transform doorOpenedPos;

	[SerializeField]
	Transform camAnchor;

	[SerializeField]
	Light[] lights;

	[SerializeField]
	MeshRenderer[] lightSprites;

	[SerializeField]
	Sprite greenLightSprite;

	Vector3 doorClosedPos;

	bool isOpened = false;
	bool closed = true;
	bool tweening = false;

	const float maxProximity = 3.5f;

	GameObject player;
	GameObject[] NPCs;


	void Start()
	{
		player = GameObject.Find("Player");
		NPCs = GameObject.FindGameObjectsWithTag("NPC");

		doorClosedPos = transform.position;
	}

	public IEnumerator Open()
	{
		Camera.main.GetComponent<MouseOrbit>().enabled = false;
		player.GetComponent<PlayerControl>().canControl = false;

		Camera.main.GetComponent<PauseManager>().cameraTweening = true;

		iTween.MoveTo(Camera.main.gameObject, camAnchor.transform.position, 2);
		iTween.RotateTo(Camera.main.gameObject,  camAnchor.transform.rotation.eulerAngles,2);

		yield return new WaitForSeconds(2f);

		foreach (MeshRenderer rend in lightSprites)
		{
			rend.material.mainTexture = greenLightSprite.texture;
		}

		foreach (Light l in lights)
		{
			l.color = new Color(0,1,0.3f);
		}
		isOpened = true;

		yield return new WaitForSeconds(1);

		Camera.main.GetComponent<PauseManager>().cameraTweening = false;

		Camera.main.GetComponent<MouseOrbit>().enabled = true;
		player.GetComponent<PlayerControl>().canControl = true;
	}

    // Update is called once per frame
	void Update () 
	{
		if(isOpened && !tweening && closed)
		{
			OpenTween();
		}
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
