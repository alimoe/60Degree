using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupMoveBy : TimeEffect {
	private int taskCount;
	private int currentTask;
	public List<Piece> pieces;
	public Vector3 directionPosition;
	public BoardDirection direction;
	public virtual void Init(List<Piece> p, Vector3 deltaTargetPosition, BoardDirection boardDirection, float time, OnComplete callback = null)
	{
		pieces = p;
		currentTask = 0;
		directionPosition = deltaTargetPosition;
		direction = boardDirection;
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
	public virtual void Init(List<Piece> p, Vector3 deltaTargetPosition, BoardDirection boardDirection, float time, OnCompleteWithParam callback = null)
	{
		Init (p, deltaTargetPosition, boardDirection,time, onCompleteCallback);
		onCompleteCallbackWithParam = callback;
	}
	private void OnAllTaskDone()
	{
		if (onCompleteCallback != null)onCompleteCallback ();
		if (onCompleteCallbackWithParam != null)onCompleteCallbackWithParam (this);
						
	}
	public void OnSingleTaskDone()
	{
		currentTask++;
		if(currentTask == taskCount)OnAllTaskDone();
	}
}
