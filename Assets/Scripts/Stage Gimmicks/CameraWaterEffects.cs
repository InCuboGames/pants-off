using UnityEngine;
using System.Collections;

public class CameraWaterEffects : MonoBehaviour {

	AudioLowPassFilter lowPass;
	DepthOfFieldScatter dof;
	GameObject water;
	public AudioClip dive, surface;
	public GameObject underwaterEffect;
	bool underwater, canDive = false;

	// Use this for initialization
	void OnEnable () 
	{	
		lowPass = GetComponent<AudioLowPassFilter>();
		dof = GetComponent<DepthOfFieldScatter>();
		water = GameObject.Find("FloodWater");

		lowPass.enabled = false;
		dof.enabled = false;

		StartCoroutine(EnableDive());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameObject.transform.position.y < water.transform.position.y && !underwater && canDive)
		{
			lowPass.enabled = true;
			dof.enabled = true;
			GetComponent<AudioSource>().PlayOneShot(dive);
			underwaterEffect.SetActive(true);
			underwater = true;
		}
		else if(gameObject.transform.position.y > water.transform.position.y && underwater)
		{
			lowPass.enabled = false;
			dof.enabled = false;
			GetComponent<AudioSource>().PlayOneShot(surface);
			underwaterEffect.SetActive(false);
			underwater = false;

		}
	}

	IEnumerator EnableDive ()
	{
		yield return new WaitForSeconds(0.5f);
		canDive = true;
	}
}
