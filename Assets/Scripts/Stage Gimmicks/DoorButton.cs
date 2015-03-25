using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorButton : MonoBehaviour {

	[SerializeField]
	ButtonControlledDoor assignedDoor;

	void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.tag == "Player")
		{
			StartCoroutine(assignedDoor.Open());
			transform.position = new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z);
			GetComponent<BoxCollider>().enabled = false;
		}
	}
}
