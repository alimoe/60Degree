using UnityEngine;
using System.Collections;
using System;
public class FadeAway : TimeEffect {

	private SpriteRenderer render;
	private byte r;
	private byte g;
	private byte b;
	public FadeAway Init(GameObject target,float time, Action<object> callback )
	{
		render = target.GetComponent<SpriteRenderer> ();
        if (render != null && TimerControl.Instance != null)
        {
			progress = new Counter(time);
			r = (byte)(render.color.r*255);
			g = (byte)(render.color.g*255);
			b = (byte)(render.color.b*255);
			onCompleteCallbackWithParam = callback;
            TimerControl.Instance.effects += FadeAwayUpdate;
		}
		return this;
	}
	public void Stop()
	{
        TimerControl.Instance.effects -= FadeAwayUpdate;
	}
    public void Cancel()
    {
        if (render!=null) render.color = new Color32(r, g, b, 255);
        Stop();
    }
	void FadeAwayUpdate ()
	{
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
				TimerControl.Instance.effects -= FadeAwayUpdate;
				if (onCompleteCallbackWithParam != null)onCompleteCallbackWithParam (render.gameObject);
                render = null;
		} else {
			render.color = new Color32(r,g,b, (byte)((1f-progress.percent)*255));
		}
	}
}
