using UnityEngine;
using System.Collections;

public class CircleFadeStart : MonoBehaviour {

	// Use this for initialization
	void Start () {

        gameObject.renderer.material.SetFloat("_Cutoff", 0);

        iTween.ValueTo(gameObject, iTween.Hash(
        "from", 0,
        "to", 1,
        "time", 1.5f,
        "onupdatetarget", gameObject,
        "onupdate", "SetCircleFadeCutoff",
        "easetype", iTween.EaseType.easeInSine));
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void SetCircleFadeCutoff(float tweenedValue)
    {
        gameObject.renderer.material.SetFloat("_Cutoff", tweenedValue);
    }
}
