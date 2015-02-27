using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Hud : Core.MonoStrictSingleton<Hud> {

	private List<UILabel> tips;
	private UILabel scoreLabel;
	private List<TipAnimateTask> animateTips;
	private List<UILabel> unusedTips;
	private List<UILabel> inusedTips;
	private SkillButton skillButton;
	private Counter progressCounter;
	private Counter threholdCounter;
	private Counter totalScoreCounter;
	private Camera nguiCamera;
	private int totalScore;
	private Vector3 initPosition;
	void Start () {
		tips = new List<UILabel> ();
		nguiCamera = GameObject.Find ("UI Root/Camera").GetComponent<Camera> ();
		animateTips = new List<TipAnimateTask> ();
		Transform[] children = this.GetComponentsInChildren<Transform> ();
		foreach (var child in children) {
			if(child.name.Contains("Tip"))
			{
				tips.Add(child.GetComponent<UILabel>());
				tips[tips.Count-1].gameObject.SetActive(false);
			}
			if(child.name.Contains("Score"))
			{
				scoreLabel = child.GetComponent<UILabel>();
			}
			if(child.name.Contains("SkillButton"))
			{
				skillButton = child.GetComponent<SkillButton>();
			}
		}
		inusedTips = new List<UILabel> ();
		unusedTips = new List<UILabel> (tips.ToArray());

		progressCounter = new Counter (.5f);
		threholdCounter = new Counter (2f);
		totalScoreCounter = new Counter (.5f);
		Board.Instance.onEliminatePieceCallback = AddScore;
		Board.Instance.onDropDownPieceCallback = AddProgress;
		initPosition = scoreLabel.transform.localPosition;
		//Debug.LogWarning (scoreLabel.transform.localPosition);
		totalScore = 0;
		totalScoreCounter.percent = 1;
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

	public void AddProgress()
	{

		if (!progressCounter.Expired ()) {
			threholdCounter.Tick(1f);
			if(threholdCounter.Expired())
			{
				//Debug.LogWarning ("AddProgress");
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
		label.transform.position = birthPosition + Vector3.up * 0.2f * lifeCounter.percent;
	}
	public bool Done()
	{
		return lifeCounter.Expired ();
	}
}