using UnityEngine;
using System.Collections;

public class Triggering : Entity {

    public bool pressed;
    public Hexagon target;
    public bool isUpper;
    public delegate void OnTrigger(Triggering sender);
    public event OnTrigger onPressedCallback;
    public event OnTrigger onReleasedCallback;

    public Triggering SetUp(Hexagon hexagon, bool upper)
    {
        this.isUpper = upper;
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition : hexagon.lowPosition;
        SpriteRenderer spriteRender = this.gameObject.GetComponent<SpriteRenderer>();
        float originalLength = spriteRender.sprite.bounds.extents.x * 2f;
        float scale = hexagon.length / originalLength;
        this.transform.localScale = new Vector3(scale, scale, scale);
        this.transform.localPosition += Vector3.forward;
        this.transform.localEulerAngles = isUpper ? Vector3.zero : new Vector3(0, 0, 180f);
        new FadeIn().Init(this.gameObject, .3f, null);
        return this;
    }

    public void Press()
    {
        pressed = true;
        if (onPressedCallback != null) onPressedCallback(this);
    }
    public void Release()
    {
        pressed = false;
        if (onReleasedCallback != null) onReleasedCallback(this);
    }
    public void ShutDown()
    {
        new FadeAway().Init(this.gameObject, .2f, Dispose);
    }

    private void Dispose(object obj)
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Triggering");
    }
}
