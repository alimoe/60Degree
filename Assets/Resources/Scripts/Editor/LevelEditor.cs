using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {

    public int step = 0;

    public override void OnInspectorGUI()
    {
        Level level = this.target as Level;
        EditorGUILayout.BeginHorizontal();
        step = level.step;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("step"), true);
        
        
        serializedObject.FindProperty("moveDirection");
        EditorGUILayout.EndHorizontal();
        if (this.step != level.step)
        {
            
            this.step = level.step;
            int[] newPieceIndex = new int[step];
            BoardDirection[] newDirection = new BoardDirection[step];
            
            for (int i = 0; i < level.pieceIndex.Length; i++)
            {
                if (i < newPieceIndex.Length) newPieceIndex[i] = level.pieceIndex[i];
            }
            for (int i = 0; i < level.moveDirection.Length; i++)
            {
                if (i < newDirection.Length) newDirection[i] = level.moveDirection[i];
            }
            level.pieceIndex = newPieceIndex;
            level.moveDirection = newDirection;
        }

        //level.step = step;

        for (int i = 0; i < step; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Piece:", EditorStyles.label, GUILayout.Width(60));
            level.pieceIndex[i] = EditorGUILayout.IntField(level.pieceIndex[i], GUILayout.Width(60));
            level.moveDirection[i] = (BoardDirection)EditorGUILayout.EnumPopup("Direction:", level.moveDirection[i]);
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(level);

        serializedObject.ApplyModifiedProperties();
    }
}
