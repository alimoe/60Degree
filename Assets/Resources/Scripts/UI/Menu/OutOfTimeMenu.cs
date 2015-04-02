using UnityEngine;
using System.Collections;

public class OutOfTimeMenu : OverlayMenu<OutOfMoveMenu>
{
    
    private UILabel failed;
   
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
        }
    }

    public override void OnOpenScreen()
    {
        base.OnOpenScreen();
       
         failed.text = "MAX LEVEL:  " + PlayerSetting.Instance.GetSetting(PlayerSetting.MAX_SPEED_LEVEL);
         SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_LOSE);
    }
   
    // Update is called once per frame
    void Update()
    {
        base.Transition();
    }
}
