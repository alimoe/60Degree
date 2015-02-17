using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Bomb : Skill {

	public override bool OnAdd ()
	{
		return base.OnAdd ();
	}
	public override bool Excute (Vector3 position)
	{
		Piece piece = Board.Instance.GetPieceFromPosition (position);
		if (piece != null) {
			List<Piece> eliminate = Board.Instance.GetSurroundPiece(piece);
			if(!eliminate.Contains(piece))eliminate.Add(piece);
			Board.Instance.EliminatePieces(eliminate);
			return true;
		}
		return false;
	}
}
