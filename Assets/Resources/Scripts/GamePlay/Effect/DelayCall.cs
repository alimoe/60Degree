using UnityEngine;
using System.Collections;

public class DelayCall : TimeEffect {
	public object data;
	public void Init(float delay,object p, OnCompleteWithParam callback = null)
	{
		TimerControl.Instance.effects += DelayCallUpdate;
		data = p;
		onCompleteCallbackWithParam = callback;
		progress = new Counter (delay);
	}
	public void DelayCallUpdate()
	{
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
			TimerControl.Instance.effects -= DelayCallUpdate;
			if(onCompleteCallbackWithParam!=null)onCompleteCallbackWithParam(data);
		} 
	}
}
