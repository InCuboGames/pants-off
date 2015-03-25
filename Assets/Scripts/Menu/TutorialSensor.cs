using UnityEngine;
using System.Collections;

public class TutorialSensor : MonoBehaviour {

	[SerializeField]
	bool enableTutorial;

	void OnTriggerEnter(Collider hit)
	{	
		if (hit.gameObject.tag == "Player")
		{
			if(enableTutorial && hit.GetComponent<NavMeshAgent>().enabled)
			{
				Camera.main.GetComponent<MainMenuCameraController>().StartTutorial();
			}

			if(!enableTutorial && !hit.GetComponent<NavMeshAgent>().enabled)
			{
				Camera.main.GetComponent<MainMenuCameraController>().EndTutorial();
			}
		}
	}
}
