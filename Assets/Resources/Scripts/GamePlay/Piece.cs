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
	public bool isCore = false;
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
		isCore = false;

		ResetScale ();

	}
	public void SetAsCore()
	{
		GameObject dot = Instantiate (Resources.Load ("Prefabs/Dot")) as GameObject;
		if (dot != null) {
			dot.transform.parent = this.transform;
			dot.transform.localPosition =  isUpper?new Vector3(0,-.12f,1f):new Vector3(0,.12f,1f);
			float scalar = .6f;
			dot.transform.localScale = new Vector3(scalar,scalar,scalar);
			if(isUpper == false)dot.transform.localEulerAngles = new Vector3(0,0,180);
		}
		isCore = true;
	}
	private void ClearChildren()
	{
		Transform[] children = this.transform.GetComponentsInChildren<Transform> ();
		foreach (var i in children) {
			//Debug.LogError("i.name"+i.name);
			if(i.name.Contains("Dot"))GameObject.Destroy(i.gameObject);
		}
	}
	public override void Dead ()
	{
		base.Dead ();
		isDead = true;
		ClearChildren ();
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
	}
	public override string ToString ()
	{
		return string.Format ("[Piece: x={0} y={1}]", x,y);
	}
}
