using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class MoveByWithAccelerate :TimeEffect {
	public Piece piece;
	public Vector3 direction;
	
	private Vector3 gravityPosition;
	public Counter trackTimer;
	public float moveSpeed;
	private bool inTrack;
	private float currentSpeed;
	private bool headGravity;
	private float G = 40f;
	public virtual void Init(Piece p, Vector3 targetPosition,Vector3 eliminatePosition, float speed,float trackTime, Action callback = null)
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
		piece.isFadeAway = true;

		//Debug.LogWarning("eliminatePosition"+eliminatePosition);

	}
    public virtual void Init(Piece p, Vector3 targetPosition, Vector3 eliminatePosition, float speed, float trackTime, Action<object> callback = null)
	{
		Init (p, targetPosition, eliminatePosition,speed,trackTime, this.onCompleteCallback);
		onCompleteCallbackWithParam = callback;
	}
	public void MoveByWithAccelerateUpdate()
	{
		trackTimer.Tick (Time.deltaTime);
		if (trackTimer.Expired ()) {
			Vector3 cheesePosition = new Vector3(piece.transform.position.x,piece.transform.position.y,gravityPosition.z);
			Vector3 gravityDirection = (gravityPosition-cheesePosition).normalized;
			float angle = (Mathf.Acos(Vector3.Dot(gravityDirection,direction.normalized))/Mathf.PI)*180f;

			if(angle>90f)
			{
				float targetSpeed = (1f-(angle - 90f)/90f)*moveSpeed;
				currentSpeed += (targetSpeed - currentSpeed)*Time.deltaTime;
				if(headGravity)
				{
					TimerControl.Instance.effects -= MoveByWithAccelerateUpdate;
					if(onCompleteCallbackWithParam!=null)onCompleteCallbackWithParam(this);
				}
			}
			else
			{
				headGravity = true;
				currentSpeed += G*Time.deltaTime;

			}
			direction=(direction+gravityDirection*.2f).normalized;
			piece.transform.localPosition += direction*currentSpeed*Time.deltaTime;
			

		} else {
			piece.transform.localPosition += direction*moveSpeed*Time.deltaTime;

		}
	}
}
