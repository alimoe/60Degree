using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum GameState
{
	GameNotStart,
	GamePlaying,
	GamePaused,
	GameOver

}
public enum GameMode
{
    Classic,
    Levels,
    Speed,
    Test
}
public class AppControl : Core.MonoSingleton<AppControl> {

	private List<Skill> skills;
	public GameState state;
    private GameMode mode;
    protected override void Awake()
    {
        base.Awake();
        InitBoard();
    }
	void Start () {
		Application.targetFrameRate = 60;
		UIControl.Instance.Initialize ();
		skills = new List<Skill> ();
		state = GameState.GameNotStart;
        

        mode = GameMode.Classic;

		UIControl.Instance.OpenMenu("StartMenu");//StartMenu // LevelSelectMenu
		SoundControl.Instance.PlayTrack (SoundControl.Instance.Track1);
        WallIcon.Instance.SetUp();
	}
    public void InitBoard()
    {
        if (GameObject.Find("Board") == null)
        {
            GameObject board = Instantiate(Resources.Load("Prefabs/Board")) as GameObject;
            board.transform.localPosition = Vector3.zero;
        }
    }
    public bool IsPlaying()
    {
        return state == GameState.GamePlaying;
    }
	public void AddSkill(Skill skill)
	{
		if (skills.Count == 0 && skill.OnAdd () == false) {
				skills.Add (skill);
				ClassicHudMenu.Instance.ShowHint (ref skill.hint);
		} else {
			Board.Instance.GeneratePiece();
		}
	}
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
			if (mode == GameMode.Classic && (state == GameState.GamePlaying || state == GameState.GameOver) && !TutorialControl.Instance.isActive)
            {
                PauseGame("PauseMenu");
                ClassicModeControl.Instance.SaveBoard();
            }
        }
    }
    public void ExitGame()
    {
        state = GameState.GameNotStart;
        UIControl.Instance.OpenMenu("StartMenu", true);
    }
	private void CheckIsTestMode()
	{
		
		if (PlayerPrefs.GetInt("TestMode") == 1)
		{
			mode = GameMode.Test;
			PlayerPrefs.SetInt("TestMode", 0);
			PlayerPrefs.Save();
		}
	}
	public void StartClassicGame()
	{
		state = GameState.GamePlaying;
		mode = GameMode.Classic;
		CheckIsTestMode ();
       	
	    if (!PlayerSetting.Instance.tutorialPlayed)
        {
			UIControl.Instance.OpenMenu("ClassicHudMenu",true);
			new DelayCall().Init(.3f, TutorialControl.Instance.InitTutorial);
		}
        else
        {
            if (mode == GameMode.Classic)
			{
				UIControl.Instance.OpenMenu("ClassicHudMenu",true);
				new DelayCall().Init(.3f, ClassicModeControl.Instance.StartPlay);
			}
            else if (mode == GameMode.Test) 
			{
				UIControl.Instance.OpenMenu("LevelHudMenu",true);
				new DelayCall().Init(.3f, LevelControl.Instance.StartTest);
			}
        }
        
	}
    public void StartSpeedGame()
    {
        state = GameState.GamePlaying;
        mode = GameMode.Speed;
        CheckIsTestMode();
        if (mode == GameMode.Speed)
        {
            new DelayCall().Init(.3f, SpeedModeControl.Instance.StartPlay);
        }
        else if (mode == GameMode.Test)
        {
            UIControl.Instance.OpenMenu("LevelHudMenu", true);
            new DelayCall().Init(.3f, LevelControl.Instance.StartTest);
        }
    }
	public void StartLevelsGame()
	{
		state = GameState.GamePlaying;
		mode = GameMode.Levels;
		CheckIsTestMode ();

		if (mode == GameMode.Levels)
		{
			new DelayCall().Init(.3f, LevelControl.Instance.StartPlay);
		}
		else if (mode == GameMode.Test) 
		{
			UIControl.Instance.OpenMenu("LevelHudMenu",true);
			new DelayCall().Init(.3f, LevelControl.Instance.StartTest);
		}
	}

   
	public void PlayTutorial()
	{
		state = GameState.GamePlaying;
        mode = GameMode.Classic;
		UIControl.Instance.OpenMenu("ClassicHudMenu",true);
		Camera3DControl.Instance.direction = Vector3.zero;
		new DelayCall ().Init (.3f, TutorialControl.Instance.InitTutorial);
		
	}
	public void ShowCrediet()
	{
		StartMenu.Instance.ShowCredit();

	}
	
    public void PauseGame(string menu)
    {
        state = GameState.GamePaused;
        UIControl.Instance.OpenMenu(menu, true, true);
    }
	public void ResetGame()
	{
		state = GameState.GamePlaying;
        SkyBoxControl.Instance.Reset();
        UIControl.Instance.CloseAllOverlay();
        if (mode == GameMode.Classic)
        {
            ClassicModeControl.Instance.ResetMode();
        }
        

		
		

	}
	public void ResumeGame()
	{
		if (state == GameState.GamePaused || state == GameState.GameOver)
        {
            state = GameState.GamePlaying;
            UIControl.Instance.CloseMenu();
        }
		
	}
    public void GameOver(string menu)
	{
		state = GameState.GameOver;
        UIControl.Instance.OpenMenu(menu, true, true);
	}

	public void ReportScore(int score)
	{
		ExternalControl.Instance.ReportGameScore (score);
	}
	public void PurchaseEnergy()
	{
		ExternalControl.Instance.Purchase (EnergyRefill);
	}
	public void ShowLeadboard()
	{
		ExternalControl.Instance.ShowGameCenterLeaderBoard ();
	}
    public void EnergyRefill()
    {
        ResumeGame();
		ClassicHudMenu.Instance.EnergyRefill();
    }

    public void HandleDrag(Vector3 position)
    {
        if (Board.Instance.selected == null)
        {
            Board.Instance.SelectFrom(position);
            if (Board.Instance.selected!=null) InputControl.Instance.ChangePressedPosition(position);
        }
    }

	public void HandleTap(Vector3 position)
	{
		if (state == GameState.GamePlaying) 
		{
            if (TutorialControl.Instance.isActive)
            {
                TutorialControl.Instance.HandleTap(position);
            }
            else
            {
                if (mode == GameMode.Classic)
                {
                    if (skills.Count > 0)
                    {
                        Skill skill = skills[0];
                        bool result = skill.Excute(position);
                        if (result)
                        {
                            skills.RemoveAt(0);
                            if (ClassicHudMenu.Instance!=null) ClassicHudMenu.Instance.HideHint();
                            Board.Instance.GeneratePiece();
                        }


                    }
                    else
                    {
                        if (ClassicHudMenu.Instance != null) ClassicHudMenu.Instance.HideHint();
                        Board.Instance.SelectFrom(position);
                    }
                }
                else if (mode == GameMode.Speed)
                {
					SpeedModeControl.Instance.HandleTap(position);
                }
            }
			
		}

	}
	public void ToggleSound()
	{
			
		SoundControl.Instance.MuteSoundEffect (!PlayerSetting.Instance.muteSE);
	}

	public void ToggleMusic()
	{
		SoundControl.Instance.MuteMusic (!PlayerSetting.Instance.muteBGM);
	}
	public void HandleSwipe(Vector3 position, BoardDirection direction)
	{
		if (state == GameState.GamePlaying) 
		{
            if (TutorialControl.Instance.isActive)
            {
                TutorialControl.Instance.HandleSwipe(position, direction);
            }
			else if(LevelControl.Instance.inGuide)
			{
				LevelControl.Instance.HandleSwipe(position,direction);
			}
            else
            {
				Vector3 d = Board.Instance.GetPhysicDirection(direction).normalized;
				Camera3DControl.Instance.direction = new Vector3(d.y,d.x,0);
                if (skills.Count == 0)
                {
                    Board.Instance.MoveFrom(position, direction);
                }
            }
			

		}
 		
	}
}
