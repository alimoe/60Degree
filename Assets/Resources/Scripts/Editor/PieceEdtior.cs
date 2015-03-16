using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Piece))]
public class PieceEdtior : Editor {
	private PieceColor type;
	private PieceState state;
		

    public override void OnInspectorGUI()
    {
        Piece piece = this.target as Piece;
        type = piece.colorType;
        type = (PieceColor)EditorGUILayout.EnumPopup("Upper Color", type);
        piece.ChangeColor(type, true);
		state = piece.state;
		state = (PieceState)EditorGUILayout.EnumPopup("Upper State", state);
		UpdatePieceState ();
    }
	public void UpdatePieceState()
	{
		Piece piece = this.target as Piece;
		piece.state = state;
		if (piece.state == PieceState.Normal) {
			if(piece.ice != null)
			{
				GameObject.DestroyImmediate(piece.ice.gameObject);
				piece.ice = null;
			}
			if(piece.twine != null)
			{
				GameObject.DestroyImmediate(piece.twine.gameObject);
				piece.twine = null;
			}
		}
		if (piece.state == PieceState.Freeze) {
			if(piece.ice == null)
			{
				GameObject iceObj = Instantiate(Resources.Load("Prefabs/Ice")) as GameObject;
				piece.ice = iceObj.GetComponent<Ice>();
				piece.ice.transform.parent = piece.transform.parent;
			}
			piece.ice.Init();
			piece.ice.SetUp(piece);

			piece.ice.gameObject.SetActive(true);
			if(piece.twine != null)
			{
				piece.twine.gameObject.SetActive(false);
				GameObject.DestroyImmediate(piece.twine.gameObject);
				piece.twine = null;
			}
		}

		if (piece.state == PieceState.Twine) {
			if(piece.twine == null)
			{
				GameObject twineObj = Instantiate(Resources.Load("Prefabs/Twine")) as GameObject;
				piece.twine = twineObj.GetComponent<Twine>();
				piece.twine.transform.parent = piece.transform.parent;
			}
			piece.twine.Init();
			piece.twine.SetUp(piece);

			piece.twine.gameObject.SetActive(true);
			if(piece.ice != null)
			{
				piece.ice.gameObject.SetActive(false);
				GameObject.DestroyImmediate(piece.ice.gameObject);
			
				piece.ice = null;
			}
		}
	}
}
