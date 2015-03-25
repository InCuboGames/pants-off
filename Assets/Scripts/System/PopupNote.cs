using UnityEngine;
using System.Collections;

public class PopupNote : MonoBehaviour {

	public int ID;

	[SerializeField]
		string noteTitle;

    [SerializeField]
        string noteText;

	[SerializeField]
	TextMesh popupTitle;
	
	[SerializeField]
	TextMesh popupText;

	[SerializeField]
	TextMesh greenText;

    [SerializeField]
    GameObject popup;
	
	[SerializeField]
	GameObject gameObj;

	[SerializeField]
	GameObject greenEffect;

	[SerializeField]
	AudioClip zoomIn, zoomOut;

    GameObject player;
    Vector3 originalScale;

	StageManager stageManager;

    bool showing = true;

	public bool collectable = false;

	[SerializeField]
	Material regularPostIt, regularPopup;

	[SerializeField]
	Material greenPostIt, greenPopup;


	// Use this for initialization
	void Start () 
    {
        player = GameObject.FindWithTag("Player");

		if(popupTitle)popupTitle.text = noteTitle;

		if(popupText && greenText)popupText.text = greenText.text = noteText;

        originalScale = popup.transform.localScale;

		showing = false;
		popup.transform.localScale = Vector3.zero;

		if(GameObject.Find("Stage Manager"))
		{
			stageManager = GameObject.Find("Stage Manager").GetComponent<StageManager>();
		}

		if(collectable)
		{
			popupText.gameObject.SetActive(false);
			if(gameObj)gameObj.renderer.material = greenPostIt;
			popup.renderer.material = greenPopup;

			var pos = popupText.transform.position;
			pos.y = -0.1f;
			popupText.transform.position = pos;
		}
		else
		{
			if(gameObj)gameObj.renderer.material = regularPostIt;
			popup.renderer.material = regularPopup;
			if(greenEffect)greenEffect.SetActive(false);
			if(greenText)greenText.gameObject.SetActive(false);
			if(popupTitle)popupTitle.gameObject.SetActive(false);
		}

	}
	
	// Update is called once per frame
	void Update () 
    {
        var dist = Vector3.Distance(player.transform.position, transform.position);

		if(dist <= 3 && !showing && player.GetComponent<PlayerControl>().canControl && !showing)
        {
			if(stageManager && collectable)
			{
				if(!stageManager.greenNotesFound.Contains(this))
				{
					stageManager.greenNotesFound.Add(this);
					if(ID != 0)
					{
						print ("Collected Post-it #"+this.ID+"!");
					}
				}
			}

            iTween.ScaleTo(popup, originalScale, 0.5f);
			Camera.main.GetComponent<AudioSource>().PlayOneShot(zoomIn);
            showing = true;

        }
		else if (dist > 3 && showing || !player.GetComponent<PlayerControl>().canControl && showing)
        {
            iTween.ScaleTo(popup, Vector3.zero, 0.5f);
			Camera.main.GetComponent<AudioSource>().PlayOneShot(zoomOut);
            showing = false;
        }
	
	}
}
