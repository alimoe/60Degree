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
    private PlayModeState _currentState = PlayModeState.Stopped;

    public BoardEditor()
    {
        EditorApplication.playmodeStateChanged = OnUnityPlayModeChanged;
        
    }

    public enum PlayModeState
    {
        Stopped,
        Playing,
        Paused
    }

    public void OnUnityPlayModeChanged()
    {
        var changedState = PlayModeState.Stopped;
        switch (_currentState)
        {
            case PlayModeState.Stopped:
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    changedState = PlayModeState.Playing;
                }
                break;
            case PlayModeState.Playing:
                if (EditorApplication.isPaused)
                {
                    changedState = PlayModeState.Paused;
                }
                else
                {
                    changedState = PlayModeState.Stopped;
                }
                break;
            case PlayModeState.Paused:
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    changedState = PlayModeState.Playing;
                }
                else
                {
                    changedState = PlayModeState.Stopped;
                }
                break;
             
        }

        _currentState = changedState;
        
        if (_currentState == PlayModeState.Stopped)
        {
            EditorApplication.OpenScene(PlayerPrefs.GetString("TestScene"));
        }
    }

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

        if (GUILayout.Button("Update Pieces Index"))
        {
            UpdatePieceID();
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
        if (GUILayout.Button("Test"))
        {
            Save(ref board, "Temp", objective, levelStep);
            EditorApplication.SaveScene();

            PlayerPrefs.SetInt("TestMode", 1);
            PlayerPrefs.SetString("TestScene", EditorApplication.currentScene);
            PlayerPrefs.Save();
            
            EditorApplication.OpenSceneAdditive("Assets/Resources/Scenes/main.unity");
            EditorApplication.isPlaying = true;
        }
	}
    public void UpdatePieceID()
    {
        Board board = this.target as Board;
        Hexagon[] hexagons = board.GetHexagons();
        int index = 0;
        for (int i = 0;i<hexagons.Length;i++)
        {
            if (hexagons[i].upper != null)
            {
                hexagons[i].upper.id = index;
                index++;
            }
            if (hexagons[i].lower != null)
            {
                hexagons[i].lower.id = index;
                index++;
            }
        }

    }
    public void UpdateWalls(int flag)
    {
        Board board = this.target as Board;
        Wall[] walls = board.GetWalls();
        foreach (Wall wall in walls)
        {
            wall.Init();
            if (flag == 0) wall.Broke();
            else if (flag == 1) wall.Normal();
            else if (flag == 2) wall.UnBroken();
        }
    }
    public void Save(ref Board board, string levelName, LevelObjective objective, int levelStep)
    {
        export = new LevelExporter();
        export.Save(ref board, levelName, objective, levelStep);
    }

}
