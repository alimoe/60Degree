using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupMoveBy : TimeEffect {
	private int taskCount;
	private int currentTask;
	public virtual void Init(List<Piece> pieces, Vector3 deltaTargetPosition, float time, OnComplete callback = null)
	{
		currentTask = 0;
		taskCount = pieces.Count;
		onCompleteCallback = callback;
		if (currentTask == taskCount) {
			OnAllTaskDone();
		} else {
			foreach (var i in pieces) 
			{
				MoveBy task = new MoveBy();
				task.Init(i,i.transform.localPosition+deltaTargetPosition,time,OnSingleTaskDone);
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
