using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GroupBounce : TimeEffect {

	private int taskCount;
	private int currentTask;
    public virtual void Init(List<Piece> pieces, Vector3 deltaTargetPosition, float time, float delay, Action callback = null)
	{
		currentTask = 0;
		taskCount = pieces.Count;
		onCompleteCallback = callback;
		if (currentTask == taskCount) {
			OnAllTaskDone();
		} else {
			float ratio = 0.5f;
			for (int i = 0;i<pieces.Count;i++) 
			{

				if(ratio>0)
				{
					Bounce task = new Bounce();
					task.Init(pieces[i],deltaTargetPosition,ratio,time,delay,OnSingleTaskDone);
				}

				ratio = Mathf.Max(0,ratio-.25f);
			}
		}
		
	}
	private void OnAllTaskDone()
	{
		if (onCompleteCallback != null)onCompleteCallback ();
	}
	public void OnSingleTaskDone()
	{
		currentTask++;
		if(currentTask == taskCount)OnAllTaskDone();
	}
}
