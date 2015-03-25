using UnityEngine;
using System.Collections;

public class Ice : Entity {

    public Piece piece;
    public int life = 2;
    private Transform crack;
    void Awake()
    {
		Init ();
    }
    public override void Init()
	{
		Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
		foreach (var child in children)
		{
			
			if (child.name.Contains("Crack")) crack = child;
			
		}
		if (crack != null) crack.gameObject.SetActive(false);
        
	}
    public void ResetIce()
    {
        if (crack != null) crack.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
    }
    public void SetUp(Piece p)
    {
        piece = p;
        piece.ice = this;
        ResetIce();
        this.transform.parent = piece.transform.parent;
        this.transform.localPosition = piece.transform.localPosition;
        this.transform.localScale = new Vector3(piece.scale, piece.scale, piece.scale);
        if (!piece.isUpper)
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            this.transform.localEulerAngles = Vector3.zero;
        }
		if(SoundControl.Instance!=null)SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_FREEZE);
    }
    public void SetLife(int l)
    {
        life = l;
        this.gameObject.SetActive(true);
        UpdateIce();
    }
    public override void Reset()
    {
        base.Reset();
        ResetIce();
        life = 2;
    }
    private void UpdateIce()
    {
        if (life == 1)
        {
            if (crack != null) crack.gameObject.SetActive(true);

        }
        else if (life == 0)
        {
            if (crack != null)crack.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    public void OnHit()
    {
        life -= 1;
        UpdateIce();
        if (life == 0)
        {
            int count = UnityEngine.Random.Range(8, 12);
            new Spread().Init(piece, count, 0.7f, 4.5f);
            SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_BROKEN);
            ShutDown();
        }
        else
        {
            int count = UnityEngine.Random.Range(6, 8);
            new Spread().Init(piece, count, 0.7f, 4.5f);
            SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_ICE);

        }
    }

    public void ShutDown()
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Ice");
        life = 0;
        piece = null;
        
    }
	void Update () {
        if (piece != null)
        {
            this.transform.localPosition = piece.transform.localPosition;
            this.transform.localPosition -= Vector3.forward;
        }
	}
}
