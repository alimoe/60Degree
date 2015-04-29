using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Bomb : Skill {
	public Bomb(OnExcute callback = null )
	{
		onExcuteCallback = callback;
		this.hint = "TapOnGrid";
	}
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
            Board.Instance.EliminatePieces(eliminate, false);
            Board.Instance.BlinkSurroundPiece(piece);
			if(onExcuteCallback!=null)onExcuteCallback();
			return true;
		}
		return false;
	}
}
