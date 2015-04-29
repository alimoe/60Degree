using UnityEngine;
using System.Collections;
using System;
public class OutOfTimeMenu : OverlayMenu<OutOfMoveMenu>
{
    
    private UILabel failed;
	private UILabel title;
    protected override void Awake()
    {
        base.Awake();
        base.Init();
        Transform[] children = this.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name.Contains("Failed"))
            {
                failed = child.GetComponent<UILabel>();
            }
			if (child.name.Contains("Title"))
			{
				title = child.GetComponent<UILabel>();
			}
        }
    }

    public override void OnOpenScreen()
    {
        base.OnOpenScreen();
		int level = Math.Max (1, PlayerSetting.Instance.GetSetting (PlayerSetting.MAX_SPEED_LEVEL));
		failed.text = Localization.Get("MaxGrade")+" " + level;
        SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_LOSE);
		title.text = SpeedModeControl.Instance.remainingTimer.Expired()? Localization.Get("OTime"):Localization.Get("CantMove");
    }
   
    // Update is called once per frame
    void Update()
    {
        base.Transition();
    }
}
