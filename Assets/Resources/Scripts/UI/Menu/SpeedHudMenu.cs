using UnityEngine;
using System.Collections;

public class SpeedHudMenu : MenuSingleton<SpeedHudMenu>
{
    private UILabel timeRemain;
    private UILabel progressInfo;
    private UILabel levelInfo;
    private UILabel hintLabel;
    private UILabel tips;
    private UILabel recordLabel;
    private int count = -1;
    private Counter tipsCounter = new Counter(3f);
    private bool showTips;
	private Counter totalScoreCounter;
    protected override void Awake()
    {
        base.Awake();
        Transform[] children = this.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name.Contains("LevelValue"))
            {
                levelInfo = child.GetComponent<UILabel>();
                //levelInfo.gameObject.SetActive(false);
            }
            if (child.name.Contains("TimeRemain"))
            {
                timeRemain = child.GetComponent<UILabel>();
            }
            if (child.name.Contains("ProgressValue"))
            {
                progressInfo = child.GetComponent<UILabel>();

            }

            if (child.name.Contains("Hint"))
            {
                hintLabel = child.GetComponent<UILabel>();
                hintLabel.gameObject.SetActive(false);
            }
            if (child.name.Contains("Tips"))
            {
                tips = child.GetComponent<UILabel>();
                tips.gameObject.SetActive(false);
            }
            if (child.name.Contains("Record"))
            {
                recordLabel = child.GetComponent<UILabel>();
                recordLabel.gameObject.SetActive(false);
            }
        }
     	
    }

    public void UpdateInfo()
    {
        float seconds = SpeedModeControl.Instance.remainingTimer.target - SpeedModeControl.Instance.remainingTimer.time;
        timeRemain.text = Utility.FormatSeconds(Mathf.Max(0f, seconds));
        if (count != SpeedModeControl.Instance.eliminateCount)
        {
            count = SpeedModeControl.Instance.eliminateCount;
            progressInfo.text = count + "/" + SpeedModeControl.Instance.targetEliminateCount;
            levelInfo.text = SpeedModeControl.Instance.level.ToString();
			totalScoreCounter.Reset();
        }
        //levelInfo.text = SpeedModeControl.Instance.level.ToString();
    }

    public void ShowHint(ref string message)
    {
        hintLabel.gameObject.SetActive(true);
		hintLabel.text = Localization.Get(message);

    }
    public void HideHint()
    {
        hintLabel.gameObject.SetActive(false);
    }
    
    public void ShowRecord(bool flag)
    {
        recordLabel.gameObject.SetActive(flag);
    }
    public void AddTime(float seconds)
    {
        SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_WIN);
        tips.text = "+" + seconds + "'";
        showTips = true;
        tips.gameObject.SetActive(true);
        tipsCounter.Reset();
    }
    void Update()
    {
        if (showTips)
        {
            tipsCounter.Tick(Time.deltaTime);
            if (tipsCounter.Expired())
            {
                tips.gameObject.SetActive(false);
                showTips = false;
            }

        }

		if (totalScoreCounter!=null && !totalScoreCounter.Expired ()) {
			totalScoreCounter.Tick (Time.deltaTime);
			float ratio = 1f + Mathf.Sin(totalScoreCounter.percent*Mathf.PI) *.2f;
			progressInfo.transform.localScale = new Vector3(ratio,ratio,ratio);
		}

    }
    public void Reset()
    {
        count = -1;
    }
    public override void OnOpenScreen()
    {
        base.OnOpenScreen();
        base.OnOpenTransitionDone();
		string hint = PlayerSetting.Instance.GetSetting (PlayerSetting.SpeedModePlayed) != 0 ? "TapToStart" : "SpeedFirstHint";
		PlayerSetting.Instance.SetSetting (PlayerSetting.SpeedModePlayed, 1);
		totalScoreCounter = new Counter (.5f);
		totalScoreCounter.percent = 1f;
		UpdateInfo ();
		progressInfo.text =  "0/10" ;
		levelInfo.text = "1";
		this.ShowHint (ref hint);

    }
	public override void OnCloseScreen()
    {
        base.OnCloseScreen();
        base.OnCloseTransitionDone();
    }
}
