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
public class AppControl : Core.MonoSingleton<AppControl> {

	private List<Skill> skills;
	private GameState state;
	void Start () {
		Application.targetFrameRate = 60;
		UIControl.Instance.Initialize ();
		skills = new List<Skill> ();
		state = GameState.GameNotStart;
		UIControl.Instance.OpenMenu("StartMenu");
		SoundControl.Instance.PlayTrack (SoundControl.Instance.Track1);
	}

	public void AddSkill(Skill skill)
	{
		if (skills.Count == 0 && skill.OnAdd () == false) {
				skills.Add (skill);
				HudMenu.Instance.ShowHint (ref skill.hint);
		} else {
			Board.Instance.GeneratePiece();
		}
	}

	public void StartGame()
	{
		state = GameState.GamePlaying;
		UIControl.Instance.OpenMenu("HudMenu",true);
        Camera3DControl.Instance.direction = Vector3.zero;

		//IOSControl.Instance.Login();
        if (!PlayerSetting.Instance.tutorialPlayed)
        {
			TutorialControl.Instance.InitTutorial ();
        }
        else
        {
			new DelayCall().Init(.3f, Board.Instance.StartPlay);
        }
        //SkyBoxControl.Instance.ChangeColor(SkyColor.Green);
	}
	public void PlayTutorial()
	{
		state = GameState.GamePlaying;
		UIControl.Instance.OpenMenu("HudMenu",true);
		Camera3DControl.Instance.direction = Vector3.zero;
		new DelayCall ().Init (.3f, TutorialControl.Instance.InitTutorial);
		
	}
	public void ShowCrediet()
	{
		StartMenu.Instance.ShowCredit();

	}
	public void PauseGame()
	{
		state = GameState.GamePaused;
		UIControl.Instance.OpenMenu ("PauseMenu", true, true);
		
	}
	public void ResetGame()
	{
		state = GameState.GamePlaying;
        SkyBoxControl.Instance.Reset();
		UIControl.Instance.CloseMenu ();
		Board.Instance.ResetBoard ();
        Camera3DControl.Instance.direction = Vector3.zero;
		HudMenu.Instance.Reset ();
		Board.Instance.StartPlay ();

	}
	public void ResumeGame()
	{
		if (state == GameState.GamePaused || state == GameState.GameOver)
        {
            state = GameState.GamePlaying;
            UIControl.Instance.CloseMenu();
        }
		
	}
	public void EndGame()
	{
		state = GameState.GameOver;
		UIControl.Instance.OpenMenu ("GameOverMenu", true, true);
	}

	public void ReportScore(int score)
	{
		IOSControl.Instance.ReportGameScore (score);
	}
	public void PurchaseEnergy()
	{
		IOSControl.Instance.Purchase (EnergyRefill);
	}
	public void ShowLeadboard()
	{
		IOSControl.Instance.ShowGameCenterLeaderBoard ();
	}
    public void EnergyRefill()
    {
        ResumeGame();
        HudMenu.Instance.EnergyRefill();
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
                if (skills.Count > 0)
                {
                    Skill skill = skills[0];
                    bool result = skill.Excute(position);
                    if (result) skills.RemoveAt(0);

					Board.Instance.GeneratePiece();
					HudMenu.Instance.HideHint();

                }
				else
				{
					Board.Instance.SelectFrom(position);
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
