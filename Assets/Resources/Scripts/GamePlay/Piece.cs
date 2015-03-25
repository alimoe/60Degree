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
    Coke,
    Clock

}

public class Piece : Entity {

	public bool isUpper = false;
	public bool isCore = false;
    public PieceState state = PieceState.Normal;
	public int x;
	public int y;
	public float length;
	public float height;
    public int id;

	public PieceColor type;
    public PieceColor colorType;
	public float scale = 0f;
	public bool isDead;
	public Twine twine;
    public Ice ice;
	public Clock clock;

	public bool coke;
    private Counter cokeCounter = new Counter(3f);
	public bool moving = false;
	private BoardDirection passSession;
	private float passSessionTime;
	private Vector3 _centerPosition;
	private static Color32 BLACK = new Color32(60,60,60,255);
	private Color32 defaultColor;
	private Shake shaker;
    public static BoardDirection sortingDirection;

    public static int ComparePiece(Piece a, Piece b)
    {
        switch (sortingDirection)
        {
            case BoardDirection.Left:

                if (a.x == b.x && a.isUpper != b.isUpper)
                {
                    return a.isUpper ? 1 : -1;
                }
                else
                {
                    return b.x - a.x;
                }

    
            case BoardDirection.Right:
                if (a.x == b.x && a.isUpper != b.isUpper)
                {
                    return a.isUpper ? -1 : 1;
                }
                else
                {
                    return a.x - b.x;
                }
    
            case BoardDirection.BottomRight:

            case BoardDirection.BottomLeft:
                if (a.y == b.y && a.isUpper != b.isUpper)
                {
                    return a.isUpper ? -1 : 1;
                }
                else
                {
                    return b.y - a.y;
                }
    
            case BoardDirection.TopLeft:
            case BoardDirection.TopRight:
                if (a.y == b.y && a.isUpper != b.isUpper)
                {
                    return a.isUpper ? 1 : -1;
                }
                else
                {
                    return a.y - b.y;
                }


    
        }

        return 0;
    }

	public Vector3 centerPosition
	{
		get{
			return _centerPosition;
		}
		set{
			 _centerPosition = value;
			this.transform.localPosition = value;
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
    void Awake()
    {
        defaultColor = this.GetComponent<SpriteRenderer>().color;
    }

	public void Shake()
	{
		if (shaker == null)shaker = new Shake ();
		if (shaker.isRunning)shaker.Stop ();
		shaker.Init (this.transform, .25f, 3);					

	}

	public void StopShake()
	{
		if (shaker!=null && shaker.isRunning)shaker.Stop ();
		shaker = null;
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
		clock = null;
		coke = false;
		shaker = null;

        colorType = type;
        defaultColor = Wall.GetColor(type);
        this.GetComponent<SpriteRenderer>().color = defaultColor;
		passSession = BoardDirection.None;
		cokeCounter.Reset ();
	}

    public bool CanEliminate()
    {
        return state != PieceState.Coke;
    }

    public bool CanMove()
    {
		return state == PieceState.Normal || state == PieceState.Coke || state == PieceState.Clock;
    }
    public void ChangeColor(PieceColor color)
    {
        colorType = color;
        defaultColor = Wall.GetColor(color);
        this.GetComponent<SpriteRenderer>().color = defaultColor;
        
    }
    public void ChangeColor(object color)
    {
        colorType = (PieceColor)color;
        defaultColor = Wall.GetColor(colorType);
        new TurnColor().Init(this.gameObject, .3f, defaultColor, null);
    }
	public void SetState(object s)
	{
		SetState ((PieceState)s);
	}

	public void ResetBuffer()
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
			new TurnColor().Init(this.gameObject, .2f, defaultColor, null);
		}
		if(clock!=null)
		{
			clock.ShutDown();
			clock = null;
		}
	}


    public void SetState(PieceState s)
    {
		if (s != state) {
			ResetBuffer();
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
		if (s == PieceState.Clock)
		{
			if (clock == null)
			{
				GameObject clockObj = EntityPool.Instance.Use("Clock") as GameObject;
				clock = clockObj.GetComponent<Clock>();
				clock.SetUp(this);
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
		if(state == PieceState.Twine)Normal ();
		if(state == PieceState.Clock)Normal ();
		DestoryGroup ();
        return true;
    }

	public void Normal ()
	{
		SetState (PieceState.Normal);
	}

	public void DestoryGroup(bool withSound = true)
	{
		if (group != null)
		{
			if(withSound)group.CutChain();
			group.RemoveChild(this);
			group = null;
		}
	}

    public void OnPassByPiece(BoardDirection direction,float time)
    {
		//Debug.LogWarning ("OnPassBy " + this);
		passSession = direction;
		passSessionTime = time;
    }

    public void OnPassHexagon(Hexagon hexagon, float time, bool upper)
    {
        if (hexagon.GetState(upper) == HexagonState.Fire)
        {
            if (this.state != PieceState.Coke)
            {
                state = PieceState.Coke;
                new DelayCall().Init(time, OnFire);
            }
            else
            {
                cokeCounter.Reset();
            }

        }
        if (hexagon.GetState(upper) == HexagonState.SwitchType)
        {
            Switcher switcher = upper ? hexagon.switchU : hexagon.switchD;
            if (switcher != null)
            {
                new DelayCall().Init(time, switcher.color, ChangeColor);
            }
        }
    }

    public void OnHitPiece(BoardDirection direction, float time)
    {
		if (clock != null) {
			clock.OnHitClock(direction);
			if(clock.triggered)new DelayCall().Init(time, OnClock);
		}
    }
	private void OnClock()
	{
		if (clock != null) {
			clock.Shake();
			//new DelayCall().Init(.5f, Normal);
		}
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
					twine.OnPass(passSession,passSessionTime);
					if (twine.life == 0)
					{
						new DelayCall().Init(passSessionTime+.2f,PieceState.Normal,SetState);
					}
				}
				passSession = BoardDirection.None;
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
            dot.transform.localPosition = new Vector3(0, -.12f, 1f);
			float scalar = .6f;
			dot.transform.localScale = new Vector3(scalar,scalar,scalar);
			dot.GetComponent<SpriteRenderer>().color = Wall.GetRevertColor(this.type);
            dot.transform.localEulerAngles = Vector3.zero;
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
		DestoryGroup (false);
		StopShake ();
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
		
        colorType = this.type;
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
