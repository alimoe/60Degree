﻿using UnityEngine;
using System.Collections;
using System;
public class FadeAway : TimeEffect {

	private SpriteRenderer render;
	private byte r;
	private byte g;
	private byte b;
	public void Init(GameObject target,float time, Action<object> callback )
	{
		render = target.GetComponent<SpriteRenderer> ();
		if (render != null) {
			progress = new Counter(time);
			r = (byte)(render.color.r*255);
			g = (byte)(render.color.g*255);
			b = (byte)(render.color.b*255);
			onCompleteCallbackWithParam = callback;
			TimerControl.Instance.effects+= FadeAwayUpdate;
		}
	}

	void FadeAwayUpdate ()
	{
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
				TimerControl.Instance.effects -= FadeAwayUpdate;
				if (onCompleteCallbackWithParam != null)onCompleteCallbackWithParam (render.gameObject);
		} else {
			render.color = new Color32(r,g,b, (byte)((1f-progress.percent)*255));
		}
	}
}
