using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CutEdget : Skill {

	public override bool OnAdd ()
	{
		List<Piece> eliminate = Board.Instance.GetEdgetPieces();
		Board.Instance.EliminatePieces(eliminate);
		return true;
	}
}
