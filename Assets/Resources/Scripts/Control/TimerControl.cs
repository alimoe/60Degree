using UnityEngine;
using System.Collections;

public class TimerControl : Core.MonoStrictSingleton<TimerControl> {

	public delegate void TimerUpdateExcute();
	public event TimerUpdateExcute effects;

	void Update () {
		if (effects != null) {
			effects();
		}
	}
}
