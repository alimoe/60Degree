using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HudMenu : MenuSingleton<HudMenu>{

	private List<UILabel> tips;
	private UILabel scoreLabel;
	private List<TipAnimateTask> animateTips;
	private List<UILabel> unusedTips;
	private List<UILabel> inusedTips;
    private UILabel wallTip;
	private UISprite pauseButton;
	private ToggleButton bgmButton;
	private UILabel roundTipLabel;
	private UILabel roundLabel;
	private UILabel roundValue;
	private UILabel recordLabel;
	private UILabel hintLabel;
	private UILabel achivementLabel;
	private SkillButton skillButton;
	private Counter progressCounter;
	private Counter threholdCounter;
	private Counter threholdMaxCounter;
	private Counter totalScoreCounter;
	private Camera nguiCamera;
	private int totalScore;
	private int totalRound;
	private Vector3 initPosition;
	
	private bool inTransitionIn;
	private Counter transitionInCounter;

	private bool inNewRound;
	private Counter roundFadeInCounter;
	private Counter roundIdleCounter;
	private Counter resetSkillButtonPositionCounter;
	private int roundStep = 0;
	private float roundXPosition;
	private float headYPosition;
	private float footYPosition;
	private float roundXRange = 200f;
	private int level = 1;
	private int historyRound;
	private int historyScore;

	private string corePieceWarningMessage = "Puzzle with core color [00ff00]can't be send out[-] the Broken wall";
	private string overFlowWarningMessage = "The Broken wall can only send [00ff00]4 puzzles[-] out at one momenet";
	void Awake () {
		base.Awake ();
		tips = new List<UILabel> ();
		nguiCamera = GameObject.Find ("UI Root/Camera").GetComponent<Camera> ();
		
		animateTips = new List<TipAnimateTask> ();
		Transform[] children = this.GetComponentsInChildren<Transform> ();
		foreach (var child in children) {
			if(child.name.Contains("Tips"))
			{
				tips.Add(child.GetComponent<UILabel>());
				tips[tips.Count-1].gameObject.SetActive(false);
			}
			if(child.name.Contains("Score"))
			{
				scoreLabel = child.GetComponent<UILabel>();
				headYPosition = scoreLabel.transform.localPosition.y;
				//scoreLabel.gameObject.SetActive(false);
			}
			if(child.name.Contains("SkillButton"))
			{
				skillButton = child.GetComponent<SkillButton>();
				footYPosition = skillButton.transform.localPosition.y;
				//skillButton.gameObject.SetActive(false);
			}
			if(child.name.Contains("PauseButton"))
			{
				pauseButton = child.GetComponent<UISprite>();

			}
			if(child.name.Contains("RoundTip"))
			{
				roundTipLabel = child.GetComponent<UILabel>();
				roundXPosition = roundTipLabel.transform.localPosition.x;
				roundTipLabel.gameObject.SetActive(false);
			}
			if(child.name.Contains("RoundValue"))
			{
				roundValue = child.GetComponent<UILabel>();
			}
			if(child.name.Contains("Hint"))
			{
				hintLabel = child.GetComponent<UILabel>();
				hintLabel.gameObject.SetActive(false);
			}
			if(child.name.Contains("RoundLabel"))
			{
				roundLabel = child.GetComponent<UILabel>();
			}
			if(child.name.Contains("Record"))
			{
				recordLabel = child.GetComponent<UILabel>();
				recordLabel.gameObject.SetActive(false);
			}
            if (child.name.Contains("WallTip"))
            {
                wallTip = child.GetComponent<UILabel>();
                wallTip.gameObject.SetActive(false);
            }
			if (child.name.Contains("BGMButton"))
			{
				bgmButton = child.GetComponent<ToggleButton>();
				
			}
			if (child.name.Contains("Achivement"))
			{
				achivementLabel = child.GetComponent<UILabel>();
				achivementLabel.gameObject.SetActive(false);
				
			}
		}
		inusedTips = new List<UILabel> ();
		unusedTips = new List<UILabel> (tips.ToArray());

		progressCounter = new Counter (.5f);
		threholdCounter = new Counter (2f);
		threholdMaxCounter = new Counter (10f);
		totalScoreCounter = new Counter (.5f);
		resetSkillButtonPositionCounter = new Counter (5f);
        initPosition = scoreLabel.transform.localPosition;
		//Debug.LogWarning (scoreLabel.transform.localPosition);
		totalScore = 0;
		totalRound = 1;
		totalScoreCounter.percent = 1;
		transitionInCounter = new Counter (.3f);
		roundFadeInCounter = new Counter (.2f);
		roundIdleCounter = new Counter (1.3f);

		this.gameObject.SetActive (false);
	}
	public int GetScore()
	{
		return totalScore;
	}
	public int GetRound()
	{
		return totalRound;
	}
	public void Reset()
	{
		totalScore = 0;
		totalRound = 1;
		roundStep = 0;
		level = 1;
		historyScore = PlayerSetting.Instance.GetSetting ("Score");
		recordLabel.gameObject.SetActive (false);
		skillButton.CostProgress ();
		scoreLabel.text = totalScore.ToString ();
		roundValue.text = level.ToString ();
	}
	public void InitLayout()
	{
		
		SkillControl.Instance.SetSkillPosition (nguiCamera.WorldToScreenPoint (skillButton.transform.position));

		/*
		scoreLabel.gameObject.SetActive(true);
		skillButton.gameObject.SetActive(true);
		pauseButton.gameObject.SetActive(true);
		*/

	}
	void Start()
	{
		/*
        scoreLabel.transform.localPosition = new Vector3(scoreLabel.transform.localPosition.x, headYPosition + 50f,scoreLabel.transform.localPosition.z);
		roundLabel.transform.localPosition = new Vector3(roundLabel.transform.localPosition.x, headYPosition + 50f,roundLabel.transform.localPosition.z);
		roundValue.transform.localPosition = new Vector3(roundValue.transform.localPosition.x, headYPosition + 50f,roundValue.transform.localPosition.z);

		skillButton.transform.localPosition = new Vector3(skillButton.transform.localPosition.x, footYPosition - 50f,skillButton.transform.localPosition.z);
		pauseButton.transform.localPosition = new Vector3(pauseButton.transform.localPosition.x, footYPosition - 50f,pauseButton.transform.localPosition.z);
         */
        //Debug.LogWarning("Screen.w" + Screen.width);
        //Debug.LogWarning("Screen.h" + Screen.height);
	}
	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		//inTransitionIn = true;
		transitionInCounter.Reset ();
		InitLayout ();
		bgmButton.isOn = !PlayerSetting.Instance.muteBGM;
		pauseButton.gameObject.SetActive(!TutorialControl.Instance.isActive);
		bgmButton.gameObject.SetActive(!TutorialControl.Instance.isActive);
		Board.Instance.onEliminatePieceCallback += AddScore;
		Board.Instance.onDropDownPieceCallback += AddProgress;
		Board.Instance.onHitRoundCallback += AddRound;
		Board.Instance.onWallProgressCallback += ReinforceWall;
		Board.Instance.OnTryToGetawayCorePieceCallback += WarnCorePiece;
		Board.Instance.OnTryToGetawayOverflowPieceCallback += WarnOverFlow;
		TutorialControl.Instance.onTutorialCompleteCallback += EnablePauseMenu;
		historyScore = PlayerSetting.Instance.GetSetting ("Score");
		historyRound = PlayerSetting.Instance.GetSetting ("Round");
	}

	private void WarnCorePiece ()
	{
		ShowHint (ref corePieceWarningMessage);
	}
	private void WarnOverFlow()
	{
		ShowHint (ref overFlowWarningMessage);
	}
    private UILabel GetAvailableLabel()
    {
        UILabel label;
        if (unusedTips.Count > 0)
        {
            label = unusedTips[0];
            unusedTips.RemoveAt(0);
        }
        else
        {
            label = inusedTips[0];
            DeleteTaskByLabel(label);
        }
        if (!inusedTips.Contains(label)) inusedTips.Add(label);
        label.gameObject.SetActive(true);

        return label;
    }

	void EnablePauseMenu ()
	{
		pauseButton.gameObject.SetActive (true);
		bgmButton.gameObject.SetActive(true);
	}

	public void AddScore(int score, PieceColor color, Vector3 worldPosition)
	{
        UILabel label = GetAvailableLabel();

		int add = (score - 2) * 100 * totalRound;
		
		label.text = "+" + add;
		label.color = convertColor(color);
		TipAnimateTask task = new TipAnimateTask ();
		task.label = label;
        task.speed = .2f;
		task.birthPosition = nguiCamera.ScreenToWorldPoint (Camera.main.WorldToScreenPoint(worldPosition));
	    
		animateTips.Add (task);



		totalScore += add;
		scoreLabel.text = totalScore.ToString ();
		totalScoreCounter.Reset ();

		if (historyScore>0 && totalScore > historyScore && !recordLabel.gameObject.activeInHierarchy) {
			if(TutorialControl.Instance.isActive == false)
			{
				recordLabel.gameObject.SetActive(true);
				SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_HIGHSCORE);
			}

		}
		if (totalScore > historyScore) {
			PlayerSetting.Instance.SetSetting("Score",totalScore);
		}
		/*
		DelayCall delayCall = new DelayCall ();
		delayCall.Init ();
		*/
	}

	public void AddRound (int round)
	{
		//Debug.LogWarning("Add Round");


		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_WIN);

		totalRound = round;
		roundStep = 0;

		roundTipLabel.text = "ROUND"+round.ToString ();

		roundTipLabel.transform.localPosition = new Vector3 (roundXPosition + roundXRange, roundTipLabel.transform.localPosition.y, roundTipLabel.transform.localPosition.z);

		roundTipLabel.gameObject.SetActive (true);

		if (totalRound > historyRound) {
			PlayerSetting.Instance.SetSetting("Round",totalRound);
			/*
			achivementLabel.gameObject.SetActive(true);
			new DelayCall().Init(4f,HideAchivement);
			SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_HIGHSCORE);
			*/
		}

		roundValue.text = round.ToString ();

		inNewRound = true;

		EnviormentControl.Instance.Blink (Wall.GetLevelColor (round - 1));

	}
	public void HideAchivement()
	{
		achivementLabel.gameObject.SetActive(false);
	}
    public void ReinforceWall(Vector3 worldPosition)
    {

        SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_UPGRADE);

        UILabel label = wallTip;
        label.gameObject.SetActive(true);
        //label.text = "+1";

        label.color = Wall.GetLevelColor(Board.Instance.round);
        TipAnimateTask task = new TipAnimateTask();
        task.label = label;
        task.speed = .1f;
        task.birthPosition = nguiCamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(worldPosition));
		level += 1;
		//roundValue.text = level.ToString ();

        animateTips.Add(task);
    }

	public void AddProgress()
	{

		if (!progressCounter.Expired ()) {
			threholdCounter.Tick(1f);
			if(threholdCounter.Expired())
			{

				threholdMaxCounter.Tick(1);
				if(!threholdMaxCounter.Expired())
				{
					skillButton.AddProgress(1);
				}
				SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_COLLECT);

			}
		} else {
			threholdCounter.Reset();
			threholdMaxCounter.Reset();
		}
		progressCounter.Reset ();
	}

    public void EnergyRefill()
    {
        int count = skillButton.GetRemainingProgress();
        for (int i = 0; i < count; i++)
        {
            new DelayCall().Init(.05f * (float)i, AddProgressImediately);
        }
    }

    public void AddProgressImediately()
    {
        skillButton.AddProgress(1);
    }

	private Color32 convertColor(PieceColor color)
	{
		switch (color) {
			case PieceColor.Blue:
			return new Color32(98,255,255,255);
			break;
			case PieceColor.Red:
			return new Color32(255,0,0,255);
			break;
			case PieceColor.Yellow:
			return new Color32(255,255,0,255);
			break;
			case PieceColor.Green:
			return new Color32(0,255,0,255);
			break;
			case PieceColor.Purple:
			return new Color32(255,0,212,255);
			break;
		}
		return new Color32(255,255,255,255);
	}
	private void DeleteTaskByLabel(UILabel label)
	{
		foreach (var i in animateTips) {
			if(i.label == label)
			{
				animateTips.Remove(i);
			}
		}

	}
	public void ShowHint(ref string message)
	{
		hintLabel.gameObject.SetActive (true);
		hintLabel.text = message;

	}
	public void HideHint()
	{
		hintLabel.gameObject.SetActive (false);
	}
	void Update () {
		if (animateTips.Count > 0) {
			List<TipAnimateTask> temp = new List<TipAnimateTask>(animateTips.ToArray());
			foreach (var i in temp) {
				i.Process();
				if(i.Done() )
				{
					animateTips.Remove(i);
					if(i.label.name.Contains("Tips"))
					{
						if(inusedTips.Contains(i.label))inusedTips.Remove(i.label);
						if(!unusedTips.Contains(i.label))unusedTips.Add(i.label);
					}

					i.label.gameObject.SetActive(false);
				}
			}
		}
		progressCounter.Tick (Time.deltaTime);
		if (!totalScoreCounter.Expired ()) {
			totalScoreCounter.Tick(Time.deltaTime);
			float ratio = 1f + Mathf.Sin(totalScoreCounter.percent*Mathf.PI) *.1f;
			scoreLabel.transform.localScale = new Vector3(ratio,ratio,ratio);
			if(recordLabel.gameObject.activeInHierarchy)recordLabel.transform.localScale = scoreLabel.transform.localScale;
		}
		resetSkillButtonPositionCounter.Tick (Time.deltaTime);
		if (resetSkillButtonPositionCounter.Expired ()) {
			resetSkillButtonPositionCounter.Reset();
			InitLayout();
		}
		if (inNewRound) {
			if(roundStep == 0)
			{
				roundFadeInCounter.Tick(Time.deltaTime);
				float percent = Mathf.Max(0f,1f-roundFadeInCounter.percent);
				roundTipLabel.transform.localPosition = new Vector3 (roundXPosition + roundXRange*percent, roundTipLabel.transform.localPosition.y, roundTipLabel.transform.localPosition.z);

				//Debug.Log("In "+(roundXPosition + 100f*percent));

				if(roundFadeInCounter.Expired())
				{
					roundStep++;
					roundFadeInCounter.Reset();
				}
			}
			else if(roundStep == 1)
			{
				roundIdleCounter.Tick(Time.deltaTime);
				if(roundIdleCounter.Expired())
				{
					roundStep++;
					roundIdleCounter.Reset();
				}
			}
			else if(roundStep == 2)
			{

				roundFadeInCounter.Tick(Time.deltaTime);
				float percent = Mathf.Min(1f,roundFadeInCounter.percent);

				//Debug.Log("Out "+(roundXPosition - 100f*percent));

				roundTipLabel.transform.localPosition = new Vector3 (roundXPosition - roundXRange*percent, roundTipLabel.transform.localPosition.y, roundTipLabel.transform.localPosition.z);
				if(roundFadeInCounter.Expired())
				{
					inNewRound = false;
					roundTipLabel.transform.gameObject.SetActive(false);
					roundFadeInCounter.Reset();
				}
			}
		}
	}
}
public class TipAnimateTask
{
	public Counter lifeCounter = new Counter(1f);
	public Vector3 birthPosition = Vector3.zero;
	public UILabel label;
    public float speed = 0.2f;
	public void Process()
	{
		lifeCounter.Tick (Time.deltaTime);
        label.transform.position = new Vector3(birthPosition.x, birthPosition.y, 0) + Vector3.up * speed * lifeCounter.percent;
	}
	public bool Done()
	{
		return lifeCounter.Expired ();
	}
}