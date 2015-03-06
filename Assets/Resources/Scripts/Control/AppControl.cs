using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

		UIControl.Instance.Initialize ();
		skills = new List<Skill> ();
		state = GameState.GameNotStart;
		UIControl.Instance.OpenMenu("StartMenu");
	}

	public void AddSkill(Skill skill)
	{
		if(!skills.Contains(skill) && skill.OnAdd() == false )skills.Add (skill);
	}

	public void StartGame()
	{
		state = GameState.GamePlaying;
		UIControl.Instance.OpenMenu("HudMenu",true);
        Camera3DControl.Instance.direction = Vector3.zero;
        if (TutorialControl.Instance.isActive)
        {

        }
        else
        {
            Board.Instance.StartPlay();
        }
        //SkyBoxControl.Instance.ChangeColor(SkyColor.Green);
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
        if (state == GameState.GamePaused)
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
                }
            }
			
		}

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
                
                Camera3DControl.Instance.direction = Vector3.Cross(Board.Instance.GetPhysicDirection(direction).normalized, Vector3.back);
                if (skills.Count == 0)
                {
                    Board.Instance.MoveFrom(position, direction);
                }
            }
			

		}
 		
	}
}
