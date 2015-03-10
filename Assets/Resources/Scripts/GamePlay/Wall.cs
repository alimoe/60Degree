using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public enum WallFace
{
	Left,
	Right,
	Bottom,
	None

}
public class Wall :MonoBehaviour {

	// Use this for initialization
	[HideInInspector]
	public Hexagon linkedHexagon;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private SpriteRenderer render;
	private int life;
	private int totalLife = 2;
	private static byte[]alpha = new byte[3]{0,95,255};
	private static Color32[]levels = new Color32[5]{new Color32(255,255,0,255),new Color32(255,0,255,255),new Color32(0,0,255,255),new Color32(0,255,0,255),new Color32(255,0,0,255)};
	private Color32 currentColor;
	private Color32 targetColor;
	private Color32 lastTimeColor;
	private Counter colorTimer;
	[HideInInspector]
	public WallFace face;
	private int repearProcess = 0;
	private bool bouncingState = false;
	private Vector3 bouncingDirection;
	private Counter bouncingCounter = new Counter (0.5f);
	private Vector3 initPosition;
	[HideInInspector]
	public bool isInvincible = false;
	private int level = -1;
	private static Color32 WHITE = new Color32 (255, 255, 255, 255);

    private SpriteRenderer icon;

    public void Awake()
    {
        Transform[] children = this.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name.Contains("Icon")) icon = child.GetComponent<SpriteRenderer>();
        }
    }
   
	public void SetLinkHaxegon(Hexagon hexagon)
	{
		linkedHexagon = hexagon;
		life = totalLife;
		initPosition = this.transform.localPosition;
		colorTimer = new Counter (0.2f);
		currentColor = WHITE;
		render = this.gameObject.GetComponent<SpriteRenderer> ();
		UpdateColor ();
        UpdateIcon();
	}
	public static Color32 GetLevelColor(int round)
	{
		return levels [(round - 1) % 5];
	}
	public static Color32 GetRevertColor(PieceColor c)
	{
		switch (c) {
			case PieceColor.Red:
				return levels[0];
			break;
			case PieceColor.Blue:
				return levels[4];
			break;
			case PieceColor.Green:
				return levels[1];
			break;
			case PieceColor.Purple:
				return levels[3];
			break;
			case PieceColor.Yellow:
				return levels[2];
			break;
		}
		return levels[0];
	}
	public void SetFace(WallFace wallFace)
	{
		face = wallFace;
		switch (face) {
		case WallFace.Bottom:
			bouncingDirection = Vector3.down;
			break;
		case WallFace.Left:
			bouncingDirection = new Vector3(-Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f));
			break;
		case WallFace.Right:
			bouncingDirection = new Vector3(Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f));
			break;
		}
	}
	public void Reset()
	{
		isInvincible = false;
		life = totalLife;
		UpdateColor ();
        UpdateIcon();
	}

	public void ResetToZero()
	{
		level = -1;
		Reset ();
	}

	public bool IsBroken()
	{
		if (isInvincible)return false;
		return life == 0;
	}
	public void Invincible()
	{
		isInvincible = true;
		level++;
		UpdateColor ();
        UpdateIcon();
	}
	public void Hit()
	{
		if (isInvincible) {
			SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_INVINCIBLE);
			return;
		}
		SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_WALL);
		life--;
		life = Math.Min(Math.Max (life, 0),totalLife);
		if (IsBroken ())repearProcess = 0;
		bouncingState = true;
		bouncingCounter.Reset ();
		UpdateColor ();
		Render ();
	}
	public void Repear()
	{
		if (IsBroken ()) {
			repearProcess++;
			if(repearProcess>1)Reset();
		}
	}
    public void UpdateIcon()
    {
        Vector3 position;
        if (level == -1)
        {
            icon.sprite = WallIcon.Instance.GetIconAndPosition(0, out position);
        }
        else
        {
            icon.sprite = WallIcon.Instance.GetIconAndPosition((level % 5 + 1), out position);
        }
        icon.transform.localPosition = position;
    }
	public void UpdateColor()
	{

		if (!isInvincible) {
			Color32 mainColor;
            if (level == -1)
            {
                mainColor = WHITE;
                
            }
            else
            {
                mainColor = levels[level % 5];
            }
			targetColor = new Color32(mainColor.r,mainColor.g,mainColor.b, alpha[life]);
			if (!currentColor.Equals (targetColor)) {
					lastTimeColor = currentColor;
					colorTimer.Reset ();
			} else {
					currentColor = targetColor;
					lastTimeColor = currentColor;
			}
		} else {
			targetColor = levels[level%5];
		}
		Render ();
	}

	public void Render()
	{
		if (render != null)render.color = currentColor;
        if (icon != null) icon.color = currentColor;
	}
	public Color32 GetColor()
	{
		return targetColor;
	}
	void Update()
	{
		if (!targetColor.Equals (currentColor)) {
			colorTimer.Tick (Time.deltaTime);
			if (colorTimer.Expired ()) {
					currentColor = targetColor;
					lastTimeColor = targetColor;
					RenderColor ();
			} else {
					currentColor = new Color32 (targetColor.r, targetColor.g, targetColor.b, GetChannelLerp (3, lastTimeColor, targetColor, colorTimer.percent));
					RenderColor ();
			}
		}
		if (bouncingState) {
			bouncingCounter.Tick(Time.deltaTime);
			if(bouncingCounter.Expired())
			{
				bouncingState = false;
				this.transform.localPosition = initPosition;

			}
			else
			{
				this.transform.localPosition = initPosition+bouncingDirection*Mathf.Sin(Mathf.PI*bouncingCounter.percent)*.1f;
				
			}
		}
	}
	public override string ToString ()
	{
		return string.Format ("[Wall] x:{0} y:{1}", linkedHexagon.x, linkedHexagon.y);
	}
	public byte GetChannelLerp(int channel,Color32 a, Color32 b, float percent)
	{
		float s = 0f;
		float e = 0f;
		if (channel == 0) {
			s = (float)a.r;
			e = (float)b.r;
		}
		if (channel == 1) {
			s = (float)a.g;
			e = (float)b.g;
		}
		if (channel == 2) {
			s = (float)a.b;
			e = (float)b.b;
		}
		if (channel == 3) {
			s = (float)a.a;
			e = (float)b.a;
		}
		return (byte)(s + (e - s) * percent);
	}
	public void RenderColor()
	{
		if (render != null)render.color = currentColor;
        if (icon != null) icon.color = currentColor;
	}

}
