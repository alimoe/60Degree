using UnityEngine;
using System.Collections;

public class Ice : Entity {

    private Piece piece;
    public int life = 2;
    private Transform crack;
    void Awake()
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
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_FREEZE);
    }

    public override void Reset()
    {
        base.Reset();
        ResetIce();
        life = 2;
    }
    public void OnHit()
    {
        life -= 1;
        if (life == 1)
        {
            if (crack != null) crack.gameObject.SetActive(true);
			SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_ICE);
        }
        else if(life == 0)
        {
			SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_BROKEN);
            ShutDown();
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
