using UnityEngine;
using System.Collections;

public class GraphicOptionsManager: MonoBehaviour {

	[SerializeField]
	GraphicalOptionsButton[] resolutionButtons;

	[SerializeField]
	GraphicalOptionsButton[] qualityButtons;

	void Start()
	{
//		resolutionButtons[0].Select();
//		qualityButtons[1].Select();
	}

	public void ClearQuality()
	{
		foreach(GraphicalOptionsButton btn in qualityButtons)
		{
			btn.Unselect();
		}
	}

	public void ClearResolution()
	{
		foreach(GraphicalOptionsButton btn in resolutionButtons)
		{
			btn.Unselect();
        }
    }
}