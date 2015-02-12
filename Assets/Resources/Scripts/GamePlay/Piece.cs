using UnityEngine;
using System.Collections;
public enum PieceColor
{
	Blue,
	Pink,
	Purple,
	Yellow,
	White,
	Red,
	Green,
	Orange,
	None

}
public class Piece : Entity {

	public bool isUpper = false;
	public PieceColor type;

}
