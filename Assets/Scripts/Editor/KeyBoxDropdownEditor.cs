using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KeyBoxData))]
public class KeyBoxDropdownEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		KeyBoxData script = (KeyBoxData)target;
		script.keyColorSelection = EditorGUILayout.Popup(new GUIContent("Key Color"), script.keyColorSelection, script.keyColors);
		script.minigame = EditorGUILayout.Popup(new GUIContent("Minigame"), script.minigame, script.minigames);
		script.difficulty = EditorGUILayout.Popup(new GUIContent("Difficulty"), script.difficulty, script.difficulties);
	}
}