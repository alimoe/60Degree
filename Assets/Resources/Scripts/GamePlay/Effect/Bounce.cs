using UnityEngine;
using System.Collections;

public class Bounce : TimeEffect {
	private Piece piece;
	private Vector3 initPosition;
	private Vector3 direction;
	private Counter delayTimer;
	private float ratioLength;
	public virtual void Init(Piece p, Vector3 delta,float ratio, float time, float delay, OnComplete callback = null)
	{
		TimerControl.Instance.effects += BounceUpdate;
		piece = p;
		initPosition = piece.transform.localPosition + delta;
		direction = delta.normalized * -1f;
		ratioLength = ratio;
		progress = new Counter (time);
		delayTimer = new Counter (delay);
		onCompleteCallback = callback;
	}
	public void BounceUpdate()
	{
		delayTimer.Tick (Time.deltaTime);
		if (delayTimer.Expired ()) {
			progress.Tick(Time.deltaTime);
			if(progress.Expired())
			{
				if(piece!=null )
				{
					piece.transform.localPosition = initPosition;
				}
				TimerControl.Instance.effects -= BounceUpdate;
				if(onCompleteCallback!=null)onCompleteCallback();
			}
			else
			{
				if(piece!=null)
				{
					piece.transform.localPosition = initPosition+ direction*ratioLength*Mathf.Sin(Mathf.PI*progress.percent);
					if(piece.isDead||piece.isFadeAway)
					{
						TimerControl.Instance.effects -= BounceUpdate;
						if(onCompleteCallback!=null)onCompleteCallback();
					}
				}
				else
				{
					progress.percent = 1f;
				}

			}
		}
	}
}
