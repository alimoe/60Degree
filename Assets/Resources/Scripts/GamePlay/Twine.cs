using UnityEngine;
using System.Collections;

public class Twine : Entity {
    private Transform left;
    private Transform right;
    private Transform verticle;

	private Transform up_left;
	private Transform up_right;
	private Transform up_verticle;

	private Transform down_left;
	private Transform down_right;
	private Transform down_verticle;

    public int life = 3;
    private Piece piece;
    void Awake()
    {
        Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {

			if (child.name.Contains("up_left")) up_left = child;
			if (child.name.Contains("up_right")) up_right = child;
			if (child.name.Contains("up_verticle")) up_verticle = child;
			if (child.name.Contains("down_left")) down_left = child;
			if (child.name.Contains("down_right")) down_right = child;
			if (child.name.Contains("down_verticle")) down_verticle = child;

			//Debug.Log("up_left "+up_left);

        }
    }
    public override void Reset()
    {
        base.Reset();
		ResetRope ();
        life = 3;
    }
	private void ResetRope()
	{

		if (up_left != null)
		{

			up_left.gameObject.SetActive(false);
			up_right.gameObject.SetActive(false);
			up_verticle.gameObject.SetActive(false);
			
			down_left.gameObject.SetActive(false);
			down_right.gameObject.SetActive(false);
			down_verticle.gameObject.SetActive(false);
			
		}
	}
	private void SetupRope()
	{
		if(left!=null)left.gameObject.SetActive(true);
		if(right!=null)right.gameObject.SetActive(true);
		if(verticle!=null)verticle.gameObject.SetActive(true);
		new FadeIn ().Init (left.gameObject, .3f, null);
		new FadeIn ().Init (right.gameObject, .3f, null);
		new FadeIn ().Init (verticle.gameObject, .3f, null);
	}
    public void ShutDown()
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Twine");
        life = 0;
        piece = null;
		ResetRope ();
    }
	private void OnFadeAway(object target)
	{
		GameObject g = target as GameObject;
		if (g != null) {
			g.SetActive(false);
		}
	}
    public void OnPass(BoardDirection direction)
    {
		//Debug.LogWarning ("OnPass " + direction);
		int last = life;
        switch (direction)
        {
            case BoardDirection.BottomLeft:
            case BoardDirection.TopRight:
				if(piece.isUpper)
				{
					if (left.gameObject.activeInHierarchy)
					{
						life--;
						new FadeAway().Init(left.gameObject,.2f,OnFadeAway);
							
					}
				}
				else
				{
					if (right.gameObject.activeInHierarchy)
					{
						life--;
						new FadeAway().Init(right.gameObject,.2f,OnFadeAway);
					}
				}
                break;
            case BoardDirection.BottomRight:
            case BoardDirection.TopLeft:
				if(piece.isUpper)
				{
					if (right.gameObject.activeInHierarchy)
					{
						life--;
						new FadeAway().Init(right.gameObject,.2f,OnFadeAway);
					}
				}
			else
			{
				if (left.gameObject.activeInHierarchy)
				{
					life--;
					new FadeAway().Init(left.gameObject,.2f,OnFadeAway);
				}
			}
               
                break;
            case BoardDirection.Left:
            case BoardDirection.Right:
                if (verticle.gameObject.activeInHierarchy)
                {
                    life--;
					new FadeAway().Init(verticle.gameObject,.2f,OnFadeAway);
                }
                break;

        }
		if (last != life)SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_ROPE);
						
        
    }
    public void SetUp(Piece p)
    {
        piece = p;
        piece.twine = this;
		ResetRope ();
        this.transform.parent = piece.transform.parent;
        this.transform.localPosition = piece.transform.localPosition;
        this.transform.localScale = new Vector3(piece.scale, piece.scale, piece.scale);
        if (!piece.isUpper)
        {
			left = down_left;
			right = down_right;
			verticle = down_verticle;
            //this.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
			left = up_left;
			right = up_right;
			verticle = up_verticle;
            //this.transform.localEulerAngles = Vector3.zero;
        }
		SetupRope ();
        this.transform.localPosition -= Vector3.forward;
    }
    void Update()
    {
        if (piece != null)
        {
            this.transform.localPosition = piece.transform.localPosition;
            this.transform.localPosition -= Vector3.forward;
        }
        
    }
    
}
