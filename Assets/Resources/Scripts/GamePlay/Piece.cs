using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum PieceColor
{
	Blue,
	Purple,
	Yellow,
	Red,
	Green,
	None
}
public enum PieceState
{
    Normal,
    Freeze,
    Twine,
    Coke

}

public class Piece : Entity {

	public bool isUpper = false;
	public bool isCore = false;
    public PieceState state = PieceState.Normal;
	public int x;
	public int y;
	public float length;
	public float height;
	private Vector3 heightVector;

	public PieceColor type;
	public float scale = 0f;
	public bool isDead;

    public Twine twine;
    public Ice ice;

	public bool moving = false;
	private BoardDirection passSession;
	private Vector3 _centerPosition;
	public Vector3 centerPosition
	{
		get{
			if(isUpper)return this.transform.localPosition+heightVector*.5f;
			else return this.transform.localPosition-heightVector*.5f;
		}
	}
	private bool _isFadeAway;
	public bool isFadeAway{
		get{return _isFadeAway;}
		set{
			_isFadeAway = value;
			if(_isFadeAway)DestoryGroup();
		}
	}
    public PieceGroup group;
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
        state = PieceState.Normal;
		ResetScale ();
        group = null;
		moving = false;
		passSession = BoardDirection.None;
	}

    public bool CanEliminate()
    {
        return state != PieceState.Coke;
    }

    public bool CanMove()
    {
        return state == PieceState.Normal || state == PieceState.Coke;
    }

    public void SetState(PieceState s)
    {
        if (s == PieceState.Normal)
        {
            if (twine != null)
            {
                twine.ShutDown();
                twine = null;
            }
        }
        if (s == PieceState.Twine)
        {
            if (twine == null)
            {
                GameObject twineObj = EntityPool.Instance.Use("Twine") as GameObject;
                twine = twineObj.GetComponent<Twine>();
                twine.SetUp(this);
            }
        }
        state = s;
    }

    public bool OnEliminate()
    {
        if (state == PieceState.Freeze)
        {
            return false;
        }
        if (state == PieceState.Twine)
        {
            return false;
        }
		DestoryGroup ();
        return true;
    }
	public void DestoryGroup()
	{
		if (group != null)
		{
			group.RemoveChild(this);
			group = null;
		}
	}

    public void OnPassByPiece(BoardDirection direction)
    {
		Debug.LogWarning ("OnPassBy " + this);
		passSession = direction;
        
    }
	public void CommitChanges()
	{
		if (moving) {
				moving = false;
				passSession = BoardDirection.None;
		} else {
			//Debug.LogWarning ("CommitChanges " + passSession);
			if(passSession != BoardDirection.None)
			{
				if (twine != null)
				{
					twine.OnPass(passSession);
					if (twine.life == 0)
					{
						SetState(PieceState.Normal);
					}
				}
				passSession = BoardDirection.None;
			}

		}
	}

    public void OnPassHexagon(HexagonState hexagon, int distance)
    {

    }

	public void SetAsCore()
	{
        GameObject dot = EntityPool.Instance.Use("Gem");
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
            if (i.name.Contains("Dot")) EntityPool.Instance.Reclaim(i.gameObject, "Gem");
           
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

	public void SetLength(float l)
	{
		length = l;
		height = Mathf.Sin (Mathf.PI / 3f) * length;
		heightVector = Vector3.up * height;
		SpriteRenderer spriteRender = this.gameObject.GetComponent<SpriteRenderer>();
		float originalLength = spriteRender.sprite.bounds.extents.x*2f;

		scale = length / originalLength;
		ResetScale ();
	}
	public override string ToString ()
	{
		return string.Format ("[Piece: x={0} y={1} isUpper={2}]", x, y, isUpper);
	}
}
