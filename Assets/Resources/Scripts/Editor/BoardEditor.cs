using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(Board))]
public class BoardEditor : Editor {
	public int segment = 8;
	public float length = 1f;
	public override void OnInspectorGUI () {
		//GUILayout.Label ("This is a Label in a Custom Editor");
		Board board = this.target as Board;
		segment = board.segment;
		length = board.length;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("segment"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("length"), true);
		serializedObject.ApplyModifiedProperties();

		if (board.segment != this.segment || board.length!=length) {
			board.GenerateHexagon();
			board.GenerateLines();
		}

	}

}
