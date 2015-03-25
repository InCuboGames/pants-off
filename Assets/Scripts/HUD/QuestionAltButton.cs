using UnityEngine;
using System.Collections;

public class QuestionAltButton : MonoBehaviour {

    public TextMesh buttonText;
	public int numKey;

	int originalNumKey;
	
	KeyCode alphaNum;

	void Awake()
	{
		originalNumKey = numKey;
	}

	void OnEnable()
	{
		numKey = originalNumKey;

		switch(numKey)
		{
			case 1:
				alphaNum = KeyCode.Alpha1;
				break;
			case 2:
				alphaNum = KeyCode.Alpha2;
				break;
			case 3:
				alphaNum = KeyCode.Alpha3;
				break;
			case 4:
				alphaNum = KeyCode.Alpha4;
				break;
			default:
				alphaNum = KeyCode.Alpha0;
				break;
		}

		gameObject.renderer.material.color = Color.white;
		gameObject.collider.enabled = true;
	}

    void OnMouseEnter()
    {
		gameObject.renderer.material.color = new Color(0.5f, 0.6f, 1f);
    }

    void OnMouseExit()
    {
		gameObject.renderer.material.color = Color.white;
    }

    void OnMouseDown()
    {
        IssueAnswer ();
    }

	void Update()
	{
		if(Input.GetKeyDown(alphaNum) && numKey != 0)
		{
			IssueAnswer();
		}
	}

	void IssueAnswer ()
	{
		if (!transform.parent.parent.GetComponent<QuestionManager> ().answered) {
			gameObject.renderer.material.color = new Color(1,1,1);
			transform.parent.parent.GetComponent<QuestionManager> ().ReceiveAnswer (buttonText);
		}
	}
}
