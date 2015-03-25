using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StageManager))]
public class EditorStageManager : Editor
{
	string[] aAVs = { "Nenhum", "Apagado", "Congelado", "Inundado" };

    public override void OnInspectorGUI()
    {
        //Carregar variáveis do Script
        var stgManager = target as StageManager;

		int IDAV = stgManager.VA;

        //Desenhar no Inspector
        DrawDefaultInspector();

		IDAV = EditorGUILayout.Popup("Variacao Ambiental", IDAV, aAVs);

		//Atualizar no Script
		stgManager.VA = IDAV;

        //Salvar o Script
        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}