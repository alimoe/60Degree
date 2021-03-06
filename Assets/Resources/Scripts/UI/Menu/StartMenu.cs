﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StartMenu : MenuSingleton<StartMenu> {

	private List<UILabel> letters;


	private UISprite helpButton;
	private UISprite staffButton;
	private UISprite leaderboardButton;


	private Counter transitionCounter;
	private bool transitionOutState;
    private bool transitionInState;
	private float labelYPosition;
	//private float buttonYPosition;
    private float footerYPosition;
    private float fadeAwayDistance = 1200f;
    private float fadeInDistance = 1200f;
	private Transform credit;
    private Transform[] footers;
		
	private float centerYPosition;
	private Transform center;
    protected override void Awake()
    {
		base.Awake ();
		Transform[] children = this.GetComponentsInChildren<Transform> (true);
		letters = new List<UILabel> ();
		foreach (var i in children) {
			if(i.name.Contains("Label"))letters.Add(i.GetComponent<UILabel>());
			
			if(i.name.Contains("HelpButton"))helpButton = i.GetComponent<UISprite>();
			if(i.name.Contains("StaffButton"))staffButton = i.GetComponent<UISprite>();

			if(i.name.Contains("Credit"))credit = i;
			if(i.name.Contains("LeadBoard"))leaderboardButton = i.GetComponent<UISprite>();


			if(i.name.Contains("Center"))center = i;

		}
		helpButton.gameObject.SetActive (false);
		staffButton.gameObject.SetActive (false);
		leaderboardButton.gameObject.SetActive (false);
		credit.gameObject.SetActive (false);

		footerYPosition = helpButton.transform.localPosition.y;
		labelYPosition = letters [0].transform.localPosition.y;
		centerYPosition = center.transform.localPosition.y;
       
		transitionCounter = new Counter (0.4f);
		footers = new Transform[4] { staffButton.transform, leaderboardButton.transform, credit.transform, helpButton.transform };

	}
	
	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
        
		staffButton.gameObject.SetActive (false);
		helpButton.gameObject.SetActive (false);
		leaderboardButton.gameObject.SetActive (false);

		transitionOutState = false;
        transitionInState = UIControl.Instance.GetLastScreen() != "";
        transitionCounter.Reset();

        if (!transitionInState)
        {


			UpdatePosition();
        }
		
	}
	private void UpdatePosition()
	{
		foreach (var i in letters)
		{
			i.transform.localPosition = new Vector3(i.transform.localPosition.x, labelYPosition, i.transform.localPosition.z);
		}
		center.transform.localPosition = new Vector3(center.transform.localPosition.x, centerYPosition, center.transform.localPosition.z);
		foreach (var t in footers)
		{
			t.localPosition = new Vector3(t.localPosition.x, footerYPosition, t.localPosition.z);
		}
		staffButton.gameObject.SetActive (PlayerSetting.Instance.tutorialPlayed);
		helpButton.gameObject.SetActive (PlayerSetting.Instance.tutorialPlayed);
		leaderboardButton.gameObject.SetActive (PlayerSetting.Instance.tutorialPlayed);
		
	}
	public void ShowCredit()
	{
		credit.gameObject.SetActive (!credit.gameObject.activeInHierarchy);
	}
	public override void OnCloseScreen ()
	{
		base.OnCloseScreen ();
		transitionCounter.Reset ();
		transitionOutState = true;
		
	}

	// Update is called once per frame
	void Update () {
		if (transitionOutState) {
			transitionCounter.Tick(Time.deltaTime);
			if(transitionCounter.Expired())
			{
                transitionOutState = false;
				base.OnCloseTransitionDone();
			}
			else
			{
				foreach (var i in letters) {
                    i.transform.localPosition += Vector3.up * Time.deltaTime * fadeAwayDistance;
				}
                Vector3 down = Vector3.down * Time.deltaTime * fadeAwayDistance;
                
				center.localPosition+=down;
                foreach (var t in footers)
                {
                    t.localPosition += down;
                }
				
			}

		}
        if (transitionInState)
        {
            transitionCounter.Tick(Time.deltaTime);
            if (transitionCounter.Expired())
            {
                transitionInState = false;
				UpdatePosition();
                base.OnOpenTransitionDone();
            }
            else
            {
                foreach (var i in letters)
                {
                    i.transform.localPosition = new Vector3(i.transform.localPosition.x, labelYPosition + fadeInDistance * (1f - transitionCounter.percent), i.transform.localPosition.z);
                }
				center.transform.localPosition = new Vector3(center.transform.localPosition.x,centerYPosition - fadeInDistance * (1f - transitionCounter.percent),center.transform.localPosition.z);


                //startButton.transform.localPosition = new Vector3(startButton.transform.localPosition.x, buttonYPosition - fadeInDistance * (1f - transitionCounter.percent), startButton.transform.localPosition.z);
                foreach (var t in footers)
                {
                    t.localPosition = new Vector3(t.localPosition.x, footerYPosition - fadeInDistance * (1f - transitionCounter.percent), t.localPosition.z);
                }
            }
        }
	}
}
