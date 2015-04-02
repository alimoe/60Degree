using UnityEngine;
using System.Collections;

public class Explode : Entity {
    private Counter progress;
    public void Setup(Piece piece)
    {
        this.transform.parent = piece.transform.parent;
        this.transform.localPosition = piece.transform.localPosition;
        this.transform.localScale = new Vector3(.5f, .5f, .5f);
        if (!piece.isUpper)
        {
            this.transform.localEulerAngles = Vector3.zero;
           
        }
        else
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        progress = new Counter(0.25f);

    }
    void Update()
    {
        if (progress != null)
        {
            if (!progress.Expired())
            {
                progress.Tick(Time.deltaTime);
                this.transform.localScale = new Vector3(.5f + progress.percent, .5f + progress.percent, 1);
            }
            else
            {
                EntityPool.Instance.Reclaim(this.gameObject, "Explode");
            }
        }
        
    }
}
