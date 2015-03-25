using UnityEngine;
using System.Collections;

public class StageMenuButton : MainMenuButton
{
    [SerializeField]
    Material
        unclearedColor, cleredColor;

	[SerializeField]
	Material
		unclearedTextColor, cleredTextColor;

    [SerializeField]
    TextMesh
        floorName, description;

	void Awake()
	{
		RefreshState();
	}

	public void RefreshState ()
	{
		if(!GlobalDataManager.GameData.UnlockedStages.Contains(int.Parse(SelectionName)))
		{
			GetComponent<Renderer>().material = unclearedColor;
			floorName.GetComponent<Renderer>().material = unclearedTextColor;
			if(description) description.gameObject.SetActive(false);
			
			GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
			GetComponent<Renderer>().material = cleredColor;
			floorName.GetComponent<Renderer>().material = cleredTextColor;
			if (description)
			{
				description.gameObject.SetActive(true);
				description.GetComponent<Renderer>().material = unclearedTextColor;
			}
			
			GetComponent<BoxCollider>().enabled = true;
		}
	}
}
