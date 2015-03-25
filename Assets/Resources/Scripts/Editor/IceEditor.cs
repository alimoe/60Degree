using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Ice))]
public class IceEditor : Editor
{
    private int life;
    public override void OnInspectorGUI()
    {
        Ice ice = this.target as Ice;
        life = ice.life;
        life = EditorGUILayout.IntField("Life:", ice.life);
        
        if (life != ice.life)
        {
        
            ice.Init();
            ice.SetLife(life);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
