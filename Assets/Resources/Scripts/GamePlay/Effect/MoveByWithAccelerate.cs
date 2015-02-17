using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MoveByWithAccelerate :TimeEffect {
	public Piece piece;
	public Vector3 direction;
	
	private Vector3 gravityPosition;
	public Counter trackTimer;
	public float moveSpeed;
	private bool inTrack;
	private float currentSpeed;
	private bool headGravity;
	public virtual void Init(Piece p, Vector3 targetPosition,Vector3 eliminatePosition, float speed,float trackTime, OnComplete callback = null)
	{
		TimerControl.Instance.effects += MoveByWithAccelerateUpdate;
		piece = p;
		moveSpeed = speed;
		currentSpeed = speed;
		inTrack = trackTime == 0;
		trackTimer = new Counter (trackTime);
		direction = (targetPosition - piece.transform.localPosition).normalized;
		gravityPosition = eliminatePosition;
		onCompleteCallback = callback;
		headGravity = false;
	}
	public virtual void Init(Piece p, Vector3 targetPosition, Vector3 eliminatePosition, float speed,float trackTime, OnCompleteWithParam callback = null)
	{
		Init (p, targetPosition, eliminatePosition,speed,trackTime, this.onCompleteCallback);
		onCompleteCallbackWithParam = callback;
	}
	public void MoveByWithAccelerateUpdate()
	{
		trackTimer.Tick (Time.deltaTime);
		if (trackTimer.Expired ()) {
			Vector3 gravityDirection = (gravityPosition-piece.transform.position).normalized;
			float angle = (Mathf.Acos(Vector3.Dot(gravityDirection,direction))/Mathf.PI)*180f;
			//Debug.LogWarning("angle"+angle);
			if(angle>90f)
			{
				currentSpeed*=0.9f;
				if(headGravity)
				{
					TimerControl.Instance.effects -= MoveByWithAccelerateUpdate;
					if(onCompleteCallbackWithParam!=null)onCompleteCallbackWithParam(this);
				}
			}
			else
			{
				headGravity = true;
				currentSpeed*=1.05f;
				//currentSpeed = Mathf.Min(moveSpeed*6f,currentSpeed);
			}
			direction=(direction+gravityDirection*.1f).normalized;
			piece.transform.localPosition += direction*currentSpeed*Time.deltaTime;
			

		} else {
			piece.transform.localPosition += direction*moveSpeed*Time.deltaTime;

		}
	}
}
