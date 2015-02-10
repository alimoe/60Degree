using UnityEngine;
using System.Collections;

public class Hexagon  {

	public Piece upper;
	public Piece lower;
	public int x;
	public int y;
	public float posX;
	public float posY;
	public int length;
	public Hexagon(int _x, int _y, int _length)
	{
		x = _x;
		y = _y;
		length = _length;
	}
}
