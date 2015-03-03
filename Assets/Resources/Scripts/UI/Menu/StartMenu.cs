﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StartMenu : MenuSingleton<StartMenu> {

	private List<UILabel> letters;
	private UISprite startButton;
	private Counter transitionCounter;
	private bool transitionOutState;
	private float labelYPosition;
	private float buttonYPosition;
	void Awake () {
		base.Awake ();
		Transform[] children = this.GetComponentsInChildren<Transform> (true);
		letters = new List<UILabel> ();
		foreach (var i in children) {
			if(i.name.Contains("Label"))letters.Add(i.GetComponent<UILabel>());
			if(i.name.Contains("PlayButton"))startButton = i.GetComponent<UISprite>();
		}
		labelYPosition = letters [0].transform.localPosition.y;
		buttonYPosition = startButton.transform.localPosition.y;
		transitionCounter = new Counter (0.4f);
	}
	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		transitionOutState = false;
		foreach (var i in letters) {
			i.transform.localPosition = new Vector3(i.transform.localPosition.x,labelYPosition,i.transform.localPosition.z);
		}
		startButton.transform.localPosition = new Vector3 (startButton.transform.localPosition.x, buttonYPosition, startButton.transform.localPosition.z);
	}
	public override void OnCloseScreen ()
	{
		base.OnCloseScreen ();
		transitionOutState = true;
		//Debug.LogWarning("Start Menu Closing");

	}

	// Update is called once per frame
	void Update () {
		if (transitionOutState) {
			transitionCounter.Tick(Time.deltaTime);
			if(transitionCounter.Expired())
			{
				base.OnCloseTransitionDone();
			}
			else
			{
				foreach (var i in letters) {
					i.transform.localPosition += Vector3.up*Time.deltaTime*1200f;
				}
				startButton.transform.localPosition += Vector3.down*Time.deltaTime*1200f;
			}

		}
	}
}