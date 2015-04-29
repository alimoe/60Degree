using UnityEngine;
using System.Collections;

public class CheatControl : Core.MonoSingleton<CheatControl> {
	private bool show = false;
	void OnGUI()
	{
		if (show) {

			if(GUI.Button(new Rect(0,0,100,30),"Bomb"))
			{
				AppControl.Instance.AddSkill(new Bomb());
			}
			if(GUI.Button(new Rect(0,30,100,30),"Lightening"))
			{
				AppControl.Instance.AddSkill(new Lightening());
			}
			if(GUI.Button(new Rect(0,60,100,30),"Cut Side"))
			{
				AppControl.Instance.AddSkill(new CutEdget());
			}
			if(GUI.Button(new Rect(0,90,100,30),"Generate Piece"))
			{
				Board.Instance.GeneratePiece();
			}
			if(GUI.Button(new Rect(0,120,100,30),"Add Skill Point"))
			{
				ClassicHudMenu.Instance.AddProgress();
			}
			if(GUI.Button(new Rect(0,150,100,30),"Enhance Wall"))
			{
                ClassicModeControl.Instance.AddWallProgress();
			}
            if (GUI.Button(new Rect(0, 180, 100, 30), "Add Rope"))
            {
                Board.Instance.GenerateRope();
            }
            if (GUI.Button(new Rect(0, 210, 100, 30), "Add Ice"))
            {
                Board.Instance.GenerateIce();
            }
            if (GUI.Button(new Rect(0, 240, 100, 30), "Add Chain"))
            {
                Board.Instance.GenerateGroup();
            }
            if (GUI.Button(new Rect(0, 270, 100, 30), "Add Fire"))
            {
                Board.Instance.GenerateFire();
            }
			if (GUI.Button(new Rect(0, 300, 100, 30), "Add Block"))
			{
				Board.Instance.GenerateBlock();
			}
			if (GUI.Button(new Rect(0, 330, 100, 30), "Add Clock"))
			{
				Board.Instance.GenerateClock();
			}
            if (GUI.Button(new Rect(0, 360, 100, 30), "Add Switcher"))
            {
                Board.Instance.GenerateSwitcher();
            }
            if (GUI.Button(new Rect(0, 390, 100, 30), "Game Over"))
			{
                ClassicModeControl.Instance.GameOver();
			}
            if (GUI.Button(new Rect(0, 420, 100, 30), "Clear Record"))
			{
                PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicScore, 0);
                PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicRound, 0);
                PlayerSetting.Instance.SetSetting(PlayerSetting.MAX_SPEED_LEVEL, 0);
                PlayerSetting.Instance.SetSetting(PlayerSetting.USER_LEVEL_PROGRESS, 0);
				PlayerSetting.Instance.SetSetting(PlayerSetting.HasUseSkill, 0);
				PlayerSetting.Instance.SetSetting(PlayerSetting.HasPlayLevel, 0);
				PlayerSetting.Instance.SetSetting(PlayerSetting.SpeedModePlayed, 0);
			}
            if (GUI.Button(new Rect(0, 450, 100, 30), "Clear Tutorial"))
			{
				PlayerSetting.Instance.TutorialComplete(0);
			}
			if(GUI.Button(new Rect(0,480,100,30),"Purchase energy"))
			{
				AppControl.Instance.EnergyRefill();
			}
            if (GUI.Button(new Rect(0, 510, 100, 30), "Guide"))
            {
                LevelControl.Instance.DisplayGuide();
            }
            if (GUI.Button(new Rect(0, 540, 100, 30), "Unlock All Level"))
            {
                PlayerSetting.Instance.SetSetting(PlayerSetting.USER_LEVEL_PROGRESS, 40);
            }
		}

	}
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.B)) {
			show = !show;
		}
	}
}
