﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelSelectMenu : MenuSingleton<LevelSelectMenu> {
	private List<LockButton> buttons;
    private UIScrollView scollBody;
    private int level = -1;
	protected override void Awake()
	{
		base.Awake ();
		Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
		buttons = new List<LockButton> ();
		foreach (var child in children)
		{
			if (child.name.Contains("LockButton"))
			{
				buttons.Add(child.GetComponent<LockButton>());
				UIButton button = child.GetComponent<UIButton>();
				button.isEnabled = false;
				UIEventListener.Get(button.gameObject).onClick+=OnClick;
			}
            if (child.name.Contains("ScrollView"))
            {
                scollBody = child.GetComponent<UIScrollView>();
            }
		}
		buttons.Sort (CompareLockButton);

	}
	private void OnClick(GameObject obj)
	{
		LockButton button = obj.GetComponent<LockButton> ();
		if (button.isLocked)return;
		level = buttons.IndexOf(button);
		LevelControl.Instance.LoadLevel (button.levelIndex,level); 

	}
	private int CompareLockButton(LockButton a, LockButton b)
	{
		return a.index - b.index;
	}

	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
        int recond = PlayerSetting.Instance.GetSetting(PlayerSetting.USER_LEVEL_PROGRESS);
        if (level < 0) level = recond;
	    
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Lock(i > recond);
        }
	    
        if (level >= 20)
        {
            //scollBody.SetDragAmount(1f, 0f, false);
			//scollBody.ResetPosition();
        }
        else
        {
            //scollBody.SetDragAmount(0f, 0f, false);
        }
	}

	public int GetLevelByIndex(int index)
	{
		if (index < buttons.Count) {
			return buttons[index].levelIndex;
		}
		return -1;
	}

	public override void OnCloseScreen()
	{
		base.OnCloseScreen();
		base.OnCloseTransitionDone();
	}
}
