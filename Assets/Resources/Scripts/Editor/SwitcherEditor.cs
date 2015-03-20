using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Switcher))]
public class SwitcherEditor : Editor {

    public override void OnInspectorGUI()
    {
        Switcher switcher = this.target as Switcher;
        switcher.isStatic = EditorGUILayout.Toggle("Static", switcher.isStatic);
        switcher.color = (PieceColor)EditorGUILayout.EnumPopup("Color", switcher.color);
        switcher.UpdateColor();
    }
}
