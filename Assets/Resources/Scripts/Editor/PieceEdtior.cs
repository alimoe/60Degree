using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Piece))]
public class PieceEdtior : Editor {
    PieceColor type;
    public override void OnInspectorGUI()
    {
        Piece piece = this.target as Piece;
        type = piece.colorType;
        type = (PieceColor)EditorGUILayout.EnumPopup("Upper State", type);
        piece.ChangeColor(type, true);

    }
}
