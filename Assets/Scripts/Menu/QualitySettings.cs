using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour {
	void OnGUI() {
		string[] names = QualitySettings.names;
		GUILayout.BeginVertical();
		int i = 0;
		while (i < names.Length) {
			if (GUILayout.Button(names[i]))
				QualitySettings.SetQualityLevel(i, true);
			
			i++;
		}
		GUILayout.EndVertical();
	}
}