using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MoveBy :TimeEffect {
	private Piece piece;
	private Vector3 initPosition;
	private Vector3 finalPosition;
	public virtual void Init(Piece p, Vector3 targetPosition, float time, OnComplete callback = null)
	{
		TimerControl.Instance.effects += MoveByUpdate;
		piece = p;

		initPosition = piece.transform.localPosition;
		finalPosition = targetPosition;
		progress = new Counter (time);
		progress.Reset ();
		onCompleteCallback = callback;
	}

	public void MoveByUpdate()
	{
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
			if(piece!=null)piece.transform.localPosition = finalPosition;
			TimerControl.Instance.effects -= MoveByUpdate;
			if (onCompleteCallback != null)onCompleteCallback ();
		} else {
			
			if(piece!=null)
			{
				piece.transform.localPosition = initPosition+(finalPosition - initPosition)*progress.percent;
			}
			else
			{
				progress.percent = 1f;
			}
		}
	}
}
