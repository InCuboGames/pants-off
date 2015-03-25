using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SecuriCam))]
public class EditorAtributosSecuriCam : Editor
{
    string[] aResolution = { "SD", "ED", "HD", "Full HD", "Ultra HD"};

    public override void OnInspectorGUI()
    {
        //Carregar variáveis do Script
        var SecuriCamScript = target as SecuriCam;

        int IDResolution = SecuriCamScript.videoResolution;

        //Desenhar no Inspector
        DrawDefaultInspector();
		
		IDResolution = EditorGUILayout.Popup("Resolução de Vídeo", IDResolution, aResolution);

		//Atualizar no Script
		SecuriCamScript.videoResolution = IDResolution;

        //Salvar o Script
        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}