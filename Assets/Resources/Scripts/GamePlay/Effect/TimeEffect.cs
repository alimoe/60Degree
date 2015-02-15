using UnityEngine;
using System.Collections;

public class TimeEffect  {

	public delegate void OnComplete();
	public OnComplete onCompleteCallback;
	public delegate void OnCompleteWithParam(object target);
	public OnCompleteWithParam onCompleteCallbackWithParam;
	protected Counter progress;
	public TimeEffect()
	{
		progress = new Counter (0);
	}
	public virtual bool isComplete()
	{
		return true;
	}
	public virtual float GetProgress()
	{
		return 0f;
	}
}
