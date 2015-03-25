using UnityEngine;
using System.Collections;

public class Treadmill : MonoBehaviour {

	[SerializeField]
	float speed = 1;

	void OnTriggerStay(Collider coll)
	{
		if(coll.tag == "Player")
		{
			if(coll.gameObject.GetComponent<PlayerControl>().canControl)
			{
				coll.transform.Translate(Vector3.right * Time.deltaTime * speed, gameObject.transform);
			}
		}

		if(coll.tag == "NPC")
		{
			coll.transform.Translate(Vector3.right * Time.deltaTime * speed, gameObject.transform);
		}
	}
}
