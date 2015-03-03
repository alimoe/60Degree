using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HudMenu : MenuSingleton<HudMenu>{

	private List<UILabel> tips;
	private UILabel scoreLabel;
	private List<TipAnimateTask> animateTips;
	private List<UILabel> unusedTips;
	private List<UILabel> inusedTips;
	private UISprite pauseButton;
	private UILabel roundTipLabel;
	private UILabel roundLabel;
	private UILabel roundValue;
	private SkillButton skillButton;
	private Counter progressCounter;
	private Counter threholdCounter;
	private Counter totalScoreCounter;
	private Camera nguiCamera;
	private int totalScore;
	private int totalRound;
	private Vector3 initPosition;
	private SpriteRenderer gameBoard;
	private bool inTransitionIn;
	private Counter transitionInCounter;

	private bool inNewRound;
	private Counter roundFadeInCounter;
	private Counter roundIdleCounter;
	private int roundStep = 0;
	private float roundXPosition;
	private float headYPosition;
	private float footYPosition;
	private float roundXRange = 200f;
	void Awake () {
		base.Awake ();
		tips = new List<UILabel> ();
		nguiCamera = GameObject.Find ("UI Root/Camera").GetComponent<Camera> ();
		GameObject enviorment = GameObject.Find ("Enviorment");
		Transform[] children = enviorment.GetComponentsInChildren<Transform> (true);

		foreach (var child in children) {
			//Debug.Log(child.name);
			if(child.name.Contains("Board"))
			{
				gameBoard = child.GetComponent<SpriteRenderer>();
			}
		}
		animateTips = new List<TipAnimateTask> ();
		children = this.GetComponentsInChildren<Transform> ();
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
				//pauseButton.gameObject.SetActive(false);
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
			if(child.name.Contains("RoundLabel"))
			{
				roundLabel = child.GetComponent<UILabel>();
					
					
			}
		}
		inusedTips = new List<UILabel> ();
		unusedTips = new List<UILabel> (tips.ToArray());

		progressCounter = new Counter (.5f);
		threholdCounter = new Counter (2f);
		totalScoreCounter = new Counter (.5f);
		Board.Instance.onEliminatePieceCallback = AddScore;
		Board.Instance.onDropDownPieceCallback = AddProgress;
		Board.Instance.onHitRoundCallback = AddRound;
		initPosition = scoreLabel.transform.localPosition;
		//Debug.LogWarning (scoreLabel.transform.localPosition);
		totalScore = 0;
		totalRound = 1;
		totalScoreCounter.percent = 1;
		transitionInCounter = new Counter (.3f);
		roundFadeInCounter = new Counter (.2f);
		roundIdleCounter = new Counter (1.3f);
		//InitLayout ();
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
		skillButton.CostProgress ();
		scoreLabel.text = totalScore.ToString ();
	}
	public void InitLayout()
	{
		if(gameBoard!=null)gameBoard.gameObject.SetActive (true);
		Board.Instance.StartPlay ();

		/*
		scoreLabel.gameObject.SetActive(true);
		skillButton.gameObject.SetActive(true);
		pauseButton.gameObject.SetActive(true);
		*/

	}
	void Start()
	{
		scoreLabel.transform.localPosition = new Vector3(scoreLabel.transform.localPosition.x, headYPosition + 50f,scoreLabel.transform.localPosition.z);
		roundLabel.transform.localPosition = new Vector3(roundLabel.transform.localPosition.x, headYPosition + 50f,roundLabel.transform.localPosition.z);
		roundValue.transform.localPosition = new Vector3(roundValue.transform.localPosition.x, headYPosition + 50f,roundValue.transform.localPosition.z);

		skillButton.transform.localPosition = new Vector3(skillButton.transform.localPosition.x, footYPosition - 50f,skillButton.transform.localPosition.z);
		pauseButton.transform.localPosition = new Vector3(pauseButton.transform.localPosition.x, footYPosition - 50f,pauseButton.transform.localPosition.z);
	}
	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		inTransitionIn = true;
		transitionInCounter.Reset ();
		InitLayout ();

	}

	public void AddScore(int score, PieceColor color, Vector3 worldPosition)
	{
		UILabel label;
		if (unusedTips.Count > 0) {
			label = unusedTips[0];
			unusedTips.RemoveAt(0);
		} else {
			label = inusedTips[0];
			DeleteTaskByLabel(label);
		}
			
		if (!inusedTips.Contains (label))inusedTips.Add (label);
		label.gameObject.SetActive (true);
		label.text = "+" + ((score-2)*100);
		label.color = convertColor(color);
		TipAnimateTask task = new TipAnimateTask ();
		task.label = label;
		//Debug.LogWarning ("AddScore " + color);
		task.birthPosition = nguiCamera.ScreenToWorldPoint (Camera.main.WorldToScreenPoint(worldPosition));
		//Debug.LogWarning ("NGUI Position " + task.birthPosition);
		animateTips.Add (task);

		totalScore += (score - 2) * 100;
		scoreLabel.text = totalScore.ToString ();
		totalScoreCounter.Reset ();

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

		roundTipLabel.text = "Round"+round.ToString ();

		roundTipLabel.transform.localPosition = new Vector3 (roundXPosition + roundXRange, roundTipLabel.transform.localPosition.y, roundTipLabel.transform.localPosition.z);

		roundTipLabel.gameObject.SetActive (true);

		roundValue.text = round.ToString ();

		inNewRound = true;
	}

	public void AddProgress()
	{

		if (!progressCounter.Expired ()) {
			threholdCounter.Tick(1f);
			if(threholdCounter.Expired())
			{

				skillButton.AddProgress(1);
			}
		} else {
			threholdCounter.Reset();
		}
		progressCounter.Reset ();
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

	void Update () {
		if (animateTips.Count > 0) {
			List<TipAnimateTask> temp = new List<TipAnimateTask>(animateTips.ToArray());
			foreach (var i in temp) {
				i.Process();
				if(i.Done())
				{
					animateTips.Remove(i);
					if(inusedTips.Contains(i.label))inusedTips.Remove(i.label);
					if(!unusedTips.Contains(i.label))unusedTips.Add(i.label);
					i.label.gameObject.SetActive(false);
				}
			}
		}
		progressCounter.Tick (Time.deltaTime);
		if (!totalScoreCounter.Expired ()) {
			totalScoreCounter.Tick(Time.deltaTime);
			float ratio = 1f + Mathf.Sin(totalScoreCounter.percent*Mathf.PI) *.1f;
			scoreLabel.transform.localScale = new Vector3(ratio,ratio,ratio);
		}
		if (inTransitionIn) {
			transitionInCounter.Tick(Time.deltaTime);
			float percent = Mathf.Max(0, 1f - transitionInCounter.percent);
			scoreLabel.transform.localPosition = new Vector3(scoreLabel.transform.localPosition.x, headYPosition + 50f*percent,scoreLabel.transform.localPosition.z);
			roundLabel.transform.localPosition = new Vector3(roundLabel.transform.localPosition.x, headYPosition + 50f*percent,roundLabel.transform.localPosition.z);
			roundValue.transform.localPosition = new Vector3(roundValue.transform.localPosition.x, headYPosition + 50f*percent,roundValue.transform.localPosition.z);
			skillButton.transform.localPosition = new Vector3(skillButton.transform.localPosition.x, footYPosition - 50f*percent,skillButton.transform.localPosition.z);
			pauseButton.transform.localPosition = new Vector3(pauseButton.transform.localPosition.x, footYPosition - 50f*percent,pauseButton.transform.localPosition.z);

			if(transitionInCounter.Expired())
			{
				inTransitionIn = false;
			}
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
	public void Process()
	{
		lifeCounter.Tick (Time.deltaTime);
		label.transform.position = new Vector3(birthPosition.x,birthPosition.y,0) + Vector3.up * 0.2f * lifeCounter.percent;
	}
	public bool Done()
	{
		return lifeCounter.Expired ();
	}
}