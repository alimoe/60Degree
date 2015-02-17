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
	public Hexagon linkedHexagon;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private LineRenderer lineRender;
	private int life;
	private int totalLife = 3;
	private static Color32[]colors = new Color32[4]{new Color32(0,0,0,50),new Color32(0,64,128,100),new Color32(0,96,192,150),new Color32(0,128,255,200)};
	private Color32 currentColor;
	private Color32 targetColor;
	private Color32 lastTimeColor;
	private Counter colorTimer;
	public WallFace face;
	private int repearProcess = 0;
	public void SetLinkHaxegon(Hexagon hexagon)
	{
		linkedHexagon = hexagon;
		life = totalLife;
		//life = 0;
		colorTimer = new Counter (0.2f);
	}
	public void SetFace(WallFace wallFace)
	{
		face = wallFace;
	}
	public void Reset()
	{
		life = totalLife;
		UpdateColor ();
	}
	public bool IsBroken()
	{
		return life == 0;
	}
	public void Hit()
	{
		life--;
		life = Math.Min(Math.Max (life, 0),totalLife);
		if (IsBroken ())repearProcess = 0;
						
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

		targetColor = colors [life];
		if (!currentColor.Equals(targetColor)) {
			lastTimeColor = currentColor;
			colorTimer.Reset ();
		} else {
			currentColor = targetColor;
			lastTimeColor = currentColor;
		}
		Render ();
	}
	public void Render()
	{
		if (lineRender == null) {
			lineRender = this.gameObject.GetComponent<LineRenderer>();

		}
		if (lineRender != null) {
			lineRender.SetVertexCount (2);
			lineRender.SetWidth (0.1f, 0.1f);
			lineRender.SetPosition (0, startPosition);
			lineRender.SetPosition (1, endPosition);
			RenderColor();
		}
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
		lineRender.material.color = currentColor;
	}
	public void AddLine(Vector3 s, Vector3 e,float ratio)
	{
		Vector3 center = s + (e - s) * .5f;
		startPosition = center-(center-s)*ratio;
		endPosition = center+(e-center)*ratio;
		UpdateColor ();
		Render ();
	}
}
