using UnityEngine;
using System.Collections;

public class Wave : TimeEffect {
	private Transform target;
	private Vector3 direction;
	public Vector3 initPosition;
	private bool isRunning;
	private int count;
	private int currentStep;
	private float distance = 1f;
	public void Init(Transform t, Vector3 d, int c, float deltaTime)
	{
		currentStep = 0;
		count = c;
		progress = new Counter (deltaTime);
		direction = d;
		target = t;
		initPosition = t.transform.position;

		if (!isRunning)TimerControl.Instance.effects += OnWaveUpdate;
		isRunning = true;
	}
	public void Stop()
	{
		TimerControl.Instance.effects -= OnWaveUpdate;
		target = null;
		isRunning = false;
	}
    public void Reset(Vector3 position)
    {
        initPosition = position;
        progress.Reset();
    }
	// Update is called once per frame
	public void OnWaveUpdate () {
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
			currentStep++;
			if (currentStep >= count)Stop ();
			else progress.Reset ();

		} else {
			target.transform.position = initPosition+Mathf.Sin(Mathf.PI*progress.percent)*direction*distance;
		}
	}
}
