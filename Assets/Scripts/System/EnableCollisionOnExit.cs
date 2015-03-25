using UnityEngine;
using System.Collections;

public class EnableCollisionOnExit : MonoBehaviour {

	PlayerControl player;

	[SerializeField]
	AutomaticDoor
		door;

	void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerControl>();
	}

	void OnTriggerExit(Collider hit)
	{	
		if (hit.gameObject.tag == "Player")
		{
			GetComponent<BoxCollider>().isTrigger = false;
			door.allowPlayer = false;
			door.allowNPCs = false;
		}
	}

	void Update()
	{
		if(player.canControl && GetComponent<BoxCollider>().isTrigger)
		{
			GetComponent<BoxCollider>().isTrigger = false;
			door.allowPlayer = false;
			door.allowNPCs = false;
		}
	}
}
