using UnityEngine;
using System.Collections;

public class MainMenuFadeIn : MonoBehaviour {

    [SerializeField]
    float fadeTime;

	// Use this for initialization
	void Start () {
        iTween.ColorTo(gameObject, iTween.Hash("a", 0, "time", fadeTime));
	}
	
	// Update is called once per frame
	public void FadeOut () {
		iTween.ColorTo(gameObject, iTween.Hash("a", 1, "time", fadeTime));
	}
}
