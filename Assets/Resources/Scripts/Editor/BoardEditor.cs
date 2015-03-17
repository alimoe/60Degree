using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
[CustomEditor(typeof(Board))]
public class BoardEditor : Editor {
	public int segment = 8;
	public float length = 1f;
    private LevelExporter export;
    private LevelObjective objective = LevelObjective.Eliminate;
    private string levelName="";
    public int levelStep = -1;
	public override void OnInspectorGUI () {
	    
		Board board = this.target as Board;
		segment = board.segment;
		length = board.length;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("segment"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("length"), true);
		serializedObject.ApplyModifiedProperties();

       
		if (board.segment != this.segment || board.length!=length) {
            //Debug.Log("Repaint");
			board.GenerateHexagon();
		    board.RenderInEditor();
            board.GenerateWall();
		}
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Disable Walls"))
        {
            UpdateWalls(0);
        }
        if (GUILayout.Button("Normalize Walls"))
        {
            UpdateWalls(1);
        }
        if (GUILayout.Button("Invincible Walls"))
        {
            UpdateWalls(2);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Separator();

        objective = (LevelObjective)EditorGUILayout.EnumPopup("Objective", objective);
        EditorGUILayout.LabelField("Level Name:");
        levelName = EditorGUILayout.TextField(levelName);
        EditorGUILayout.LabelField("Level Step:");
        levelStep = EditorGUILayout.IntField(levelStep);
        if (GUILayout.Button("Save"))
        {
            Save(ref board, levelName, objective, levelStep);
        }
	}
    public void UpdateWalls(int flag)
    {
        Board board = this.target as Board;
        List<Wall> walls = board.GetWalls();
        foreach (Wall wall in walls)
        {
            if (flag == 0) wall.Broke();
            else if (flag == 1) wall.Normal();
            else if (flag == 2) wall.Invincible();
        }
    }
    public void Save(ref Board board, string levelName, LevelObjective objective, int levelStep)
    {
        export = new LevelExporter();
        export.Save(ref board, levelName, objective, levelStep);
    }

}
