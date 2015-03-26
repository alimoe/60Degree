using UnityEngine;
using System.Collections;

public class Clock : Entity {
    public Piece piece;
	
  	
    public HexagonEdget triggerEdget;
    private Transform face;
    
    private Transform trigger;
	private float radius;
	public bool triggered;
	private Vector3 offset;
    void Awake()
    {
        Init();
    }
    public override void Init()
    {
		Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);

		foreach (var child in children) {

			if(child.name.Contains("Face"))face = child;
			if(child.name.Contains("Trigger"))trigger = child;

		}
		radius = face.GetComponent<SpriteRenderer> ().sprite.bounds.extents.x * 1.2f;
    }
    public Clock SetUp(Piece p)
    {
        piece = p;
		triggered = false;
     	Random();
		offset = piece.isUpper ? Vector3.down * .15f : Vector3.up * .15f;
		this.transform.parent = piece.transform.parent;
		this.transform.localPosition = piece.transform.localPosition + offset;
		this.transform.localScale = new Vector3(piece.scale, piece.scale, piece.scale);


		UpdateTrigger ();

        return this;
    }

	public void UpdateTrigger()
	{
		float angle = 0;
		//Debug.Log (triggerEdget);
		switch (triggerEdget) {
		case HexagonEdget.UpperRight:
			angle = -60f;
			break;
		case HexagonEdget.UpperLeft:
			angle = 60f;
			break;
		case HexagonEdget.UpperDown:
			angle = 180f;
			break;
		case HexagonEdget.DownUp:
			angle = 0;
			break;
		case HexagonEdget.DownRight:
			angle = -120f;
			break;
		case HexagonEdget.DownLeft:
			angle = 120f;
			break;
		}
		trigger.transform.localEulerAngles = new Vector3 (0, 0, angle);
		trigger.transform.localPosition = radius * new Vector3 (-Mathf.Sin (Mathf.PI * angle / 180f), Mathf.Cos (Mathf.PI * angle / 180f), 0);
	}

    public void Random()
    {
		triggerEdget = Hexagon.GetRandomEdget(piece.isUpper);
    }
    

    public void ShutDown()
    {
		piece = null;
        EntityPool.Instance.Reclaim(this.gameObject, "Clock");
    }

    public void OnHitClock(BoardDirection direction)
    {
     	
		if (Hexagon.IsAgainst (triggerEdget, direction, piece.isUpper)) {

			triggered = true;
		}
        	
    }

	public void Shake()
	{
        triggered = true;
		new Shake ().Init (this.transform, .7f, 10, 1f, 10f);
		new DelayCall ().Init (.7f, Expolde);
	}
    
    public void Expolde()
    {

		
		Board.Instance.CokeSurroundPiece (piece);
    }

    void Update()
    {
        if (piece != null)
        {
			this.transform.localPosition = piece.transform.localPosition + offset;
            this.transform.localPosition -= Vector3.forward;
        }
    }
}
