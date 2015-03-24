using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Twine))]
public class TwineEditor : Editor
{
    private bool[] sideInfo;
    public override void OnInspectorGUI()
    {
        Twine twine = this.target as Twine;
        sideInfo = new bool[6] { (twine.state & (int)(HexagonEdget.UpperLeft)) != 0, (twine.state & (int)(HexagonEdget.UpperRight)) != 0, (twine.state & (int)(HexagonEdget.UpperDown)) != 0, (twine.state & (int)(HexagonEdget.DownLeft)) != 0, (twine.state & (int)(HexagonEdget.DownRight)) != 0, (twine.state & (int)(HexagonEdget.DownUp)) != 0 };

        sideInfo[0] = EditorGUILayout.Toggle("TL Side", sideInfo[0]);
        sideInfo[1] = EditorGUILayout.Toggle("TR Side", sideInfo[1]);
        sideInfo[2] = EditorGUILayout.Toggle("MD Side", sideInfo[2]);
        sideInfo[3] = EditorGUILayout.Toggle("BL Side", sideInfo[3]);
        sideInfo[4] = EditorGUILayout.Toggle("BR Side", sideInfo[4]);
        sideInfo[5] = EditorGUILayout.Toggle("MU Side", sideInfo[5]);

        UpdateEdgetState();
            



        this.serializedObject.ApplyModifiedProperties();

    }


    public void UpdateEdgetState()
    {
        int state = 0;
        for (int i = 0; i < sideInfo.Length; i++)
        {
            if (sideInfo[i] == true)
            {
                state |= (int)Math.Pow(2, i);
            }
        }

        Twine twine = this.target as Twine;
        twine.Init();
        twine.SetState(state);


    }
}
