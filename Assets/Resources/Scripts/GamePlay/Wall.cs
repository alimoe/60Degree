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

public enum WallState
{
    Normal,
    Invincible,
    Broken
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
	private static Color32[]levels = new Color32[5]{new Color32(255,255,0,255),new Color32(255,0,255,255),new Color32(89,255,255,255),new Color32(0,255,0,255),new Color32(255,0,0,255)};
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
    public WallState state;

    /** for Editor**/
    public void Broke()
    {
        life = 0;
        state = WallState.Broken;
        Color32 defaultRGB = render.color;
        render.color = new Color32(defaultRGB.r, defaultRGB.g, defaultRGB.b, 0);
        if (icon != null) icon.color = render.color;
    }
    /** for Editor**/
    public void Normal()
    {
        life = totalLife;
        state = WallState.Normal;
        Color32 defaultRGB = new Color32(255, 255, 255, 255);
        render.color = new Color32(defaultRGB.r, defaultRGB.g, defaultRGB.b, 255);
        if (icon != null) icon.color = render.color;
    }

    /** for Editor**/
    public void UnBroken()
    {
        state = WallState.Invincible;
        Color32 defaultRGB = levels[(level+1)%5];
        render.color = new Color32(defaultRGB.r, defaultRGB.g, defaultRGB.b, 255);
        if (icon != null) icon.color = render.color;
    }

    public static int CompareWall(Wall a, Wall b)
    {
        if (a.face == WallFace.Left && b.face != WallFace.Left) return -1;
        if (a.face == WallFace.Left && b.face == WallFace.Left)
        {
            return a.linkedHexagon.y - b.linkedHexagon.y;
        }

        if (a.face != WallFace.Bottom && b.face == WallFace.Bottom) return -1;

        if (a.face == WallFace.Right && b.face == WallFace.Right)
        {
            return -(a.linkedHexagon.y - b.linkedHexagon.y);
        }
        if (a.face == WallFace.Bottom && b.face == WallFace.Bottom)
        {
            return -(a.linkedHexagon.x - b.linkedHexagon.x);
        }
        return 0;
    }

    public void Awake()
    {
        Init();
    }
    public void Init()
    {
        Transform[] children = this.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name.Contains("Icon")) icon = child.GetComponent<SpriteRenderer>();
        }
        state = WallState.Normal;
        render = this.gameObject.GetComponent<SpriteRenderer>();
    }
	public void SetLinkHaxegon(Hexagon hexagon)
	{
		linkedHexagon = hexagon;
		life = totalLife;
		initPosition = this.transform.localPosition;
		colorTimer = new Counter (0.2f);
		currentColor = WHITE;
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

			case PieceColor.Blue:
				return levels[4];

			case PieceColor.Green:
				return levels[1];

			case PieceColor.Purple:
				return levels[3];

			case PieceColor.Yellow:
				return levels[2];

		}
		return levels[0];
	}
    public static Color32 GetColor(PieceColor c)
    {
        switch (c) {
			case PieceColor.Red:
				return levels[4];

			case PieceColor.Blue:
				return levels[2];

			case PieceColor.Green:
				return levels[3];

			case PieceColor.Purple:
				return levels[1];

			case PieceColor.Yellow:
				return levels[0];

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
        state = WallState.Normal;
		UpdateColor ();
        UpdateIcon();
	}

	public void ResetToZero()
	{
		level = -1;
        state = WallState.Normal;
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
        state = WallState.Invincible;
		level++;
		UpdateColor ();
        UpdateIcon();
	}
    public void SetLevel(int l)
    {
        level = l;
        UpdateColor();
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
        if (IsBroken())
        {
            repearProcess = 0;
            state = WallState.Broken;
        }
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
        if (WallIcon.Instance == null) return;
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
