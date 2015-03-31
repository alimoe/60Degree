using UnityEngine;
using System.Collections;

public class GameOverMenu : OverlayMenu<GameOverMenu>
{
	
	private UILabel scoreValue;
	private UILabel playerValue;
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
			if(child.name.Contains("PlayerValue"))
			{
				playerValue = child.GetComponent<UILabel>();
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
		playerValue.text = userScore.ToString();

		if (userScore > historyScore) {
            PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicScore, userScore);
			AppControl.Instance.ReportScore(userScore);
		}
		if (userRound > historyRound) {
            PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicRound, userRound);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
        Transition();
	}
}
