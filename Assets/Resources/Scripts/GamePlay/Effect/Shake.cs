using UnityEngine;
using System.Collections;

public class Shake : TimeEffect {
	private static float Distance = 5f;
	private Piece target;
	private int count;
	private int times;
	public bool isRunning;
	private float defaultRotation;
	private float shakeDistance;
	public void Init(Piece t,float time ,int c)
	{
		
		defaultRotation = t.transform.localEulerAngles.z;
		target = t;
		count = c;
		times = 0;
		shakeDistance = Distance;
		progress = new Counter (time / (float)c);
		TimerControl.Instance.effects += OnShakeUpdate;
		isRunning = true;

	}
	public void OnShakeUpdate()
	{
		if (times < count) {
			progress.Tick(Time.deltaTime);
			float delta = Mathf.Sin(progress.percent*Mathf.PI*2f)*shakeDistance;
			target.transform.localEulerAngles = new Vector3(0,0,defaultRotation+delta);
			if(progress.Expired())
			{
				times++;
				shakeDistance*=.3f;
				progress.Reset();
			}

		} else {
			Stop();
		}
	}
	public void Stop()
	{
		TimerControl.Instance.effects -= OnShakeUpdate;
		isRunning = false;
        if (target!=null) target.transform.localEulerAngles = new Vector3(0, 0, defaultRotation);
        target = null;
	}
}
