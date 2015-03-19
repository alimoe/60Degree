using UnityEngine;
using System.Collections;

public class Switcher : MonoBehaviour {

    private SpriteRenderer render;
    public PieceColor color;

    void Awake()
    {
        render = this.GetComponent<SpriteRenderer>();
    }

    public Switcher SetUp(Hexagon hexagon, bool isUpper)
    {
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition : hexagon.lowPosition;
        this.transform.localScale = new Vector3(Hexagon.Scale * .85f, Hexagon.Scale * .85f, Hexagon.Scale * .85f);
        this.transform.localPosition += Vector3.forward;
        this.transform.localEulerAngles = isUpper ? Vector3.zero : new Vector3(0, 0, 180f);
        render.color = new Color32(255, 255, 255, 255);
        new FadeIn().Init(this.gameObject, .3f, null);
        
        return this;
    }
    public void ShutDown()
    {
        new FadeAway().Init(this.gameObject, .2f, Dispose);
    }

    private void Dispose(object obj)
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Switcher");
    }
}
