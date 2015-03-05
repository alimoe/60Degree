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
	public bool coke;
	private Counter cokeCounter = new Counter(3f);
	public bool moving = false;
	private BoardDirection passSession;
	private Vector3 _centerPosition;
	private static Color32 BLACK = new Color32(60,60,60,0);
	private static Color32 WHITE = new Color32(255,255,255,255);
	public Vector3 centerPosition
	{
		get{
			return this.transform.localPosition;
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
		twine = null;
		ice = null;
		coke = false;
		this.GetComponent<SpriteRenderer> ().color = WHITE;
		passSession = BoardDirection.None;
		cokeCounter.Reset ();
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
            if (ice != null)
            {
                ice.ShutDown();
                ice = null;
            }
			if(coke)
			{
				coke = false;
				new TurnColor().Init(this.gameObject,.2f,WHITE,null);
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
        if (s == PieceState.Freeze)
        {
            if (ice == null)
            {
                GameObject iceObj = EntityPool.Instance.Use("Ice") as GameObject;
                ice = iceObj.GetComponent<Ice>();
                ice.SetUp(this);
            }
        }
		if (s == PieceState.Coke)
		{
			if(!coke)
			{
				coke = true;
				new TurnColor().Init(this.gameObject,.2f,BLACK,null);
			}
		}
        state = s;
    }

    public bool OnEliminate()
    {
        if (state == PieceState.Freeze)
        {
            if (ice != null)
            {
                ice.OnHit();
                if (ice.life == 0)
                {
                    SetState(PieceState.Normal);
                }
            }
           
            return false;
        }
		if(state == PieceState.Twine)DestoryTwine ();
		DestoryGroup ();
        return true;
    }

	public void DestoryTwine ()
	{
		SetState (PieceState.Normal);
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
		//Debug.LogWarning ("OnPassBy " + this);
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

    public void OnPassHexagon(HexagonState hexagonState, float time)
    {
        //Debug.LogWarning("Pass by Hexagon " + hexagonState);
		if (hexagonState == HexagonState.Fire) {
			if(this.state!=PieceState.Coke)
			{
				state = PieceState.Coke;
				new DelayCall().Init(time,OnFire);

			}

		}
    }
	private void OnFire()
	{
		SetState (PieceState.Coke);
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_FIRE);
		cokeCounter.Reset ();
	}
	public void SetAsCore()
	{
        GameObject dot = EntityPool.Instance.Use("Gem");
		if (dot != null) {
			dot.transform.parent = this.transform;
			dot.transform.localPosition =  isUpper?new Vector3(0,-.12f,1f):new Vector3(0,.12f,1f);
			float scalar = .6f;
			dot.transform.localScale = new Vector3(scalar,scalar,scalar);
			dot.GetComponent<SpriteRenderer>().color = Wall.GetLevelColor(Board.Instance.round);
			if(isUpper == false)dot.transform.localEulerAngles = new Vector3(0,0,180);
			else dot.transform.localEulerAngles = Vector3.zero;
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

	public void Tick()
	{
		if (coke) {
			cokeCounter.Tick(1f);
			if(cokeCounter.Expired())
			{
				SetState(PieceState.Normal);
			}
		}
	}

	public override void Dead ()
	{
		base.Dead ();
		isDead = true;
		SetState (PieceState.Normal);
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
