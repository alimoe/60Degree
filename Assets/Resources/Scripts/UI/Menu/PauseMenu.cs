using UnityEngine;
using System.Collections;
using System;
public class PauseMenu : OverlayMenu<PauseMenu>
{

	
	private UILabel scoreValue;
	private UILabel maxRoundValue;
    
    protected override void Awake()
    {
		base.Awake ();
        base.Init();
		Transform[] children = this.GetComponentsInChildren<Transform> ();
		foreach (var child in children) {
			
			if(child.name.Contains("ScoreValue"))
			{
				scoreValue = child.GetComponent<UILabel>();
			}
			if(child.name.Contains("RoundValue"))
			{
				maxRoundValue = child.GetComponent<UILabel>();
			}
			
		}
		    
	}

	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		
		int userScore = ClassicHudMenu.Instance.GetScore ();
		int userRound = ClassicHudMenu.Instance.GetRound ();
        int historyScore = PlayerSetting.Instance.GetSetting(PlayerSetting.ClassicScore);
        int historyRound = PlayerSetting.Instance.GetSetting(PlayerSetting.ClassicRound);
		
		maxRoundValue.text = userRound > historyRound ? userRound.ToString () : historyRound.ToString ();
		scoreValue.text = userScore > historyScore ? userScore.ToString () : historyScore.ToString ();

		if (userScore >= historyScore) {
            PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicScore, userScore);
			AppControl.Instance.ReportScore(userScore);
		}
		if (userRound > historyRound) {
            PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicRound, userRound);
		}
		
        SoundControl.Instance.PlaySound(SoundControl.Instance.UI_TRANSITION_IN);

	}
	
	// Update is called once per frame
	void Update () {
        Transition();
	}
}
