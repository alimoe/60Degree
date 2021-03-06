﻿using UnityEngine;
using System.Collections;

public class Popup : Menu {

    private UISprite line1;
    private UISprite line2;
    private UISprite line3;
    
    private bool inTransitionIn;
    private float line1YPosition;
    private float line2YPosition;
    private Counter transitionInCounter;
    private ToggleButton soundBtn;
    protected virtual void Init()
    {
        
        Transform[] children = this.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name.Contains("Line1"))
            {
                line1 = child.GetComponent<UISprite>();
                line1YPosition = line1.transform.localPosition.y;
            }
            if (child.name.Contains("Line2"))
            {
                line2 = child.GetComponent<UISprite>();
                line2YPosition = line2.transform.localPosition.y;
            }
            if (child.name.Contains("Line3"))
            {
                line3 = child.GetComponent<UISprite>();
            }
            if (child.name.Contains("MusicButton"))
            {
                soundBtn = child.GetComponent<ToggleButton>();
            }

        }
        transitionInCounter = new Counter(.3f);
    }
    public override void OnOpenScreen()
    {
        base.OnOpenScreen();
        transitionInCounter.Reset();
        inTransitionIn = true;
        soundBtn.isOn = !PlayerSetting.Instance.muteSE;
       
      	SoundControl.Instance.ToggleMusic(false);

    }
    public override void OnCloseScreen()
    {
        base.OnCloseScreen();
        SoundControl.Instance.PlaySound(SoundControl.Instance.UI_TRANSITION_OUT);
        SoundControl.Instance.ToggleMusic(true);
    }
    public virtual void Transition()
    {
        if (inTransitionIn)
        {
            transitionInCounter.Tick(Time.deltaTime);
            float percent = Mathf.Max(0, 1f - transitionInCounter.percent);
            line1.transform.localPosition = new Vector3(line1.transform.localPosition.x, percent * 35f + line1YPosition, line1.transform.localPosition.z);
            line2.transform.localPosition = new Vector3(line2.transform.localPosition.x, percent * -35f + line2YPosition, line2.transform.localPosition.z);
            line3.transform.localPosition = new Vector3(line3.transform.localPosition.x, percent * -35f + line2YPosition, line3.transform.localPosition.z);
            if (transitionInCounter.Expired())
            {
                inTransitionIn = false;
            }
        }
    }
    
}
