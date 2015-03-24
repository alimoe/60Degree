using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
[CustomEditor(typeof(Piece))]
[CanEditMultipleObjects]
public class PieceEdtior : Editor {
	private PieceColor type;
	private PieceState state;
		

    public override void OnInspectorGUI()
    {
        Piece piece = this.target as Piece;
        EditorGUILayout.LabelField("ID:", piece.id.ToString());
        

        type = piece.colorType;
        type = (PieceColor)EditorGUILayout.EnumPopup("Upper Color", type);
        piece.ChangeColor(type);
		state = piece.state;
		state = (PieceState)EditorGUILayout.EnumPopup("Upper State", state);

		UpdatePieceState ();

        if (this.targets.Length >= 2 && this.targets.Length <= 3)
        {
            if (GUILayout.Button("Make Chain"))
            {
                
                PieceGroup pieceGroup = new PieceGroup();
                for (int i = 0; i < this.targets.Length; i++)
                {
                    Piece member = this.targets[i] as Piece;
                    if (member.group != null) DestoryChain(member.group);
                    pieceGroup.AddChild(member);
                }
                pieceGroup.Sort();
                MakeChain(pieceGroup);
            }
            if (GUILayout.Button("Destory Chain"))
            {
                for (int i = 0; i < this.targets.Length; i++)
                {
                    Piece member = this.targets[i] as Piece;
                    if (member.group != null) DestoryChain(member.group);
                    member.group = null;
                }
            }

        }
        
    }
    private void DestoryChain(PieceGroup pieceGroup)
    {
        if (pieceGroup != null)
        {
            if (pieceGroup.chains != null)
            {
                foreach (Chain chain in pieceGroup.chains)
                {
                    if (chain!=null) GameObject.DestroyImmediate(chain.gameObject);
                }
            }
            pieceGroup = null;
        }
    }
    private void MakeChain(PieceGroup pieceGroup)
    {
        List<Chain> chains = new List<Chain>();
        for (int i = 1; i < pieceGroup.children.Count; i++)
        {
            Piece start = pieceGroup.children[i - 1];
            Piece end = pieceGroup.children[i];
            
            if (!pieceGroup.HasChained(start, end))
            {
                GameObject chainObj = Instantiate(Resources.Load("Prefabs/Chain")) as GameObject;
                Chain chain = chainObj.GetComponent<Chain>();
                chain.transform.parent = start.transform.parent;
                chain.SetUp(start, end);
                chains.Add(chain);
            }
        }
        pieceGroup.chains = chains;
    }

    public void DestoryOldState()
    {
        Piece piece = this.target as Piece;

        if ((int)(piece.state & PieceState.Freeze) == 0)
        {
            if (piece.ice != null)
            {
                piece.ice.gameObject.SetActive(false);
                GameObject.DestroyImmediate(piece.ice.gameObject);

                piece.ice = null;
            }
        }
        if ((int)(piece.state & PieceState.Twine) == 0)
        {
            if (piece.twine != null)
            {
                piece.twine.gameObject.SetActive(false);
                GameObject.DestroyImmediate(piece.twine.gameObject);
                piece.twine = null;
            }
        }
        if ((int)(piece.state & PieceState.Clock) == 0)
        {
            if (piece.clock != null)
            {
                piece.clock.gameObject.SetActive(false);
                GameObject.DestroyImmediate(piece.clock.gameObject);
                piece.clock = null;
            }
        }
    }

	public void UpdatePieceState()
	{
		Piece piece = this.target as Piece;
		piece.state = state;
        DestoryOldState();

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
			
		}

        if (piece.state == PieceState.Clock)
        {
            if (piece.clock == null)
            {
                GameObject twineObj = Instantiate(Resources.Load("Prefabs/Clock")) as GameObject;
                piece.clock = twineObj.GetComponent<Clock>();
                piece.clock.transform.parent = piece.transform.parent;
            }
            piece.clock.Init();
            piece.clock.SetUp(piece);
            piece.clock.gameObject.SetActive(true);
            
        }
	}
}
