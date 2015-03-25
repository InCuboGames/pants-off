using UnityEngine;
using System.Collections;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField]
    protected MonitorTextWriter
        monitor;

	protected MainMenuCameraController controller;

    [SerializeField]
	protected string
        SelectionName;

    [SerializeField]
	protected int
        optionIndex;

	protected Vector3 originalScale;
	protected Color originalColor;

	[SerializeField]
	Color textColor;

	protected void Start()
    {
        controller = Camera.main.GetComponent<MainMenuCameraController>();

        originalScale = transform.localScale;
        originalColor = renderer.material.color;
    }

    // Use this for initialization
	protected void OnMouseEnter()
    {
        if (!controller.tweening)
        {
            iTween.ScaleTo(gameObject, iTween.Hash(
                "scale", new Vector3(originalScale.x * 1.2f, originalScale.y * 1.2f, originalScale.z * 1.2f),
                "time", 0.2f,
                "onupdatetarget", gameObject));
            
            //        iTween.ColorTo(gameObject, iTween.Hash(
            //            "color", new Color(originalColor.r + 0.4f, originalColor.g + 0.4f, originalColor.b + 0.4f),
            //            "time", 0.2f,
            //            "onupdatetarget", gameObject));

            if (controller.camIndex == 2)
            {
                monitor.ChangeText(SelectionName);
				monitor.gameObject.renderer.material.color = textColor;
            }
        }
    }
	
    // Update is called once per frame
	protected void OnMouseDown()
    {
		if(optionIndex == 3)
		{
			controller.CalculateOpenedSectors();
		}

        if (optionIndex < 0)
        {
            if (optionIndex == -1)
            {
				if(monitor.GetComponent<TextMesh>().text != "Tem certeza?")
				{
                	monitor.ChangeText("Tem certeza?");

				}
				else
				{
					SaveLoad.Instance.ClearData();
					SaveLoad.Instance.Save();
					monitor.ChangeText("Dados apagados.");
				}

            } else if (optionIndex == -2)
            {
                Application.Quit();
            } else if (optionIndex == -3)
            {
                Application.OpenURL("https://www.facebook.com/pantsoffgame");
            } else if (optionIndex == -4)
            {
                Application.OpenURL("http://www.pantsoffgame.com/");
            }
        } else
        {
            if (!controller.tweening)
            {
                if (optionIndex == 3)
                {
                    controller.mapScreen = true;
                } else
                {
                    controller.mapScreen = false;
                }
                controller.MenuOption(optionIndex);
            }

            if (optionIndex == 4)
            {
                controller.stageToLoad = SelectionName;
            }
        }

    }

	protected void OnMouseExit()
    {
        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", originalScale,
            "time", 0.2f,
            "onupdatetarget", gameObject));
        
//        iTween.ColorTo(gameObject, iTween.Hash(
//            "color", originalColor,
//            "time", 0.2f,
//            "onupdatetarget", gameObject));
    }
}
