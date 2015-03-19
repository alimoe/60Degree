using UnityEngine;
using System.Collections;

public class Shake : TimeEffect {
	
	private Transform target;
	private int count;
	private int times;
	public bool isRunning;
	private float defaultRotation;
	private float shakeDistance;
	private float ratio;
	public void Init(Transform t,float time ,int c, float r = .3f, float distance = 5f)
	{
		
		defaultRotation = t.transform.localEulerAngles.z;
		target = t;
		count = c;
		times = 0;
		ratio = r;
		shakeDistance = distance;
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
				shakeDistance*=ratio;
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
