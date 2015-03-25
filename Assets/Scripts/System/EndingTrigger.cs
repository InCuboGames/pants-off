using UnityEngine;
using System.Collections;

public class EndingTrigger : MonoBehaviour {

	GameObject player;
	ShameMeter shameScript;

	[SerializeField]
	Transform stageEnd;

	[SerializeField]
	AudioClip endJingle;

	bool stageEnded = false;

	void Awake()
	{
		player = GameObject.Find("Player");
		shameScript = player.GetComponent<ShameMeter>();
	}

	void OnTriggerEnter(Collider hit)
	{	
		if (hit.gameObject.tag == "Player" && !stageEnded)
		{
			stageEnded = true;

			player.GetComponent<AutoControl>().enabled = true;
			player.GetComponent<NavMeshAgent>().enabled = true;
			player.GetComponent<PlayerControl>().canControl = false;
			player.GetComponent<PlatformInputController>().enabled = false;
			player.GetComponent<NavMeshObstacle>().enabled = false;
			Camera.main.GetComponent<MouseOrbit>().enabled = false;
			Camera.main.GetComponent<PauseManager>().enabled = false;
			player.GetComponent<NavMeshAgent>().SetDestination(stageEnd.position);
			shameScript.LevelEnd();
		}
	}
}
