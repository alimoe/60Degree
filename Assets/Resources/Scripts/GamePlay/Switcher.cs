﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Switcher : Entity {

    private SpriteRenderer render;
    public PieceColor color;
    public bool isStatic = true;
    public bool isUpper;
    public Hexagon target;
    private Color32 defaultColor;
	private FadeAway fadeAway;
	private FadeIn fadeIn;
    void Awake()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        render = this.GetComponent<SpriteRenderer>();
        defaultColor = render.color;
    }
    public Switcher SetUp(Hexagon hexagon, bool upper)
    {
        target = hexagon;
        isUpper = upper;
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition + Vector3.down * .1f : hexagon.lowPosition + Vector3.up * .1f;
        this.transform.localScale = new Vector3(1f , 1f  , 1f);
        this.transform.localPosition += Vector3.forward;
        this.transform.localEulerAngles = isUpper ? Vector3.zero : new Vector3(0, 0, 180f);
        
        Random();
        UpdateColor();
		if (fadeAway != null)fadeAway.Cancel ();
		if (fadeIn != null)fadeIn.Cancel ();
						
		fadeIn = new FadeIn ();
		fadeIn.Init(this.gameObject, .3f, null);
        
        return this;
    }
    public void UpdateColor()
    {
        Color32 rgbColor = Wall.GetColor(color);
        render.color = new Color32(rgbColor.r, rgbColor.g, rgbColor.b, defaultColor.a);
    }
    public void Random()
    {
        
        float seed = UnityEngine.Random.Range(0, 5f);
        if (seed < 1f)
        {
            color = PieceColor.Blue;
            return; 
        }
        if (seed < 2f)
        {
            color = PieceColor.Red;
            return;
        }
        if (seed < 3f)
        {
            color = PieceColor.Green;
            return;
        }
        if (seed < 4f)
        {
            color = PieceColor.Yellow;
            return;
        }
        if (seed < 5f)
        {
            color = PieceColor.Purple;
            return;
        }
        
    }
	public void ChangeColor(PieceColor c , float time)
	{
		new DelayCall ().Init (time, c, ChangeColor);
	}
	public void ChangeColor(object c)
	{
		PieceColor color = (PieceColor)c;
		ChangeColor (color);
	}
    public void ChangeColor(PieceColor c)
    {

						
        if (isStatic) return;
		color = c;
        new TurnColor().Init(this.gameObject, .1f, Wall.GetColor(color), null);
    }
	public override void Dead ()
	{
		base.Dead ();
		if (fadeAway != null)fadeAway.Cancel ();
						
	}
    public void ShutDown()
    {
        target = null;
		if (this.gameObject.activeInHierarchy) {
			fadeAway = new FadeAway ();
			fadeAway.Init(this.gameObject, .2f, Dispose);
		}

    }

    private void Dispose(object obj)
    {
		EntityPool.Instance.Reclaim(this.gameObject, "Switcher");
    }
}
