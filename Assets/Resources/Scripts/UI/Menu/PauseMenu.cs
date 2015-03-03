using UnityEngine;
using System.Collections;

public class PauseMenu : MenuSingleton<PauseMenu> {

	private UISprite line1;
	private UISprite line2;
	private UISprite line3;
	private bool inTransitionIn;
	private Counter transitionInCounter;
	private UILabel scoreValue;
	private UILabel maxRoundValue;
	private float line1YPosition;
	private float line2YPosition;
	void Awake () {
		base.Awake ();
		Transform[] children = this.GetComponentsInChildren<Transform> ();
		foreach (var child in children) {
			if(child.name.Contains("Line1"))
			{
				line1 = child.GetComponent<UISprite>();
				line1YPosition = line1.transform.localPosition.y;
			}
			if(child.name.Contains("Line2"))
			{
				line2 = child.GetComponent<UISprite>();
				line2YPosition = line2.transform.localPosition.y;
			}
			if(child.name.Contains("Line3"))
			{
				line3 = child.GetComponent<UISprite>();
			}
			if(child.name.Contains("ScoreValue"))
			{
				scoreValue = child.GetComponent<UILabel>();
			}
			if(child.name.Contains("RoundValue"))
			{
				maxRoundValue = child.GetComponent<UILabel>();
			}
		}
		transitionInCounter = new Counter (.3f);
	}

	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		transitionInCounter.Reset ();
		inTransitionIn = true;
		int userScore = HudMenu.Instance.GetScore ();
		int userRound = HudMenu.Instance.GetRound ();
		int historyScore = PlayerSetting.Instance.GetSetting ("Score");
		int historyRound = PlayerSetting.Instance.GetSetting ("Round");
		
		maxRoundValue.text = userRound > historyRound ? userRound.ToString () : historyRound.ToString ();
		scoreValue.text = userScore > historyScore ? userScore.ToString () : historyScore.ToString ();

		if (userScore > historyScore) {
			PlayerSetting.Instance.SetSetting("Score",userScore);
			AppControl.Instance.ReportScore(userScore);
		}
		if (userRound > historyRound) {
			PlayerSetting.Instance.SetSetting("Round",userRound);
		}
		SoundControl.Instance.PlaySound (SoundControl.Instance.UI_TRANSITION_IN);
	}
	public override void OnCloseScreen ()
	{
		base.OnCloseScreen ();
		SoundControl.Instance.PlaySound (SoundControl.Instance.UI_TRANSITION_OUT);
	}
	// Update is called once per frame
	void Update () {
		if (inTransitionIn) {
			transitionInCounter.Tick(Time.deltaTime);
			float percent = Mathf.Max(0, 1f - transitionInCounter.percent);
			line1.transform.localPosition = new Vector3(line1.transform.localPosition.x,percent*35f + line1YPosition,line1.transform.localPosition.z);
			line2.transform.localPosition = new Vector3(line2.transform.localPosition.x,percent*-35f + line2YPosition,line2.transform.localPosition.z);
			line3.transform.localPosition = new Vector3(line3.transform.localPosition.x,percent*-35f + line2YPosition,line3.transform.localPosition.z);
			if(transitionInCounter.Expired())
			{
				inTransitionIn = false;
			}
		}
	}
}
