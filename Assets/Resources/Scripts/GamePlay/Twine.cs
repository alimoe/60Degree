using UnityEngine;
using System.Collections;

public class Twine : Entity {
    private Transform left;
    private Transform right;
    private Transform verticle;
    public int life = 3;
    private Piece piece;
    void Start()
    {
        Transform[] children = this.transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name.Contains("left")) left = child;
            if (child.name.Contains("right")) right = child;
            if (child.name.Contains("verticle")) verticle = child;
        }
    }
    public override void Reset()
    {
        base.Reset();
        if (left != null)
        {
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
            verticle.gameObject.SetActive(true);
        }
       
        life = 3;
    }
    public void ShutDown()
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Twine");
        life = 0;
        piece = null;
    }
    public void OnPass(BoardDirection direction)
    {
        switch (direction)
        {
            case BoardDirection.BottomLeft:
            case BoardDirection.TopRight:
                if (right.gameObject.activeInHierarchy)
                {
                    life--;
                    right.gameObject.SetActive(false);
                }
                break;
            case BoardDirection.BottomRight:
            case BoardDirection.TopLeft:
                if (left.gameObject.activeInHierarchy)
                {
                    life--;
                    left.gameObject.SetActive(false);
                }
                break;
            case BoardDirection.Left:
            case BoardDirection.Right:
                if (verticle.gameObject.activeInHierarchy)
                {
                    life--;
                    verticle.gameObject.SetActive(false);
                }
                break;

        }
        
    }
    public void SetUp(Piece p)
    {
        piece = p;
        piece.twine = this;
        //Debug.Log("Twine Setup");
        this.transform.parent = piece.transform.parent;
        this.transform.localPosition = piece.transform.localPosition;
        this.transform.localScale = new Vector3(1, 1, 1);
        if (!piece.isUpper)
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            this.transform.localEulerAngles = Vector3.zero;
        }
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
