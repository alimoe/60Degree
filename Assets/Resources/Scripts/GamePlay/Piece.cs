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
	public int x;
	public int y;
	public PieceColor type;
	public float scale = 0f;
	public bool isDead;
	public bool isFadeAway;
	public string iditentyType
	{
		get{
			int id = isUpper?0:1;
			return type.ToString()+ id;
		}
	}
	public override void Reset ()
	{
		base.Reset ();
		x = -1;
		y = -1;
		isDead = false;
		isFadeAway = false;
		ResetScale ();

	}
	public override void Dead ()
	{
		base.Dead ();
		isDead = true;
	}
	public void ResetScale()
	{
		if (scale != 0f) {
			this.transform.localScale = new Vector3 (scale, scale, 1);
		}
	}
	public void SetLength(float length)
	{
		SpriteRenderer spriteRender = this.gameObject.GetComponent<SpriteRenderer>();
		float originalLength = spriteRender.sprite.bounds.extents.x*2f;

		scale = length / originalLength;
		ResetScale ();
		//Debug.Log ("originalLength " + originalLength+" scale "+scale);

	}
	public override string ToString ()
	{
		return string.Format ("[Piece: x={0} y={1}]", x,y);
	}
}
