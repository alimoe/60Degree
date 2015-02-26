using UnityEngine;
using System.Collections;
using System;
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
	private static Color32[]colors = new Color32[3]{new Color32(255,255,255,0),new Color32(255,255,255,95),new Color32(255,255,255,255)};
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
	private int level = 0;
	public void SetLinkHaxegon(Hexagon hexagon)
	{
		linkedHexagon = hexagon;
		life = totalLife;
		initPosition = this.transform.localPosition;
		colorTimer = new Counter (0.2f);
		currentColor = colors [life];
		render = this.gameObject.GetComponent<SpriteRenderer> ();
		UpdateColor ();
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
	}

	public bool IsBroken()
	{
		if (isInvincible)return false;
		return life == 0;
	}
	public void Invincible()
	{
		isInvincible = true;
		UpdateColor ();
		level++;
	}
	public void Hit()
	{
		if (isInvincible)return;
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
	public void UpdateColor()
	{
		if (!isInvincible) {
			targetColor = colors [life];
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
					currentColor = new Color32 (GetChannelLerp (0, lastTimeColor, targetColor, colorTimer.percent), GetChannelLerp (1, lastTimeColor, targetColor, colorTimer.percent), GetChannelLerp (2, lastTimeColor, targetColor, colorTimer.percent), GetChannelLerp (3, lastTimeColor, targetColor, colorTimer.percent));
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
	}

}
