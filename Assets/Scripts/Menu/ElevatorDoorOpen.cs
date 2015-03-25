using UnityEngine;
using System.Collections;

public class ElevatorDoorOpen : MonoBehaviour {

    [SerializeField] float delay;
    [SerializeField] float amount;
    [SerializeField] float time;
    [SerializeField] AudioClip elevatorDing;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	public IEnumerator OpenDoor () {
        yield return new WaitForSeconds(delay);

        if(elevatorDing)GetComponent<AudioSource>().PlayOneShot(elevatorDing);

        iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(transform.position.x,
                                              transform.position.y,
                                              transform.position.z + amount), "time", 
                      time, "easetype", iTween.EaseType.easeInOutCubic));
        yield return null;
	}
}
