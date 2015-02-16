using UnityEngine;
using System.Collections;

public class DropDown : TimeEffect {

	private float XSpeed;
	private float G = 40f;
	private bool inDropdownState;
	public Piece piece;
	private float ratio = 0.8f;
	private Vector3 initPosition;
	private Vector3 finalPosition;
	private Vector3 implusDirection;
	private Vector3 direction;
	private float duration;
	private Counter upTimer;
	private Counter delayTimer;
	public virtual void Init(Piece p, Vector3 targetPosition, Vector3 d,float delay,float upTime = .4f, OnCompleteWithParam callback = null)
	{
		TimerControl.Instance.effects += DropDownUpdate;
		piece = p;
		piece.isFadeAway = true;
		implusDirection = d;
		
		inDropdownState = (upTime == 0f);
		finalPosition = targetPosition;
		initPosition = piece.transform.localPosition;
		upTimer = new Counter (upTime);
		delayTimer = new Counter (delay);

		direction = finalPosition - piece.transform.position;

		//Debug.LogWarning ("Init direction" + direction);
		//Debug.LogWarning ("Init inDropdownState" + inDropdownState);
		duration = Mathf.Sqrt(direction.magnitude*2f/G);
		progress = new Counter(duration);

		onCompleteCallbackWithParam = callback;
	}

	public void DropDownUpdate()
	{
		if (!inDropdownState) {
			if(delayTimer.Expired())
			{
				upTimer.Tick(Time.deltaTime);
				if(upTimer.Expired())
				{
					initPosition = piece.transform.localPosition;
					direction = finalPosition - piece.transform.position;
					duration = Mathf.Sqrt(direction.magnitude*2f/G);
					
					progress = new Counter(duration);
					inDropdownState = true;
					delayTimer.Reset();
				}
				else
				{
					piece.transform.localPosition = initPosition+Mathf.Sin(Mathf.PI*.5f*upTimer.percent)*implusDirection*2f;
				}

			}
			else
			{
				delayTimer.Tick(Time.deltaTime);
			}

		} else {
			//if(delayTimer.Expired())
			{
				progress.Tick(Time.deltaTime);
				if(progress.Expired())
				{
					TimerControl.Instance.effects -= DropDownUpdate;
					if(onCompleteCallbackWithParam!=null)onCompleteCallbackWithParam(this);
				}
				else
				{
					float time = progress.percent*duration;
					Debug.LogWarning (" DropDownUpdate direction" + direction);
					Debug.LogWarning (" DropDownUpdate time" + time);
					piece.transform.localPosition = initPosition+direction.normalized*G*time*time;
				}
			}
			//else
			//{
				//delayTimer.Tick(Time.deltaTime);
			//}
				

			}
			

		

	}
}
