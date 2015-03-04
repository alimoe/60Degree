using UnityEngine;
using System.Collections;
using System;

public class TimeEffect  {

    
    public Action onCompleteCallback;
    public Action<object> onCompleteCallbackWithParam;

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
