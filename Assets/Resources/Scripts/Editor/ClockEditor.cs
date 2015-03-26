using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Clock))]
public class ClockEditor : Editor {

    public override void OnInspectorGUI()
    {
        Clock clock = this.target as Clock;
        clock.triggerEdget = (HexagonEdget)EditorGUILayout.EnumPopup("Trigger Side", clock.triggerEdget);
        this.serializedObject.ApplyModifiedProperties();
        clock.UpdateTrigger();
    }
}
