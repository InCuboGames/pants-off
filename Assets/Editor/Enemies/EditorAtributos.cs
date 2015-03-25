using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IA))]
public class EditorAtributos : Editor
{
	string[] aSexos = { "Aleatório", "Masculino", "Feminino"};
    string[] aPersonalidades = { "Comum", "Atarefado", "Entediado", "Impaciente", "Histérico" };
    string[] aHumores = { "Inalterado", "Feliz", "Triste", "Irritado", "Avoado" };
    string[] aVisoes = { "Quase-Cego", "Míope", "Saudável", "Excepcional", "Olho-de-águia" };
    string[] aCargos = { "Estagiário", "Empregado", "Veterano", "Gerente", "Diretor" };

    public override void OnInspectorGUI()
    {
        //Carregar variáveis do Script
        var IAScript = target as IA;

		int IDSexo = IAScript.sexo;
        int IDPersonalidade = IAScript.personalidade;
        int IDHumor = IAScript.humor;
        int IDVisao = IAScript.visao;
        int IDCargo = IAScript.cargo;

        //Desenhar no Inspector
        DrawDefaultInspector();

		IDSexo = EditorGUILayout.Popup("Sexo", IDSexo, aSexos);
        IDPersonalidade = EditorGUILayout.Popup("Personalidade", IDPersonalidade, aPersonalidades);
        IDHumor = EditorGUILayout.Popup("Humor", IDHumor, aHumores);
        IDVisao = EditorGUILayout.Popup("Visão", IDVisao, aVisoes);
		IDCargo = EditorGUILayout.Popup("Cargo", IDCargo, aCargos);

		//Atualizar no Script
		IAScript.sexo = IDSexo;
		IAScript.personalidade = IDPersonalidade;
		IAScript.humor = IDHumor;
		IAScript.visao = IDVisao;
		IAScript.cargo = IDCargo;
		
//		MeshRenderer face = IAScript.face;
//		Sprite[] expressions = IAScript.faceExpressions;
		//Mudar face
		//face.sharedMaterial.mainTexture = expressions[IDHumor].texture;

        //Salvar o Script
        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}