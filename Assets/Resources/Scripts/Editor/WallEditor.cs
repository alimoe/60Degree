using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        Wall wall = this.target as Wall;
        WallState state = wall.state;
        state = (WallState)EditorGUILayout.EnumPopup("Wall State", state);
        
        if (wall.state != state)
        {
            
            wall.state = state;
            if (state == WallState.Broken) wall.Broke();
            if (state == WallState.Normal) wall.Normal();
            if (state == WallState.Invincible) wall.UnBroken();
        }
        
        
        
    }
}
