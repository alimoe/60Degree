using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Lightening : Skill {
	public override bool OnAdd ()
	{
		return base.OnAdd ();
	}
	public override bool Excute (Vector3 position)
	{
		Piece piece = Board.Instance.GetPieceFromPosition (position);
		if (piece != null) {
			List<Piece> eliminate = Board.Instance.GetSameColorPieces(piece);
			Board.Instance.EliminatePieces(eliminate);
			return true;
		}
		return false;
	}

}
