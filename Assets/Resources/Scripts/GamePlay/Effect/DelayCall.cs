using UnityEngine;
using System.Collections;
using System;

public class DelayCall : TimeEffect {
	public object data;
    public void Init(float delay, object p, Action<object> callback = null)
	{

		TimerControl.Instance.effects += DelayCallUpdate;
		data = p;
		onCompleteCallbackWithParam = callback;
		progress = new Counter (delay);
	}
    public void Init(float delay, Action callback = null)
	{
		TimerControl.Instance.effects += DelayCallUpdate;
		onCompleteCallback = callback;
		progress = new Counter (delay);
	}
	public void DelayCallUpdate()
	{
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
			TimerControl.Instance.effects -= DelayCallUpdate;
			if(onCompleteCallbackWithParam!=null)onCompleteCallbackWithParam(data);
			if(onCompleteCallback!=null)onCompleteCallback();
            data = null;
		} 
	}
}
